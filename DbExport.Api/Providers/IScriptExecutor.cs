using System.Linq;
using System.Text.RegularExpressions;

namespace DbExport.Providers;

/// <summary>
/// Defines an abstraction for executing SQL scripts against a database connection.
/// </summary>
public interface IScriptExecutor
{
    /// <summary>
    /// Executes a SQL script against the specified database connection.
    /// </summary>
    /// <param name="connectionString">The connection string used to connect to the database.</param>
    /// <param name="script">The SQL script to be executed.</param>
    void Execute(string connectionString, string script);
}

/// <summary>
/// Provides a simple implementation of the <see cref="IScriptExecutor"/> interface that executes SQL scripts
/// against a database connection using the <see cref="SqlHelper"/> class.
/// </summary>
/// <param name="providerName">The name of the database provider used to establish the connection.</param>
public class SimpleScriptExecutor(string providerName) : IScriptExecutor
{
    public void Execute(string connectionString, string script)
    {
        using var helper = new SqlHelper(providerName, connectionString);
        helper.Execute(script);
    }
}

/// <summary>
/// An implementation of the <see cref="IScriptExecutor"/> interface that executes SQL scripts
/// using batch processing when supported by the database provider.
/// </summary>
/// <remarks>
/// This class processes scripts by splitting them into individual SQL statements and executing
/// them as a batch if the provider permits it. If batch execution is not supported, individual
/// commands are executed sequentially.
/// </remarks>
/// <param name="providerName">The name of the database provider used to establish the connection.</param>
public partial class BatchScriptExecutor(string providerName) : IScriptExecutor
{
    public virtual void Execute(string connectionString, string script)
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

    [GeneratedRegex(@";(?=(?:[^']*'[^']*')*[^']*$)")]
    private static partial Regex DelimiterRegex();
}