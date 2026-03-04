using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DbExport.Providers;
using DbExport.Providers.Firebird;
using DbExport.Providers.MySqlClient;
using DbExport.Providers.Npgsql;
using DbExport.Providers.SqlClient;
using DbExport.Schema;

namespace DbExport;

/// <summary>
/// A helper class for executing SQL queries and scripts against a database.
/// It provides methods for querying data, executing non-query commands,
/// and executing SQL scripts with support for different database providers.
/// The class implements IDisposable to ensure proper disposal of database connections when necessary.
/// </summary>
public sealed partial class SqlHelper : IDisposable
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
    /// Indicates whether the SqlHelper instance is responsible for disposing of the database connection.
    /// When set to true, the connection will be disposed upon disposing the SqlHelper instance.
    /// </summary>
    private readonly bool disposeConnection;

    /// <summary>
    /// Represents the database connection used by the SqlHelper instance to communicate with the database.
    /// This connection is essential for executing queries, commands, and scripts.
    /// The connection must be properly initialized and associated with a compatible database provider.
    /// </summary>
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
        var lastDot = fullName!.LastIndexOf('.');
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
        connection.Open();
        disposeConnection = true;
    }

    /// <summary>
    /// Initializes a new instance of the SqlHelper class with the specified Database object.
    /// </summary>
    /// <param name="database">The Database object containing the provider name and connection string for the database connection.</param>
    public SqlHelper(Database database) : this(Utility.GetConnection(database))
    {
        connection.Open();
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
        command.CommandText = sql;
        PrepareCommand(command);
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
        var result = 0;

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        PrepareCommand(command);

        foreach (var paramSource in paramSources)
        {
            binder(command, paramSource);
            result += command.ExecuteNonQuery();
        }

        return result;
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

    #region OpenTable method

    /// <summary>
    /// Executes a SELECT query to retrieve all columns from the specified table, with options to skip identity and row version columns.
    /// </summary>
    /// <param name="table">The Table object representing the database table to be queried.</param>
    /// <param name="skipIdentity">A boolean value indicating whether to skip identity columns in the SELECT query.</param>
    /// <param name="skipRowVersion">A boolean value indicating whether to skip row version columns in the SELECT query.</param>
    /// <returns>A RowSet object containing the database connection, command, and data reader for the executed SELECT query.</returns>
    public static DbDataReader OpenTable(Table table, bool skipIdentity, bool skipRowVersion)
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
        
        return command.ExecuteReader(CommandBehavior.CloseConnection);
        
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

    #endregion
    
    #region Helper methods

    /// <summary>
    /// Prepares the specified database command by ensuring that all parameters
    /// referenced in the command's text are added to its parameters collection
    /// and are ready for use during execution.
    /// </summary>
    /// <param name="command">The database command to be prepared.</param>
    private static void PrepareCommand(DbCommand command)
    {
        var paramNames = ParameterRegex().Matches(command.CommandText)
                                         .Select(m => m.Groups[1].Value);
        
        foreach (var paramName in paramNames)
        {
            if (command.Parameters.Contains(paramName)) continue;
            
            var parameter = command.CreateParameter();
            parameter.ParameterName = paramName;
            command.Parameters.Add(parameter);
        }
        
        command.Prepare();
    }

    [GeneratedRegex(@"[@:$](\w+)")]
    private static partial Regex ParameterRegex();
    
    #endregion
}