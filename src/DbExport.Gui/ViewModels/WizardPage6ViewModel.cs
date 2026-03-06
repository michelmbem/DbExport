using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using DbExport.Gui.Models;
using DbExport.Gui.Utilities;

namespace DbExport.Gui.ViewModels;

public partial class WizardPage6ViewModel : WizardPageViewModel
{
    [ObservableProperty]
    private MigrationSummary? summary;

    [ObservableProperty]
    private string? summaryText;

    public WizardPage6ViewModel()
    {
        Header.Title = "Migration summary";
        Header.Description = "Review the migration summary and make any necessary adjustments.";
        
        Progress.Message = "Generating migration summary...";
    }

    partial void OnSummaryChanged(MigrationSummary? value)
    {
        if (value == null) return;
        
        IsBusy = true;
        Task.Run(() => SummaryText = RazorTemplateLoader.LoadTemplate("summary", value))
            .GetAwaiter()
            .OnCompleted(() => IsBusy = false);
    }
}