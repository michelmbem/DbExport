using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DbExport.Gui.Models;
using DbExport.Providers;
using DbExport.Providers.Npgsql;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Serilog;

namespace DbExport.Gui.ViewModels;

public partial class WizardPage7ViewModel : WizardPageViewModel
{
    private bool hadError;
    
    [ObservableProperty]
    private MigrationSummary? summary;

    [ObservableProperty]
    private bool isBusy;

    public override bool CanMoveForward => false;
    
    public string SqlScript { get; set; } = string.Empty;

    public WizardPage7ViewModel()
    {
        Header.Title = "Proceed with migration";
        Header.Description = """
                             Below is the SQL script that was generated to migrate your database.
                             You can run it directly from this wizard, or save it to a file and load it in a dedicated tool.
                             """;
        
        Progress.IsIndeterminate = true;
    }

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
                             if (hadError)
                                 _ = MessageBoxManager
                                     .GetMessageBoxStandard("Something went wrong",
                                                            """
                                                            An error occurred while executing the migration script.
                                                            Check the logs for more details.
                                                            """,
                                                            ButtonEnum.Ok,
                                                            Icon.Error)
                                     .ShowAsync();
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
    
    private static bool CanUndo(TextEditor editor) => editor.CanUndo;
    
    private static bool CanRedo(TextEditor editor) => editor.CanRedo;
    
    private static bool CanCut(TextEditor editor) => editor.CanCut;
    
    private static bool CanCopy(TextEditor editor) => editor.CanCopy;
    
    private static bool CanPaste(TextEditor editor) => editor.CanPaste;
    
    private static bool CanDelete(TextEditor editor) => editor.CanDelete;

    private bool CanExecuteScript() =>
        Summary != null &&
        Summary.TargetProvider.HasFeature(ProviderFeatures.SupportsScriptExecution) &&
        ScriptIsReady();

    private bool CanSaveScript() =>
        Summary != null &&
        Summary.TargetProvider.HasFeature(ProviderFeatures.SupportsDDL) &&
        ScriptIsReady();

    private bool ScriptIsReady() => !(IsBusy || string.IsNullOrWhiteSpace(SqlScript));
    
    private static string GenerateSqlScript(MigrationSummary? summary)
    {
        if (summary == null) return string.Empty;
        
        using var sqlWriter = new StringWriter();
        var codegen = CodeGenerator.Get(summary.TargetProvider.Name, sqlWriter);
        codegen.ExportOptions = summary.ExportOptions;

        if (codegen is NpgsqlCodeGenerator npgsqlCodeGen)
        {
            var settings = Utility.ParseConnectionString(summary.TargetConnectionString);
            if (settings.TryGetValue("username", out var username))
                npgsqlCodeGen.DbOwner = username;
        }
        
        Utility.Encoding = Encoding.UTF8;

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
    private static void GenerateMdb(MigrationSummary? summary)
    {
        if (summary == null) return;
        
        var builder = new AccessSchemaBuilder(summary.TargetConnectionString)
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
#endif

    private void RunSql()
    {
        using var helper = new SqlHelper(Summary?.TargetProvider.Name, Summary?.TargetConnectionString);

        try
        {
            helper.ExecuteScript(SqlScript);
        }
        catch (Exception e)
        {
            hadError = true;
            Log.Error(e, "Failed to execute SQL script");
        }
    }

    partial void OnSummaryChanged(MigrationSummary? value)
    {
        if (value == null) return;
        
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