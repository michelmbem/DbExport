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

public sealed partial class SqlHelper : IDisposable
{
    #region Fields

    private readonly bool disposeConnection;
    private readonly DbConnection connection;

    #endregion

    #region Constructors

    public SqlHelper(DbConnection connection)
    {
        this.connection = connection;

        var fullName = connection.GetType().FullName;
        var lastDot = fullName.LastIndexOf('.');
        ProviderName = fullName[..lastDot];
    }

    public SqlHelper(string providerName, string connectionString) :
        this(Utility.GetConnection(providerName, connectionString))
    {
        disposeConnection = true;
    }

    public SqlHelper(Database database) : this(Utility.GetConnection(database))
    {
        disposeConnection = true;
    }

    #endregion

    #region Properties

    public string ProviderName { get; }

    #endregion

    #region IDisposable interface

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (disposing && disposeConnection)
            connection.Dispose();
    }

    #endregion

    #region Query and Execute methods

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

    public static RowSet OpenTable(Table table, bool skipIdentity, bool skipRowVersion)
    {
        StringBuilder sb = new("SELECT ");

        foreach (var column in table.Columns.Where(ShouldNotSkip))
            sb.Append(Utility.Escape(column.Name, table.Database.ProviderName)).Append(", ");

        sb.Length -= 2;
        sb.Append(" FROM ");
        if (!string.IsNullOrEmpty(table.Owner))
            sb.Append(Utility.Escape(table.Owner, table.Database.ProviderName)).Append('.');
        sb.Append(Utility.Escape(table.Name, table.Database.ProviderName));

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

    public static object[] ToArray(DbDataReader dataReader)
    {
        if (!dataReader.Read()) return null;
        
        var result = new object[dataReader.FieldCount];
        dataReader.GetValues(result);
        return result;
    }

    public static Dictionary<string, object> ToDictionary(DbDataReader dataReader)
    {
        if (!dataReader.Read()) return null;
        
        var result = Utility.EmptyDictionary<object>();
        for (var i = 0; i < dataReader.FieldCount; ++i)
            result[dataReader.GetName(i)] = dataReader.GetValue(i);
        return result;
    }

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