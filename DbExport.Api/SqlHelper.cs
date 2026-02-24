using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DbExport.Providers;
using DbExport.Schema;

namespace DbExport;

public sealed partial class SqlHelper(DbConnection connection) : IDisposable
{
    private readonly bool disposeConnection;

    public SqlHelper(string providerName, string connectionString) :
        this(Utility.GetConnection(providerName, connectionString))
    {
        disposeConnection = true;
    }

    public SqlHelper(Database database) : this(Utility.GetConnection(database))
    {
        disposeConnection = true;
    }

    public string ProviderName
    {
        get
        {
            var fullName = connection.GetType().FullName;
            var lastDot = fullName!.LastIndexOf('.');
            return fullName[..lastDot];
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (disposing && disposeConnection)
            connection.Dispose();
    }

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
            
            using (var helper1 = new SqlHelper(ProviderNames.POSTGRESQL, connectionString))
                helper1.Execute(createDb);
            
            var builder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString) { Database = dbName };
            connectionString = builder.ToString();
            
            script = script[(match.Index + match.Length)..];
            match = new Regex($@"\\[Cc]\s+({dbName})\s*;").Match(script);
            
            if (match.Success)
                script = script[(match.Index + match.Length)..];
        }
        
        ExecuteBatch(ProviderNames.POSTGRESQL, connectionString, script);
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
            using var helper = new SqlHelper(conn);

            foreach (var statement in statements)
                helper.Execute(statement);
        }
    }

    public static (DbDataReader, DbConnection) OpenTable(Table table, bool skipIdentity, bool skipRowVersion)
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

        using var command = connection.CreateCommand();
        command.CommandText = sb.ToString();
        
        var dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
        return (dataReader, connection);
        
        bool ShouldNotSkip(Column c) => !(skipIdentity && c.IsIdentity ||
                                          skipRowVersion && c.ColumnType == ColumnType.RowVersion);
    }

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

    [GeneratedRegex(@"CREATE DATABASE (.+)\s*;", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex CreateDbRegex();
    
    [GeneratedRegex(@";(?=(?:[^']*'[^']*')*[^']*$)", RegexOptions.Compiled)]
    private static partial Regex DelimiterRegex();

    [GeneratedRegex(@"CREATE DATABASE (.+)\s+GO", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex SqlCreateDbRegex();
    
    [GeneratedRegex(@"[\r\n]GO[\r\n]", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex SqlDelimiterRegex();
}