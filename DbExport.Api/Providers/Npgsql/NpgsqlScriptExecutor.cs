using System.Text.RegularExpressions;
using Npgsql;

namespace DbExport.Providers.Npgsql;

public partial class NpgsqlScriptExecutor : BatchScriptExecutor
{
    public NpgsqlScriptExecutor() : base(ProviderNames.POSTGRESQL) { }

    public override void Execute(string connectionString, string script)
    {
        var match = CreateDbRegex().Match(script);

        if (match.Success)
        {
            var createDb = match.Value[..^1];
            var dbName = match.Groups[1].Value;

            using (var helper = new SqlHelper(ProviderNames.POSTGRESQL, connectionString))
                helper.Execute(createDb);

            var builder = new NpgsqlConnectionStringBuilder(connectionString) { Database = dbName.ToLower() };
            connectionString = builder.ToString();

            script = script[(match.Index + match.Length)..];
            match = new Regex($@"\\[Cc]\s+({dbName})\s*;").Match(script);

            if (match.Success)
                script = script[(match.Index + match.Length)..];
        }

        base.Execute(connectionString, script);
    }

    [GeneratedRegex(@"\bCREATE\s+DATABASE\s+(\w[\w\d]*)[^;]*;", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex CreateDbRegex();
}
