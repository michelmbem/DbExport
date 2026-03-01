using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace DbExport.Providers.SqlClient;

/// <summary>
/// Represents an implementation of the <see cref="IScriptExecutor"/> interface for executing SQL scripts against
/// a SQL Server database using a given connection string.
/// </summary>
/// <remarks>
/// This class processes and executes SQL scripts, handling specific constructs such as database creation,
/// database switching (USE statements), and user-defined type creation. The scripts are parsed and executed
/// in multiple steps, ensuring proper handling of dependencies and execution order.
/// </remarks>
/// <example>
/// This class is intended for use with SQL Server environments and supports SQL scripts containing
/// CREATE DATABASE, USE, and CREATE TYPE commands.
/// </example>
public partial class SqlScripExecutor : IScriptExecutor
{
    public void Execute(string connectionString, string script)
    {
        using var conn = new SqlConnection(connectionString);
        using var cmd = conn.CreateCommand();

        script = DelimiterRegex().Replace(script, ";\n");
        conn.Open();

        var createDbRegex = CreateDbRegex();
        var match = createDbRegex.Match(script);

        if (match.Success)
        {
            cmd.CommandText = match.Value.TrimEnd()[..^1];
            cmd.ExecuteNonQuery();

            script = createDbRegex.Replace(script, string.Empty);

            var dbName = match.Groups[1].Value;
            var useDbRegex = new Regex($@"\bUSE\s+({Regex.Escape(dbName)})\s*;\s*", RegexOptions.IgnoreCase);

            if (useDbRegex.IsMatch(script))
            {
                conn.ChangeDatabase(Unescape(dbName));
                script = useDbRegex.Replace(script, string.Empty);
            }
        }

        var createTypeRegex = CreateTypeRegex();

        foreach (Match match2 in createTypeRegex.Matches(script))
        {
            cmd.CommandText = match2.Value.TrimEnd()[..^1];
            cmd.ExecuteNonQuery();
        }

        script = createTypeRegex.Replace(script, string.Empty);

        cmd.CommandText = script;
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Removes surrounding square brackets from the input string if they exist.
    /// </summary>
    /// <param name="name">The string to unescape, potentially surrounded by square brackets.</param>
    /// <returns>
    /// The input string without surrounding square brackets, or the original string if no brackets are present.
    /// </returns>
    private static string Unescape(string name) => name.StartsWith('[') ? name[1..^1] : name;

    [GeneratedRegex(@"(?:\s|\r)+GO\s*(?:\r|\n)", RegexOptions.IgnoreCase)]
    private static partial Regex DelimiterRegex();

    [GeneratedRegex(@"\bCREATE\s+DATABASE\s+([\[\]\w\.]+);\s*", RegexOptions.IgnoreCase)]
    private static partial Regex CreateDbRegex();

    [GeneratedRegex(@"\bCREATE\s+TYPE\s+[\[\]\w\.]+\s+FROM\s+[^;]+;\s*", RegexOptions.IgnoreCase)]
    private static partial Regex CreateTypeRegex();
}
