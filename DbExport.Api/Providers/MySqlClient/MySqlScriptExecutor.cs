using System.Text.RegularExpressions;
using MySqlConnector;

namespace DbExport.Providers.MySqlClient;

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

    private static string Unescape(string name) => name.StartsWith('`') ? name[1..^1] : name;

    [GeneratedRegex(@"\bCREATE\s+DATABASE\s+([\w`]+)[^;]*;\s*", RegexOptions.IgnoreCase)]
    private static partial Regex CreateDbRegex();
}
