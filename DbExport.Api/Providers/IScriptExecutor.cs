using System.Linq;
using System.Text.RegularExpressions;

namespace DbExport.Providers;

/// <summary>
/// Common interface to all components that can be used to execute SQL scripts against a database.
/// </summary>
public interface IScriptExecutor
{
    /// <summary>
    /// Executes the specified SQL script against a database using the provided connection string.
    /// </summary>
    /// <param name="connectionString">The connection string for the database connection.</param>
    /// <param name="script">A string containing the SQL script to be executed.</param>
    void Execute(string connectionString, string script);
}

/// <summary>
/// An implementation of IScriptExecutor that simply runs the script as a single command.
/// </summary>
/// <param name="providerName">The name of the database provider.</param>
public class DefaultScriptExecutor(string providerName) : IScriptExecutor
{
    public virtual void Execute(string connectionString, string script)
    {
        using var helper = new SqlHelper(providerName, connectionString);
        helper.Execute(script);
    }
}

/// <summary>
/// An implementation of IScriptExecutor that executes the specified SQL script against a database using batch execution
/// if supported by the provider, or executes each statement individually if batch execution is not supported.
/// </summary>
/// <param name="providerName">The name of the database provider.</param>
public partial class BatchScriptExecutor(string providerName) : DefaultScriptExecutor(providerName)
{
    public override void Execute(string connectionString, string script)
    {
        using var conn = Utility.GetConnection(providerName, connectionString);
        var statements = DelimiterRegex().Split(script)
                                         .Select(s => s.Trim())
                                         .Where(s => s.Length > 0);

        if (conn.CanCreateBatch)
        {
            var batch = conn.CreateBatch();

            foreach (var statement in statements)
            {
                var batchCmd = batch.CreateBatchCommand();
                batchCmd.CommandText = statement;
                batch.BatchCommands.Add(batchCmd);
            }

            conn.Open();
            batch.ExecuteNonQuery();
        }
        else
        {
            using var cmd = conn.CreateCommand();

            conn.Open();

            foreach (var statement in statements)
            {
                cmd.CommandText = statement;
                cmd.ExecuteNonQuery();
            }
        }
    }

    [GeneratedRegex(@";(?=(?:[^']*'[^']*')*[^']*$)", RegexOptions.Compiled)]
    private static partial Regex DelimiterRegex();
}