using System.Text.RegularExpressions;
using MySqlConnector;

namespace DbExport.Providers.MySqlClient;

/// <summary>
/// Provides functionality for executing MySQL scripts, including management of database creation
/// commands and connection string updates for the target database. Extends the functionality of
/// the BatchScriptExecutor class for MySQL-specific use cases.
/// </summary>
public partial class MySqlScriptExecutor() : BatchScriptExecutor(ProviderNames.MYSQL)
{
    public override void Execute(string connectionString, string script)
    {
        var createDbRegex = CreateDbRegex();
        var match = createDbRegex.Match(script);

        if (match.Success)
        {
            var createDb = match.Value.TrimEnd()[..^1];
            var dbName = match.Groups[1].Value;

            using (var helper = new SqlHelper(ProviderNames.MYSQL, connectionString))
                helper.Execute(createDb);

            var builder = new MySqlConnectionStringBuilder(connectionString) { Database = Unescape(dbName) };
            connectionString = builder.ToString();
            
            script = createDbRegex.Replace(script, string.Empty);
            
            var useDbRegex = new Regex($@"\bUSE\s+({Regex.Escape(dbName)})\s*;\s*", RegexOptions.IgnoreCase);
            script = useDbRegex.Replace(script, string.Empty);
        }

        base.Execute(connectionString, script);
    }

    /// <summary>
    /// Removes enclosing backticks from a database or table name if present.
    /// </summary>
    /// <param name="name">The name of the database or table, which may be enclosed in backticks.</param>
    /// <returns>The unescaped name without backticks, or the original name if backticks are not present.</returns>
    private static string Unescape(string name) => name.StartsWith('`') ? name[1..^1] : name;

    [GeneratedRegex(@"\bCREATE\s+DATABASE\s+([\w`]+)[^;]*;\s*", RegexOptions.IgnoreCase)]
    private static partial Regex CreateDbRegex();
}
