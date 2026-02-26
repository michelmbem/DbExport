using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using DbExport.Providers.Firebird;

namespace DbExport.Gui.ViewModels;

public partial class FirebirdOptionsViewModel : ProviderOptionsViewModel
{
    [ObservableProperty]
    private string dataDirectory = string.Empty;
    
    [ObservableProperty]
    private ComboBoxItem? defaultCharSet;
    
    [ObservableProperty]
    private int? pageSize = FirebirdOptions.PageSize;
    
    [ObservableProperty]
    private bool forcedWrites = FirebirdOptions.ForcedWrites;
    
    [ObservableProperty]
    private bool overwrite = FirebirdOptions.Overwrite;

    public FirebirdOptionsViewModel()
    {
        DefaultCharSet = CharacterSets[1];
    }

    public static int DefaultPageSize => FirebirdOptions.PageSize;

    public ObservableCollection<ComboBoxItem> CharacterSets { get; } =
        [..FirebirdOptions.CharacterSets.Select(CharacterSet2ComboBoxItem)];

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
                DefaultCharSet = DefaultCharSet?.Content!.ToString()
            };
        }
    }

    private static ComboBoxItem CharacterSet2ComboBoxItem(string charSet) =>
        charSet.StartsWith("--")
            ? new ComboBoxItem
            {
                Content = charSet[2..^2],
                FontWeight = FontWeight.Bold,
                Background = Brushes.Gray,
                IsEnabled = false,
            }
            : new ComboBoxItem
            {
                Content = charSet,
                Margin = new Thickness(10, 0, 0, 0),
            };
}