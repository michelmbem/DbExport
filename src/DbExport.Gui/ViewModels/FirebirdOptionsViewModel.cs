using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DbExport.Providers.Firebird;

namespace DbExport.Gui.ViewModels;

public partial class FirebirdOptionsViewModel : ProviderOptionsViewModel
{
    [ObservableProperty]
    private string dataDirectory = string.Empty;
    
    [ObservableProperty]
    private string defaultCharSet = "UTF8";
    
    [ObservableProperty]
    private int? pageSize = FirebirdOptions.PageSize;
    
    [ObservableProperty]
    private bool forcedWrites = FirebirdOptions.ForcedWrites;
    
    [ObservableProperty]
    private bool overwrite = FirebirdOptions.Overwrite;

    public static int DefaultPageSize => FirebirdOptions.PageSize;

    public ObservableCollection<string> CharacterSets { get; } = [..FirebirdOptions.CharacterSets];

    public override string Title => "Firebird options";

    public override object Options
    {
        get
        {
            FirebirdOptions.ForcedWrites = ForcedWrites;
            FirebirdOptions.Overwrite = Overwrite;
            
            if (PageSize.HasValue) FirebirdOptions.PageSize = PageSize.Value;
            
            return new FirebirdOptions
            {
                DataDirectory = DataDirectory,
                DefaultCharSet = DefaultCharSet
            };
        }
    }
}