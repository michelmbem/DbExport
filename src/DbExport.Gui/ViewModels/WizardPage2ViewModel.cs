using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DbExport.Gui.Models;

namespace DbExport.Gui.ViewModels;

public partial class WizardPage2ViewModel : WizardPageViewModel
{
    protected readonly FileConnectionViewModel fileConnection = new();
    protected readonly ServerConnectionViewModel serverConnection = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Connection))]
    private DataProvider selectedProvider;

    public WizardPage2ViewModel()
    {
        AllProviders = new ObservableCollection<DataProvider>(DataProvider.All);
        SelectedProvider = AllProviders[0];
        
        Header.Title = "Source database";
        Header.Description = "Select the database you want to migrate from.";
    }

    public ObservableCollection<DataProvider> AllProviders { get; }

    public ConnectionViewModel Connection =>
        SelectedProvider.HasFeature(ProviderFeatures.IsFileBased) ? fileConnection : serverConnection;
    
    public string ConnectionString => Connection.ConnectionString;

    partial void OnSelectedProviderChanged(DataProvider value)
    {
        Connection.DataProvider = value;
    }
}