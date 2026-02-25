using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DbExport.Schema;

namespace DbExport.Providers.Firebird;

public class FirebirdSchemaProvider : ISchemaProvider
{
    public FirebirdSchemaProvider(string connectionString)
    {
        ConnectionString = connectionString;

        var properties = Utility.ParseConnectionString(connectionString);
        DatabaseName = Path.GetFileNameWithoutExtension(properties["initial catalog"]);
    }

    #region ISchemaProvider Members

    public string ProviderName => ProviderNames.FIREBIRD;

    public string ConnectionString { get; }

    public string DatabaseName { get; }

    public (string, string)[] GetTableNames()
    {
        const string sql = """
            SELECT TRIM(RDB$RELATION_NAME)
            FROM RDB$RELATIONS
            WHERE RDB$SYSTEM_FLAG = 0
              AND RDB$VIEW_BLR IS NULL
            ORDER BY RDB$RELATION_NAME
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(sql, SqlHelper.ToList);

        return [.. list.Select(t => (t.ToString(), string.Empty))];
    }

    public string[] GetColumnNames(string tableName, string tableOwner)
    {
        const string sql = """
            SELECT TRIM(RDB$FIELD_NAME)
            FROM RDB$RELATION_FIELDS
            WHERE RDB$RELATION_NAME = '{0}'
            ORDER BY RDB$FIELD_POSITION
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName), SqlHelper.ToList);

        return [.. list.Select(c => c.ToString())];
    }

    public string[] GetIndexNames(string tableName, string tableOwner)
    {
        const string sql = """
            SELECT TRIM(RDB$INDEX_NAME)
            FROM RDB$INDICES
            WHERE RDB$RELATION_NAME = '{0}'
            ORDER BY RDB$INDEX_NAME
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName), SqlHelper.ToList);

        return [.. list.Select(i => i.ToString())];
    }

    public string[] GetFKNames(string tableName, string tableOwner)
    {
        const string sql = """
            SELECT TRIM(RDB$CONSTRAINT_NAME)
            FROM RDB$RELATION_CONSTRAINTS
            WHERE RDB$RELATION_NAME = '{0}'
              AND RDB$CONSTRAINT_TYPE = 'FOREIGN KEY'
            ORDER BY RDB$CONSTRAINT_NAME
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName), SqlHelper.ToList);

        return [.. list.Select(fk => fk.ToString())];
    }

    public Dictionary<string, object> GetTableMeta(string tableName, string tableOwner)
    {
        const string sql = """
            SELECT
                TRIM(rc.RDB$CONSTRAINT_NAME),
                TRIM(seg.RDB$FIELD_NAME)
            FROM RDB$RELATION_CONSTRAINTS rc
            JOIN RDB$INDEX_SEGMENTS seg
                ON rc.RDB$INDEX_NAME = seg.RDB$INDEX_NAME
            WHERE rc.RDB$RELATION_NAME = '{0}'
              AND rc.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY'
            ORDER BY seg.RDB$FIELD_POSITION
            """;

        Dictionary<string, object> metadata = new()
        {
            ["name"] = tableName,
            ["owner"] = tableOwner
        };

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName), SqlHelper.ToArrayList);

        List<string> pkColumns = [];
        string pkName = string.Empty;

        foreach (var row in list)
        {
            pkName = row[0].ToString();
            pkColumns.Add(row[1].ToString());
        }

        if (!string.IsNullOrEmpty(pkName))
        {
            metadata["pk_name"] = pkName;
            metadata["pk_columns"] = pkColumns.ToArray();
        }

        return metadata;
    }

    public Dictionary<string, object> GetColumnMeta(string tableName, string tableOwner, string columnName)
    {
        const string sql = """
            SELECT
                f.RDB$FIELD_TYPE,
                f.RDB$FIELD_LENGTH,
                f.RDB$FIELD_PRECISION,
                f.RDB$FIELD_SCALE,
                rf.RDB$NULL_FLAG,
                f.RDB$DEFAULT_SOURCE
            FROM RDB$RELATION_FIELDS rf
            JOIN RDB$FIELDS f
                ON rf.RDB$FIELD_SOURCE = f.RDB$FIELD_NAME
            WHERE rf.RDB$RELATION_NAME = '{0}'
              AND rf.RDB$FIELD_NAME = '{1}'
            """;

        Dictionary<string, object> metadata = new()
        {
            ["name"] = columnName,
            ["description"] = string.Empty
        };

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var values = helper.Query(string.Format(sql, tableName, columnName), SqlHelper.ToArray);

        var fieldType = Convert.ToInt32(values[0]);
        var length = Utility.ToInt16(values[1]);
        var precision = Utility.ToByte(values[2]);
        var scale = (byte)Math.Abs(Convert.ToInt32(values[3]));

        metadata["type"] = GetColumnType(fieldType);
        metadata["nativeType"] = metadata["type"].ToString().ToUpper();
        metadata["size"] = length;
        metadata["precision"] = precision;
        metadata["scale"] = scale;

        metadata["defaultValue"] = DBNull.Value;

        var attributes = ColumnAttributes.None;

        if (values[4] != DBNull.Value)
            attributes |= ColumnAttributes.Required;

        metadata["attributes"] = attributes;

        return metadata;
    }

    public Dictionary<string, object> GetIndexMeta(string tableName, string tableOwner, string indexName)
    {
        const string sql = """
            SELECT
                i.RDB$UNIQUE_FLAG,
                seg.RDB$FIELD_NAME
            FROM RDB$INDICES i
            JOIN RDB$INDEX_SEGMENTS seg
                ON i.RDB$INDEX_NAME = seg.RDB$INDEX_NAME
            WHERE i.RDB$RELATION_NAME = '{0}'
              AND i.RDB$INDEX_NAME = '{1}'
            ORDER BY seg.RDB$FIELD_POSITION
            """;

        Dictionary<string, object> metadata = new()
        {
            ["name"] = indexName
        };

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, indexName), SqlHelper.ToArrayList);

        List<string> columns = [];
        bool unique = false;

        foreach (var row in list)
        {
            unique = row[0] != DBNull.Value;
            columns.Add(row[1].ToString().Trim());
        }

        metadata["unique"] = unique;
        metadata["primaryKey"] = false;
        metadata["columns"] = columns.ToArray();

        return metadata;
    }

    public Dictionary<string, object> GetForeignKeyMeta(string tableName, string tableOwner, string fkName)
    {
        const string sql = """
            SELECT
                seg.RDB$FIELD_NAME,
                refseg.RDB$FIELD_NAME,
                rc2.RDB$RELATION_NAME,
                refc.RDB$UPDATE_RULE,
                refc.RDB$DELETE_RULE
            FROM RDB$RELATION_CONSTRAINTS rc
            JOIN RDB$REF_CONSTRAINTS refc
                ON rc.RDB$CONSTRAINT_NAME = refc.RDB$CONSTRAINT_NAME
            JOIN RDB$RELATION_CONSTRAINTS rc2
                ON refc.RDB$CONST_NAME_UQ = rc2.RDB$CONSTRAINT_NAME
            JOIN RDB$INDEX_SEGMENTS seg
                ON rc.RDB$INDEX_NAME = seg.RDB$INDEX_NAME
            JOIN RDB$INDEX_SEGMENTS refseg
                ON rc2.RDB$INDEX_NAME = refseg.RDB$INDEX_NAME
               AND seg.RDB$FIELD_POSITION = refseg.RDB$FIELD_POSITION
            WHERE rc.RDB$RELATION_NAME = '{0}'
              AND rc.RDB$CONSTRAINT_NAME = '{1}'
            ORDER BY seg.RDB$FIELD_POSITION
            """;

        Dictionary<string, object> metadata = [];

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, fkName), SqlHelper.ToArrayList);

        List<string> columns = [];
        List<string> relatedColumns = [];
        string relatedTable = string.Empty;
        ForeignKeyRule updateRule = ForeignKeyRule.None;
        ForeignKeyRule deleteRule = ForeignKeyRule.None;

        foreach (var row in list)
        {
            columns.Add(row[0].ToString().Trim());
            relatedColumns.Add(row[1].ToString().Trim());
            relatedTable = row[2].ToString().Trim();
            updateRule = GetFKRule(row[3]?.ToString());
            deleteRule = GetFKRule(row[4]?.ToString());
        }

        metadata["name"] = fkName;
        metadata["columns"] = columns.ToArray();
        metadata["relatedName"] = relatedTable;
        metadata["relatedOwner"] = string.Empty;
        metadata["relatedColumns"] = relatedColumns.ToArray();
        metadata["updateRule"] = updateRule;
        metadata["deleteRule"] = deleteRule;

        return metadata;
    }

    #endregion

    #region Utilities

    private static ColumnType GetColumnType(int fbType) =>
        fbType switch
        {
            7 => ColumnType.SmallInt,
            8 => ColumnType.Integer,
            16 => ColumnType.BigInt,
            10 => ColumnType.SinglePrecision,
            27 => ColumnType.DoublePrecision,
            12 => ColumnType.Date,
            13 => ColumnType.Time,
            35 => ColumnType.DateTime,
            14 => ColumnType.Char,
            37 => ColumnType.VarChar,
            261 => ColumnType.Blob,
            _ => ColumnType.Unknown
        };

    private static ForeignKeyRule GetFKRule(string rule) =>
        rule?.ToUpperInvariant() switch
        {
            "RESTRICT" => ForeignKeyRule.Restrict,
            "CASCADE" => ForeignKeyRule.Cascade,
            "SET NULL" => ForeignKeyRule.SetNull,
            "SET DEFAULT" => ForeignKeyRule.SetDefault,
            _ => ForeignKeyRule.None
        };

    #endregion
}