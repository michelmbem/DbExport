using System.Text;

namespace DbExport.Gui.Models;

public interface IConnectionStringBuilder
{
    string Build(string dataSource, int? portNumber, string? database,
                 bool trustedConnection, string? username, string? password);
}

#if WINDOWS
public class OleDbConnectionStringBuilder : IConnectionStringBuilder
{
    public string Build(string dataSource, int? portNumber, string? database,
                        bool trustedConnection, string? username, string? password)
    {
        var sb = new StringBuilder("Provider=Microsoft.ACE.OLEDB.12.0");

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

public class SqlConnectionStringBuilder : IConnectionStringBuilder
{
    public string Build(string dataSource, int? portNumber, string? database,
                        bool trustedConnection, string? username, string? password)
    {
        var sb = new StringBuilder($"Data Source={dataSource}");
        
        if (portNumber.HasValue)
            sb.Append($",{portNumber.Value}");
        
        if (!string.IsNullOrWhiteSpace(database))
            sb.Append($";Initial Catalog={database}");
        
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

public class OracleConnectionStringBuilder : IConnectionStringBuilder
{
    public string Build(string dataSource, int? portNumber, string? database,
                        bool trustedConnection, string? username, string? password)
    {
        var sb = new StringBuilder($"Data Source={dataSource}");
        
        if (portNumber.HasValue)
            sb.Append($":{portNumber.Value}");
        
        if (!string.IsNullOrWhiteSpace(database))
            sb.Append($"/{database}");
        
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

public class MySqlConnectionStringBuilder : IConnectionStringBuilder
{
    public string Build(string dataSource, int? portNumber, string? database,
                        bool trustedConnection, string? username, string? password)
    {
        var sb = new StringBuilder($"Server={dataSource}");
        if (portNumber.HasValue) sb.Append($";Port={portNumber.Value}");
        if (!string.IsNullOrWhiteSpace(database)) sb.Append($";Database={database}");
        if (!string.IsNullOrWhiteSpace(username)) sb.Append($";Uid={username}");
        if (!string.IsNullOrWhiteSpace(password)) sb.Append($";Pwd={password}");
        
        return sb.ToString();
    }
}

public class NpgsqlConnectionStringBuilder : IConnectionStringBuilder
{
    public string Build(string dataSource, int? portNumber, string? database,
                        bool trustedConnection, string? username, string? password)
    {
        var sb = new StringBuilder($"Host={dataSource}");
        if (portNumber.HasValue) sb.Append($";Port={portNumber.Value}");
        if (!string.IsNullOrWhiteSpace(database)) sb.Append($";Database={database}");
        if (!string.IsNullOrWhiteSpace(username)) sb.Append($";Username={username}");
        if (!string.IsNullOrWhiteSpace(password)) sb.Append($";Password={password}");
        
        return sb.ToString();
    }
}

public class SQLiteConnectionStringBuilder : IConnectionStringBuilder
{
    public string Build(string dataSource, int? portNumber, string? database,
                        bool trustedConnection, string? username, string? password)
    {
        var sb = new StringBuilder($"Data Source={dataSource};Version=3");
        if (!string.IsNullOrWhiteSpace(password)) sb.Append($";Password={password}");
        
        return sb.ToString();
    }
}