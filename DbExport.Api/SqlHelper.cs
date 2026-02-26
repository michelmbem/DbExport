using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DbExport.Providers;
using DbExport.Providers.Firebird;
using DbExport.Schema;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Isql;
using Npgsql;

namespace DbExport;

/// <summary>
/// A helper class for executing SQL queries and scripts against a database.
/// It provides methods for querying data, executing non-query commands,
/// and executing SQL scripts with support for different database providers.
/// The class implements IDisposable to ensure proper disposal of database connections when necessary.
/// </summary>
public sealed partial class SqlHelper : IDisposable
{
    #region Fields

    private readonly bool disposeConnection;
    private readonly DbConnection connection;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the SqlHelper class with the specified database connection.
    /// </summary>
    /// <param name="connection">The database connection to be used by the SqlHelper instance.</param>
    public SqlHelper(DbConnection connection)
    {
        this.connection = connection;

        var fullName = connection.GetType().FullName;
        var lastDot = fullName.LastIndexOf('.');
        ProviderName = fullName[..lastDot];
    }

    /// <summary>
    /// Initializes a new instance of the SqlHelper class with the specified provider name and connection string.
    /// </summary>
    /// <param name="providerName">The name of the database provider.</param>
    /// <param name="connectionString">The connection string for the database connection.</param>
    public SqlHelper(string providerName, string connectionString) :
        this(Utility.GetConnection(providerName, connectionString))
    {
        disposeConnection = true;
    }

    /// <summary>
    /// Initializes a new instance of the SqlHelper class with the specified Database object.
    /// </summary>
    /// <param name="database">The Database object containing the provider name and connection string for the database connection.</param>
    public SqlHelper(Database database) : this(Utility.GetConnection(database))
    {
        disposeConnection = true;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the database provider being used by the SqlHelper instance.
    /// </summary>
    public string ProviderName { get; }

    #endregion

    #region IDisposable interface

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the SqlHelper instance and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"></param>
    private void Dispose(bool disposing)
    {
        if (disposing && disposeConnection)
            connection.Dispose();
    }

    #endregion

    #region Query and Execute methods

    /// <summary>
    /// Executes the specified SQL query and uses the provided extractor function to process the results from the data reader.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the extractor function.</typeparam>
    /// <param name="sql">The SQL query to be executed.</param>
    /// <param name="extractor">The function that processes the data reader and returns a result of type TResult.</param>
    /// <returns>A result of type TResult obtained by processing the data reader with the extractor function.</returns>
    public TResult Query<TResult>(string sql, Func<DbDataReader, TResult> extractor)
    {
        TResult result;

        try
        {
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = sql;

            using var dataReader = command.ExecuteReader();
            result = extractor(dataReader);
        }
        finally
        {
            connection.Close();
        }

        return result;
    }

    /// <summary>
    /// Executes the specified SQL query and returns the value of the first column of the first row in the result set.
    /// </summary>
    /// <param name="sql">The SQL query to be executed.</param>
    /// <returns>The value of the first column of the first row in the result set, or null if the result set is empty.</returns>
    public object QueryScalar(string sql)
    {
        object result;

        try
        {
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            result = command.ExecuteScalar();
        }
        finally
        {
            connection.Close();
        }

        return result;
    }


    /// <summary>
    /// Executes the specified SQL command and returns the number of rows affected by the command.
    /// </summary>
    /// <param name="sql">The SQL command to be executed.</param>
    /// <returns>The number of rows affected by the command.</returns>
    public int Execute(string sql)
    {
        int result;

        try
        {
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            result = command.ExecuteNonQuery();
        }
        finally
        {
            connection.Close();
        }

        return result;
    }

    #endregion

    #region ExecuteScript methods

    /// <summary>
    /// Executes the specified SQL script against the database using the provided provider name and connection string.
    /// </summary>
    /// <param name="providerName">The name of the database provider.</param>
    /// <param name="connectionString">The connection string for the database connection.</param>
    /// <param name="script">A string containing the SQL script to be executed.</param>
    public static void ExecuteScript(string providerName, string connectionString, string script)
    {
        switch (providerName)
        {
            case ProviderNames.SQLSERVER:
                ExecuteSqlScript(connectionString, script);
                break;
            case ProviderNames.POSTGRESQL:
                ExecutePgScript(connectionString, script);
                break;
            case ProviderNames.FIREBIRD:
                ExecuteFbScript(connectionString, script);
                break;
            case ProviderNames.ORACLE:
                ExecuteBatch(providerName, connectionString, script);
                break;
            default:
            {
                using var helper = new SqlHelper(providerName, connectionString);
                helper.Execute(script);
                break;
            }
        }
    }

    /// <summary>
    /// Executes the specified SQL script against a SQL Server database using the provided connection string.
    /// </summary>
    /// <param name="connectionString">The connection string for the database connection.</param>
    /// <param name="script">A string containing the SQL script to be executed.</param>
    public static void ExecuteSqlScript(string connectionString, string script)
    {
        using var helper = new SqlHelper(ProviderNames.SQLSERVER, connectionString);
        var match = SqlCreateDbRegex().Match(script);
        
        if (match.Success)
        {
            var createDb = match.Value[..^2].TrimEnd();
            helper.Execute(createDb);
            script = script[(match.Index + match.Length)..];
        }

        script = SqlDelimiterRegex().Replace(script, ";\n");
        helper.Execute(script);
    }

    /// <summary>
    /// Executes the specified SQL script against a PostgreSQL database using the provided connection string.
    /// </summary>
    /// <param name="connectionString">The connection string for the database connection.</param>
    /// <param name="script">A string containing the SQL script to be executed.</param>
    public static void ExecutePgScript(string connectionString, string script)
    {
        var match = CreateDbRegex().Match(script);
        
        if (match.Success)
        {
            var createDb = match.Value[..^1];
            var dbName = match.Groups[1].Value;
            
            using (var helper = new SqlHelper(ProviderNames.POSTGRESQL, connectionString))
                helper.Execute(createDb);
            
            var builder = new NpgsqlConnectionStringBuilder(connectionString) { Database = dbName };
            connectionString = builder.ToString();
            
            script = script[(match.Index + match.Length)..];
            match = new Regex($@"\\[Cc]\s+({dbName})\s*;").Match(script);
            
            if (match.Success)
                script = script[(match.Index + match.Length)..];
        }
        
        ExecuteBatch(ProviderNames.POSTGRESQL, connectionString, script);
    }

    /// <summary>
    /// Executes the specified SQL script against a Firebird database using the provided connection string.
    /// </summary>
    /// <param name="connectionString">The connection string for the database connection.</param>
    /// <param name="script">A string containing the SQL script to be executed.</param>
    public static void ExecuteFbScript(string connectionString, string script)
    {
        var match = FbCreateDbRegex().Match(script);
        
        if (match.Success)
        {
            var dbName = match.Groups[1].Value;
            var builder = new FbConnectionStringBuilder(connectionString) { Database = dbName };

            connectionString = builder.ToString();
            FbConnection.CreateDatabase(connectionString, FirebirdOptions.PageSize,
                                        FirebirdOptions.ForcedWrites, FirebirdOptions.Overwrite);

            script = script[(match.Index + match.Length)..];
        }
        
        using var conn = Utility.GetConnection(ProviderNames.FIREBIRD, connectionString);
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

    /// <summary>
    /// Executes the specified SQL script against a database using batch execution if supported by the provider,
    /// or executes each statement individually if batch execution is not supported.
    /// </summary>
    /// <param name="providerName">The name of the database provider.</param>
    /// <param name="connectionString">The connection string for the database connection.</param>
    /// <param name="script">A string containing the SQL script to be executed.</param>
    public static void ExecuteBatch(string providerName, string connectionString, string script)
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
                var command = batch.CreateBatchCommand();
                command.CommandText = statement;
                batch.BatchCommands.Add(command);
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

    #endregion

    #region OpenTable method

    /// <summary>
    /// Executes a SELECT query to retrieve all columns from the specified table, with options to skip identity and row version columns.
    /// </summary>
    /// <param name="table">The Table object representing the database table to be queried.</param>
    /// <param name="skipIdentity">A boolean value indicating whether to skip identity columns in the SELECT query.</param>
    /// <param name="skipRowVersion">A boolean value indicating whether to skip row version columns in the SELECT query.</param>
    /// <returns>A RowSet object containing the database connection, command, and data reader for the executed SELECT query.</returns>
    public static RowSet OpenTable(Table table, bool skipIdentity, bool skipRowVersion)
    {
        StringBuilder sb = new("SELECT ");
        var providerName = table.Database.ProviderName;

        foreach (var column in table.Columns.Where(ShouldNotSkip))
            sb.Append(Utility.Escape(column.Name, providerName)).Append(", ");

        sb.Length -= 2;
        sb.Append(" FROM ");
        
        if (!string.IsNullOrEmpty(table.Owner))
            sb.Append(Utility.Escape(table.Owner, providerName)).Append('.');
        sb.Append(Utility.Escape(table.Name, providerName));

        var connection = Utility.GetConnection(table.Database);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = sb.ToString();
        
        var dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
        return new RowSet(connection, command, dataReader);
        
        bool ShouldNotSkip(Column c) => !(skipIdentity && c.IsIdentity ||
                                          skipRowVersion && c.ColumnType == ColumnType.RowVersion);
    }

    #endregion

    #region DataReader extractors

    /// <summary>
    /// Extracts the values of the columns from the first row of the data reader and returns them as an array of objects.
    /// </summary>
    /// <param name="dataReader">The DbDataReader from which to extract the column values.</param>
    /// <returns>An array of objects containing the values of the columns from the first row of the data reader,
    /// or null if the data reader is empty.</returns>
    public static object[] ToArray(DbDataReader dataReader)
    {
        if (!dataReader.Read()) return null;
        
        var result = new object[dataReader.FieldCount];
        dataReader.GetValues(result);
        return result;
    }

    /// <summary>
    /// Extracts the values of the columns from the first row of the data reader and returns them as a dictionary,
    /// where the keys are the column names and the values are the corresponding column values.
    /// </summary>
    /// <param name="dataReader">The DbDataReader from which to extract the column values.</param>
    /// <returns>A dictionary containing the column names as keys and the corresponding column values as values from the first row of the data reader,
    /// or null if the data reader is empty.</returns>
    public static Dictionary<string, object> ToDictionary(DbDataReader dataReader)
    {
        if (!dataReader.Read()) return null;
        
        var result = Utility.EmptyDictionary<object>();
        for (var i = 0; i < dataReader.FieldCount; ++i)
            result[dataReader.GetName(i)] = dataReader.GetValue(i);
        return result;
    }

    /// <summary>
    /// Extracts the values of the columns from all rows of the data reader and returns them as a list of object arrays,
    /// where each object array represents a row of data, and the elements of the array correspond to the column values in that row.
    /// </summary>
    /// <param name="dataReader">The DbDataReader from which to extract the column values.</param>
    /// <returns>A list of object arrays, where each object array contains the column values for a row of data from the data reader.</returns>
    public static List<object[]> ToArrayList(DbDataReader dataReader)
    {
        List<object[]> result = [];

        while (dataReader.Read())
        {
            var values = new object[dataReader.FieldCount];
            dataReader.GetValues(values);
            result.Add(values);
        }

        return result;
    }

    /// <summary>
    /// Extracts the values of the columns from all rows of the data reader and returns them as a list of dictionaries,
    /// where each dictionary represents a row of data, and the keys of the dictionary are the column names,
    /// and the values are the corresponding column values for that row.
    /// </summary>
    /// <param name="dataReader">The DbDataReader from which to extract the column values.</param>
    /// <returns>A list of dictionaries, where each dictionary contains the column names as keys and
    /// the corresponding column values as values for a row of data from the data reader.</returns>
    public static List<Dictionary<string, object>> ToDictionaryList(DbDataReader dataReader)
    {
        List<Dictionary<string, object>> result = [];

        while (dataReader.Read())
        {
            var values = Utility.EmptyDictionary<object>();
            for (var i = 0; i < dataReader.FieldCount; ++i)
                values[dataReader.GetName(i)] = dataReader.GetValue(i);
            result.Add(values);
        }

        return result;
    }

    /// <summary>
    /// Extracts the values of the first column from all rows of the data reader and returns them as a list of objects,
    /// where each object in the list corresponds to the value of the first column for a row of data from the data reader.
    /// </summary>
    /// <param name="dataReader">The DbDataReader from which to extract the column values.</param>
    /// <returns>A list of objects containing the values of the first column for each row of data from the data reader.</returns>
    public static List<object> ToList(DbDataReader dataReader)
    {
        List<object> result = [];

        while (dataReader.Read())
            result.Add(dataReader.GetValue(0));

        return result;
    }

    #endregion

    #region Regular expressions

    [GeneratedRegex(@"CREATE\s+DATABASE\s+(\w[\w\d]*)[^;]*;", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex CreateDbRegex();
    
    [GeneratedRegex(@";(?=(?:[^']*'[^']*')*[^']*$)", RegexOptions.Compiled)]
    private static partial Regex DelimiterRegex();

    [GeneratedRegex(@"CREATE\s+DATABASE\s+(.+)\s+GO", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex SqlCreateDbRegex();
    
    [GeneratedRegex(@"[\r\n]GO[\r\n]", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex SqlDelimiterRegex();

    [GeneratedRegex(@"CREATE\s+DATABASE\s+'([^']+)'[^;]*;", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex FbCreateDbRegex();

    #endregion
}