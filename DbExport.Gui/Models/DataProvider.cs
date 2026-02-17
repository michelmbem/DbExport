using System;
using System.Linq;
using DbExport.Providers;

namespace DbExport.Gui.Models;

[Flags]
public enum ProviderFeatures
{
    None = 0,
    IsFileBased = 1,
    SupportsTrustedConnection = 2,
    SupportsDatabaseCreation = 4,
    SupportsDDL = 8,
    SupportsScriptExecution = 16,
    
    Access = IsFileBased | SupportsDatabaseCreation | SupportsScriptExecution,
    SqlServer = SupportsTrustedConnection | SupportsDatabaseCreation | SupportsDDL| SupportsScriptExecution,
    LocalDB = IsFileBased | SupportsDDL| SupportsScriptExecution,
    Oracle = SupportsTrustedConnection | SupportsDDL,
    MySql = SupportsDatabaseCreation | SupportsDDL| SupportsScriptExecution,
    PostgreSql = SupportsDDL | SupportsScriptExecution,
    SQLite = IsFileBased | SupportsDatabaseCreation | SupportsDDL | SupportsScriptExecution ,
}

public sealed class DataProvider(
    string name,
    string description,
    ProviderFeatures features,
    IConnectionStringBuilder connectionStringBuilder,
    string? databaseListQuery = null)
{
#if WINDOWS
    private const string ACCESS_DATABASE_FILE_PATTERN = "Microsoft Access Database (*.accdb;*.mdb)|*.accdb;*.mdb";
    private const string SQLSERVER_DATABASE_FILE_PATTERN = "SQL Server Database (*.mdf)|*.mdf";
#endif
    private const string SQLITE_DATABASE_FILE_PATTERN = "SQLite Database (*.db)|*.db";
    private const string SQLSERVER_DATABASE_LIST_QUERY = "EXEC sp_databases";
    private const string MYSQL_DATABASE_LIST_QUERY = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA ORDER BY SCHEMA_NAME";
    private const string POSTGRESQL_DATABASE_LIST_QUERY = "SELECT datname FROM pg_catalog.pg_database ORDER BY datname";
    
    public static DataProvider[] All { get; } =
    [
#if WINDOWS
        new(ProviderNames.ACCESS, "Microsoft Access", ProviderFeatures.Access, new OleDbConnectionStringBuilder(), ACCESS_DATABASE_FILE_PATTERN),
        new(ProviderNames.SQLSERVER, "SQL Server LocalDB", ProviderFeatures.SqlServer, new LocalDBConnectionStringBuilder(), SQLSERVER_DATABASE_FILE_PATTERN),
#endif
        new(ProviderNames.SQLSERVER, "Microsoft SQL Server", ProviderFeatures.SqlServer, new SqlConnectionStringBuilder(), SQLSERVER_DATABASE_LIST_QUERY),
        new(ProviderNames.ORACLE, "Oracle Database", ProviderFeatures.Oracle, new OracleConnectionStringBuilder()),
        new(ProviderNames.MYSQL, "MySQL", ProviderFeatures.MySql, new MySqlConnectionStringBuilder(), MYSQL_DATABASE_LIST_QUERY),
        new(ProviderNames.POSTGRESQL, "PostgreSQL", ProviderFeatures.PostgreSql, new NpgsqlConnectionStringBuilder(), POSTGRESQL_DATABASE_LIST_QUERY),
        new(ProviderNames.SQLITE, "SQLite 3", ProviderFeatures.SQLite, new SQLiteConnectionStringBuilder(), SQLITE_DATABASE_FILE_PATTERN)
    ];

    public string Name { get; } = name;
    
    public string Description { get; } = description;
    
    public ProviderFeatures Features { get; } = features;
    
    public IConnectionStringBuilder ConnectionStringBuilder { get; } = connectionStringBuilder;
    
    public string? DatabaseListQuery { get; } = databaseListQuery;

    public static DataProvider? Get(string name) => All.FirstOrDefault(provider => provider.Name == name);
    
    public bool HasFeature(ProviderFeatures feature) => Features.HasFlag(feature);
}