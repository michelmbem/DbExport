using System.Text.RegularExpressions;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Isql;

namespace DbExport.Providers.Firebird;

/// <summary>
/// Provides functionality to execute Firebird SQL scripts against a Firebird database.
/// </summary>
/// <remarks>
/// This class processes SQL scripts, identifies and handles specific commands such as database creation,
/// and executes the remaining statements using the Firebird ADO.NET provider.
/// If a "CREATE DATABASE" statement is identified within the script, the database connection string is updated,
/// the database is created, and the statement is removed from the script before execution.
/// </remarks>
public partial class FirebirdScriptExecutor : IScriptExecutor
{
    public void Execute(string connectionString, string script)
    {
        var createDbRegex = CreateDbRegex();
        var match = createDbRegex.Match(script);

        if (match.Success)
        {
            var dbName = match.Groups[1].Value;
            var builder = new FbConnectionStringBuilder(connectionString) { Database = dbName };

            connectionString = builder.ToString();
            FbConnection.CreateDatabase(connectionString, FirebirdOptions.PageSize,
                                        FirebirdOptions.ForcedWrites, FirebirdOptions.Overwrite);

            script = createDbRegex.Replace(script, string.Empty);
        }

        using var conn = new FbConnection(connectionString);
        using var cmd = conn.CreateCommand();

        var fbScript = new FbScript(script);
        fbScript.Parse();

        conn.Open();

        foreach (var statement in fbScript.Results)
        {
            cmd.CommandText = statement.Text;
            cmd.ExecuteNonQuery();
        }
    }

    [GeneratedRegex(@"\bCREATE\s+DATABASE\s+'([^']+)'[^;]*;\s*", RegexOptions.IgnoreCase)]
    private static partial Regex CreateDbRegex();
}
