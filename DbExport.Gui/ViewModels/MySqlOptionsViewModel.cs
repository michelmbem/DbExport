using System;
using System.Collections.ObjectModel;
using System.Linq;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using DbExport.Providers.MySqlClient;

namespace DbExport.Gui.ViewModels;

public partial class MySqlOptionsViewModel : ProviderOptionsViewModel
{
    public MySqlOptionsViewModel()
    {
        StorageEngines = [..MySqlOptions.StorageEngines];
        CharacterSets = [..MySqlOptions.CharacterSets];
        SortOrders = [];

        StorageEngine = StorageEngines.FirstOrDefault();
        CharacterSet = CharacterSets.LastOrDefault();
    }

    public ObservableCollection<string> StorageEngines { get; }

    public ObservableCollection<string> CharacterSets { get; }

    public ObservableCollection<string> SortOrders { get; }

    [ObservableProperty]
    private string? storageEngine;

    [ObservableProperty]
    private string? characterSet;

    [ObservableProperty]
    private string? sortOrder;
    
    partial void OnCharacterSetChanged(string? value)
    {
        SortOrders.Clear();
        
        if (value is null)
            SortOrder = null;
        else
        {
            SortOrders.AddRange(MySqlOptions.GetSortOrders(value));
            SortOrder = SortOrders.FirstOrDefault(so => so.EndsWith("general_ci", StringComparison.OrdinalIgnoreCase)) ??
                        SortOrders.FirstOrDefault();
        }
    }

    public override string Title => "MySQL Options";
    
    public override object Options => new MySqlOptions
    {
        StorageEngine = StorageEngine,
        CharacterSet = CharacterSet,
        SortOrder = SortOrder
    };
}