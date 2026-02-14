using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DbExport.Providers.MySqlClient;

namespace DbExport.Gui.ViewModels;

public partial class MySqlOptionsViewModel : ProviderOptionsViewModel
{
    public ObservableCollection<string> StorageEngines { get; } = [..MySqlOptions.StorageEngines];

    public ObservableCollection<string> CharacterSets { get; } = [..MySqlOptions.CharacterSets];

    [ObservableProperty] private string? storageEngine;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SortOrders))]
    private string? characterSet;

    [ObservableProperty] private string? sortOrder;

    public string[] SortOrders => MySqlOptions.GetSortOrders(CharacterSet);
    
    partial void OnCharacterSetChanged(string? value)
    {
        SortOrder = null;
    }

    public override string Title => "MySQL Options";
    
    public override object Options => new MySqlOptions
    {
        StorageEngine = StorageEngine,
        CharacterSet = CharacterSet,
        SortOrder = SortOrder
    };
}