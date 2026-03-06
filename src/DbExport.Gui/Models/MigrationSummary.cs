using DbExport.Schema;

namespace DbExport.Gui.Models;

public sealed class MigrationSummary(
    DataProvider sourceProvider,
    string sourceConnectionString,
    DataProvider targetProvider,
    string targetConnectionString,
    ExportOptions exportOptions,
    Database database)
{
    public DataProvider SourceProvider { get; } = sourceProvider;

    public string SourceConnectionString { get; } = sourceConnectionString;

    public DataProvider TargetProvider { get; } = targetProvider;

    public string TargetConnectionString { get; } = targetConnectionString;

    public ExportOptions ExportOptions { get; } = exportOptions;

    public Database Database { get; } = database;
}