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

    private ServerConnectionViewModel? ViewModel => (ServerConnectionViewModel?)DataContext;

    private void OnCatalogComboBoxDropDownOpened(object? sender, EventArgs e)
    {
        ViewModel?.ReloadCatalogs();
    }

    private void OnSchemaComboBoxDropDownOpened(object? sender, EventArgs e)
    {
        ViewModel?.ReloadSchemas();
    }
}