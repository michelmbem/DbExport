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
    public DataProvider SourceProvider => sourceProvider;

    public string SourceConnectionString => sourceConnectionString;

    public DataProvider TargetProvider => targetProvider;

    public string TargetConnectionString => targetConnectionString;
    
    public ExportOptions ExportOptions => exportOptions;

    public Database Database => database;
}