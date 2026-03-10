using System.Collections.Generic;
using System.Linq;
using DbExport.Schema;

namespace DbExport.Providers.DB2;

/// <summary>
/// Provides schema-related functionalities for a DB2 database, such as accessing
/// table names, column names, index names, and foreign key names.
/// </summary>
public class DB2SchemaProvider : ISchemaProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DB2SchemaProvider"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string used to connect to the DB2 database.</param>
    public DB2SchemaProvider(string connectionString)
    {
        ConnectionString = connectionString;

        var properties = Utility.ParseConnectionString(connectionString);
        DatabaseName = properties.TryGetValue("database", out var db)
            ? db
            : string.Empty;
    }
    
    #region ISchemaProvider Members

    /// <inheritdoc/>
    public string ProviderName => ProviderNames.DB2;

    /// <inheritdoc/>
    public string ConnectionString { get; }

    /// <inheritdoc/>
    public string DatabaseName { get; }

    /// <inheritdoc/>
    public NameOwnerPair[] GetTableNames()
    {
        const string sql = """
            SELECT
                TABSCHEMA AS Owner,
                TABNAME AS Name
            FROM SYSCAT.TABLES
            WHERE TYPE = 'T'
            ORDER BY TABSCHEMA, TABNAME
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        return [..helper.Query(sql, SqlHelper.ToEntityList<NameOwnerPair>)];
    }

    /// <inheritdoc/>
    public string[] GetColumnNames(string tableName, string tableOwner)
    {
        const string sql = """
            SELECT COLNAME
            FROM SYSCAT.COLUMNS
            WHERE TABNAME = '{0}'
              AND TABSCHEMA = '{1}'
            ORDER BY COLNO
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToList);

        return [..list.Select(x => x.ToString())];
    }

    /// <inheritdoc/>
    public string[] GetIndexNames(string tableName, string tableOwner)
    {
        const string sql = """
            SELECT INDNAME
            FROM SYSCAT.INDEXES
            WHERE TABNAME = '{0}'
              AND TABSCHEMA = '{1}'
            ORDER BY INDNAME
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToList);

        return [..list.Select(x => x.ToString())];
    }

    /// <inheritdoc/>
    public string[] GetForeignKeyNames(string tableName, string tableOwner)
    {
        const string sql = """
            SELECT CONSTNAME
            FROM SYSCAT.REFERENCES
            WHERE TABNAME = '{0}'
              AND TABSCHEMA = '{1}'
            ORDER BY CONSTNAME
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToList);

        return [..list.Select(x => x.ToString())];
    }

    /// <inheritdoc/>
    public MetaData GetTableMeta(string tableName, string tableOwner)
    {
        const string sql = """
            SELECT
                K.COLNAME,
                K.CONSTNAME
            FROM SYSCAT.KEYCOLUSE K
            JOIN SYSCAT.TABCONST C
              ON C.CONSTNAME = K.CONSTNAME
             AND C.TABNAME = K.TABNAME
             AND C.TABSCHEMA = K.TABSCHEMA
            WHERE C.TYPE = 'P'
              AND K.TABNAME = '{0}'
              AND K.TABSCHEMA = '{1}'
            ORDER BY K.COLSEQ
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableName, tableOwner), SqlHelper.ToArrayList);

        string pkName = null;
        List<string> columns = [];

        foreach (var row in list)
        {
            pkName ??= row[1].ToString();
            columns.Add(row[0].ToString());
        }

        var meta = new MetaData
        {
            ["name"] = tableName,
            ["owner"] = tableOwner
        };

        if (pkName != null)
        {
            meta["pk_name"] = pkName;
            meta["pk_columns"] = columns.ToArray();
        }

        return meta;
    }

    /// <inheritdoc/>
    public MetaData GetColumnMeta(string tableName, string tableOwner, string columnName)
    {
        const string sql = """
            SELECT
                TYPENAME,
                LENGTH,
                SCALE,
                DEFAULT,
                NULLS,
                IDENTITY
            FROM SYSCAT.COLUMNS
            WHERE TABNAME = '{0}'
              AND TABSCHEMA = '{1}'
              AND COLNAME = '{2}'
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var row = helper.Query(string.Format(sql, tableName, tableOwner, columnName), SqlHelper.ToArray);
        var type = row[0].ToString();

        MetaData meta = new()
        {
            ["name"] = columnName,
            ["nativeType"] = type,
            ["type"] = GetColumnType(type),
            ["size"] = Utility.ToInt16(row[1]),
            ["precision"] = Utility.ToByte(row[1]),
            ["scale"] = Utility.ToByte(row[2]),
            ["defaultValue"] = row[3],
            ["description"] = string.Empty,
        };

        var attrs = ColumnAttributes.None;

        if (row[4].Equals("N"))
            attrs |= ColumnAttributes.Required;

        if (row[5].Equals("Y"))
            attrs |= ColumnAttributes.Identity;

        meta["attributes"] = attrs;

        return meta;
    }

    /// <inheritdoc/>
    public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)
    {
        const string sql = """
            SELECT
              COLNAMES,
              UNIQUERULE
            FROM SYSCAT.INDEXES
            WHERE INDNAME = '{2}'
              AND TABNAME = '{0}'
              AND TABSCHEMA = '{1}'
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var values = helper.Query(string.Format(sql, tableName, tableOwner, indexName), SqlHelper.ToArray);
        var columns = Utility.Split(values[0].ToString()!.Replace('+', ' ').Replace('-', ' '), ' ');

        return new MetaData
        {
            ["name"] = indexName,
            ["unique"] = "U".Equals(values[1]),
            ["primaryKey"] = "P".Equals(values[1]),
            ["columns"] = columns
        };
    }

    /// <inheritdoc/>
    public MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName)
    {
        const string sql = """
            SELECT
              REFTABNAME,
              REFTABSCHEMA,
              FK_COLNAMES,
              PK_COLNAMES,
              DELETERULE,
              UPDATERULE
            FROM SYSCAT.REFERENCES
            WHERE TABNAME = '{0}'
              AND TABSCHEMA = '{1}'
              AND CONSTNAME = '{2}'
            """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var values = helper.Query(string.Format(sql, tableName, tableOwner, fkName), SqlHelper.ToArray);

        return new MetaData
        {
            ["name"] = fkName,
            ["columns"] = Utility.Split(values[2].ToString(), ' '),
            ["relatedName"] = values[0].ToString(),
            ["relatedOwner"] = values[1].ToString(),
            ["relatedColumns"] = Utility.Split(values[3].ToString(), ' '),
            ["deleteRule"] = ParseForeignKeyRule(values[4].ToString()),
            ["updateRule"] = ParseForeignKeyRule(values[5].ToString())
        };
    }

    #endregion
    
    #region Utilities

    /// <summary>
    /// Maps the provided DB2 column type string to a corresponding <see cref="ColumnType"/> enumeration value.
    /// </summary>
    /// <param name="type">The string representation of the DB2 column data type.</param>
    /// <returns>A <see cref="ColumnType"/> that represents the mapped data type, or <see cref="ColumnType.Unknown"/> if the type cannot be mapped.</returns>
    private static ColumnType GetColumnType(string type) =>
        type switch
        {
            "SMALLINT" => ColumnType.SmallInt,
            "INTEGER" => ColumnType.Integer,
            "BIGINT" => ColumnType.BigInt,
            "REAL" => ColumnType.SinglePrecision,
            "DOUBLE" => ColumnType.DoublePrecision,
            "DECIMAL" => ColumnType.Decimal,
            "DATE" => ColumnType.Date,
            "TIME" => ColumnType.Time,
            "TIMESTAMP" => ColumnType.DateTime,
            "CHAR" => ColumnType.Char,
            "VARCHAR" => ColumnType.VarChar,
            "CLOB" => ColumnType.Text,
            "BLOB" => ColumnType.Blob,
            "XML" => ColumnType.Xml,
            _ => ColumnType.Unknown
        };

    /// <summary>
    /// Converts a string representation of a foreign key rule into its corresponding <see cref="ForeignKeyRule"/> enumeration value.
    /// </summary>
    /// <param name="value">The string representation of the foreign key rule, such as "A", "R", "C", "D", or "N".</param>
    /// <returns>The corresponding <see cref="ForeignKeyRule"/> value for the specified string representation.</returns>
    private static ForeignKeyRule ParseForeignKeyRule(string value) =>
        value switch
        {
            "A" => ForeignKeyRule.None,
            "R" => ForeignKeyRule.Restrict,
            "C" => ForeignKeyRule.Cascade,
            "D" => ForeignKeyRule.SetDefault,
            "N" => ForeignKeyRule.SetNull,
            _ => ForeignKeyRule.None
        };

    #endregion
}