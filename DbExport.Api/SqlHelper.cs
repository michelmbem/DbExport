using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using DbExport.Providers;
using DbExport.Providers.Firebird;
using DbExport.Providers.MySqlClient;
using DbExport.Providers.Npgsql;
using DbExport.Providers.SqlClient;
using DbExport.Schema;

namespace DbExport;

[Flags]
public enum QueryOptions
{
    None = 0,
    SkipIdentity = 1,
    SkipComputed = 2,
    SkipRowVersion = 4,
    SkipGenerated = SkipIdentity | SkipComputed | SkipRowVersion
}

/// <summary>
/// A helper class for executing SQL queries and scripts against a database.
/// It provides methods for querying data, executing non-query commands,
/// and executing SQL scripts with support for different database providers.
/// The class implements IDisposable to ensure proper disposal of database connections when necessary.
/// </summary>
public sealed class SqlHelper : IDisposable
{
    #region Constants

    /// <summary>
    /// Defines a set of binding flags used for property access within the SqlHelper class.
    /// Includes flags for public, instance-level, and case-insensitive member access.
    /// </summary>
    private const BindingFlags PROPERTY_FLAGS = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

    /// <summary>
    /// Represents a combination of binding flags used to access and modify properties
    /// within the SqlHelper class. Includes public, instance-level, case-insensitive
    /// member access, and property-setting capabilities.
    /// </summary>
    private const BindingFlags GET_PROPERTY_FLAGS = PROPERTY_FLAGS | BindingFlags.GetProperty;

    /// <summary>
    /// Defines a combination of binding flags used to set property values on objects,
    /// including public, instance-level, and case-insensitive member access.
    /// Extends the base property flags by adding support for property value setting.
    /// </summary>
    private const BindingFlags SET_PROPERTY_FLAGS = PROPERTY_FLAGS | BindingFlags.SetProperty;
    
    #endregion
    
    #region Fields

    /// <summary>
    /// Represents the database connection used by the SqlHelper instance to communicate with the database.
    /// This connection is essential for executing queries, commands, and scripts.
    /// The connection must be properly initialized and associated with a compatible database provider.
    /// </summary>
    private readonly DbConnection connection;

    /// <summary>
    /// Indicates whether the SqlHelper instance is responsible for disposing of the database connection.
    /// When set to true, the connection will be disposed upon disposing the SqlHelper instance.
    /// </summary>
    private readonly bool disposeConnection;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the SqlHelper class with the specified database connection.
    /// </summary>
    /// <param name="connection">The database connection to be used by the SqlHelper instance.</param>
    public SqlHelper(DbConnection connection)
    {
        this.connection = connection;

        if (connection.State == ConnectionState.Closed)
        {
            connection.Open();
            disposeConnection = true;
        }

        var fullName = connection.GetType().FullName;
        var lastDot = fullName!.LastIndexOf('.');
        ProviderName = fullName[..lastDot];
    }

    /// <summary>
    /// Initializes a new instance of the SqlHelper class with the specified provider name and connection string.
    /// </summary>
    /// <param name="providerName">The name of the database provider.</param>
    /// <param name="connectionString">The connection string for the database connection.</param>
    public SqlHelper(string providerName, string connectionString) :
        this(Utility.GetConnection(providerName, connectionString)) { }

    /// <summary>
    /// Initializes a new instance of the SqlHelper class with the specified Database object.
    /// </summary>
    /// <param name="database">The Database object containing the provider name and connection string for the database connection.</param>
    public SqlHelper(Database database) : this(Utility.GetConnection(database)) { }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the database provider being used by the SqlHelper instance.
    /// </summary>
    public string ProviderName { get; }

    #endregion

    #region IDisposable interface
    
    /// <inheritdoc/>
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
        using var command = connection.CreateCommand();
        command.CommandText = sql;

        using var dataReader = command.ExecuteReader();
        return extractor(dataReader);
    }

    /// <summary>
    /// Executes a SQL query with the specified parameters, binds them to the command,
    /// and extracts the result using the provided extractor function.
    /// </summary>
    /// <param name="sql">The SQL query to be executed.</param>
    /// <param name="paramSource">The source object containing parameters to be bound to the query.</param>
    /// <param name="binder">An action that binds parameters from the source object to the database command.</param>
    /// <param name="extractor">A function that processes the data reader and extracts the desired result.</param>
    /// <typeparam name="TSource">The type of the parameter source object.</typeparam>
    /// <typeparam name="TResult">The type of the result to be returned.</typeparam>
    /// <returns>The result extracted from the data reader based on the extractor function.</returns>
    public TResult Query<TSource, TResult>(
        string sql, TSource paramSource, Action<DbCommand, TSource> binder, Func<DbDataReader, TResult> extractor)
    {
        using var command = connection.CreateCommand();
        PrepareCommand(command, sql);
        binder(command, paramSource);

        using var dataReader = command.ExecuteReader();
        return extractor(dataReader);
    }

    /// <summary>
    /// Executes the specified SQL query and returns the value of the first column of the first row in the result set.
    /// </summary>
    /// <param name="sql">The SQL query to be executed.</param>
    /// <returns>The value of the first column of the first row in the result set, or null if the result set is empty.</returns>
    public object QueryScalar(string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        return command.ExecuteScalar();
    }

    /// <summary>
    /// Executes the specified SQL query with parameters, using a custom binder to bind the parameters,
    /// and returns a single scalar value resulting from the query.
    /// </summary>
    /// <typeparam name="TSource">The type of the parameter source.</typeparam>
    /// <param name="sql">The SQL query to be executed.</param>
    /// <param name="paramSource">The object containing the parameter values to be used in the SQL query.</param>
    /// <param name="binder">An action that binds the parameter values to the SQL command.</param>
    /// <returns>The scalar value returned from the execution of the SQL query.</returns>
    public object QueryScalar<TSource>(string sql, TSource paramSource, Action<DbCommand, TSource> binder)
    {
        using var command = connection.CreateCommand();
        PrepareCommand(command, sql);
        binder(command, paramSource);
        return command.ExecuteScalar();
    }


    /// <summary>
    /// Executes the specified SQL command and returns the number of rows affected by the command.
    /// </summary>
    /// <param name="sql">The SQL command to be executed.</param>
    /// <returns>The number of rows affected by the command.</returns>
    public int Execute(string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// Executes a parameterized SQL command using the specified parameter source and binder.
    /// </summary>
    /// <param name="sql">The SQL command to execute.</param>
    /// <param name="paramSource">The source object providing parameter values for the command.</param>
    /// <param name="binder">The action to bind the parameter values from the source object to the command.</param>
    /// <typeparam name="TSource">The type of the parameter source object.</typeparam>
    /// <returns>The number of rows affected by the command execution.</returns>
    public int Execute<TSource>(string sql, TSource paramSource, Action<DbCommand, TSource> binder)
    {
        using var command = connection.CreateCommand();
        PrepareCommand(command, sql);
        binder(command, paramSource);
        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// Executes a batch of SQL commands using a provided SQL statement and a collection of parameters.
    /// </summary>
    /// <param name="sql">The SQL command to be executed for each item in the collection.</param>
    /// <param name="paramSources">The collection of parameter sources, where each item represents the parameters for a single execution of the SQL command.</param>
    /// <param name="binder">A delegate that binds the parameters from a parameter source to a database command.</param>
    /// <typeparam name="TSource">The type of the parameter source used for binding.</typeparam>
    /// <returns>The total number of rows affected by all executed commands in the batch.</returns>
    public int ExecuteBatch<TSource>(string sql, IEnumerable<TSource> paramSources, Action<DbCommand, TSource> binder)
    {
        int affectedRows = 0;

        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        PrepareCommand(command, sql);

        foreach (var paramSource in paramSources)
        {
            binder(command, paramSource);
            affectedRows += command.ExecuteNonQuery();
        }

        transaction.Commit();

        return affectedRows;
    }

    /// <summary>
    /// Executes a batch of SQL statements using the provided SQL command template and a data reader as the source for parameters.
    /// </summary>
    /// <param name="sql">The SQL command template to be executed for each row of data read from the data reader.</param>
    /// <param name="dataReader">The data reader containing the rows of data to be processed in the batch.</param>
    /// <returns>The total number of rows affected by executing the batch.</returns>
    public int ExecuteBatch(string sql, DbDataReader dataReader)
    {
        int affectedRows = 0;

        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        PrepareCommand(command, sql);

        while (dataReader.Read())
        {
            FromDataReader(command, dataReader);
            affectedRows += command.ExecuteNonQuery();
        }

        transaction.Commit();

        return affectedRows;
    }

    /// <summary>
    /// Configures the specified <see cref="DbCommand"/> with the provided SQL statement,
    /// sets its command text, and prepares parameters for execution.
    /// </summary>
    /// <param name="command">The <see cref="DbCommand"/> to be configured.</param>
    /// <param name="sql">The SQL query or command text to be assigned to the <paramref name="command"/>.</param>
    private static void PrepareCommand(DbCommand command, string sql)
    {
        command.CommandText = sql;

        foreach (var paramName in ParameterParser.Extract(sql))
        {
            if (command.Parameters.Contains(paramName)) continue;

            var parameter = command.CreateParameter();
            parameter.ParameterName = paramName;
            command.Parameters.Add(parameter);
        }

        command.Prepare();
    }

    #endregion

    #region ExecuteScript method

    /// <summary>
    /// Executes the specified SQL script against the database using the provided provider name and connection string.
    /// </summary>
    /// <param name="providerName">The name of the database provider.</param>
    /// <param name="connectionString">The connection string for the database connection.</param>
    /// <param name="script">A string containing the SQL script to be executed.</param>
    public static void ExecuteScript(string providerName, string connectionString, string script)
    {
        GetScripExecutor(providerName).Execute(connectionString, script);
    }

    /// <summary>
    /// Gets the script executor that matches the given provider name.
    /// </summary>
    /// <param name="providerName">The name of the database provider.</param>
    /// <returns>An instance of a class that implements the <see cref="IScriptExecutor"/> interface.</returns>
    public static IScriptExecutor GetScripExecutor(string providerName) =>
        providerName switch
        {
            ProviderNames.SQLSERVER => new SqlScripExecutor(),
            ProviderNames.MYSQL => new MySqlScriptExecutor(),
            ProviderNames.POSTGRESQL => new NpgsqlScriptExecutor(),
            ProviderNames.FIREBIRD => new FirebirdScriptExecutor(),
            ProviderNames.ORACLE => new BatchScriptExecutor(providerName),
            _ => new SimpleScriptExecutor(providerName)
        };

    #endregion

    #region OpenTable and CopyTable methods

    /// <summary>
    /// Opens a database table for reading and returns a data reader for the resulting query.
    /// </summary>
    /// <param name="table">The table to be queried.</param>
    /// <param name="options">Specifies which columns to exclude from the query.</param>
    /// <returns>A data reader containing the results of the query.</returns>
    public static DbDataReader OpenTable(Table table, QueryOptions options)
    {
        var connection = Utility.GetConnection(table.Database);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = GenerateSelect(table, options);
        
        return command.ExecuteReader(CommandBehavior.CloseConnection);
    }

    /// <summary>
    /// Copies the data from the specified source table to the target database connection.
    /// </summary>
    /// <remarks>
    /// There should be a table with the same name and similar structure in the target database.
    /// </remarks>
    /// <param name="targetConnection">The database connection where the table data will be inserted.</param>
    /// <param name="table">The source table containing the data to be copied.</param>
    /// <param name="options">Specifies which columns to exclude from the SELECT and INSERT queries.</param>
    public static void CopyTable(DbConnection targetConnection, Table table, QueryOptions options)
    {
        using var helper = new SqlHelper(targetConnection);
        using var sourceReader = OpenTable(table, options);
        var insertSql = GenerateInsert(table, helper.ProviderName, options);
        helper.ExecuteBatch(insertSql, sourceReader);
    }

    #endregion

    #region SQL generation methods

    /// <summary>
    /// Generates a SQL SELECT statement for the specified table,
    /// optionally skipping identity, computed, and row version columns.
    /// </summary>
    /// <param name="table">The table for which the SELECT statement is generated.</param>
    /// <param name="options">Specifies which columns to exclude from the query.</param>
    /// <returns>A string representing the generated SQL SELECT statement.</returns>
    public static string GenerateSelect(Table table, QueryOptions options)
    {
        StringBuilder sb = new("SELECT ");
        var providerName = table.Database.ProviderName;

        foreach (var column in table.Columns.Where(c => ShouldNotSkip(c, options)))
            sb.Append(Utility.Escape(column.Name, providerName)).Append(", ");

        sb.Length -= 2;
        sb.Append(" FROM ");
        
        if (!string.IsNullOrEmpty(table.Owner))
            sb.Append(Utility.Escape(table.Owner, providerName)).Append('.');
        sb.Append(Utility.Escape(table.Name, providerName));
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates a SQL SELECT statement for the specified table and key,
    /// optionally skipping identity, computed, and row version columns.
    /// </summary>
    /// <param name="table">The table for which the SELECT statement is generated.</param
    /// <param name="key">The key for which to filter the selected rows.</param>
    /// <param name="options">Specifies which columns to exclude from the query.</param>
    /// <returns>A string representing the generated SQL SELECT statement.</returns>
    public static string GenerateSelect(Table table, Key key, QueryOptions options)
    {
        StringBuilder sb = new();
        sb.Append(GenerateSelect(table, options));
        GenerateFilter(key, table.Database.ProviderName, sb);

        return sb.ToString();
    }

    /// <summary>
    /// Generates an SQL insert statement for the specified table based on the given provider.
    /// </summary>
    /// <param name="table">The table for which the insert statement is to be generated.</param>
    /// <param name="providerName">The name of the database provider, used for escaping identifiers.</param>
    /// <param name="options">Specifies which columns to exclude from the query.</param>
    /// <returns>A string containing the generated SQL insert statement.</returns>
    public static string GenerateInsert(Table table, string providerName, QueryOptions options)
    {
        StringBuilder sb = new("INSERT INTO ");
        sb.Append(Utility.Escape(table.Name, providerName)).Append(" (");

        foreach (var column in table.Columns.Where(c => ShouldNotSkip(c, options)))
            sb.Append(Utility.Escape(column.Name, providerName)).Append(", ");

        sb.Length -= 2;
        sb.Append(") VALUES (");

        foreach (var column in table.Columns.Where(c => ShouldNotSkip(c, options)))
            sb.Append(Utility.ToParameterName(column.Name, providerName)).Append(", ");

        sb.Length -= 2;
        sb.Append(')');
        
        return sb.ToString();
    }

    /// <summary>
    /// Generates an SQL UPDATE statement for the specified table, based on the provided database provider name.
    /// </summary>
    /// <param name="table">The table for which the SQL UPDATE statement will be generated.</param>
    /// <param name="providerName">The name of the database provider, used for escaping identifiers properly.</param>
    /// <param name="options">Specifies which columns to exclude from the query.</param>
    /// <returns>A string containing the generated SQL UPDATE statement.</returns>
    public static string GenerateUpdate(Table table, string providerName, QueryOptions options)
    {
        StringBuilder sb = new("UPDATE ");
        sb.Append(Utility.Escape(table.Name, providerName)).Append(" SET ");

        foreach (var columnName in table.Columns
                                        .Where(c => ShouldNotSkip(c, options))
                                        .Select(c => c.Name))
        {
            sb.Append(Utility.Escape(columnName, providerName)).Append(" = ");
            sb.Append(Utility.ToParameterName(columnName, providerName)).Append(", ");
        }

        sb.Length -= 2;
        GenerateFilter(table.PrimaryKey, providerName, sb);

        return sb.ToString();
    }

    /// <summary>
    /// Generates a SQL DELETE statement for the specified table and provider.
    /// </summary>
    /// <param name="table">The table for which the DELETE statement will be generated.</param>
    /// <param name="providerName">The name of the database provider to determine escaping and parameter conventions.</param>
    /// <returns>A string containing the SQL DELETE statement.</returns>
    public static string GenerateDelete(Table table, string providerName)
    {
        StringBuilder sb = new("DELETE FROM ");
        sb.Append(Utility.Escape(table.Name, providerName));
        GenerateFilter(table.PrimaryKey, providerName, sb);

        return sb.ToString();
    }

    private static void GenerateFilter(Key key, string providerName, StringBuilder sb)
    {
        sb.Append(" WHERE ");

        foreach (var columnName in key.Columns.Select(c => c.Name))
        {
            sb.Append(Utility.Escape(columnName, providerName)).Append(" = ");
            sb.Append(Utility.ToParameterName(columnName, providerName)).Append(" AND ");
        }

        sb.Length -= 5;
    }

    private static bool ShouldNotSkip(Column c, QueryOptions o) =>
        !((o.HasFlag(QueryOptions.SkipIdentity) && c.IsIdentity) ||
          (o.HasFlag(QueryOptions.SkipComputed) && c.IsComputed) ||
          (o.HasFlag(QueryOptions.SkipRowVersion) && c.ColumnType == ColumnType.RowVersion));

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
    /// Converts the current row of the specified <see cref="DbDataReader"/> into an instance of the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to create. Must be a reference type with a parameterless constructor.</typeparam>
    /// <param name="dataReader">The <see cref="DbDataReader"/> positioned on the row to be converted.</param>
    /// <returns>
    /// An instance of <typeparamref name="TEntity"/> populated with values from the current row of the
    /// <paramref name="dataReader"/>, or null if no rows are available to read.
    /// </returns>
    public static TEntity ToEntity<TEntity>(DbDataReader dataReader) where TEntity : class, new()
    {
        if (!dataReader.Read()) return null;

        var entity = new TEntity();
        var entityType = typeof(TEntity);
        
        for (var i = 0; i < dataReader.FieldCount; ++i)
            entityType.InvokeMember(dataReader.GetName(i), SET_PROPERTY_FLAGS, null, entity, [dataReader.GetValue(i)]);
        
        return entity;
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
    /// Converts the data retrieved by a DbDataReader into a list of entities of the specified type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to convert each row into. This type must be a class and have a parameterless constructor.</typeparam>
    /// <param name="dataReader">The DbDataReader containing the data to be converted.</param>
    /// <returns>A list of entities of type TEntity, with each entity representing a row from the DbDataReader.</returns>
    public static List<TEntity> ToEntityList<TEntity>(DbDataReader dataReader) where TEntity : class, new()
    {
        List<TEntity> entities = [];
        var entityType = typeof(TEntity);
        
        while (dataReader.Read())
        {
            var entity = new TEntity();
            for (var i = 0; i < dataReader.FieldCount; ++i)
                entityType.InvokeMember(dataReader.GetName(i), SET_PROPERTY_FLAGS, null, entity, [dataReader.GetValue(i)]);
            entities.Add(entity);
        }
        
        return entities;
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
    
    #region Parameter binders

    /// <summary>
    /// Populates the parameters of a database command with the specified array of values.
    /// </summary>
    /// <param name="command">The database command whose parameters will be populated.</param>
    /// <param name="values">An array of values to assign to the command's parameters.</param>
    public static void FromArray(DbCommand command, object[] values)
    {
        for (var i = 0; i < values.Length; ++i)
            command.Parameters[i].Value = values[i];
    }

    /// <summary>
    /// Populates the parameters of a database command with values from the provided dictionary.
    /// </summary>
    /// <param name="command">The database command whose parameters will be populated.</param>
    /// <param name="values">A dictionary containing parameter names as keys and their corresponding values.</param>
    public static void FromDictionary(DbCommand command, Dictionary<string, object> values)
    {
        foreach (var (key, value) in values)
            command.Parameters[key].Value = value;
    }

    /// <summary>
    /// Sets the parameter values of the specified <see cref="DbCommand"/>
    /// using the property values of the provided entity.
    /// </summary>
    /// <param name="command">The database command whose parameters will be updated.</param>
    /// <param name="entity">The entity from which the parameter values are retrieved.</param>
    /// <typeparam name="TEntity">The type of the entity, which must be a reference type.</typeparam>
    public static void FromEntity<TEntity>(DbCommand command, TEntity entity) where TEntity : class
    {
        var entityType = typeof(TEntity);

        foreach (DbParameter parameter in command.Parameters)
        {
            var value = entityType.InvokeMember(parameter.ParameterName, GET_PROPERTY_FLAGS, null, entity, null);
            parameter.Value = value;
        }
    }

    /// <summary>
    /// Populates the parameters of a given database command using the current row of the provided data reader.
    /// </summary>
    /// <param name="command">The database command whose parameters will be populated.</param>
    /// <param name="dataReader">The data reader containing the source data for the command's parameters.</param>
    private static void FromDataReader(DbCommand command, DbDataReader dataReader)
    {
        foreach (DbParameter parameter in command.Parameters)
            parameter.Value = dataReader[parameter.ParameterName];
    }

    #endregion

    #region Inner ParameterParser class

    /// <summary>
    /// Provides methods for parsing SQL queries and extracting parameter names.
    /// This utility is used to identify and process parameter placeholders within a given SQL string
    /// while handling edge cases such as string literals, quoted identifiers, and comments.
    /// </summary>
    private static class ParameterParser
    {
        /// <summary>
        /// Extracts parameter names from the specified SQL string by identifying and parsing parameter placeholders.
        /// Handles SQL syntax rules such as skipping string literals, quoted identifiers, and comments.
        /// </summary>
        /// <param name="sql">The SQL string to parse for parameter placeholders.</param>
        /// <returns>A read-only list of unique parameter names found in the SQL string.</returns>
        public static IReadOnlyList<string> Extract(string sql)
        {
            HashSet<string> paramNames = [];
            var limit = sql.Length;

            for (var i = 0; i < limit; ++i)
            {
                switch (sql[i])
                {
                    // skip string literals
                    case '\'':
                        i = SkipSingleQuote(sql, i, limit);
                        continue;
                    // skip quoted identifiers
                    case '"':
                        i = SkipDoubleQuote(sql, i, limit);
                        continue;
                    // skip line comment
                    case '-' when i + 1 < limit && sql[i + 1] == '-':
                        i = SkipLineComment(sql, i, limit);
                        continue;
                    // skip block comment
                    case '/' when i + 1 < limit && sql[i + 1] == '*':
                        i = SkipBlockComment(sql, i, limit);
                        continue;
                    // parameter start
                    case var c when IsParamPrefix(c):
                    {
                        var start = ++i;
                        while (i < limit && IsParamChar(sql[i])) ++i;
                        var name = sql[start..(i--)];
                        paramNames.Add(name);
                        break;
                    }
                }
            }

            return [..paramNames];
        }

        /// <summary>
        /// Determines whether the specified character is a valid prefix for a SQL parameter.
        /// </summary>
        /// <param name="c">The character to evaluate as a potential parameter prefix.</param>
        /// <returns>True if the character is a valid parameter prefix; otherwise, false.</returns>
        private static bool IsParamPrefix(char c) => c is '@' or ':' or '$' or '?';

        /// <summary>
        /// Determines whether the specified character is valid as part of a SQL parameter name.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>True if the character is a letter, digit, or underscore; otherwise, false.</returns>
        private static bool IsParamChar(char c) => char.IsLetterOrDigit(c) || c == '_';

        /// <summary>
        /// Skips over a single-quoted string literal in a SQL string, starting at the given index.
        /// Handles escaped single quotes by advancing the index past them.
        /// </summary>
        /// <param name="sql">The SQL string being processed.</param>
        /// <param name="i">The current index within the SQL string where the single quote starts.</param>
        /// <param name="limit">The length of the SQL string, used as a boundary for the processing loop.</param>
        /// <returns>The index immediately following the closing single quote, or the last index of the string if no matching quote is found.</returns>
        private static int SkipSingleQuote(string sql, int i, int limit)
        {
            ++i;

            while (i < limit)
            {
                if (sql[i] == '\'')
                {
                    if (i + 1 >= limit || sql[i + 1] != '\'') return i;
                    i += 2;
                    continue;
                }

                ++i;
            }

            return limit - 1;
        }

        /// <summary>
        /// Skips over a quoted identifier in the SQL string, starting at the specified index.
        /// Advances the index to the position after the closing double-quote or to the end of the string
        /// if no closing double-quote is found.
        /// </summary>
        /// <param name="sql">The SQL string being parsed.</param>
        /// <param name="i">The current position in the SQL string where the quoted identifier begins, including the opening double-quote.</param>
        /// <param name="limit">The maximum index to evaluate within the SQL string.</param>
        /// <returns>The index of the closing double-quote or the index of the last evaluable character if the closing double-quote is not found.</returns>
        private static int SkipDoubleQuote(string sql, int i, int limit)
        {
            ++i;

            while (i < limit)
            {
                if (sql[i] == '"') return i;
                ++i;
            }

            return limit - 1;
        }

        /// <summary>
        /// Skips a line comment within the SQL string, advancing the index to the end of the comment.
        /// Line comments are identified as sequences starting with "--" and ending at the next newline character or the end of the string.
        /// </summary>
        /// <param name="sql">The SQL string containing the line comment to skip.</param>
        /// <param name="i">The current index in the SQL string, positioned at the start of the line comment.</param>
        /// <param name="limit">The length of the SQL string to determine the upper bounds for processing.</param>
        /// <returns>The updated index positioned at the end of the line comment or the end of the string, whichever comes first.</returns>
        private static int SkipLineComment(string sql, int i, int limit)
        {
            i += 2;
            while (i < limit && sql[i] != '\n') ++i;
            return i;
        }

        /// <summary>
        /// Skips a block comment in the SQL string and returns the updated index after the block comment ends.
        /// This method is called when a block comment is detected and ensures that parsing continues from
        /// the correct position after the comment.
        /// </summary>
        /// <param name="sql">The SQL string being parsed.</param>
        /// <param name="i">The current index in the SQL string where the block comment starts.</param>
        /// <param name="limit">The length of the SQL string, used as the upper boundary for parsing.</param>
        /// <returns>The index immediately after the end of the block comment, or the last index of the SQL string if the comment is unclosed.</returns>
        private static int SkipBlockComment(string sql, int i, int limit)
        {
            i += 2;

            while (i + 1 < limit)
            {
                if (sql[i] == '*' && sql[i + 1] == '/')
                    return i + 1;
                i++;
            }

            return limit - 1;
        }
    }

    #endregion
}