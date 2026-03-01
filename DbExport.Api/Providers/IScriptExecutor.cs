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
/// Provides a default implementation of the IScriptExecutor interface that executes SQL scripts
/// against a database connection using a specified provider.
/// </summary>
/// <remarks>
/// This class uses the SqlHelper class to execute the script against the database.
/// </remarks>
/// <param name="providerName">The name of the database provider used to establish the connection.</param>
public class DefaultScriptExecutor(string providerName) : IScriptExecutor
{
    public virtual void Execute(string connectionString, string script)
    {
        using var helper = new SqlHelper(providerName, connectionString);
        helper.Execute(script);
    }
}

/// <summary>
/// A specialized implementation of the DefaultScriptExecutor class that executes SQL scripts
/// using batch processing when supported by the database provider.
/// </summary>
/// <remarks>
/// This class processes scripts by splitting them into individual SQL statements and executing
/// them as a batch if the provider permits it. If batch execution is not supported, individual
/// commands are executed sequentially.
/// </remarks>
/// <param name="providerName">The name of the database provider used to establish the connection.</param>
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
public partial class BatchScriptExecutor(string providerName) : DefaultScriptExecutor(providerName)
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
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

    [GeneratedRegex(@";(?=(?:[^']*'[^']*')*[^']*$)")]
    private static partial Regex DelimiterRegex();
}