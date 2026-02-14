using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DbExport.Gui.Models;
using DbExport.Providers;
using DbExport.Providers.Npgsql;
using Serilog;

namespace DbExport.Gui.ViewModels;

public partial class WizardPage7ViewModel : WizardPageViewModel
{
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

    [RelayCommand(CanExecute = nameof(ScriptIsReady))]
    private void ExecuteScript()
    {
        Progress.Message = "Executing SQL script...";
        IsBusy = true;
        
        Task.Run(RunSql)
            .GetAwaiter()
            .OnCompleted(() => IsBusy = false);
    }

    [RelayCommand(CanExecute = nameof(ScriptIsReady))]
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

    private void RunSql()
    {
        using var helper = new SqlHelper(Summary?.TargetProvider?.Name, Summary?.TargetConnectionString);

        try
        {
            helper.ExecuteScript(SqlScript);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to execute SQL script");
        }
    }

    private bool ScriptIsReady() => !(IsBusy || string.IsNullOrWhiteSpace(SqlScript));
}