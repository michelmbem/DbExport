using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DbExport.Schema;

namespace DbExport;

public class SqlHelper(DbConnection connection) : IDisposable
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
            var lastDot = fullName.LastIndexOf('.');
            return fullName[..lastDot];
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public TResult Query<TResult>(string sql, Func<DbDataReader, TResult> converter)
    {
        TResult result;

        try
        {
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = sql;

            using var dataReader = command.ExecuteReader();
            result = converter(dataReader);
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

    public void ExecuteScript(string script)
    {
        switch (ProviderName)
        {
            case "Microsoft.Data.SqlClient":
                ExecuteSqlScript(script);
                break;
            case "Oracle.ManagedDataAccess.Client":
                ExecuteOracleScript(script);
                break;
            default:
                Execute(script);
                break;
        }
    }

    public static DbDataReader OpenTable(Table table, bool skipIdentity, bool skipRowVersion)
    {
        var sb = new StringBuilder("SELECT ");
        var comma = false;

        foreach (var column in table.Columns.Where(column => (!skipIdentity || !column.IsIdentity) &&
                                                             (!skipRowVersion || column.ColumnType != ColumnType.RowVersion)))
        {
            if (comma) sb.Append(", ");
            sb.Append(Utility.Escape(column.Name, table.Database.ProviderName));
            comma = true;
        }

        sb.Append(" FROM ").Append(Utility.Escape(table.Name, table.Database.ProviderName));

        var connection = Utility.GetConnection(table.Database);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = sb.ToString();

        return command.ExecuteReader(CommandBehavior.CloseConnection);
    }

    public static object[] ToArray(DbDataReader dataReader)
    {
        object[] result = null;

        if (dataReader.Read())
        {
            result = new object[dataReader.FieldCount];
            dataReader.GetValues(result);
        }

        return result;
    }

    public static Dictionary<string, object> ToDictionary(DbDataReader dataReader)
    {
        Dictionary<string, object> result = null;

        if (dataReader.Read())
        {
            result = new Dictionary<string, object>();
            for (int i = 0; i < dataReader.FieldCount; ++i)
                result[dataReader.GetName(i)] = dataReader.GetValue(i);
        }

        return result;
    }

    public static List<object[]> ToArrayList(DbDataReader dataReader)
    {
        var result = new List<object[]>();

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
        var result = new List<Dictionary<string, object>>();

        while (dataReader.Read())
        {
            var values = new Dictionary<string, object>();
            for (int i = 0; i < dataReader.FieldCount; ++i)
                values[dataReader.GetName(i)] = dataReader.GetValue(i);
            result.Add(values);
        }

        return result;
    }

    public static List<object> ToList(DbDataReader dataReader)
    {
        var result = new List<object>();

        while (dataReader.Read())
            result.Add(dataReader.GetValue(0));

        return result;
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && disposeConnection)
            connection.Dispose();
    }

    private void ExecuteSqlScript(string script)
    {
        var match = Regex.Match(script, @"CREATE DATABASE (.+)\s+GO", RegexOptions.IgnoreCase);

        if (match.Success)
        {
            var sqlCreateDb = match.Value[..(match.Length - 2)].TrimEnd();
            Execute(sqlCreateDb);
            script = script[(match.Index + match.Length)..];
        }

        script = Regex.Replace(script, @"[\r\n]GO[\r\n]", ";\n");
        Execute(script);
    }

    private void ExecuteOracleScript(string script) =>
        throw new InvalidOperationException(
            """
            The Oracle Data Provider for .NET does not support running commands in batch mode.
            Please save the script to a file then run it using the "SQL*Plus" custom command from the "Tools" menu.
            """);
}