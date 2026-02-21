using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DbExport.Schema;

namespace DbExport.Providers.Npgsql;

public partial class NpgsqlSchemaProvider : ISchemaProvider
{
    public NpgsqlSchemaProvider(string connectionString)
    {
        ConnectionString = connectionString;
            
        var properties = Utility.ParseConnectionString(connectionString);
        DatabaseName = properties["database"];
    }

    #region ISchemaProvider Members

    public string ProviderName => ProviderNames.POSTGRESQL;

    public string ConnectionString { get; }

    public string DatabaseName { get; }

    public (string, string)[] GetTableNames()
    {
        const string sql = """
                           SELECT TABLE_SCHEMA, TABLE_NAME
                           FROM INFORMATION_SCHEMA.TABLES
                           WHERE TABLE_TYPE = 'BASE TABLE'
                             AND TABLE_SCHEMA NOT IN ('pg_catalog', 'information_schema')
                             AND HAS_TABLE_PRIVILEGE(
                                FORMAT('%I.%I', TABLE_SCHEMA, TABLE_NAME),
                                'SELECT')
                           ORDER BY TABLE_SCHEMA, TABLE_NAME
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(sql, SqlHelper.ToArrayList);
        return [..list.Select(item => (item[1].ToString(), item[0].ToString()))];
    }

    public string[] GetColumnNames(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT
                           	    COLUMN_NAME
                           FROM
                           	    INFORMATION_SCHEMA.COLUMNS
                           WHERE
                           	    TABLE_SCHEMA = '{1}'
                           	    AND TABLE_NAME = '{0}'
                           ORDER BY
                           	    ORDINAL_POSITION
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public string[] GetIndexNames(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT
                           	    INDEXNAME
                           FROM
                           	    PG_CATALOG.PG_INDEXES
                           WHERE
                           	    SCHEMANAME = '{1}'
                           	    AND TABLENAME = '{0}'
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public string[] GetFKNames(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT
                           	    CONSTRAINT_NAME
                           FROM
                           	    INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                           WHERE
                           	    CONSTRAINT_TYPE = 'FOREIGN KEY'
                           	    AND TABLE_SCHEMA = '{1}'
                           	    AND TABLE_NAME = '{0}'
                           ORDER BY
                           	    CONSTRAINT_NAME
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToList);
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
                           	    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE C
                           	    ON PK.TABLE_SCHEMA = C.TABLE_SCHEMA
                           	    AND PK.TABLE_NAME = C.TABLE_NAME
                           	    AND PK.CONSTRAINT_NAME = C.CONSTRAINT_NAME
                           WHERE
                           	    PK.CONSTRAINT_TYPE = 'PRIMARY KEY'
                           	    AND PK.TABLE_SCHEMA = '{1}'
                           	    AND PK.TABLE_NAME = '{0}'
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
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToArrayList);
        
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
                           	    CHARACTER_SET_NAME,
                           	    IS_NULLABLE,
                           	    IS_IDENTITY,
                           	    IDENTITY_START,
                           	    IDENTITY_INCREMENT,
                           	    IS_GENERATED,
                           	    GENERATION_EXPRESSION
                           FROM
                           	    INFORMATION_SCHEMA.COLUMNS
                           WHERE
                           	    TABLE_SCHEMA = '{1}'
                           	    AND TABLE_NAME = '{0}'
                           	    AND COLUMN_NAME = '{2}'
                           """;

        Dictionary<string, object> metadata = new()
        {
            ["name"] = columnName,
            ["description"] = string.Empty
        };

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        ColumnType columnType;
        var attributes = ColumnAttributes.None;
        var values = helper.Query(string.Format(sql, tableName, tableOwner, columnName), SqlHelper.ToArray);
                
        metadata["type"] = columnType = GetColumnType(values[0].ToString());
        metadata["nativeType"] = values[0].ToString();
        metadata["size"] = Utility.ToInt16(values[1]);
        metadata["precision"] = Utility.ToByte(values[2]);
        metadata["scale"] = Utility.ToByte(values[3]);
        metadata["defaultValue"] = Parse(Convert.ToString(values[4]), columnType);

        if (Utf8Regex().IsMatch(Convert.ToString(values[5])!))
            attributes |= ColumnAttributes.Unicode;

        if (values[6].Equals("NO"))
            attributes |= ColumnAttributes.Required;

        if (values[7].Equals("YES"))
        {
            attributes |= ColumnAttributes.Identity;
            metadata["ident_seed"] = Convert.ToString(values[8]);
            metadata["ident_incr"] = Convert.ToString(values[9]);
        }
        else if (values[0].Equals("serial"))
        {
            attributes |= ColumnAttributes.Identity;
            metadata["ident_seed"] = metadata["ident_incr"] = 1L;
        }

        if (values[10].Equals("YES"))
        {
            attributes |= ColumnAttributes.Computed;
            metadata["expression"] = Convert.ToString(values[11]);
        }

        metadata["attributes"] = attributes;

        return metadata;
    }

    public Dictionary<string, object> GetIndexMeta(string tableName, string tableOwner, string indexName)
    {
        const string sql1 = """
                            SELECT
                            	INDEXDEF
                            FROM
                            	PG_CATALOG.PG_INDEXES
                            WHERE
                            	SCHEMANAME = '{1}'
                            	AND TABLENAME = '{0}'
                            	AND INDEXNAME = '{2}'
                            """;

        const string sql2 = """
                            SELECT
                                COUNT(*)
                            FROM
                                INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                            WHERE
                                TABLE_SCHEMA = '{1}'
                                AND TABLE_NAME = '{0}'
                                AND CONSTRAINT_NAME = '{2}'
                                AND CONSTRAINT_TYPE = 'PRIMARY KEY'
                            """;

        Dictionary<string, object> metadata = new()
        {
            ["name"] = indexName
        };

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var def = helper.QueryScalar(string.Format(sql1, tableName, tableOwner, indexName)).ToString();
        var lparen = def!.IndexOf('(');
        var rparen = def.LastIndexOf(')');
        var columnNames = def.Substring(lparen + 1, rparen - lparen - 1);

        metadata["unique"] = def.StartsWith("CREATE UNIQUE INDEX");
        metadata["columns"] = CommaRegex().Split(columnNames);
        metadata["primaryKey"] = helper.QueryScalar(string.Format(sql2, tableName, tableOwner, indexName)).Equals(1);

        return metadata;
    }

    public Dictionary<string, object> GetForeignKeyMeta(string tableName, string tableOwner, string fkName)
    {
        const string sql = """
                           SELECT
                               KCU1.COLUMN_NAME FK_COLUMN_NAME,
                               KCU2.COLUMN_NAME PK_COLUMN_NAME,
                               TC2.TABLE_NAME PK_TABLE_NAME,
                               TC2.TABLE_SCHEMA PK_TABLE_OWNER,
                               RC.UPDATE_RULE,
                               RC.DELETE_RULE
                           FROM
                               INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
                               JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC1
                                   ON RC.CONSTRAINT_SCHEMA = TC1.CONSTRAINT_SCHEMA
                                   AND RC.CONSTRAINT_NAME = TC1.CONSTRAINT_NAME
                               JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU1
                                   ON TC1.CONSTRAINT_SCHEMA = KCU1.CONSTRAINT_SCHEMA
                                   AND TC1.CONSTRAINT_NAME = KCU1.CONSTRAINT_NAME
                               JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC2
                                   ON RC.CONSTRAINT_SCHEMA = TC2.CONSTRAINT_SCHEMA
                                   AND RC.UNIQUE_CONSTRAINT_NAME = TC2.CONSTRAINT_NAME
                               JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU2
                                   ON TC2.CONSTRAINT_SCHEMA = KCU2.CONSTRAINT_SCHEMA
                                   AND TC2.CONSTRAINT_NAME = KCU2.CONSTRAINT_NAME
                                   AND KCU1.ORDINAL_POSITION = KCU2.ORDINAL_POSITION
                           WHERE
                               TC1.TABLE_SCHEMA = '{1}'
                               AND TC1.TABLE_NAME = '{0}'
                               AND TC1.CONSTRAINT_NAME = '{2}'
                           ORDER BY
                               KCU1.ORDINAL_POSITION
                           """;

        Dictionary<string, object> metadata = [];
        List<string> fkColumns = [];
        List<string> relatedColumns = [];
        var relatedTable = string.Empty;
        var relatedOwner = string.Empty;
        var updateRule = ForeignKeyRule.None;
        var deleteRule = ForeignKeyRule.None;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner, fkName), SqlHelper.ToArrayList);
        
        foreach (object[] values in list)
        {
            fkColumns.Add(values[0].ToString());
            relatedColumns.Add(values[1].ToString());
            relatedTable = values[2].ToString();
            relatedOwner = values[3].ToString();
            updateRule = GetFKRule(values[4].ToString());
            deleteRule = GetFKRule(values[5].ToString());
        }

        metadata["name"] = fkName;
        metadata["columns"] = fkColumns.ToArray();
        metadata["relatedName"] = relatedTable;
        metadata["relatedOwner"] = relatedOwner;
        metadata["relatedColumns"] = relatedColumns.ToArray();
        metadata["updateRule"] = updateRule;
        metadata["deleteRule"] = deleteRule;

        return metadata;
    }

    #endregion

    #region Utility

    private static ColumnType GetColumnType(string npgsqlType) =>
        npgsqlType switch
        {
            "bool" or "boolean" => ColumnType.Boolean,
            "int2" or "smallint" => ColumnType.SmallInt,
            "int" or "int4" or "integer" => ColumnType.Integer,
            "int8" or "bigint" or "serial" => ColumnType.BigInt,
            "real" or "float4" => ColumnType.SinglePrecision,
            "double precision" or "float8" => ColumnType.DoublePrecision,
            "money" => ColumnType.Currency,
            "numeric" => ColumnType.Decimal,
            "date" => ColumnType.Date,
            "time" => ColumnType.Time,
            "datetime" => ColumnType.DateTime,
            "interval" => ColumnType.Interval,
            "char" or "character" or "bpchar" => ColumnType.Char,
            "varchar" or "character varying" => ColumnType.VarChar,
            "text" => ColumnType.Text,
            "bit" or "bit varying" or "varbit" => ColumnType.Bit,
            "bytea" => ColumnType.Blob,
            "uuid" => ColumnType.Guid,
            "xml" => ColumnType.Xml,
            "json" or "jsonb" => ColumnType.Json,
            "geometry" or "geography" => ColumnType.Geometry,
            "user-defined" => ColumnType.UserDefined,
            _ when npgsqlType.StartsWith("timestamp") => ColumnType.DateTime,
            _ => ColumnType.Unknown
        };

    private static object Parse(string value, ColumnType columnType)
    {
        if (Utility.IsEmpty(value) || value.ToUpper() == "NULL")
            return DBNull.Value;

        return columnType switch
        {
            ColumnType.Boolean => value.ToLower() switch
            {
                "0" or "n" or "no" or "f" or "false" => false,
                "1" or "y" or "yes" or "t" or "true" => true,
                _ => DBNull.Value
            },
            ColumnType.SmallInt => Utility.IsNumeric(value) ? Convert.ToInt16(value) : DBNull.Value,
            ColumnType.Integer => Utility.IsNumeric(value) ? Convert.ToInt32(value) : DBNull.Value,
            ColumnType.BigInt => Utility.IsNumeric(value) ? Convert.ToInt64(value) : DBNull.Value,
            ColumnType.SinglePrecision => Utility.IsNumeric(value) ? Convert.ToSingle(value) : DBNull.Value,
            ColumnType.DoublePrecision => Utility.IsNumeric(value) ? Convert.ToDouble(value) : DBNull.Value,
            ColumnType.Currency or ColumnType.Decimal => Utility.IsNumeric(value)
                ? Convert.ToDecimal(value)
                : DBNull.Value,
            ColumnType.Date or ColumnType.Time or ColumnType.DateTime => Utility.IsDate(value)
                ? Convert.ToDateTime(value)
                : DBNull.Value,
            ColumnType.Char or ColumnType.VarChar or ColumnType.Text => value,
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
    
    [GeneratedRegex(@"\s*\,\s*", RegexOptions.Compiled)]
    private static partial Regex CommaRegex();
    
    [GeneratedRegex("utf8", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex Utf8Regex();

    #endregion
}