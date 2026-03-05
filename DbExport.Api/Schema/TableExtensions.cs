using System;
using System.Collections.Generic;
using System.Data.Common;

namespace DbExport.Schema;

public static class TableExtensions
{
    #region Query methods

    public static TRowSet Select<TRowSet>(this Table table, Func<DbDataReader, TRowSet> extractor)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = SqlHelper.GenerateSelect(table, QueryOptions.None);
        return helper.Query(sql, extractor);
    }

    public static List<TRow> Select<TRow>(this Table table) where TRow : class, new() =>
        Select(table, SqlHelper.ToEntityList<TRow>);

    public static TRowSet Select<TRowSet, TKey>(this Table table, Key key, TKey keyValue,
        Action<DbCommand, TKey> keyBinder, Func<DbDataReader, TRowSet> extractor)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = SqlHelper.GenerateSelect(table, key, QueryOptions.None);
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
        var sql = SqlHelper.GenerateInsert(table, table.Database.ProviderName, QueryOptions.SkipGenerated);
        return helper.Execute(sql, rowValue, rowBinder) > 0;
    }

    public static bool Insert<TRow>(this Table table, TRow rowValue) where TRow : class, new() =>
        Insert(table, rowValue, SqlHelper.FromEntity);

    public static bool Insert(this Table table, params object[] rowValues) =>
        Insert(table, rowValues, SqlHelper.FromArray);

    public static bool Update<TRow>(this Table table, TRow rowValue, Action<DbCommand, TRow> rowBinder)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = SqlHelper.GenerateUpdate(table, table.Database.ProviderName, QueryOptions.SkipGenerated);
        return helper.Execute(sql, rowValue, rowBinder) > 0;
    }

    public static bool Update<TRow>(this Table table, TRow rowValue) where TRow : class, new() =>
        Update(table, rowValue, SqlHelper.FromEntity);

    public static bool Delete<TKey>(this Table table, TKey keyValue, Action<DbCommand, TKey> keyBinder)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = SqlHelper.GenerateDelete(table, table.Database.ProviderName);
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
        var sql = SqlHelper.GenerateInsert(table, table.Database.ProviderName, QueryOptions.SkipGenerated);
        return helper.ExecuteBatch(sql, rowValues, rowBinder) > 0;
    }

    public static bool InsertBatch<TRow>(this Table table, IEnumerable<TRow> rowValues) where TRow : class, new() =>
        InsertBatch(table, rowValues, SqlHelper.FromEntity);

    public static bool UpdateBatch<TRow>(this Table table, IEnumerable<TRow> rowValues, Action<DbCommand, TRow> rowBinder)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = SqlHelper.GenerateUpdate(table, table.Database.ProviderName, QueryOptions.SkipGenerated);
        return helper.ExecuteBatch(sql, rowValues, rowBinder) > 0;
    }

    public static bool UpdateBatch<TRow>(this Table table, IEnumerable<TRow> rowValues) where TRow : class, new() =>
        UpdateBatch(table, rowValues, SqlHelper.FromEntity);

    public static bool DeleteBatch<TKey>(this Table table, IEnumerable<TKey> keyValues, Action<DbCommand, TKey> keyBinder)
    {
        using var helper = new SqlHelper(table.Database);
        var sql = SqlHelper.GenerateDelete(table, table.Database.ProviderName);
        return helper.ExecuteBatch(sql, keyValues, keyBinder) > 0;
    }

    public static bool DeleteBatch<TKey>(this Table table, IEnumerable<TKey> keyValues) where TKey : class, new() =>
        DeleteBatch(table, keyValues, SqlHelper.FromEntity);

    #endregion
}
