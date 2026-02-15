using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DbExport.Gui.Models;

namespace DbExport.Gui.ViewModels;

public partial class SidebarViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedItem))]
    private int selectedIndex = -1;
    
    public ObservableCollection<SidebarItem> Items { get; } = [];
    
    public SidebarItem? SelectedItem =>
        SelectedIndex >= 0 && SelectedIndex < Items.Count ? Items[SelectedIndex] : null;

    partial void OnSelectedIndexChanged(int oldValue, int newValue)
    {
        if (oldValue >= 0 && oldValue < Items.Count) Items[oldValue].IsSelected = false;
        if (newValue >= 0 && newValue < Items.Count) Items[newValue].IsSelected = true;
    }
}