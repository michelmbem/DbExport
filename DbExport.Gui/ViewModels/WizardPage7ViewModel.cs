using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.Search;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DbExport.Gui.Models;
using DbExport.Providers;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Serilog;

namespace DbExport.Gui.ViewModels;

public partial class WizardPage7ViewModel : WizardPageViewModel
{
    private bool hadError;
    private SearchPanel? searchPanel;
    
    [ObservableProperty]
    private MigrationSummary? summary;

    [ObservableProperty]
    private string? sqlScript;

    public WizardPage7ViewModel()
    {
        Header.Title = "Proceed with migration";
        Header.Description = "Below is the SQL script that was generated to migrate your database. " +
                             "Depending on the target database, you can run it directly from this wizard, " +
                             "or save it to a file and load it in a dedicated tool.";
    }

    public override bool CanMoveForward => false;

    [RelayCommand(CanExecute = nameof(CanExecuteScript))]
    private void ExecuteScript()
    {
        Progress.Message = "Executing SQL script...";
        IsBusy = true;
        hadError = false;
        
        Task.Run(RunSql)
            .GetAwaiter()
            .OnCompleted(() =>
                         {
                             IsBusy = false;
                             ShowScriptExecutionMessage(hadError);
                         });
    }

    [RelayCommand(CanExecute = nameof(CanSaveScript))]
    private async Task SaveScript(Window window)
    {
        var file = await window.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                Title = "Save migration script as",
                DefaultExtension = ".sql",
                FileTypeChoices =
                [
                    new FilePickerFileType("SQL Script") { Patterns = ["*.sql"] },
                    FilePickerFileTypes.All
                ]
            });

        if (file is null) return;
        
        await using var output = new StreamWriter(file.Path.LocalPath, false, Utility.Encoding);
        await output.WriteAsync(SqlScript);
        await output.FlushAsync();
    }
    
    [RelayCommand]
    private void ReloadScript()
    {
        OnSummaryChanged(Summary);
    }

    [RelayCommand]
    private void FindReplace(TextEditor editor)
    {
        if (searchPanel == null)
        {
            editor.SearchPanel.Uninstall();
            searchPanel = SearchPanel.Install(editor);
        }
        
        var selection = editor.TextArea.Selection;
        searchPanel.SearchPattern = selection.IsEmpty || selection.IsMultiline ? string.Empty : selection.GetText();
        searchPanel.Open();
    }
    
    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo(TextEditor editor) => editor.Undo();
    
    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo(TextEditor editor) => editor.Redo();
    
    [RelayCommand(CanExecute = nameof(CanCut))]
    private void Cut(TextEditor editor) => editor.Cut();
    
    [RelayCommand(CanExecute = nameof(CanCopy))]
    private void Copy(TextEditor editor) => editor.Copy();
    
    [RelayCommand(CanExecute = nameof(CanPaste))]
    private void Paste(TextEditor editor) => editor.Paste();
    
    [RelayCommand(CanExecute = nameof(CanDelete))]
    private void Delete(TextEditor editor) => editor.Delete();
    
    [RelayCommand]
    private void SelectAll(TextEditor editor) => editor.SelectAll();
    
    private static string GenerateSqlScript(MigrationSummary? summary)
    {
        if (summary == null) return string.Empty;
        
        using var sqlWriter = new StringWriter();
        var codegen = CodeGenerator.Get(summary.TargetProvider.Name, sqlWriter);
        codegen.ExportOptions = summary.ExportOptions;

        try
        {
            summary.Database.AcceptVisitor(codegen);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to generate SQL script");
        }
        
        return sqlWriter.ToString();
    }

#if WINDOWS
    private static void GenerateAccessDb(MigrationSummary? summary)
    {
        if (summary == null) return;
        
        var builder = new Providers.Access.AccessSchemaBuilder(summary.TargetConnectionString)
        {
            ExportOptions = summary.ExportOptions
        };
        
        try
        {
            summary.Database.AcceptVisitor(builder);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to generate MS ACCESS database");
        }
    }

    private static void GenerateSqlLocalDb(MigrationSummary? summary)
    {
        if (summary == null) return;

        var settings = Utility.ParseConnectionString(summary.TargetConnectionString);
        var tmpConStr = $"Server={settings["server"]};Integrated Security=true";

        using (var helper1 = new SqlHelper(summary.TargetProvider.Name, tmpConStr))
        {
            try
            {
                helper1.Execute($"DROP Database {summary.Database.Name}");
            }
            catch
            {
                // Ignore
            }

            helper1.Execute($"""
                    CREATE DATABASE {summary.Database.Name}
                    ON (NAME= N'{summary.Database.Name}',
                    FILENAME='{settings["attachdbfilename"]}')
                    """);
        }

        SqlHelper.ExecuteSqlScript(summary.TargetConnectionString, GenerateSqlScript(summary));
    }
#endif

    private static void ShowScriptExecutionMessage(bool hadError)
    {
        string title, message;
        Icon icon;

        if (hadError)
        {
            title = "Migration completed with errors";
            message = """
                The migration process has completed,
                but some errors occurred while executing the migration script.
                Please check the logs for more details.
                """;
            icon = Icon.Error;
        }
        else
        {
            title = "Migration completed successfully";
            message = """
                The migration process has completed successfully.
                You can now close this wizard.
                """;
            icon = Icon.Success;
        }

        _ = MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.Ok, icon)
                             .ShowAsync();
    }

    private void RunSql()
    {
       try
       {
#if WINDOWS
           switch (Summary?.TargetProvider.Name)
           {
                case ProviderNames.ACCESS:
                    GenerateAccessDb(Summary);
                    return;
                case ProviderNames.SQLSERVER when Summary.TargetProvider.HasFeature(ProviderFeatures.IsFileBased):
                    GenerateSqlLocalDb(Summary);
                    return;
            }
#endif
           SqlHelper.ExecuteScript(Summary?.TargetProvider.Name, Summary?.TargetConnectionString, SqlScript);
       }
       catch (Exception e)
       {
           hadError = true;
           Log.Error(e, "Failed to execute SQL script");
       }
    }

    private bool CanExecuteScript() =>
        Summary != null && (ScriptIsReady() || !Summary.TargetProvider.HasFeature(ProviderFeatures.SupportsDDL));

    private bool CanSaveScript() =>
        Summary != null && Summary.TargetProvider.HasFeature(ProviderFeatures.SupportsDDL) && ScriptIsReady();

    private bool ScriptIsReady() => !(IsBusy || string.IsNullOrWhiteSpace(SqlScript));
    
    private static bool CanUndo(TextEditor editor) => editor.CanUndo;
    
    private static bool CanRedo(TextEditor editor) => editor.CanRedo;
    
    private static bool CanCut(TextEditor editor) => editor.CanCut;
    
    private static bool CanCopy(TextEditor editor) => editor.CanCopy;
    
    private static bool CanPaste(TextEditor editor) => editor.CanPaste;
    
    private static bool CanDelete(TextEditor editor) => editor.CanDelete;

    partial void OnSummaryChanged(MigrationSummary? value)
    {
        if (value == null) return;

        if (!value.TargetProvider.HasFeature(ProviderFeatures.SupportsDDL))
        {
            SqlScript = """
                /*
                The target database provider does not support DDL statements,
                so no migration script can be generated.
                You can proceed with the migration by clicking the "Execute" button above,
                which will run the necessary operations directly against the target database.
                */
                """;
            
            ExecuteScriptCommand.NotifyCanExecuteChanged();
            return;
        }
        
        Progress.Message = "Generating migration script...";
        IsBusy = true;
        
        Task.Run(() => SqlScript = GenerateSqlScript(value))
            .GetAwaiter()
            .OnCompleted(() =>
                         {
                             IsBusy = false;
                             ExecuteScriptCommand.NotifyCanExecuteChanged();
                             SaveScriptCommand.NotifyCanExecuteChanged();
                         });
    }
}