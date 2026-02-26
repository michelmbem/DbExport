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

    public NameOwnerPair[] GetTableNames()
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

        return [..list.Select(t => new NameOwnerPair(t.ToString()))];
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

        return [..list.Select(c => c.ToString())];
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

        return [..list.Select(i => i.ToString())];
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

        return [..list.Select(fk => fk.ToString())];
    }

    public MetaData GetTableMeta(string tableName, string tableOwner)
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

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName), SqlHelper.ToArrayList);

        string pkName = null;
        List<string> pkColumns = [];
        MetaData metadata = new()
        {
            ["name"] = tableName,
            ["owner"] = tableOwner
        };

        foreach (var row in list)
        {
            pkName ??= row[0].ToString();
            pkColumns.Add(row[1].ToString());
        }

        if (!string.IsNullOrEmpty(pkName))
        {
            metadata["pk_name"] = pkName;
            metadata["pk_columns"] = pkColumns.ToArray();
        }

        return metadata;
    }

    public MetaData GetColumnMeta(string tableName, string tableOwner, string columnName)
    {
        const string sql = """
            SELECT
                rf.RDB$FIELD_SOURCE,
                f.RDB$FIELD_TYPE,
                f.RDB$FIELD_SUB_TYPE,
                f.RDB$CHARACTER_LENGTH,
                f.RDB$FIELD_PRECISION,
                f.RDB$FIELD_SCALE,
                rf.RDB$NULL_FLAG,
                rf.RDB$DEFAULT_SOURCE,
                rf.RDB$IDENTITY_TYPE,
                g.RDB$INITIAL_VALUE,
                g.RDB$GENERATOR_INCREMENT
            FROM RDB$RELATION_FIELDS rf
            JOIN RDB$FIELDS f
                ON rf.RDB$FIELD_SOURCE = f.RDB$FIELD_NAME
            LEFT JOIN RDB$GENERATORS g
                ON rf.RDB$GENERATOR_NAME = g.RDB$GENERATOR_NAME
            WHERE rf.RDB$RELATION_NAME = '{0}'
              AND rf.RDB$FIELD_NAME = '{1}'
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var values = helper.Query(string.Format(sql, tableName, columnName), SqlHelper.ToArray);

        var fieldSource = values[0].ToString().Trim();
        var fieldType = Convert.ToInt32(values[1]);
        var subType = Convert.IsDBNull(values[2]) ? (int?)null : Convert.ToInt32(values[2]);
        var length = Utility.ToInt16(values[3]);
        var precision = Utility.ToByte(values[4]);
        var scale = (byte)Math.Abs(Convert.ToInt32(values[5]));

        MetaData metadata = new()
        {
            ["name"] = columnName,
            ["size"] = length,
            ["precision"] = precision,
            ["scale"] = scale,
            ["defaultValue"] = DBNull.Value,
            ["description"] = string.Empty,
        };

        if (fieldSource.StartsWith("RDB$", StringComparison.OrdinalIgnoreCase))
        {
            metadata["type"] = ResolveColumnType(fieldType, subType, scale);
            metadata["nativeType"] = GetNativeTypeName(fieldType, subType, scale);
        }
        else
        {
            metadata["type"] = ColumnType.UserDefined;
            metadata["nativeType"] = fieldSource;
        }

        var attributes = ColumnAttributes.None;

        if (values[6] != DBNull.Value)
            attributes |= ColumnAttributes.Required;

        if (values[8] != DBNull.Value)
        {
            attributes |= ColumnAttributes.Identity;

            metadata["ident_seed"] = values[9] != DBNull.Value
                ? Convert.ToInt64(values[9])
                : 0L;

            metadata["ident_incr"] = values[10] != DBNull.Value
                ? Convert.ToInt64(values[10])
                : 1L;
        }

        metadata["attributes"] = attributes;

        return metadata;
    }

    public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)
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

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, indexName), SqlHelper.ToArrayList);

        List<string> columns = [];
        bool unique = false;

        foreach (var row in list)
        {
            unique = row[0] != DBNull.Value;
            columns.Add(row[1].ToString().Trim());
        }

        return new MetaData()
        {
            ["name"] = indexName,
            ["unique"] = unique,
            ["primaryKey"] = false,
            ["columns"] = columns.ToArray()
        };
    }

    public MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName)
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

        return new MetaData
        {
            ["name"] = fkName,
            ["columns"] = columns.ToArray(),
            ["relatedName"] = relatedTable,
            ["relatedOwner"] = string.Empty,
            ["relatedColumns"] = relatedColumns.ToArray(),
            ["updateRule"] = updateRule,
            ["deleteRule"] = deleteRule
        };
    }

    public NameOwnerPair[] GetTypeNames()
    {
        const string sql = """
            SELECT TRIM(RDB$FIELD_NAME)
            FROM RDB$FIELDS
            WHERE COALESCE(RDB$SYSTEM_FLAG, 0) = 0
                AND NOT (RDB$FIELD_NAME STARTING WITH 'RDB$')
            ORDER BY RDB$FIELD_NAME
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(sql, SqlHelper.ToList);

        return [.. list.Select(t => new NameOwnerPair(t.ToString()))];
    }

    public MetaData GetTypeMeta(string typeName, string typeOwner)
    {
        const string sql = """
            SELECT
                RDB$FIELD_TYPE,
                RDB$FIELD_SUB_TYPE,
                RDB$CHARACTER_LENGTH,
                RDB$FIELD_PRECISION,
                RDB$FIELD_SCALE,
                RDB$DEFAULT_SOURCE,
                RDB$NULL_FLAG
            FROM RDB$FIELDS
            WHERE RDB$FIELD_NAME = '{0}'
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var values = helper.Query(string.Format(sql, typeName), SqlHelper.ToArray);

        var fieldType = Convert.ToInt32(values[0]);
        var subType = Convert.IsDBNull(values[1]) ? (int?)null : Convert.ToInt32(values[1]);
        var length = Utility.ToInt16(values[2]);
        var precision = Utility.ToByte(values[3]);
        var scale = (byte)Math.Abs(Convert.ToInt32(values[4]));

        return new MetaData()
        {
            ["name"] = typeName,
            ["owner"] = typeOwner,
            ["type"] = ResolveColumnType(fieldType, subType, scale),
            ["nativeType"] = GetNativeTypeName(fieldType, subType, scale),
            ["size"] = length,
            ["precision"] = precision,
            ["scale"] = scale,
            ["defaultValue"] = DBNull.Value,
            ["nullable"] = values[6] == DBNull.Value,
            ["enumerated"] = false,
            ["possibleValues"] = Array.Empty<string>()
        };
    }

    #endregion

    #region Utilities

    private static ColumnType ResolveColumnType(int fbType, int? subType, int scale) =>
       fbType switch
       {
           7 => scale < 0 ? ColumnType.Decimal : ColumnType.SmallInt,
           8 => scale < 0 ? ColumnType.Decimal : ColumnType.Integer,
           16 => subType switch
           {
               1 or 2 => ColumnType.Decimal,
               _ => ColumnType.BigInt
           },
           10 => ColumnType.SinglePrecision,
           27 => ColumnType.DoublePrecision,
           12 => ColumnType.Date,
           13 => ColumnType.Time,
           28 => ColumnType.Time,
           35 => ColumnType.DateTime,
           29 => ColumnType.DateTime,
           14 => ColumnType.Char,
           37 => ColumnType.VarChar,
           261 => subType == 1 ? ColumnType.Text : ColumnType.Blob,
           _ => ColumnType.Unknown
       };

    private static string GetNativeTypeName(int fbType, int? subType, byte scale) =>
        fbType switch
        {
            7 => scale > 0 ? "NUMERIC" : "SMALLINT",
            8 => scale > 0 ? "NUMERIC" : "INTEGER",
            16 => subType switch
            {
                1 => "NUMERIC",
                2 => "DECIMAL",
                _ => "BIGINT"
            },
            10 => "FLOAT",
            27 => "DOUBLE PRECISION",
            12 => "DATE",
            13 => "TIME",
            28 => "TIME WITH TIME ZONE",
            35 => "TIMESTAMP",
            29 => "TIMESTAMP WITH TIME ZONE",
            14 => "CHAR",
            37 => "VARCHAR",
            261 => subType switch
            {
                1 => "BLOB SUB_TYPE TEXT",
                _ => "BLOB"
            },
            _ => "UNKNOWN"
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