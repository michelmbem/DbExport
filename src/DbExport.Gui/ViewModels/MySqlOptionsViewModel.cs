using System.Collections.ObjectModel;
using System.Linq;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using DbExport.Providers.MySqlClient;

namespace DbExport.Gui.ViewModels;

public partial class MySqlOptionsViewModel : ProviderOptionsViewModel
{
    [ObservableProperty]
    private string? storageEngine;

    [ObservableProperty]
    private CharacterSet? characterSet;

    [ObservableProperty]
    private string? collation;

    [ObservableProperty]
    private bool isMariaDb;

    public MySqlOptionsViewModel()
    {
        StorageEngines = [..MySqlOptions.StorageEngines];
        CharacterSets = [..MySqlOptions.CharacterSets];
        Collations = [];

        StorageEngine = StorageEngines.FirstOrDefault();
        CharacterSet = CharacterSets.FirstOrDefault(cs => cs.Name == "utf8mb3");
    }

    public ObservableCollection<string> StorageEngines { get; }

    public ObservableCollection<CharacterSet> CharacterSets { get; }

    public ObservableCollection<string> Collations { get; }

    public override string Title => "MySQL options";

    public override object Options => new MySqlOptions
    {
        StorageEngine = StorageEngine,
        CharacterSet = CharacterSet,
        Collation = Collation,
        IsMariaDb = IsMariaDb
    };

    partial void OnCharacterSetChanged(CharacterSet? value)
    {
        Collations.Clear();
        
        if (value is null)
            Collation = null;
        else
        {
            Collations.AddRange(value.Collations);
            Collation = value.DefaultCollation;
        }
    }
}