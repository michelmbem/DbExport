using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DbExport.Gui.Models;
using DbExport.Providers;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Serilog;

namespace DbExport.Gui.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly WizardPage1ViewModel wizardPage1 = new();
    private readonly WizardPage2ViewModel wizardPage2 = new();
    private readonly WizardPage3ViewModel wizardPage3 = new();
    private readonly WizardPage4ViewModel wizardPage4 = new();
    private readonly WizardPage5ViewModel wizardPage5 = new();
    private readonly WizardPage6ViewModel wizardPage6 = new();
    private readonly WizardPage7ViewModel wizardPage7 = new(); 
    private readonly WizardPageViewModel[] wizardPages;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NavigateToPreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(NavigateToNextPageCommand))]
    private WizardPageViewModel? currentPage;

    public MainWindowViewModel()
    {
        wizardPages = [ wizardPage1, wizardPage2, wizardPage3, wizardPage4, wizardPage5, wizardPage6, wizardPage7 ];
        CurrentPage = wizardPages[0];
    }
    
    public SidebarViewModel Sidebar { get; } = new();

    [RelayCommand(CanExecute = nameof(CanNavigateToPreviousPage))]
    private void NavigateToPreviousPage()
    {
        var currentIndex = Array.IndexOf(wizardPages, CurrentPage);
        CurrentPage = wizardPages[currentIndex - 1];
    }

    [RelayCommand(CanExecute = nameof(CanNavigateToNextPage))]
    private void NavigateToNextPage()
    {
        var currentIndex = Array.IndexOf(wizardPages, CurrentPage);
        CurrentPage = wizardPages[currentIndex + 1];
    }

    [RelayCommand]
    private static void Close(Window window)
    {
        window.Close();
    }

    [RelayCommand]
    private static async Task OpenAboutDialog(Window window)
    {
        await MessageBoxManager
              .GetMessageBoxStandard("About DbExport",
                                     "DbExport is a tool for exporting database schemas to various formats.",
                                     ButtonEnum.Ok,
                                     Icon.Info)
              .ShowAsync();
    }

    private bool CanNavigateToPreviousPage() => CurrentPage is { CanMoveBackward: true };

    private bool CanNavigateToNextPage() => CurrentPage is { CanMoveForward: true };

    private void LoadDatabaseSchema()
    {
        var schemaProvider = SchemaProvider.GetProvider(wizardPage2.SelectedProvider.Name,
                                                        wizardPage2.ConnectionString);
        
        try
        {
            wizardPage5.IsBusy = true;
            wizardPage5.Database = SchemaProvider.GetDatabase(schemaProvider);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting database schema");
        }
        finally
        {
            wizardPage5.IsBusy = false;
        }
    }

    partial void OnCurrentPageChanging(WizardPageViewModel oldValue, WizardPageViewModel newValue)
    {
        if (oldValue == wizardPage3 && newValue == wizardPage4)
        {
            wizardPage4.ProviderName = wizardPage3.SelectedProvider.Name;
        }
        else if (oldValue == wizardPage4 && newValue == wizardPage5)
        {
            Task.Run(LoadDatabaseSchema)
                .GetAwaiter()
                .OnCompleted(NavigateToNextPageCommand.NotifyCanExecuteChanged);
        }
        else if (oldValue == wizardPage5 && newValue == wizardPage6)
        {
            wizardPage6.Summary = new MigrationSummary(wizardPage2.SelectedProvider,
                                                       wizardPage2.ConnectionString,
                                                       wizardPage3.SelectedProvider,
                                                       wizardPage3.ConnectionString,
                                                       wizardPage4.ExportOptions,
                                                       wizardPage5.Database);
        }
        else if (oldValue == wizardPage6 && newValue == wizardPage7)
        {
            wizardPage7.Summary = wizardPage6.Summary;
        }
    }
}