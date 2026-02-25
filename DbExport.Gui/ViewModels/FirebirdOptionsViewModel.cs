using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DbExport.Providers.Firebird;

namespace DbExport.Gui.ViewModels;

public partial class FirebirdOptionsViewModel : ProviderOptionsViewModel
{
    [ObservableProperty]
    private string dataDirectory = string.Empty;
    
    [ObservableProperty]
    private string characterSet = FirebirdOptions.CharacterSet;
    
    [ObservableProperty]
    private int pageSize = FirebirdOptions.PageSize;
    
    [ObservableProperty]
    private bool forcesWrites = FirebirdOptions.ForcesWrites;
    
    [ObservableProperty]
    private bool overwrite = FirebirdOptions.Overwrite;

    public ObservableCollection<string> CharacterSets { get; } = [..FirebirdOptions.CharacterSets];

    public override string Title => "Firebird options";

    public override object Options
    {
        get
        {
            FirebirdOptions.CharacterSet = CharacterSet;
            FirebirdOptions.PageSize = PageSize;
            FirebirdOptions.ForcesWrites = ForcesWrites;
            FirebirdOptions.Overwrite = Overwrite;
            
            return new FirebirdOptions { DataDirectory = DataDirectory };
        }
    }
}