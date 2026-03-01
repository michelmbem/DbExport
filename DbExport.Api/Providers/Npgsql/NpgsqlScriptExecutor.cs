using System.Text.RegularExpressions;
using Npgsql;

namespace DbExport.Providers.Npgsql;

/// <summary>
/// Represents a script executor specifically designed for executing Npgsql (PostgreSQL)
/// database scripts. This class extends the <see cref="BatchScriptExecutor"/> and overrides
/// its behavior to handle PostgreSQL-specific use cases, such as 'CREATE DATABASE' commands
/// and connection string adjustments for the target database.
/// </summary>
public partial class NpgsqlScriptExecutor() : BatchScriptExecutor(ProviderNames.POSTGRESQL)
{
    public override void Execute(string connectionString, string script)
    {
        var createDbRegex = CreateDbRegex();
        var match = createDbRegex.Match(script);

        if (match.Success)
        {
            var createDb = match.Value.TrimEnd()[..^1];
            var dbName = match.Groups[1].Value;

            using (var helper = new SqlHelper(ProviderNames.POSTGRESQL, connectionString))
                helper.Execute(createDb);

            var builder = new NpgsqlConnectionStringBuilder(connectionString) { Database = dbName.ToLower() };
            connectionString = builder.ToString();
            
            script = createDbRegex.Replace(script, string.Empty);
            
            var connectRegex = new Regex($@"\\c\s+({Regex.Escape(dbName)})\s*;\s*", RegexOptions.IgnoreCase);
            script = connectRegex.Replace(script, string.Empty);
        }

        base.Execute(connectionString, script);
    }

    [GeneratedRegex(@"\bCREATE\s+DATABASE\s+(\w+)[^;]*;\s*", RegexOptions.IgnoreCase)]
    private static partial Regex CreateDbRegex();
}
