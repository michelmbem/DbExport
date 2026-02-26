namespace DbExport.Providers;

/// <summary>
/// A static class that contains constant string values representing the names of supported database providers.
/// These names are typically used to identify the specific database provider when configuring database connections
/// or performing database operations.
/// </summary>
public static class ProviderNames
{
    public const string ACCESS = "System.Data.OleDb";
    public const string SQLSERVER = "Microsoft.Data.SqlClient";
    public const string ORACLE = "Oracle.ManagedDataAccess.Client";
    public const string MYSQL = "MySql.Data.MySqlClient";
    public const string POSTGRESQL = "Npgsql";
    public const string FIREBIRD = "FirebirdSql.Data.FirebirdClient";
    public const string SQLITE = "System.Data.SQLite";
}