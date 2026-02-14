using CommunityToolkit.Mvvm.ComponentModel;
using DbExport.Providers;

namespace DbExport.Gui.ViewModels;

public partial class WizardPage4ViewModel : WizardPageViewModel
{
    private readonly MySqlOptionsViewModel mysqlOptions = new();
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProviderOptions))]
    [NotifyPropertyChangedFor(nameof(ProviderHasOptions))]
    private string? providerName;

    [ObservableProperty]
    private bool exportSchema;
    
    [ObservableProperty]
    private bool exportData;

    [ObservableProperty]
    private bool exportSchemaAndData = true;
    
    [ObservableProperty]
    private bool exportPrimaryKeys = true;
    
    [ObservableProperty]
    private bool exportForeignKeys = true;
    
    [ObservableProperty]
    private bool exportIndexes = true;
    
    [ObservableProperty]
    private bool exportDefaults;
    
    [ObservableProperty]
    private bool exportIdentities;

    public WizardPage4ViewModel()
    {
        Header.Title = "Migration options";
        Header.Description = "Configure the migration process.";
    }

    public bool ProviderHasOptions => ProviderOptions is not null;
    
    public ProviderOptionsViewModel? ProviderOptions => ProviderName switch
    {
        ProviderNames.MYSQL => mysqlOptions,
        _ => null
    };
    
    public ExportOptions ExportOptions
    {
        get
        {
            var flags = ExportFlags.ExportNothing;
            if (ExportPrimaryKeys) flags |= ExportFlags.ExportPrimaryKeys;
            if (ExportForeignKeys) flags |= ExportFlags.ExportForeignKeys;
            if (ExportIndexes) flags |= ExportFlags.ExportIndexes;
            if (ExportDefaults) flags |= ExportFlags.ExportDefaults;
            if (ExportIdentities) flags |= ExportFlags.ExportIdentities;
            
            return new ExportOptions
            {
                Flags = flags,
                ExportSchema = ExportSchema || ExportSchemaAndData,
                ExportData = ExportData || ExportSchemaAndData,
                ProviderSpecific = ProviderOptions?.Options
            };
        }
    }
}