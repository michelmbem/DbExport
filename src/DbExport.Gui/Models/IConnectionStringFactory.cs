#if WINDOWS
using System.Data.OleDb;
#endif
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using MySqlConnector;
using Npgsql;
using FirebirdSql.Data.FirebirdClient;
using System.Data.SQLite;

namespace DbExport.Gui.Models;

public interface IConnectionStringFactory
{
    string Build(string dataSource, int? portNumber, string? database,
                 bool trustedConnection, string? username, string? password);
}

#if WINDOWS
public class OleDbConnectionStringFactory : IConnectionStringFactory
{
#pragma warning disable CA1416 // Verify platform compatibility
    public string Build(string dataSource, int? portNumber, string? database,
                        bool trustedConnection, string? username, string? password)
    {
        var builder = new OleDbConnectionStringBuilder
        {
            Provider = "Microsoft.ACE.OLEDB.12.0",
            DataSource = dataSource
        };

        if (!string.IsNullOrWhiteSpace(username))
            builder["User ID"] = username;

        if (!string.IsNullOrWhiteSpace(password))
        {
            builder["Password"] = password;
            builder["Persist Security Info"] = true;
        }

        return builder.ConnectionString;
    }
#pragma warning restore CA1416 // Verify platform compatibility
}

public class LocalDBConnectionStringFactory : IConnectionStringFactory
{
    public string Build(string dataSource, int? portNumber, string? database,
        bool trustedConnection, string? username, string? password)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = @"(LocalDB)\MSSQLLocalDB",
            IntegratedSecurity = true,
            AttachDBFilename = dataSource
        };

        return builder.ConnectionString;
    }
}
#endif

public class SqlConnectionStringFactory : IConnectionStringFactory
{
    public string Build(string dataSource, int? portNumber, string? database,
                        bool trustedConnection, string? username, string? password)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = portNumber.HasValue
                ? $"{dataSource},{portNumber.Value}"
                : dataSource,
            Encrypt = true,
            TrustServerCertificate = true
        };

        if (!string.IsNullOrWhiteSpace(database))
            builder.InitialCatalog = database;

        if (trustedConnection)
        {
            builder.IntegratedSecurity = true;
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(username))
                builder.UserID = username;

            if (!string.IsNullOrWhiteSpace(password))
                builder.Password = password;
        }

        return builder.ConnectionString;
    }
}

public class OracleConnectionStringFactory : IConnectionStringFactory
{
    public string Build(string dataSource, int? portNumber, string? database,
                        bool trustedConnection, string? username, string? password)
    {
        var dataSourceString = portNumber.HasValue
            ? $"{dataSource}:{portNumber}"
            : dataSource;

        if (!string.IsNullOrWhiteSpace(database))
            dataSourceString += $"/{database}";

        var builder = new OracleConnectionStringBuilder
        {
            DataSource = dataSourceString
        };

        if (trustedConnection)
        {
            builder.UserID = "/";
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(username))
                builder.UserID = username;

            if (!string.IsNullOrWhiteSpace(password))
                builder.Password = password;
        }

        return builder.ConnectionString;
    }
}

public class MySqlConnectionStringFactory : IConnectionStringFactory
{
    public string Build(string dataSource, int? portNumber, string? database,
                        bool trustedConnection, string? username, string? password)
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = dataSource
        };

        if (portNumber.HasValue)
            builder.Port = (uint)portNumber.Value;

        if (!string.IsNullOrWhiteSpace(database))
            builder.Database = database;

        if (!string.IsNullOrWhiteSpace(username))
            builder.UserID = username;

        if (!string.IsNullOrWhiteSpace(password))
            builder.Password = password;

        return builder.ConnectionString;
    }
}

public class NpgsqlConnectionStringFactory : IConnectionStringFactory
{
    public string Build(string dataSource, int? portNumber, string? database,
                        bool trustedConnection, string? username, string? password)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = dataSource
        };

        if (portNumber.HasValue)
            builder.Port = portNumber.Value;

        if (!string.IsNullOrWhiteSpace(database))
            builder.Database = database;

        if (!string.IsNullOrWhiteSpace(username))
            builder.Username = username;

        if (!string.IsNullOrWhiteSpace(password))
            builder.Password = password;

        return builder.ConnectionString;
    }
}

public class FirebirdConnectionStringFactory : IConnectionStringFactory
{
    public string Build(string dataSource, int? portNumber, string? database,
                        bool trustedConnection, string? username, string? password)
    {
        var builder = new FbConnectionStringBuilder
        {
            DataSource = dataSource,
            Dialect = 3,
            Charset = "UTF8"
        };

        if (portNumber.HasValue)
            builder.Port = portNumber.Value;

        if (!string.IsNullOrWhiteSpace(database))
            builder.Database = database;

        if (!string.IsNullOrWhiteSpace(username))
            builder.UserID = username;

        if (!string.IsNullOrWhiteSpace(password))
            builder.Password = password;

        return builder.ConnectionString;
    }
}

public class SQLiteConnectionStringFactory : IConnectionStringFactory
{
    public string Build(string dataSource, int? portNumber, string? database,
                        bool trustedConnection, string? username, string? password)
    {
        var builder = new SQLiteConnectionStringBuilder
        {
            DataSource = dataSource,
            Version = 3
        };

        if (!string.IsNullOrWhiteSpace(password))
            builder.Password = password;

        return builder.ConnectionString;
    }
}