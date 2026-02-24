using System;
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
    
    Access = IsFileBased,
    LocalDB = Access | SupportsTrustedConnection,
    SqlServer = SupportsTrustedConnection | SupportsDatabaseCreation | SupportsDDL,
    Oracle = SupportsTrustedConnection | SupportsDDL,
    MySql = SupportsDatabaseCreation | SupportsDDL,
    SQLite = Access | SupportsDDL
}

public sealed class DataProvider(
    string name,
    string description,
    ProviderFeatures features,
    IConnectionStringFactory connectionStringFactory,
    string? databaseListQuery = null)
{
#if WINDOWS
    private const string ACCESS_DATABASE_FILE_PATTERN = "Microsoft Access Database (*.accdb;*.mdb)|*.accdb;*.mdb";
    private const string LOCALDB_DATABASE_FILE_PATTERN = "SQL Server Database (*.mdf)|*.mdf";
#endif
    private const string SQLITE_DATABASE_FILE_PATTERN = "SQLite Database (*.db)|*.db";
    private const string SQLSERVER_DATABASE_LIST_QUERY = "EXEC sp_databases";
    private const string ORACLE_DATABASE_LIST_QUERY = "SELECT * FROM GLOBAL_NAME";
    private const string MYSQL_DATABASE_LIST_QUERY = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA ORDER BY SCHEMA_NAME";
    private const string POSTGRESQL_DATABASE_LIST_QUERY = "SELECT datname FROM pg_catalog.pg_database ORDER BY datname";
    
    public static DataProvider[] All { get; } =
    [
#if WINDOWS
        new(ProviderNames.ACCESS, "Microsoft Access", ProviderFeatures.Access, new OleDbConnectionStringFactory(), ACCESS_DATABASE_FILE_PATTERN),
        new(ProviderNames.SQLSERVER, "Microsoft SQL Server LocalDB", ProviderFeatures.LocalDB, new LocalDBConnectionStringFactory(), LOCALDB_DATABASE_FILE_PATTERN),
#endif
        new(ProviderNames.SQLSERVER, "Microsoft SQL Server", ProviderFeatures.SqlServer, new SqlConnectionStringFactory(), SQLSERVER_DATABASE_LIST_QUERY),
        new(ProviderNames.ORACLE, "Oracle Database", ProviderFeatures.Oracle, new OracleConnectionStringFactory(), ORACLE_DATABASE_LIST_QUERY),
        new(ProviderNames.MYSQL, "MySQL", ProviderFeatures.MySql, new MySqlConnectionStringFactory(), MYSQL_DATABASE_LIST_QUERY),
        new(ProviderNames.POSTGRESQL, "PostgreSQL", ProviderFeatures.MySql, new NpgsqlConnectionStringFactory(), POSTGRESQL_DATABASE_LIST_QUERY),
        new(ProviderNames.FIREBIRD, "Firebird", ProviderFeatures.MySql, new FirebirdConnectionStringFactory()),
        new(ProviderNames.SQLITE, "SQLite 3", ProviderFeatures.SQLite, new SQLiteConnectionStringFactory(), SQLITE_DATABASE_FILE_PATTERN)
    ];

    public string Name { get; } = name;
    
    public string Description { get; } = description;
    
    public ProviderFeatures Features { get; } = features;
    
    public IConnectionStringFactory ConnectionStringFactory { get; } = connectionStringFactory;
    
    public string? DatabaseListQuery { get; } = databaseListQuery;
    
    public bool HasFeature(ProviderFeatures feature) => Features.HasFlag(feature);
}