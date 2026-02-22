using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DbExport.Schema;

namespace DbExport.Providers.MySqlClient;

public partial class MySqlSchemaProvider : ISchemaProvider
{
    public MySqlSchemaProvider(string connectionString)
    {
        ConnectionString = connectionString;
            
        var properties = Utility.ParseConnectionString(connectionString);
        
        if (properties.TryGetValue("initial catalog", out var catalog))
            DatabaseName = catalog;
        else if (properties.TryGetValue("database", out var database))
            DatabaseName = database;
    }

    #region ISchemaProvider Members

    public string ProviderName => ProviderNames.MYSQL;

    public string ConnectionString { get; }

    public string DatabaseName { get; }

    public (string, string)[] GetTableNames()
    {
        const string sql = """
                           SELECT
                               TABLE_NAME
                           FROM
                               INFORMATION_SCHEMA.TABLES
                           WHERE
                               TABLE_TYPE = 'BASE TABLE' AND
                               TABLE_SCHEMA = '{0}'
                           ORDER BY
                               TABLE_NAME
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, DatabaseName), SqlHelper.ToList);
        return [..list.Select(item => (item.ToString(), string.Empty))];
    }

    public string[] GetColumnNames(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT
                               COLUMN_NAME
                           FROM
                               INFORMATION_SCHEMA.COLUMNS
                           WHERE
                               TABLE_SCHEMA = '{0}' AND
                               TABLE_NAME = '{1}'
                           ORDER BY
                               ORDINAL_POSITION
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, DatabaseName, tableName), SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public string[] GetIndexNames(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT
                               DISTINCT INDEX_NAME
                           FROM
                               INFORMATION_SCHEMA.STATISTICS
                           WHERE
                               TABLE_SCHEMA = '{0}' AND
                               TABLE_NAME = '{1}'
                           ORDER BY
                               INDEX_NAME
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, DatabaseName, tableName), SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public string[] GetFKNames(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT
                               CONSTRAINT_NAME
                           FROM
                               INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                           WHERE
                               CONSTRAINT_SCHEMA = '{0}' AND
                               TABLE_NAME = '{1}'
                           ORDER BY
                               CONSTRAINT_NAME
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, DatabaseName, tableName), SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public Dictionary<string, object> GetTableMeta(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT
                               PK.CONSTRAINT_NAME,
                               C.COLUMN_NAME
                           FROM
                               INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK
                           INNER JOIN
                               INFORMATION_SCHEMA.KEY_COLUMN_USAGE C ON
                                   PK.TABLE_SCHEMA = C.TABLE_SCHEMA AND
                                   PK.TABLE_NAME = C.TABLE_NAME AND
                                   PK.CONSTRAINT_NAME = C.CONSTRAINT_NAME
                           WHERE
                               PK.TABLE_SCHEMA = '{0}'AND
                               PK.TABLE_NAME = '{1}' AND
                               PK.CONSTRAINT_TYPE = 'PRIMARY KEY'
                           ORDER BY
                               C.ORDINAL_POSITION
                           """;

        Dictionary<string, object> metadata = new()
        {
            ["name"] = tableName,
            ["owner"] = tableOwner
        };
        
        List<string> pkColumns = [];
        var pkName = string.Empty;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, DatabaseName, tableName), SqlHelper.ToArrayList);
        
        foreach (var values in list)
        {
            pkName = values[0].ToString();
            pkColumns.Add(values[1].ToString());
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
                               DATA_TYPE,
                               CHARACTER_MAXIMUM_LENGTH,
                               NUMERIC_PRECISION,
                               NUMERIC_SCALE,
                               COLUMN_DEFAULT,
                               COLUMN_COMMENT,
                               IS_NULLABLE,
                               CHARACTER_SET_NAME,
                               EXTRA
                           FROM
                               INFORMATION_SCHEMA.COLUMNS
                           WHERE
                               TABLE_SCHEMA = '{0}' AND
                               TABLE_NAME = '{1}' AND
                               COLUMN_NAME = '{2}'
                           """;

        Dictionary<string, object> metadata = new()
        {
            ["name"] = columnName
        };

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var values = helper.Query(string.Format(sql, DatabaseName, tableName, columnName), SqlHelper.ToArray);
        var nativeType = values[0].ToString()!;

        if (UserTypeRegex().IsMatch(nativeType))
        {
            metadata["type"] = ColumnType.UserDefined;
            metadata["nativeType"] = $"{tableName}_{columnName}";
        }
        else
        {
            metadata["type"] = GetColumnType(nativeType);
            metadata["nativeType"] = nativeType;
        }

        metadata["size"] = Utility.ToInt16(values[1]);
        metadata["precision"] = Utility.ToByte(values[2]);
        metadata["scale"] = Utility.ToByte(values[3]);
        metadata["defaultValue"] = Parse(Convert.ToString(values[4]), (ColumnType)metadata["type"]);
        metadata["description"] = Convert.ToString(values[5]);

        var attributes = ColumnAttributes.None;
        
        if (values[6].Equals("NO"))
            attributes |= ColumnAttributes.Required;

        if (Utf8Regex().IsMatch(Convert.ToString(values[7])!))
            attributes |= ColumnAttributes.Unicode;

        if (values[8].Equals("auto_increment"))
        {
            attributes |= ColumnAttributes.Identity;
            metadata["ident_seed"] = metadata["ident_incr"] = 1L;
        }

        metadata["attributes"] = attributes;

        return metadata;
    }

    public Dictionary<string, object> GetIndexMeta(string tableName, string tableOwner, string indexName)
    {
        const string sql = """
                           SELECT
                               A.NON_UNIQUE,
                               A.COLUMN_NAME,
                               B.CONSTRAINT_TYPE
                           FROM
                               INFORMATION_SCHEMA.STATISTICS A LEFT OUTER JOIN
                               INFORMATION_SCHEMA.TABLE_CONSTRAINTS B ON
                                   A.TABLE_SCHEMA = B.TABLE_SCHEMA AND
                                   A.TABLE_NAME = B.TABLE_NAME AND
                                   A.INDEX_NAME = B.CONSTRAINT_NAME
                           WHERE
                               A.TABLE_SCHEMA = '{0}' AND
                               A.TABLE_NAME = '{1}' AND
                               A.INDEX_NAME = '{2}'
                           ORDER BY
                               A.SEQ_IN_INDEX
                           """;

        Dictionary<string, object> metadata = new()
        {
            ["name"] = indexName
        };

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, DatabaseName, tableName, indexName), SqlHelper.ToArrayList);
        List<string> indexColumns = [];
        var unique = false;
        var primaryKey = false;

        foreach (var values in list)
        {
            unique = values[0].Equals(0);
            indexColumns.Add(values[1].ToString());
            primaryKey = values[2].Equals("PRIMARY KEY");
        }

        metadata["unique"] = unique;
        metadata["primaryKey"] = primaryKey;
        metadata["columns"] = indexColumns.ToArray();

        return metadata;
    }

    public Dictionary<string, object> GetForeignKeyMeta(string tableName, string tableOwner, string fkName)
    {
        const string sql = """
                           SELECT
                               C.COLUMN_NAME,
                               C.REFERENCED_COLUMN_NAME,
                               C.REFERENCED_TABLE_NAME,
                               FK.UPDATE_RULE,
                               FK.DELETE_RULE
                           FROM
                               INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS FK
                           INNER JOIN
                               INFORMATION_SCHEMA.KEY_COLUMN_USAGE C ON
                               FK.CONSTRAINT_SCHEMA = C.CONSTRAINT_SCHEMA AND
                               FK.TABLE_NAME = C.TABLE_NAME AND
                               FK.CONSTRAINT_NAME = C.CONSTRAINT_NAME
                           WHERE
                               FK.CONSTRAINT_SCHEMA = '{0}' AND
                               FK.TABLE_NAME = '{1}' AND
                               FK.CONSTRAINT_NAME = '{2}'
                           ORDER BY
                               C.ORDINAL_POSITION
                           """;

        Dictionary<string, object> metadata = [];
        List<string> fkColumns = [];
        List<string> relatedColumns = [];
        var relatedTable = string.Empty;
        var updateRule = ForeignKeyRule.None;
        var deleteRule = ForeignKeyRule.None;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, DatabaseName, tableName, fkName), SqlHelper.ToArrayList);
        
        foreach (var values in list)
        {
            fkColumns.Add(values[0].ToString());
            relatedColumns.Add(values[1].ToString());
            relatedTable = values[2].ToString();
            updateRule = GetFKRule(values[3].ToString());
            deleteRule = GetFKRule(values[4].ToString());
        }

        metadata["name"] = fkName;
        metadata["columns"] = fkColumns.ToArray();
        metadata["relatedName"] = relatedTable;
        metadata["relatedOwner"] = string.Empty;
        metadata["relatedColumns"] = relatedColumns.ToArray();
        metadata["updateRule"] = updateRule;
        metadata["deleteRule"] = deleteRule;

        return metadata;
    }

    public (string, string)[] GetTypeNames()
    {
        const string sql = """
                           SELECT
                               CONCAT(C.TABLE_NAME, '_', C.COLUMN_NAME) AS DOMAIN_NAME
                           FROM
                               INFORMATION_SCHEMA.COLUMNS C
                           JOIN INFORMATION_SCHEMA.TABLES T ON
                               C.TABLE_SCHEMA = T.TABLE_SCHEMA
                                   AND C.TABLE_NAME = T.TABLE_NAME
                                   AND T.TABLE_TYPE = 'BASE TABLE'
                           WHERE
                               C.DATA_TYPE IN('enum', 'set') AND C.TABLE_SCHEMA = '{0}'
                           ORDER BY
                               DOMAIN_NAME
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, DatabaseName), SqlHelper.ToList);
        return [..list.Select(item => (item.ToString(), string.Empty))];
    }

    public Dictionary<string, object> GetTypeMeta(string typeName, string typeOwner)
    {
        const string sql = """
                           SELECT *
                           FROM INFORMATION_SCHEMA.COLUMNS
                           WHERE TABLE_SCHEMA = '{1}' AND CONCAT(TABLE_NAME, '_', COLUMN_NAME) = '{0}'
                           """;

        Dictionary<string, object> metadata = new()
        {
            ["name"] = typeName,
            ["owner"] = typeOwner
        };

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var values = helper.Query(string.Format(sql, typeName, DatabaseName), SqlHelper.ToDictionary);
        var nativeType = values["COLUMN_TYPE"].ToString();
        var isEnum = nativeType!.StartsWith("enum(", StringComparison.OrdinalIgnoreCase);
                
        metadata["nativeType"] = "varchar";
        metadata["type"] = ColumnType.VarChar;
        metadata["size"] = Utility.ToInt16(values["character_maximum_length"]);
        metadata["precision"] = Utility.ToByte(values["numeric_precision"]);
        metadata["scale"] = Utility.ToByte(values["numeric_scale"]);
        metadata["defaultValue"] = Parse(Convert.ToString(values["column_default"]), ColumnType.VarChar);
        metadata["nullable"] = "YES".Equals(values["is_nullable"].ToString(), StringComparison.OrdinalIgnoreCase);
        metadata["enumerated"] = isEnum;
        metadata["possibleValues"] = isEnum
            ? nativeType[5..^1].Split(',').Select(s => Parse(s, ColumnType.VarChar)).ToArray()
            : nativeType[4..^1].Split(',').Select(s => Parse(s, ColumnType.VarChar)).ToArray();

        return metadata;
    }

    #endregion

    #region Utility

    private static ColumnType GetColumnType(string mysqlType) =>
        mysqlType switch
        {
            "bool" or "tinyint" => ColumnType.TinyInt,
            "tinyint unsigned" or "year" => ColumnType.UnsignedTinyInt,
            "smallint" => ColumnType.SmallInt,
            "smallint unsigned" => ColumnType.UnsignedSmallInt,
            "int" or "integer" or "mediumint" => ColumnType.Integer,
            "int unsigned" or "integer unsigned" or "mediumint unsigned" => ColumnType.UnsignedInt,
            "bigint" or "serial" => ColumnType.BigInt,
            "bigint unsigned" => ColumnType.UnsignedBigInt,
            "float" => ColumnType.SinglePrecision,
            "double" or "real" => ColumnType.DoublePrecision,
            "money" or "smallmoney" => ColumnType.Currency,
            "dec" or "decimal" or "numeric" => ColumnType.Decimal,
            "date" => ColumnType.Date,
            "time" => ColumnType.Time,
            "datetime" or "timestamp" => ColumnType.DateTime,
            "char" => ColumnType.Char,
            "varchar" => ColumnType.VarChar,
            "text" or "tinytext" or "mediumtext" or "longtext" => ColumnType.Text,
            "bit" => ColumnType.Bit,
            "blob" or "tinyblob" or "mediumblob" or "longblob" => ColumnType.Blob,
            "json" => ColumnType.Json,
            "geometry" => ColumnType.Geometry,
            _ => ColumnType.Unknown
        };

    private static object Parse(string value, ColumnType columnType)
    {
        if (Utility.IsEmpty(value) || value.Equals("NULL", StringComparison.CurrentCultureIgnoreCase))
            return DBNull.Value;
        
        var ci = CultureInfo.InvariantCulture;

        return columnType switch
        {
            ColumnType.Boolean => value.ToLower() switch
            {
                "0" or "false" => false,
                "1" or "true" => true,
                _ => DBNull.Value
            },
            ColumnType.TinyInt => Utility.IsNumeric(value) ? Convert.ToSByte(value, ci) : DBNull.Value,
            ColumnType.UnsignedTinyInt => Utility.IsNumeric(value) ? Convert.ToByte(value) : DBNull.Value,
            ColumnType.SmallInt => Utility.IsNumeric(value) ? Convert.ToInt16(value, ci) : DBNull.Value,
            ColumnType.UnsignedSmallInt => Utility.IsNumeric(value) ? Convert.ToUInt16(value, ci) : DBNull.Value,
            ColumnType.Integer => Utility.IsNumeric(value) ? Convert.ToInt32(value, ci) : DBNull.Value,
            ColumnType.UnsignedInt => Utility.IsNumeric(value) ? Convert.ToUInt32(value, ci) : DBNull.Value,
            ColumnType.BigInt => Utility.IsNumeric(value) ? Convert.ToInt64(value, ci) : DBNull.Value,
            ColumnType.UnsignedBigInt => Utility.IsNumeric(value) ? Convert.ToUInt64(value, ci) : DBNull.Value,
            ColumnType.SinglePrecision => Utility.IsNumeric(value) ? Convert.ToSingle(value, ci) : DBNull.Value,
            ColumnType.DoublePrecision => Utility.IsNumeric(value) ? Convert.ToDouble(value, ci) : DBNull.Value,
            ColumnType.Currency or ColumnType.Decimal => Utility.IsNumeric(value)
                ? Convert.ToDecimal(value, ci)
                : DBNull.Value,
            ColumnType.Date or ColumnType.Time or ColumnType.DateTime => Utility.IsDate(value)
                ? DateTime.Parse(value.ToString(ci), ci)
                : DBNull.Value,
            ColumnType.Char or ColumnType.VarChar or ColumnType.Text => Utility.UnquotedStr(value),
            ColumnType.Bit => Utility.FromBitString(value),
            _ => DBNull.Value
        };
    }

    private static ForeignKeyRule GetFKRule(string rule) =>
        rule switch
        {
            "RESTRICT" => ForeignKeyRule.Restrict,
            "CASCADE" => ForeignKeyRule.Cascade,
            "SET DEFAULT" => ForeignKeyRule.SetDefault,
            "SET NULL" => ForeignKeyRule.SetNull,
            _ => ForeignKeyRule.None
        };
    
    [GeneratedRegex("(enum|set)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex UserTypeRegex();
    
    [GeneratedRegex("utf8", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex Utf8Regex();

    #endregion
}