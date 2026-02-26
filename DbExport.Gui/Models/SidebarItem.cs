using CommunityToolkit.Mvvm.ComponentModel;

namespace DbExport.Gui.Models;

public partial class SidebarItem(string title) : ObservableObject
{
    [ObservableProperty]
    private bool isSelected;

    public string Title { get; } = title;
}