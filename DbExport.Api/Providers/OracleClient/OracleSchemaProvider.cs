using System;
using System.Collections.Generic;
using System.Linq;
using DbExport.Schema;

namespace DbExport.Providers.OracleClient;

/// <summary>
/// Provides schema metadata for Oracle databases by implementing the
/// ISchemaProvider interface. This class allows retrieval of database
/// objects such as tables, columns, indexes, and foreign keys,
/// as well as their associated metadata.
/// </summary>
public class OracleSchemaProvider : ISchemaProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OracleSchemaProvider"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string used to connect to the Oracle database.</param>
    public OracleSchemaProvider(string connectionString)
    {
        ConnectionString = connectionString;
            
        var properties = Utility.ParseConnectionString(connectionString);
        
        if (properties.TryGetValue("user id", out var userId))
            DatabaseName = userId;
        else if (properties.TryGetValue("uid", out var uid))
            DatabaseName = uid;
    }

    #region ISchemaProvider Members

    public string ProviderName => ProviderNames.ORACLE;

    public string ConnectionString { get; }

    public string DatabaseName { get; }

    public NameOwnerPair[] GetTableNames()
    {
        const string sql = """
                           SELECT
                               OWNER,
                               TABLE_NAME
                           FROM
                               ALL_TABLES
                           WHERE
                               TEMPORARY = 'N'
                               AND SECONDARY = 'N'
                               AND OWNER IN (
                                   SELECT USERNAME
                                   FROM ALL_USERS
                                   WHERE ORACLE_MAINTAINED = 'N' OR CREATED > (
                                       SELECT CREATED
                                       FROM ALL_USERS
                                       WHERE USERNAME = 'OPS$ORACLE'))
                           ORDER BY
                               OWNER,
                               TABLE_NAME
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(sql, SqlHelper.ToArrayList);
        return [..list.Select(item => new NameOwnerPair(item[1].ToString(), item[0].ToString()))];
    }

    public string[] GetColumnNames(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT
                           	    COLUMN_NAME
                           FROM
                           	    ALL_TAB_COLUMNS
                           WHERE
                           	    TABLE_NAME = '{0}'
                           	    AND OWNER = '{1}'
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public string[] GetIndexNames(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT
                               INDEX_NAME
                           FROM
                               ALL_INDEXES
                           WHERE
                               TABLE_NAME = '{0}'
                               AND OWNER = '{1}'
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public string[] GetForeignKeyNames(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT
                           	    CONSTRAINT_NAME
                           FROM
                           	    ALL_CONSTRAINTS
                           WHERE
                           	    TABLE_NAME = '{0}'
                                AND OWNER = '{1}'
                           	    AND CONSTRAINT_TYPE = 'R'
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToList);
        return [..list.Select(item => item.ToString())];
    }

    public MetaData GetTableMeta(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT
                               C.CONSTRAINT_NAME,
                               K.COLUMN_NAME
                           FROM
                               ALL_CONSTRAINTS C
                               JOIN ALL_CONS_COLUMNS K
                                    ON C.OWNER = K.OWNER
                                        AND C.TABLE_NAME = K.TABLE_NAME
                                        AND C.CONSTRAINT_NAME = K.CONSTRAINT_NAME
                           WHERE
                               C.TABLE_NAME = '{0}'
                                   AND C.OWNER = '{1}'
                                   AND C.CONSTRAINT_TYPE = 'P'
                           ORDER BY
                               K.POSITION
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToArrayList);

        string pkName = null;
        List<string> pkColumns = [];
        MetaData metadata = new()
        {
            ["name"] = tableName,
            ["owner"] = tableOwner
        };

        foreach (var values in list)
        {
            pkName ??= values[0].ToString();
            pkColumns.Add(values[1].ToString());
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
                           	    DATA_TYPE,
                           	    DATA_LENGTH,
                           	    DATA_PRECISION,
                           	    DATA_SCALE,
                           	    DATA_DEFAULT,
                           	    NULLABLE
                           FROM
                           	    ALL_TAB_COLUMNS
                           WHERE
                           	    TABLE_NAME =  '{0}'
                                AND OWNER = '{1}'
                           	    AND COLUMN_NAME = '{2}'
                           """;

        MetaData metadata = new()
        {
            ["name"] = columnName,
            ["description"] = string.Empty
        };

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var values = helper.Query(string.Format(sql, tableName, tableOwner, columnName), SqlHelper.ToArray);
        var attributes = ColumnAttributes.None;
        ColumnType columnType;

        metadata["nativeType"] = values[0].ToString();
        metadata["size"] = Utility.ToInt16(values[1]);
        metadata["precision"] = Utility.ToByte(values[2]);
        metadata["scale"] = Utility.ToByte(values[3]);
        metadata["type"] = columnType = GetColumnType(
            (string)metadata["nativeType"], (byte)metadata["precision"], (byte)metadata["scale"]);
        metadata["defaultValue"] = Parse(Convert.ToString(values[4]), columnType);
                
        if (values[5].Equals("N")) attributes |= ColumnAttributes.Required;
        metadata["attributes"] =  attributes;

        return metadata;
    }

    public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)
    {
        const string sql1 = """
                            SELECT
                                COLUMN_NAME,
                                DESCEND
                            FROM
                                ALL_IND_COLUMNS
                            WHERE
                                TABLE_NAME = '{0}'
                                    AND TABLE_OWNER = '{1}'
                                    AND INDEX_NAME = '{2}'
                                    AND COLUMN_NAME NOT LIKE 'SYS_NC%'
                            ORDER BY
                                COLUMN_POSITION
                            """;

        const string sql2 = """
                            SELECT
                                UNIQUENESS
                            FROM
                                ALL_INDEXES
                            WHERE
                                TABLE_NAME = '{0}'
                                    AND TABLE_OWNER = '{1}'
                                    AND INDEX_NAME = '{2}'
                            """;

        const string sql3 = """
                            SELECT
                                COUNT(*)
                            FROM
                                ALL_CONSTRAINTS
                            WHERE
                                TABLE_NAME = '{0}'
                                AND OWNER = '{1}'
                                AND CONSTRAINT_NAME = '{2}'
                                AND CONSTRAINT_TYPE = 'P'
                            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql1, tableName, tableOwner, indexName), SqlHelper.ToArrayList);
        List<string> indexColumns = [..list.Select(values => values[0].ToString())];

        return new MetaData
        {
            ["name"] = indexName,
            ["columns"] = indexColumns.ToArray(),
            ["unique"] = helper.QueryScalar(string.Format(sql2, tableName, tableOwner, indexName)).Equals("UNIQUE"),
            ["primaryKey"] = helper.QueryScalar(string.Format(sql3, tableName, tableOwner, indexName)).Equals(1)
        };
    }

    public MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName)
    {
        const string sql = """
                           SELECT
                               FK.COLUMN_NAME AS FK_Column,
                               PK.COLUMN_NAME AS PK_Column,
                               PT.TABLE_NAME AS PK_Table,
                               PT.OWNER AS PK_Owner,
                               C.DELETE_RULE
                           FROM
                               ALL_CONSTRAINTS C
                                   JOIN ALL_CONS_COLUMNS FK
                                        ON C.OWNER = FK.OWNER
                                            AND C.TABLE_NAME = FK.TABLE_NAME
                                            AND C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
                                   JOIN ALL_CONSTRAINTS PT
                                        ON C.R_OWNER = PT.OWNER
                                            AND C.R_CONSTRAINT_NAME = PT.CONSTRAINT_NAME
                                            AND PT.CONSTRAINT_TYPE = 'P'
                                   JOIN ALL_CONS_COLUMNS PK
                                        ON PT.OWNER = PK.OWNER
                                            AND PT.TABLE_NAME = PK.TABLE_NAME
                                            AND PT.CONSTRAINT_NAME = PK.CONSTRAINT_NAME
                                            AND FK.POSITION = PK.POSITION
                           WHERE
                               C.TABLE_NAME = '{0}'
                                   AND C.OWNER = '{1}'
                                   AND C.CONSTRAINT_NAME = '{2}'
                           ORDER BY
                               FK.POSITION
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner, fkName), SqlHelper.ToArrayList);

        List<string> fkColumns = [];
        List<string> relatedColumns = [];
        var relatedTable = string.Empty;
        var relatedOwner = string.Empty;
        var deleteRule = ForeignKeyRule.None;

        foreach (object[] values in list)
        {
            fkColumns.Add(values[0].ToString());
            relatedColumns.Add(values[1].ToString());
            relatedTable = values[2].ToString();
            relatedOwner = values[3].ToString();
            deleteRule = values[4].Equals("NO ACTION")? ForeignKeyRule.None : ForeignKeyRule.Cascade;
        }

        return new MetaData
        {
            ["name"] = fkName,
            ["columns"] = fkColumns.ToArray(),
            ["relatedName"] = relatedTable,
            ["relatedOwner"] = relatedOwner,
            ["relatedColumns"] = relatedColumns.ToArray(),
            ["updateRule"] = ForeignKeyRule.None,
            ["deleteRule"] = deleteRule
        };
    }

    #endregion

    #region Utility

    /// <summary>
    /// Determines the corresponding <see cref="ColumnType"/> for the provided Oracle database type based on its type name, precision, and scale.
    /// </summary>
    /// <param name="oracleType">The name of the Oracle database type (e.g., "NUMBER", "CHAR").</param>
    /// <param name="precision">The precision of the Oracle type, used for numeric types.</param>
    /// <param name="scale">The scale of the Oracle type, used for numeric types.</param>
    /// <returns>The <see cref="ColumnType"/> representing the equivalent column type for the specified Oracle type.</returns>
    private static ColumnType GetColumnType(string oracleType, byte precision, byte scale)
    {
        return oracleType switch
        {
            "SIMPLE_INTEGER" => ColumnType.Integer,
            "BINARY_FLOAT" => ColumnType.SinglePrecision,
            "BINARY_DOUBLE" => ColumnType.DoublePrecision,
            "NUMBER" => scale switch
            {
                0 when precision < 3 => ColumnType.TinyInt,
                0 when precision < 5 => ColumnType.SmallInt,
                0 when precision < 10 => ColumnType.Integer,
                0 when precision < 20 => ColumnType.BigInt,
                _ => ColumnType.Decimal
            },
            "DATE" => ColumnType.DateTime,
            "CHAR" => ColumnType.Char,
            "NCHAR" => ColumnType.NChar,
            "VARCHAR2" => ColumnType.VarChar,
            "NVARCHAR2" => ColumnType.NVarChar,
            "CLOB" or "LONG" => ColumnType.Text,
            "NCLOB" => ColumnType.NText,
            "BLOB" or "RAW" or "LONG RAW" => ColumnType.Blob,
            "BFILE" => ColumnType.File,
            "XMLType" => ColumnType.Xml,
            "JSON" => ColumnType.Json,
            "SDO_GEOMETRY" => ColumnType.Geometry,
            _ when oracleType.StartsWith("TIMESTAMP") => ColumnType.DateTime,
            _ => ColumnType.Unknown
        };
    }

    /// <summary>
    /// Parses a specified string value into an object of the specified database column type.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <param name="columnType">The column type that determines the target data type of the parsed value.</param>
    /// <returns>
    /// The parsed object of the requested column type, or <see cref="DBNull.Value"/> if the value
    /// is null, empty, or cannot be converted to the specified column type.
    /// </returns>
    private static object Parse(string value, ColumnType columnType)
    {
        if (Utility.IsEmpty(value) || "NULL".Equals(value, StringComparison.OrdinalIgnoreCase))
            return DBNull.Value;

        return columnType switch
        {
            ColumnType.TinyInt => Utility.IsNumeric(value, out var number) ? (sbyte)number : DBNull.Value,
            ColumnType.SmallInt => Utility.IsNumeric(value, out var number) ? (short)number : DBNull.Value,
            ColumnType.Integer => Utility.IsNumeric(value, out var number) ? (int)number : DBNull.Value,
            ColumnType.BigInt => Utility.IsNumeric(value, out var number) ? (long)number : DBNull.Value,
            ColumnType.SinglePrecision => Utility.IsNumeric(value, out var number) ? (float)number : DBNull.Value,
            ColumnType.DoublePrecision => Utility.IsNumeric(value, out var number) ? (double)number : DBNull.Value,
            ColumnType.Decimal => Utility.IsNumeric(value, out var number) ? number : DBNull.Value,
            ColumnType.DateTime => Utility.IsDate(value, out var date) ? date : DBNull.Value,
            ColumnType.Char or ColumnType.NChar or ColumnType.VarChar or ColumnType.NVarChar or ColumnType.Text
                or ColumnType.NText => value,
            _ => DBNull.Value
        };
    }

    #endregion
}