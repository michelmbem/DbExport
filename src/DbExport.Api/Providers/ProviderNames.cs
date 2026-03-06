namespace DbExport.Providers;

/// <summary>
/// A static class that contains constant string values representing the names of supported database providers.
/// These names are typically used to identify the specific database provider when configuring database connections
/// or performing database operations.
/// </summary>
public static class ProviderNames
{
    /// <summary>
    /// A constant string representing the database provider name for Microsoft Access.
    /// </summary>
    /// <remarks>
    /// This value is typically used in database operations and configuration
    /// when connecting to an Access database using the OleDb provider.
    /// </remarks>
    public const string ACCESS = "System.Data.OleDb";

    /// <summary>
    /// A constant string representing the database provider name for Microsoft SQL Server.
    /// </summary>
    /// <remarks>
    /// This value is commonly used for identifying Microsoft SQL Server as the target database
    /// in various database operations, configurations, or code generation tasks.
    /// </remarks>
    public const string SQLSERVER = "Microsoft.Data.SqlClient";

    /// <summary>
    /// A constant string representing the database provider name for Oracle using the Oracle Managed Data Access client.
    /// </summary>
    /// <remarks>
    /// This value is commonly used for database operations and configuration when connecting to an Oracle database
    /// via the Oracle.ManagedDataAccess.Client library.
    /// </remarks>
    public const string ORACLE = "Oracle.ManagedDataAccess.Client";

    /// <summary>
    /// A constant string representing the database provider name for MySQL.
    /// </summary>
    /// <remarks>
    /// This value is typically used in database operations and configuration
    /// when connecting to a MySQL database using the MySqlConnector library.
    /// </remarks>
    public const string MYSQL = "MySqlConnector";

    /// <summary>
    /// A constant string representing the database provider name for PostgreSQL.
    /// </summary>
    /// <remarks>
    /// This value is commonly used in database operations and configuration
    /// when connecting to a PostgreSQL database using the Npgsql provider.
    /// </remarks>
    public const string POSTGRESQL = "Npgsql";

    /// <summary>
    /// A constant string representing the database provider name for Firebird.
    /// </summary>
    /// <remarks>
    /// This value is used in database operations and configuration
    /// when connecting to a Firebird database using the Firebird ADO.NET provider.
    /// </remarks>
    public const string FIREBIRD = "FirebirdSql.Data.FirebirdClient";

    /// <summary>
    /// A constant string representing the database provider name for SQLite.
    /// </summary>
    /// <remarks>
    /// This value is commonly used in database-related operations and configuration
    /// for identifying or connecting to a SQLite database.
    /// </remarks>
    public const string SQLITE = "System.Data.SQLite";
}