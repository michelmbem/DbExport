using System;
using System.Collections.ObjectModel;
using System.Linq;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;

namespace DbExport.Gui.ViewModels;

public partial class ServerConnectionViewModel : ConnectionViewModel
{
    [ObservableProperty]
    private string host = "localhost";
    
    [ObservableProperty]
    private int? port;
    
    [ObservableProperty]
    private bool trustedConnection;
    
    [ObservableProperty]
    private string? username;
    
    [ObservableProperty]
    private string? password;
    
    [ObservableProperty]
    private string? catalog;
    
    public ObservableCollection<string> AllCatalogs { get; } = [];

    public override string ConnectionString =>
        DataProvider?.ConnectionStringBuilder.Build(Host, Port, Catalog, TrustedConnection, Username, Password) ?? string.Empty;

    public void ReloadCatalogs()
    {
        AllCatalogs.Clear();
        
        try
        {
            using var helper = new SqlHelper(DataProvider?.Name, ConnectionString);
            var catalogNames = helper.Query(DataProvider?.DatabaseListQuery, SqlHelper.ToList);
            AllCatalogs.AddRange(catalogNames.Cast<string>());
        }
        catch(Exception e)
        {
            Log.Warning(e, "Failed to reload catalogs");
        }
    }

    partial void OnTrustedConnectionChanged(bool value)
    {
        if (value) Username = Password = null;
    }
}