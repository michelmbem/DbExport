using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DbExport.Gui.Models;

public partial class SidebarItem(string title) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Foreground))]
    private bool isSelected;

    public string Title { get; } = title;
    
    public IBrush Foreground => IsSelected ? Brushes.Yellow : Brushes.White;
}