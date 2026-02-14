using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DbExport.Gui.Models;
using DbExport.Providers;

namespace DbExport.Gui.ViewModels;

public partial class WizardPage2ViewModel : WizardPageViewModel
{
    protected readonly FileConnectionViewModel fileConnection;
    protected readonly ServerConnectionViewModel serverConnection;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ConnectionPane))]
    private DataProvider selectedProvider;

    public ObservableCollection<DataProvider> AllProviders { get; }

    public ConnectionViewModel? ConnectionPane => SelectedProvider.Name switch
    {
        ProviderNames.ACCESS or ProviderNames.SQLITE => fileConnection,
        ProviderNames.SQLSERVER or ProviderNames.ORACLE or ProviderNames.MYSQL or ProviderNames.POSTGRESQL =>
            serverConnection,
        _ => null
    };
    
    public string? ConnectionString => ConnectionPane?.ConnectionString;

    public WizardPage2ViewModel()
    {
        fileConnection = new FileConnectionViewModel();
        serverConnection = new ServerConnectionViewModel();
        AllProviders = new ObservableCollection<DataProvider>(DataProvider.All);
        SelectedProvider = AllProviders[0];
    }

    partial void OnSelectedProviderChanged(DataProvider value)
    {
        ConnectionPane?.DataProvider = value;
    }
}