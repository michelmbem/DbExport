using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DbExport.Schema;

[Flags]
public enum QueryOptions
{
    None = 0,
    SkipIdentity = 1,
    SkipComputed = 2,
    SkipRowVersion = 4,
    SkipGenerated = SkipIdentity | SkipComputed | SkipRowVersion,
    QualifyTableName = 8,
    All = SkipGenerated | QualifyTableName
}

public static class TableExtensions
{
    #region SQL generation methods

    public static string GenerateSelect(this Table table, QueryOptions options)
    {
        StringBuilder queryBuilder = new("SELECT ");
        var providerName = table.Database.ProviderName;

        foreach (var column in table.Columns.Where(c => c.IsMatch(options)))
            queryBuilder.Append(Utility.Escape(column.Name, providerName)).Append(", ");

        queryBuilder.Length -= 2;
        queryBuilder.Append(" FROM ");
        queryBuilder.AppendNameOf(table, providerName, options);

        return queryBuilder.ToString();
    }

    public static string GenerateSelect(this Table table, Key key, QueryOptions options)
    {
        Trace.Assert(table == key.Table); // The key should belong to the table

        StringBuilder queryBuilder = new();
        queryBuilder.Append(table.GenerateSelect(options));
        queryBuilder.AppendFilterBy(key, table.Database.ProviderName);

        return queryBuilder.ToString();
    }

    public static string GenerateInsert(this Table table, string providerName, QueryOptions options)
    {
        StringBuilder queryBuilder = new("INSERT INTO ");
        queryBuilder.AppendNameOf(table, providerName, options).Append(" (");

        foreach (var column in table.Columns.Where(c => c.IsMatch(options)))
            queryBuilder.Append(Utility.Escape(column.Name, providerName)).Append(", ");

        queryBuilder.Length -= 2;
        queryBuilder.Append(") VALUES (");

        foreach (var column in table.Columns.Where(c => c.IsMatch(options)))
            queryBuilder.Append(Utility.ToParameterName(column.Name, providerName)).Append(", ");

        queryBuilder.Length -= 2;
        queryBuilder.Append(')');

        return queryBuilder.ToString();
    }

    public static string GenerateUpdate(this Table table, string providerName, QueryOptions options)
    {
        StringBuilder queryBuilder = new("UPDATE ");
        queryBuilder.AppendNameOf(table, providerName, options).Append(" SET ");

        foreach (var columnName in table.Columns
                                        .Where(c => !c.IsPKColumn && c.IsMatch(options))
                                        .Select(c => c.Name))
        {
            queryBuilder.Append(Utility.Escape(columnName, providerName)).Append(" = ");
            queryBuilder.Append(Utility.ToParameterName(columnName, providerName)).Append(", ");
        }

        queryBuilder.Length -= 2;
        queryBuilder.AppendFilterBy(table.PrimaryKey, providerName);

        return queryBuilder.ToString();
    }

    public static string GenerateDelete(this Table table, string providerName, QueryOptions options)
    {
        StringBuilder queryBuilder = new("DELETE FROM ");
        queryBuilder.AppendNameOf(table, providerName, options);
        queryBuilder.AppendFilterBy(table.PrimaryKey, providerName);

        return queryBuilder.ToString();
    }

    private static bool IsMatch(this Column c, QueryOptions o) =>
        !((o.HasFlag(QueryOptions.SkipIdentity) && c.IsIdentity) ||
          (o.HasFlag(QueryOptions.SkipComputed) && c.IsComputed) ||
          (o.HasFlag(QueryOptions.SkipRowVersion) && c.ColumnType == ColumnType.RowVersion));

    private static StringBuilder AppendNameOf(this StringBuilder sb, Table table, string providerName, QueryOptions options)
    {
        if (options.HasFlag(QueryOptions.QualifyTableName) && !string.IsNullOrEmpty(table.Owner))
            sb.Append(Utility.Escape(table.Owner, providerName)).Append('.');

        sb.Append(Utility.Escape(table.Name, providerName));

        return sb;
    }

    private static StringBuilder AppendFilterBy(this StringBuilder sb, Key key, string providerName)
    {
        sb.Append(" WHERE ");

        foreach (var columnName in key.Columns.Select(c => c.Name))
        {
            sb.Append(Utility.Escape(columnName, providerName)).Append(" = ");
            sb.Append(Utility.ToParameterName(columnName, providerName)).Append(" AND ");
        }

        sb.Length -= 5;

        return sb;
    }

    #endregion

    #region Raw data extraction/migration

    public static DbDataReader OpenReader(this Table table, QueryOptions options)
    {
        var connection = Utility.GetConnection(table.Database);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = table.GenerateSelect(options);

        return command.ExecuteReader(CommandBehavior.CloseConnection);
    }

    public static void CopyTo(this Table table, DbConnection targetConnection, QueryOptions sourceOptions, QueryOptions targetOptions)
    {
        using var helper = new SqlHelper(targetConnection);
        using var sourceReader = table.OpenReader(sourceOptions);
        var insertSql = table.GenerateInsert(helper.ProviderName, targetOptions);
        helper.ExecuteBatch(insertSql, sourceReader);
    }

    #endregion

    #region Query methods

    public static TRowSet Select<TRowSet>(this Table table, Func<DbDataReader, TRowSet> extractor)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = table.GenerateSelect(QueryOptions.QualifyTableName);
        return helper.Query(sql, extractor);
    }

    public static List<TRow> Select<TRow>(this Table table) where TRow : class, new() =>
        Select(table, SqlHelper.ToEntityList<TRow>);

    public static TRowSet Select<TRowSet, TKey>(this Table table, Key key, TKey keyValue,
        Action<DbCommand, TKey> keyBinder, Func<DbDataReader, TRowSet> extractor)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = table.GenerateSelect(key, QueryOptions.QualifyTableName);
        return helper.Query(sql, keyValue, keyBinder, extractor);
    }

    public static List<TRow> Select<TRow>(this Table table, Key key, params object[] keyValues)
        where TRow : class, new() =>
        Select(table, key, keyValues, SqlHelper.FromArray, SqlHelper.ToEntityList<TRow>);

    #endregion

    #region Simple update methods

    public static bool Insert<TRow>(this Table table, TRow rowValue, Action<DbCommand, TRow> rowBinder)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = table.GenerateInsert(table.Database.ProviderName, QueryOptions.All);
        return helper.Execute(sql, rowValue, rowBinder) > 0;
    }

    public static bool Insert<TRow>(this Table table, TRow rowValue) where TRow : class, new() =>
        Insert(table, rowValue, SqlHelper.FromEntity);

    public static bool Insert(this Table table, params object[] rowValues) =>
        Insert(table, rowValues, SqlHelper.FromArray);

    public static bool Update<TRow>(this Table table, TRow rowValue, Action<DbCommand, TRow> rowBinder)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = table.GenerateUpdate(table.Database.ProviderName, QueryOptions.All);
        return helper.Execute(sql, rowValue, rowBinder) > 0;
    }

    public static bool Update<TRow>(this Table table, TRow rowValue) where TRow : class, new() =>
        Update(table, rowValue, SqlHelper.FromEntity);

    public static bool Delete<TKey>(this Table table, TKey keyValue, Action<DbCommand, TKey> keyBinder)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = table.GenerateDelete(table.Database.ProviderName, QueryOptions.QualifyTableName);
        return helper.Execute(sql, keyValue, keyBinder) > 0;
    }

    public static bool Delete<TKey>(this Table table, TKey keyValue) where TKey : class, new() =>
        Delete(table, keyValue, SqlHelper.FromEntity);

    public static bool Delete(this Table table, params object[] keyValues) =>
        Delete(table, keyValues, SqlHelper.FromArray);

    #endregion

    #region Batch update methods

    public static bool InsertBatch<TRow>(this Table table, IEnumerable<TRow> rowValues, Action<DbCommand, TRow> rowBinder)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = table.GenerateInsert(table.Database.ProviderName, QueryOptions.All);
        return helper.ExecuteBatch(sql, rowValues, rowBinder) > 0;
    }

    public static bool InsertBatch<TRow>(this Table table, IEnumerable<TRow> rowValues) where TRow : class, new() =>
        InsertBatch(table, rowValues, SqlHelper.FromEntity);

    public static bool UpdateBatch<TRow>(this Table table, IEnumerable<TRow> rowValues, Action<DbCommand, TRow> rowBinder)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = table.GenerateUpdate(table.Database.ProviderName, QueryOptions.All);
        return helper.ExecuteBatch(sql, rowValues, rowBinder) > 0;
    }

    public static bool UpdateBatch<TRow>(this Table table, IEnumerable<TRow> rowValues) where TRow : class, new() =>
        UpdateBatch(table, rowValues, SqlHelper.FromEntity);

    public static bool DeleteBatch<TKey>(this Table table, IEnumerable<TKey> keyValues, Action<DbCommand, TKey> keyBinder)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = table.GenerateDelete(table.Database.ProviderName, QueryOptions.QualifyTableName);
        return helper.ExecuteBatch(sql, keyValues, keyBinder) > 0;
    }

    public static bool DeleteBatch<TKey>(this Table table, IEnumerable<TKey> keyValues) where TKey : class, new() =>
        DeleteBatch(table, keyValues, SqlHelper.FromEntity);

    #endregion
}
