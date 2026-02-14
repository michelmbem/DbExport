using System;
using Avalonia.Controls;
using DbExport.Gui.ViewModels;

namespace DbExport.Gui.Views;

public partial class ServerConnectionView : UserControl
{
    public ServerConnectionView()
    {
        InitializeComponent();
    }

    private ServerConnectionViewModel? ViewModel => DataContext as ServerConnectionViewModel;
    
    private void DatabaseComboBox_OnDropDownOpened(object? sender, EventArgs e)
    {
        ViewModel?.ReloadCatalogs();
    }
}