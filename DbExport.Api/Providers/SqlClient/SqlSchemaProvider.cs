using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DbExport.Schema;

namespace DbExport.Providers.SqlClient;

public partial class SqlSchemaProvider : ISchemaProvider
{
    
    public SqlSchemaProvider(string connectionString)
    {
        ConnectionString = connectionString;
            
        var properties = Utility.ParseConnectionString(connectionString);
        
        if (properties.TryGetValue("initial catalog", out var catalog))
            DatabaseName = catalog;
        else if (properties.TryGetValue("database", out var database))
            DatabaseName = database;
        else if (properties.TryGetValue("attachdbfilename", out var filename))
            DatabaseName = Path.GetFileNameWithoutExtension(filename);
    }

    #region ISchemaProvider Members

    public string ProviderName => ProviderNames.SQLSERVER;

    public string ConnectionString { get; }

    public string DatabaseName { get; }

    public (string, string)[] GetTableNames()
    {
        const string sql = """
                           SELECT
                               TABLE_SCHEMA,
                               TABLE_NAME
                           FROM INFORMATION_SCHEMA.TABLES
                           WHERE TABLE_TYPE = 'BASE TABLE'
                             AND TABLE_SCHEMA NOT IN ('sys', 'INFORMATION_SCHEMA')
                             AND HAS_PERMS_BY_NAME(
                                   QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME),
                                   'OBJECT',
                                   'SELECT'
                                 ) = 1
                           ORDER BY TABLE_SCHEMA, TABLE_NAME;
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
                           	    TABLE_NAME = '{0}'
                           	    AND TABLE_SCHEMA = '{1}'
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public string[] GetIndexNames(string tableName, string tableOwner)
    {
        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query($"EXEC sp_helpindex '{tableOwner}.{tableName}'", SqlHelper.ToArrayList);
        return [..list.Select(values => values[0].ToString())];
    }

    public string[] GetFKNames(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT
                           	    CONSTRAINT_NAME
                           FROM
                           	    INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                           WHERE
                           	    TABLE_NAME = '{0}'
                                AND TABLE_SCHEMA = '{1}'
                           	    AND CONSTRAINT_TYPE = 'FOREIGN KEY'
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public Dictionary<string, object> GetTableMeta(string tableName, string tableOwner)
    {
        const string sql = """
                            SELECT
                                T.CONSTRAINT_NAME,
                                K.COLUMN_NAME
                            FROM
                                INFORMATION_SCHEMA.TABLE_CONSTRAINTS T
                                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE K
                                ON T.CONSTRAINT_NAME = K.CONSTRAINT_NAME
                            WHERE
                                T.CONSTRAINT_TYPE = 'PRIMARY KEY'
                                AND T.TABLE_NAME = '{0}'
                                AND T.TABLE_SCHEMA = '{1}'
                            ORDER BY
                                K.ORDINAL_POSITION
                            """;

        var metadata = new Dictionary<string, object>();
        var pkColumns = new List<string>();
        var pkName = string.Empty;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        metadata["name"] = tableName;
        metadata["owner"] = tableOwner;

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
        const string sql1 = """
                            SELECT
                            	DATA_TYPE,
                            	CHARACTER_MAXIMUM_LENGTH,
                            	NUMERIC_PRECISION,
                            	NUMERIC_SCALE,
                            	COLUMN_DEFAULT,
                            	IS_NULLABLE,
                                COLUMNPROPERTY(OBJECT_ID(TABLE_NAME), COLUMN_NAME, 'IsIdentity') IS_IDENTITY,
                                COLUMNPROPERTY(OBJECT_ID(TABLE_NAME), COLUMN_NAME, 'IsComputed') IS_COMPUTED
                            FROM
                            	INFORMATION_SCHEMA.COLUMNS
                            WHERE
                            	TABLE_NAME =  '{0}'
                                AND TABLE_SCHEMA = '{1}'
                            	AND COLUMN_NAME = '{2}'
                            """;

        const string sql2 = """
                            SELECT
                                ex.value
                            FROM
                                sys.columns c
                                LEFT OUTER JOIN sys.extended_properties ex
                                ON ex.major_id = c.object_id
                                AND ex.minor_id = c.column_id
                                AND ex.name = 'MS_Description'
                            WHERE
                                OBJECT_NAME(c.object_id) = '{0}'
                                AND c.name = '{1}'
                            """; // TODO: Check this!

        Dictionary<string, object> metadata = new()
        {
            ["name"] = columnName,
            ["defaultValue"] = string.Empty
        };

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var values = helper.Query(string.Format(sql1, tableName, tableOwner, columnName), SqlHelper.ToArray);
        var attributes = ColumnAttributes.None;
        ColumnType columnType;
                
        metadata["type"] = columnType = GetColumnType(values[0].ToString());
        metadata["nativeType"] = values[0].ToString();
        metadata["size"] = Utility.ToInt16(values[1]);
        metadata["precision"] = Utility.ToByte(values[2]);
        metadata["scale"] = Utility.ToByte(values[3]);

        if (values[4] != DBNull.Value)
        {
            var defaultValue = values[5].ToString()!;
            metadata["defaultValue"] = Parse(defaultValue[2..], columnType);
        }

        if (values[5].Equals("NO"))
            attributes |= ColumnAttributes.Required;

        if (values[6].Equals(1))
            attributes |= ColumnAttributes.Identity;

        if (values[7].Equals(1))
            attributes |= ColumnAttributes.Computed;

        metadata["attributes"] = attributes;
        metadata["description"] = Convert.ToString(helper.QueryScalar(string.Format(sql2, tableName, columnName)));

        if (attributes.HasFlag(ColumnAttributes.Identity))
        {
            metadata["ident_seed"] = Convert.ToInt64(helper.QueryScalar($"SELECT IDENT_SEED('{tableOwner}.{tableName}')"));
            metadata["ident_incr"] = Convert.ToInt64(helper.QueryScalar($"SELECT IDENT_INCR('{tableOwner}.{tableName}')"));
        }

        return metadata;
    }

    public Dictionary<string, object> GetIndexMeta(string tableName, string tableOwner, string indexName)
    {
        var metadata = new Dictionary<string, object>();
        var indexDescription = string.Empty;
        var indexKeys = string.Empty;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query($"EXEC sp_helpindex '{tableOwner}.{tableName}'", SqlHelper.ToArrayList);
        
        foreach (object[] values in list)
        {
            if (!values[0].Equals(indexName)) continue;
            indexDescription = values[1].ToString();
            indexKeys = values[2].ToString();
            break;
        }

        metadata["name"] = indexName;
        metadata["unique"] = UniqueRegex().IsMatch(indexDescription);
        metadata["primaryKey"] = PrimaryKeyRegex().IsMatch(indexDescription);
        metadata["columns"] = CommaRegex().Split(indexKeys);

        return metadata;
    }

    public Dictionary<string, object> GetForeignKeyMeta(string tableName, string tableOwner, string fkName)
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
                           		    ON RC.CONSTRAINT_NAME = TC1.CONSTRAINT_NAME
                           	    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU1
                           		    ON TC1.CONSTRAINT_NAME = KCU1.CONSTRAINT_NAME
                           	    JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC2
                           		    ON RC.UNIQUE_CONSTRAINT_NAME = TC2.CONSTRAINT_NAME
                           	    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU2
                           		    ON TC2.CONSTRAINT_NAME = KCU2.CONSTRAINT_NAME
                           		    AND KCU1.ORDINAL_POSITION = KCU2.ORDINAL_POSITION
                           WHERE
                           	    TC1.TABLE_NAME = '{0}'
                                AND TC1.TABLE_SCHEMA = '{1}'
                           	    AND TC1.CONSTRAINT_NAME = '{2}'
                           ORDER BY
                           	    KCU1.ORDINAL_POSITION
                           """;

        var metadata = new Dictionary<string, object>();
        var fkColumns = new List<string>();
        var relatedColumns = new List<string>();
        var relatedTable = string.Empty;
        var updateRule = ForeignKeyRule.None;
        var deleteRule = ForeignKeyRule.None;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner, fkName), SqlHelper.ToArrayList);
        
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

    private static ColumnType GetColumnType(string sqlType) =>
        sqlType switch
        {
            "bit" => ColumnType.Boolean,
            "tinyint" => ColumnType.UnsignedTinyInt,
            "smallint" => ColumnType.SmallInt,
            "int" => ColumnType.Integer,
            "bigint" => ColumnType.BigInt,
            "real" => ColumnType.SinglePrecision,
            "float" => ColumnType.DoublePrecision,
            "money" or "smallmoney" => ColumnType.Currency,
            "decimal" or "numeric" => ColumnType.Decimal,
            "datetime" or "smalldatetime" => ColumnType.DateTime,
            "char" => ColumnType.Char,
            "nchar" => ColumnType.NChar,
            "varchar" => ColumnType.VarChar,
            "nvarchar" => ColumnType.NVarChar,
            "text" => ColumnType.Text,
            "ntext" => ColumnType.NText,
            "binary" or "varbinary" or "image" => ColumnType.Blob,
            "uniqueidentifier" => ColumnType.Guid,
            "rowversion" or "timestamp" => ColumnType.RowVersion,
            "xml" => ColumnType.Xml,
            _ => ColumnType.Unknown
        };

    private static object Parse(string value, ColumnType columnType)
    {
        if (Utility.IsEmpty(value) || value.Equals("NULL", StringComparison.OrdinalIgnoreCase))
            return DBNull.Value;

        return columnType switch
        {
            ColumnType.Boolean => value.ToLower() switch
            {
                "1" => true,
                "0" => false,
                _ => DBNull.Value
            },
            ColumnType.UnsignedTinyInt => Utility.IsNumeric(value) ? Convert.ToByte(value) : DBNull.Value,
            ColumnType.SmallInt => Utility.IsNumeric(value) ? Convert.ToInt16(value) : DBNull.Value,
            ColumnType.Integer => Utility.IsNumeric(value) ? Convert.ToInt32(value) : DBNull.Value,
            ColumnType.BigInt => Utility.IsNumeric(value) ? Convert.ToInt64(value) : DBNull.Value,
            ColumnType.SinglePrecision => Utility.IsNumeric(value) ? Convert.ToSingle(value) : DBNull.Value,
            ColumnType.DoublePrecision => Utility.IsNumeric(value) ? Convert.ToDouble(value) : DBNull.Value,
            ColumnType.Currency or ColumnType.Decimal => Utility.IsNumeric(value)
                ? Convert.ToDecimal(value)
                : DBNull.Value,
            ColumnType.DateTime => Utility.IsDate(value) ? Convert.ToDateTime(value) : DBNull.Value,
            ColumnType.Char or ColumnType.NChar or ColumnType.VarChar or ColumnType.NVarChar or ColumnType.Text
                or ColumnType.NText => value,
            _ => DBNull.Value
        };
    }

    private static ForeignKeyRule GetFKRule(string rule) =>
        rule switch
        {
            "CASCADE" => ForeignKeyRule.Cascade,
            _ => ForeignKeyRule.None
        };

    [GeneratedRegex(@"\bunique\b", RegexOptions.Compiled)]
    private static partial Regex UniqueRegex();

    [GeneratedRegex(@"\bprimary key\b", RegexOptions.Compiled)]
    private static partial Regex PrimaryKeyRegex();

    [GeneratedRegex(@"\s*\,\s*", RegexOptions.Compiled)]
    private static partial Regex CommaRegex();

    #endregion
}