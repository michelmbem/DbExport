using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace DbExport.Providers.SqlClient;

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

    private static string Unescape(string name) => name.StartsWith('[') ? name[1..^1] : name;

    [GeneratedRegex(@"(?:\s|\r)+GO\s*(?:\r|\n)", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex DelimiterRegex();

    [GeneratedRegex(@"\bCREATE\s+DATABASE\s+([\[\]\w\.]+);\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex CreateDbRegex();

    [GeneratedRegex(@"\bCREATE\s+TYPE\s+[\[\]\w\.]+\s+FROM\s+[^;]+;\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex CreateTypeRegex();
}
