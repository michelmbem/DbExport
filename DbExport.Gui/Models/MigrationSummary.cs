using DbExport.Providers;
using DbExport.Schema;

namespace DbExport.Gui.Models;

public sealed class MigrationSummary
{
    public MigrationSummary(DataProvider sourceProvider,
                            string sourceConnectionString,
                            DataProvider targetProvider,
                            string targetConnectionString,
                            ExportOptions exportOptions,
                            Database database)
    {
        SourceProvider = sourceProvider;
        SourceConnectionString = sourceConnectionString;
        TargetProvider = targetProvider;
        TargetConnectionString = targetConnectionString;
        ExportOptions = exportOptions;
        Database = database;
        
        if (TargetProvider.Name == ProviderNames.SQLSERVER)
            ExportOptions.ProviderSpecific = TargetProvider.HasFeature(ProviderFeatures.IsFileBased);
    }

    public DataProvider SourceProvider { get; }

    public string SourceConnectionString { get; }

    public DataProvider TargetProvider { get; }

    public string TargetConnectionString { get; }

    public ExportOptions ExportOptions { get; }

    public Database Database { get; }
}