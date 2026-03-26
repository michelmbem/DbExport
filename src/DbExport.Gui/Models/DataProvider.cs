using System;
using System.Collections.Generic;
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
    SupportsSchemas = 16,
    UsesPathAsDatabaseName = 32,

    Access = IsFileBased,
    LocalDB = IsFileBased | SupportsTrustedConnection | SupportsSchemas,
    SqlServer = SupportsTrustedConnection | SupportsDatabaseCreation | SupportsDDL | SupportsSchemas,
    Oracle = SupportsTrustedConnection | SupportsDDL | SupportsSchemas,
    MySql = SupportsDatabaseCreation | SupportsDDL,
    PostgreSQL = SupportsDatabaseCreation | SupportsDDL | SupportsSchemas,
    Firebird = SupportsDatabaseCreation | SupportsDDL | UsesPathAsDatabaseName,
    SQLite = IsFileBased | SupportsDDL | SupportsSchemas,
    DB2 = SupportsDDL | SupportsSchemas
}

public sealed class DataProvider(
    string name,
    string description,
    ProviderFeatures features,
    IConnectionStringFactory connectionStringFactory,
    string? databaseListQuery = null)
{
    private const string ACCESS_DATABASE_FILE_PATTERN = "Microsoft Access Database (*.accdb;*.mdb)|*.accdb;*.mdb";
    private const string LOCALDB_DATABASE_FILE_PATTERN = "SQL Server Database (*.mdf)|*.mdf";
    private const string SQLITE_DATABASE_FILE_PATTERN = "SQLite Database (*.db;*.sqlite)|*.db;*.sqlite";
    private const string SQLSERVER_DATABASE_LIST_QUERY = "EXEC sp_databases";
    private const string ORACLE_DATABASE_LIST_QUERY = "SELECT * FROM GLOBAL_NAME";
    private const string MYSQL_DATABASE_LIST_QUERY = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA ORDER BY SCHEMA_NAME";
    private const string POSTGRESQL_DATABASE_LIST_QUERY = "SELECT datname FROM pg_catalog.pg_database ORDER BY datname";

    static DataProvider()
    {
        List<DataProvider> allProviders = [];

        if (OperatingSystem.IsWindows())
        {
            allProviders.Add(new(ProviderNames.ACCESS, "Microsoft Access", ProviderFeatures.Access, new OleDbConnectionStringFactory(), ACCESS_DATABASE_FILE_PATTERN));
            allProviders.Add(new(ProviderNames.SQLSERVER, "Microsoft SQL Server LocalDB", ProviderFeatures.LocalDB, new LocalDBConnectionStringFactory(), LOCALDB_DATABASE_FILE_PATTERN));
        }
        
        allProviders.Add(new(ProviderNames.SQLSERVER, "Microsoft SQL Server", ProviderFeatures.SqlServer, new SqlConnectionStringFactory(), SQLSERVER_DATABASE_LIST_QUERY));
        allProviders.Add(new(ProviderNames.ORACLE, "Oracle Database", ProviderFeatures.Oracle, new OracleConnectionStringFactory(), ORACLE_DATABASE_LIST_QUERY));
        allProviders.Add(new(ProviderNames.MYSQL, "MySQL", ProviderFeatures.MySql, new MySqlConnectionStringFactory(), MYSQL_DATABASE_LIST_QUERY));
        allProviders.Add(new(ProviderNames.POSTGRESQL, "PostgreSQL", ProviderFeatures.PostgreSQL, new NpgsqlConnectionStringFactory(), POSTGRESQL_DATABASE_LIST_QUERY));
        allProviders.Add(new(ProviderNames.FIREBIRD, "Firebird", ProviderFeatures.Firebird, new FirebirdConnectionStringFactory()));
        allProviders.Add(new(ProviderNames.SQLITE, "SQLite", ProviderFeatures.SQLite, new SQLiteConnectionStringFactory(), SQLITE_DATABASE_FILE_PATTERN));

        if (Utility.IsDb2Supported())
            allProviders.Add(new(ProviderNames.DB2, "IBM DB2 (experimental)", ProviderFeatures.DB2, new DB2ConnectionStringFactory()));

        All = [..allProviders];
    }
    
    public static DataProvider[] All { get; }

    public string Name { get; } = name;
    
    public string Description { get; } = description;
    
    public ProviderFeatures Features { get; } = features;
    
    public IConnectionStringFactory ConnectionStringFactory { get; } = connectionStringFactory;
    
    public string? DatabaseListQuery { get; } = databaseListQuery;
    
    public bool HasFeature(ProviderFeatures feature) => Features.HasFlag(feature);
}