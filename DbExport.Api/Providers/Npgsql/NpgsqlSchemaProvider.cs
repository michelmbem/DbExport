using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DbExport.Schema;

namespace DbExport.Providers.Npgsql;

public class NpgsqlSchemaProvider : ISchemaProvider
{
    private readonly string connectionString;
    private readonly string databaseName;

    public NpgsqlSchemaProvider(string connectionString)
    {
        this.connectionString = connectionString;
            
        var properties = Utility.ParseConnectionString(connectionString);
        databaseName = properties["database"];
    }

    #region ISchemaProvider Members

    public string ProviderName => ProviderNames.POSTGRESQL;

    public string ConnectionString => connectionString;

    public string DatabaseName => databaseName;

    public string[] GetTableNames()
    {
        const string sql = """
                           SELECT
                                TABLE_NAME
                           FROM
                                INFORMATION_SCHEMA.TABLES
                           WHERE
                                TABLE_SCHEMA = 'public'
                           ORDER BY
                                TABLE_NAME
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(sql, SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public string[] GetColumnNames(string tableName)
    {
        const string sql = """
                           SELECT
                           	    COLUMN_NAME
                           FROM
                           	    INFORMATION_SCHEMA.COLUMNS
                           WHERE
                           	    TABLE_SCHEMA = 'public'
                           	    AND TABLE_NAME = '{0}'
                           ORDER BY
                           	    ORDINAL_POSITION
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName), SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public string[] GetIndexNames(string tableName)
    {
        const string sql = """
                           SELECT
                           	    INDEXNAME
                           FROM
                           	    PG_CATALOG.PG_INDEXES
                           WHERE
                           	    SCHEMANAME = 'public'
                           	    AND TABLENAME = '{0}'
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName), SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public string[] GetFKNames(string tableName)
    {
        const string sql = """
                           SELECT
                           	    CONSTRAINT_NAME
                           FROM
                           	    INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                           WHERE
                           	    CONSTRAINT_TYPE = 'FOREIGN KEY'
                           	    AND TABLE_SCHEMA = 'public'
                           	    AND TABLE_NAME = '{0}'
                           ORDER BY
                           	    CONSTRAINT_NAME
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName), SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public Dictionary<string, object> GetTableMeta(string tableName)
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
                           	    AND PK.TABLE_SCHEMA = 'public'
                           	    AND PK.TABLE_NAME = '{0}'
                           ORDER BY
                           	    C.ORDINAL_POSITION
                           """;

        Dictionary<string, object> metadata = new ()
        {
            ["name"] = tableName,
            ["owner"] = "public"
        };

        List<string> pkColumns = [];
        var pkName = string.Empty;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName), SqlHelper.ToArrayList);
        
        foreach (object[] values in list)
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

    public Dictionary<string, object> GetColumnMeta(string tableName, string columnName)
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
                           	    TABLE_SCHEMA = 'public'
                           	    AND TABLE_NAME = '{0}'
                           	    AND COLUMN_NAME = '{1}'
                           """;

        Dictionary<string, object> metadata = new ()
        {
            ["name"] = columnName,
            ["description"] = string.Empty
        };

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        ColumnType columnType;
        var attributes = ColumnAttribute.None;
        var values = helper.Query(string.Format(sql, tableName, columnName), SqlHelper.ToArray);
                
        metadata["type"] = columnType = GetColumnType(values[0].ToString());
        metadata["nativeType"] = values[0].ToString();
        metadata["size"] = Utility.ToInt16(values[1]);
        metadata["precision"] = Utility.ToByte(values[2]);
        metadata["scale"] = Utility.ToByte(values[3]);
        metadata["defaultValue"] = Parse(Convert.ToString(values[4]), columnType);

        if (Regex.IsMatch(Convert.ToString(values[5]), "utf8", RegexOptions.IgnoreCase))
            attributes |= ColumnAttribute.Unicode;

        if (values[6].Equals("NO"))
            attributes |= ColumnAttribute.Required;

        if (values[7].Equals("YES"))
        {
            attributes |= ColumnAttribute.Identity;
            metadata["ident_seed"] = Convert.ToString(values[8]);
            metadata["ident_incr"] = Convert.ToString(values[9]);
        }
        else if (values[0].Equals("serial"))
        {
            attributes |= ColumnAttribute.Identity;
            metadata["ident_seed"] = metadata["ident_incr"] = 1L;
        }

        if (values[10].Equals("YES"))
        {
            attributes |= ColumnAttribute.Computed;
            metadata["expression"] = Convert.ToString(values[11]);
        }

        metadata["attributes"] = attributes;

        return metadata;
    }

    public Dictionary<string, object> GetIndexMeta(string tableName, string indexName)
    {
        const string sql1 = """
                            SELECT
                            	INDEXDEF
                            FROM
                            	PG_CATALOG.PG_INDEXES
                            WHERE
                            	SCHEMANAME = 'public'
                            	AND TABLENAME = '{0}'
                            	AND INDEXNAME = '{1}'
                            """;

        const string sql2 = """
                            SELECT
                                COUNT(*)
                            FROM
                                INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                            WHERE
                                TABLE_SCHEMA = 'public'
                                AND TABLE_NAME = '{0}'
                                AND CONSTRAINT_NAME = '{1}'
                                AND CONSTRAINT_TYPE = 'PRIMARY KEY'
                            """;

        Dictionary<string, object> metadata = new ()
        {
            ["name"] = indexName
        };

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var def = helper.QueryScalar(string.Format(sql1, tableName, indexName)).ToString();
        var lparen = def.IndexOf('(');
        var rparent = def.LastIndexOf(')');
        var columnNames = def.Substring(lparen + 1, rparent - lparen - 1);

        metadata["unique"] = def.StartsWith("CREATE UNIQUE INDEX");
        metadata["columns"] = Regex.Split(columnNames, @"\s*\,\s*");
        metadata["primaryKey"] = helper.QueryScalar(string.Format(sql2, tableName, indexName)).Equals(1);

        return metadata;
    }

    public Dictionary<string, object> GetForeignKeyMeta(string tableName, string fkName)
    {
        const string sql = """
                           SELECT
                               KCU1.COLUMN_NAME FK_COLUMN_NAME,
                               KCU2.COLUMN_NAME PK_COLUMN_NAME,
                               TC2.TABLE_NAME PK_TABLE_NAME,
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
                               TC1.TABLE_SCHEMA = 'public'
                               AND TC1.TABLE_NAME = '{0}'
                               AND TC1.CONSTRAINT_NAME = '{1}'
                           ORDER BY
                               KCU1.ORDINAL_POSITION
                           """;

        Dictionary<string, object> metadata = [];
        List<string> fkColumns = [];
        List<string> relatedColumns = [];
        var relatedTable = string.Empty;
        var updateRule = ForeignKeyRule.None;
        var deleteRule = ForeignKeyRule.None;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, fkName), SqlHelper.ToArrayList);
        
        foreach (object[] values in list)
        {
            fkColumns.Add(values[0].ToString());
            relatedColumns.Add(values[1].ToString());
            relatedTable = values[2].ToString();
            updateRule = GetFKRule(values[3].ToString());
            deleteRule = GetFKRule(values[4].ToString());
        }

        metadata["name"] = fkName;
        metadata["columns"] = fkColumns.ToArray();
        metadata["relatedTable"] = relatedTable;
        metadata["relatedColumns"] = relatedColumns.ToArray();
        metadata["updateRule"] = updateRule;
        metadata["deleteRule"] = deleteRule;

        return metadata;
    }

    #endregion

    #region Utility

    private static ColumnType GetColumnType(string mysqlType) =>
        mysqlType switch
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
            _ => mysqlType.StartsWith("timestamp") ? ColumnType.DateTime : ColumnType.Unknown
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

    #endregion
}