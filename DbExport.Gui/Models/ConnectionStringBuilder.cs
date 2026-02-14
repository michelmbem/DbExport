using System.Text;

namespace DbExport.Gui.Models;

public abstract class ConnectionStringBuilder
{
    public abstract string Build(string dataSource, int? port, string? database,
                                 bool trustedConnection, string? username,
                                 string? password);
}

#if WINDOWS
public class OleDbConnectionStringBuilder : ConnectionStringBuilder
{
    public override string Build(string dataSource, int? port, string? database,
                                 bool trustedConnection, string? username,
                                 string? password)
    {
        var sb = new StringBuilder(dataSource.EndsWith(".mdb", StringComparison.OrdinalIgnoreCase)
                                       ? "Provider=Microsoft.Jet.OLEDB.4.0"
                                       : "Provider=Microsoft.ACE.OLEDB.12.0");

        sb.Append($";Data Source={dataSource}");
        
        if (!string.IsNullOrWhiteSpace(password))
        {
            sb.Append($";User ID={username}");
            sb.Append($";Password={password}");
            sb.Append(";Persist Security Info=True");
        }

        return sb.ToString();
    }
}
#endif

public class SqlConnectionStringBuilder : ConnectionStringBuilder
{
    public override string Build(string dataSource, int? port, string? database,
                                 bool trustedConnection, string? username,
                                 string? password)
    {
        var sb = new StringBuilder($"Data Source={dataSource}");
        if (port.HasValue) sb.Append($",{port.Value}");
        if (!string.IsNullOrWhiteSpace(database)) sb.Append($";Initial Catalog={database}");
        
        if (trustedConnection)
            sb.Append(";Trusted_Connection=True");
        else
        {
            sb.Append($";User ID={username}");
            sb.Append($";Password={password}");
        }
        
        sb.Append(";Encrypt=True;TrustServerCertificate=True");
        
        return sb.ToString();
    }
}

public class OracleConnectionStringBuilder : ConnectionStringBuilder
{
    public override string Build(string dataSource, int? port, string? database,
                                 bool trustedConnection, string? username,
                                 string? password)
    {
        var sb = new StringBuilder($"Data Source={dataSource}");
        if (port.HasValue) sb.Append($":{port.Value}");
        if (!string.IsNullOrWhiteSpace(database)) sb.Append($"/{database}");
        
        if (trustedConnection)
            sb.Append(";User ID=/");
        else
        {
            sb.Append($";User ID={username}");
            sb.Append($";Password={password}");
        }
        
        return sb.ToString();
    }
}

public class MySqlConnectionStringBuilder : ConnectionStringBuilder
{
    public override string Build(string dataSource, int? port, string? database,
                                 bool trustedConnection, string? username,
                                 string? password)
    {
        var sb = new StringBuilder($"Server={dataSource}");
        if (port.HasValue) sb.Append($";Port={port.Value}");
        if (!string.IsNullOrWhiteSpace(database)) sb.Append($";Database={database}");
        if (!string.IsNullOrWhiteSpace(username)) sb.Append($";Uid={username}");
        if (!string.IsNullOrWhiteSpace(password)) sb.Append($";Pwd={password}");
        
        return sb.ToString();
    }
}

public class NpgsqlConnectionStringBuilder : ConnectionStringBuilder
{
    public override string Build(string dataSource, int? port, string? database,
                                 bool trustedConnection, string? username,
                                 string? password)
    {
        var sb = new StringBuilder($"Host={dataSource}");
        if (port.HasValue) sb.Append($";Port={port.Value}");
        if (!string.IsNullOrWhiteSpace(database)) sb.Append($";Database={database}");
        if (!string.IsNullOrWhiteSpace(username)) sb.Append($";Username={username}");
        if (!string.IsNullOrWhiteSpace(password)) sb.Append($";Password={password}");
        
        return sb.ToString();
    }
}

public class SQLiteConnectionStringBuilder : ConnectionStringBuilder
{
    public override string Build(string dataSource, int? port, string? database,
                                 bool trustedConnection, string? username,
                                 string? password)
    {
        var sb = new StringBuilder($"Data Source={dataSource};Version=3");
        if (!string.IsNullOrWhiteSpace(password)) sb.Append($";Password={password}");
        
        return sb.ToString();
    }
}