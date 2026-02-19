using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DbExport.Gui.Models;
using DbExport.Gui.Views;
using DbExport.Providers;
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
    [NotifyCanExecuteChangedFor(nameof(NavigateToPreviousPageCommand), nameof(NavigateToNextPageCommand))]
    private WizardPageViewModel? currentPage;
    
    [ObservableProperty]
    private bool isPageTransitionReversed;

    public MainWindowViewModel()
    {
        wizardPages = [wizardPage1, wizardPage2, wizardPage3, wizardPage4, wizardPage5, wizardPage6, wizardPage7];
        Sidebar.Items.AddRange(wizardPages.Select(p => new SidebarItem(p.Header.Title)));
        CurrentPage = wizardPages[0];
        
        App.AddThemeListener(OnThemeChanged);
    }
    
    public static string ThemeSwitchIcon => App.IsDarkMode ? "fa-sun" : "fa-moon";

    public SidebarViewModel Sidebar { get; } = new();

    [RelayCommand]
    private static void SwitchTheme()
    {
        App.ToggleDarkMode();
    }

    [RelayCommand]
    private static void Close(Window window)
    {
        window.Close();
    }

    [RelayCommand]
    private static async Task OpenAboutDialog(Window window)
    {
        var aboutDialog = new AboutDialog { DataContext = new AboutDialogViewModel() };
        await aboutDialog.ShowDialog<bool>(window);
    }

    [RelayCommand(CanExecute = nameof(CanNavigateToPreviousPage))]
    private void NavigateToPreviousPage()
    {
        IsPageTransitionReversed = true;
        
        var currentIndex = Array.IndexOf(wizardPages, CurrentPage);
        CurrentPage = wizardPages[currentIndex - 1];
    }

    [RelayCommand(CanExecute = nameof(CanNavigateToNextPage))]
    private void NavigateToNextPage()
    {
        IsPageTransitionReversed = false;
        
        var currentIndex = Array.IndexOf(wizardPages, CurrentPage);
        CurrentPage = wizardPages[currentIndex + 1];
    }

    private bool CanNavigateToPreviousPage() => CurrentPage is { CanMoveBackward: true };

    private bool CanNavigateToNextPage() => CurrentPage is { CanMoveForward: true };

    private void LoadDatabaseSchema()
    {
        try
        {
            var provider = SchemaProvider.GetProvider(wizardPage2.SelectedProvider.Name, wizardPage2.ConnectionString);
            wizardPage5.Database = SchemaProvider.GetDatabase(provider, wizardPage2.Connection.SelectedSchema);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error getting database schema");
        }
    }

    partial void OnCurrentPageChanging(WizardPageViewModel? oldValue, WizardPageViewModel? newValue)
    {
        if (oldValue == wizardPage3 && newValue == wizardPage4)
        {
            wizardPage4.ProviderName = wizardPage3.SelectedProvider.Name;
        }
        else if (oldValue == wizardPage4 && newValue == wizardPage5)
        {
            wizardPage5.IsBusy = true;
            Task.Run(LoadDatabaseSchema)
                .GetAwaiter()
                .OnCompleted(() => wizardPage5.IsBusy = false);
        }
        else if (oldValue == wizardPage5 && newValue == wizardPage6)
        {
            wizardPage6.Summary = new MigrationSummary(wizardPage2.SelectedProvider,
                                                       wizardPage2.ConnectionString,
                                                       wizardPage3.SelectedProvider,
                                                       wizardPage3.ConnectionString,
                                                       wizardPage4.ExportOptions,
                                                       wizardPage5.Database!);
        }
        else if (oldValue == wizardPage6 && newValue == wizardPage7)
        {
            wizardPage7.Summary = wizardPage6.Summary;
        }
    }

    partial void OnCurrentPageChanged(WizardPageViewModel? oldValue, WizardPageViewModel? newValue)
    {
        oldValue?.PropertyChanged -= OnCurrentPageIsBusyChanged;
        newValue!.PropertyChanged += OnCurrentPageIsBusyChanged;
        
        var currentIndex = Array.IndexOf(wizardPages, newValue);
        Sidebar.SelectedIndex = currentIndex;
    }

    private void OnCurrentPageIsBusyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(WizardPageViewModel.IsBusy)) return;
        
        NavigateToPreviousPageCommand.NotifyCanExecuteChanged();
        NavigateToNextPageCommand.NotifyCanExecuteChanged();
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(ThemeSwitchIcon));
    }
}