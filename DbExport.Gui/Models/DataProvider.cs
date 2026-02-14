using System.Linq;
using DbExport.Providers;

namespace DbExport.Gui.Models;

public sealed class DataProvider(
    string name,
    string description,
    bool supportsTrustedConnection,
    ConnectionStringBuilder connectionStringBuilder,
    string? databaseListQuery = null)
{
#if WINDOWS
    private const string ACCESS_DATABASE_FILE_PATTERN = "Microsoft Access Database File (*.mdb;*.accdb)|*.mdb;*.accdb|All Files (*.*)|*.*";
#endif
    private const string SQLITE_DATABASE_FILE_PATTERN = "SQLite Database (*.db)|*.db|All Files (*.*)|*.*";
    private const string SQLSERVER_DATABASE_LIST_QUERY = "EXEC sp_databases";
    private const string MYSQL_DATABASE_LIST_QUERY = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA ORDER BY SCHEMA_NAME";
    private const string POSTGRESQL_DATABASE_LIST_QUERY = "SELECT datname FROM pg_catalog.pg_database ORDER BY datname";
    
    public static DataProvider[] All { get; } =
    [
#if WINDOWS
        new(ProviderNames.ACCESS, "Microsoft Access", false, new OleDbConnectionStringBuilder(), ACCESS_DATABASE_FILE_PATTERN),
#endif
        new(ProviderNames.SQLSERVER, "Microsoft SQL Server", true, new SqlConnectionStringBuilder(), SQLSERVER_DATABASE_LIST_QUERY),
        new(ProviderNames.ORACLE, "Oracle Database", true, new OracleConnectionStringBuilder()),
        new(ProviderNames.MYSQL, "MySQL", false, new MySqlConnectionStringBuilder(), MYSQL_DATABASE_LIST_QUERY),
        new(ProviderNames.POSTGRESQL, "PostgreSQL", false, new NpgsqlConnectionStringBuilder(), POSTGRESQL_DATABASE_LIST_QUERY),
        new(ProviderNames.SQLITE, "SQLite 3", false, new SQLiteConnectionStringBuilder(), SQLITE_DATABASE_FILE_PATTERN)
    ];

    public string Name { get; } = name;
    
    public string Description { get; } = description;
    
    public bool SupportsTrustedConnection { get; } = supportsTrustedConnection;
    
    public ConnectionStringBuilder ConnectionStringBuilder { get; } = connectionStringBuilder;
    
    public string? DatabaseListQuery { get; } = databaseListQuery;

    public static DataProvider? Get(string name) => All.FirstOrDefault(provider => provider.Name == name);
}