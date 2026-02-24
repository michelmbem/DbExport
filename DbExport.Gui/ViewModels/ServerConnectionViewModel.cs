using System;
using System.Collections.ObjectModel;
using System.Linq;
using AvaloniaEdit.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DbExport.Providers;
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

    [ObservableProperty]
    private string? schema;

    public ObservableCollection<string> AllCatalogs { get; } = [];

    public ObservableCollection<string> AvailableSchemas { get; } = [];

    public override string ConnectionString =>
        DataProvider?.ConnectionStringFactory.Build(Host, Port, Catalog, TrustedConnection, Username, Password) ?? string.Empty;

    public override string SelectedSchema => Schema ?? string.Empty;

    [RelayCommand]
    private void ReloadCatalogs()
    {
        AllCatalogs.Clear();

        if (string.IsNullOrWhiteSpace(DataProvider?.DatabaseListQuery)) return;
        
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

    private void ReloadSchemas()
    {
        AvailableSchemas.Clear();

        if (string.IsNullOrWhiteSpace(Catalog)) return;

        try
        {
            var schemaProvider = SchemaProvider.GetProvider(DataProvider?.Name, ConnectionString);
            var schemas = schemaProvider.GetTableNames()
                                        .Select(t => t.Item2)
                                        .Where(s => !string.IsNullOrEmpty(s))
                                        .Distinct();

            AvailableSchemas.AddRange(schemas);
        }
        catch (Exception e)
        {
            Log.Warning(e, "Failed to reload schemas");
        }
    }

    partial void OnTrustedConnectionChanged(bool value)
    {
        if (value) Username = Password = null;
    }
    
    partial void OnCatalogChanged(string? value)
    {
        ReloadSchemas();
    }
}