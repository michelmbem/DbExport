using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using DbExport.Schema;

namespace DbExport.Providers.SQLite;

/// <summary>
/// Provides schema information for SQLite databases. This class implements
/// the ISchemaProvider interface, enabling retrieval of database schema
/// metadata such as table names, column names, foreign key names, and associated metadata.
/// </summary>
public class SQLiteSchemaProvider : ISchemaProvider
{
    private readonly MetaData tableColumns = [];
    private readonly MetaData tableIndexes = [];
    private readonly MetaData tableForeignKeys = [];
    
    /// <summary>
    /// Initializes a new instance of the <see cref="SQLiteSchemaProvider"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string used to connect to the SQLite database.</param>
    public SQLiteSchemaProvider(string connectionString)
    {
        ConnectionString = connectionString;
            
        var properties = Utility.ParseConnectionString(connectionString);
        var dbFilename = properties["data source"];
        DatabaseName = Path.GetFileNameWithoutExtension(dbFilename);
    }

    #region ISchemaProvider Members

    public string ProviderName => ProviderNames.SQLITE;

    public string ConnectionString { get; }

    public string DatabaseName { get; }

    public NameOwnerPair[] GetTableNames()
    {
        const string sql = """
                           SELECT schema, name
                           FROM pragma_table_list()
                           WHERE type = 'table'
                               AND name NOT LIKE 'sqlite_%'
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(sql, SqlHelper.ToArrayList);
        return [..list.Select(item => new NameOwnerPair(item[1].ToString(), item[0].ToString()))];
    }

    public string[] GetColumnNames(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT *
                           FROM pragma_table_info('{1}', '{0}')
                           ORDER BY cid
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableOwner, tableName), SqlHelper.ToDictionaryList);
        RegisterList(tableColumns, tableOwner, tableName, list);
        
        return [..list.Select(item => item["name"].ToString())];
    }

    public string[] GetIndexNames(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT *
                           FROM pragma_index_list('{1}', '{0}')
                           ORDER BY seq
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableOwner, tableName), SqlHelper.ToDictionaryList);
        RegisterList(tableIndexes, tableOwner, tableName, list);
        
        return [..list.Select(item => item["name"].ToString())];
    }

    public string[] GetForeignKeyNames(string tableName, string tableOwner)
    {
        const string sql = """
                           SELECT *
                           FROM pragma_foreign_key_list('{1}', '{0}')
                           ORDER BY seq
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(string.Format(sql, tableOwner, tableName), SqlHelper.ToDictionaryList);
        RegisterList(tableForeignKeys, tableOwner, tableName, list);
        
        foreach (var item in list)
            item["name"] = $"fk_{tableName}_{item["id"]}";
        
        return [..list.Select(item => item["name"].ToString())];
    }

    public MetaData GetTableMeta(string tableName, string tableOwner)
    {
        MetaData metadata = new()
        {
            ["name"] = tableName,
            ["owner"] = tableOwner
        };
        
        var pkIndex = FindFirst(tableIndexes, tableOwner, tableName, item => "pk".Equals(item["origin"]));
        var pkName = pkIndex?["name"];

        if (pkName != null)
        {
            const string sql = """
                            SELECT name
                            FROM pragma_index_info('{0}')
                            ORDER BY seqno
                            """;

            using var helper = new SqlHelper(ProviderName, ConnectionString);
            var pkColumns = helper.Query(string.Format(sql, pkName), SqlHelper.ToList).Cast<string>();
            
            metadata["pk_name"] = pkName;
            metadata["pk_columns"] = pkColumns.ToArray();
        }
        
        return metadata;
    }

    public MetaData GetColumnMeta(string tableName, string tableOwner, string columnName)
    {
        var column = FindFirst(tableColumns, tableOwner, tableName, item => columnName.Equals(item["name"]));

        ResolveColumnType(column!["type"].ToString(), out ColumnType columnType, out string nativeType,
                          out short size, out byte precision, out byte scale);

        MetaData metadata = new()
        {
            ["name"] = columnName,
            ["type"] = columnType,
            ["nativeType"] = nativeType,
            ["size"] = size,
            ["precision"] = precision,
            ["scale"] = scale,
            ["defaultValue"] = ParseValue(column["dflt_value"], columnType),
            ["description"] = string.Empty,
        };

        var attributes = ColumnAttributes.None;
        
        if (column["notnull"].Equals(1))
            attributes |= ColumnAttributes.Required;

        metadata["attributes"] = attributes;

        return metadata;
    }

    public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)
    {
        var index = FindFirst(tableIndexes, tableOwner, tableName, item => indexName.Equals(item["name"]));
        MetaData metadata = new()
        {
            ["name"] = indexName,
            ["unique"] = index!["unique"].Equals(1),
            ["primaryKey"] = index["origin"].Equals("pk")
        };
        
        const string sql = """
                           SELECT name
                           FROM pragma_index_info('{0}')
                           ORDER BY seqno
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var indexColumns = helper.Query(string.Format(sql, indexName), SqlHelper.ToList).Cast<string>();

        metadata["columns"] = indexColumns.ToArray();

        return metadata;
    }

    public MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName)
    {
        var fk = FindFirst(tableForeignKeys, tableOwner, tableName, item => fkName.Equals(item["name"]));

        return new MetaData
        {
            ["name"] = fkName,
            ["columns"] = Utility.Split(fk!["from"].ToString(), ','),
            ["relatedName"] = fk["table"].ToString(),
            ["relatedOwner"] = tableOwner,
            ["relatedColumns"] = Utility.Split(fk["to"].ToString(), ','),
            ["updateRule"] = ParseForeignKeyRule(fk["on_update"].ToString()),
            ["deleteRule"] = ParseForeignKeyRule(fk["on_delete"].ToString()),
        };
    }

    #endregion

    #region Utility

    private static string Combine(string tableOwner, string tableName) => $"{tableOwner}.{tableName}";

    private static void RegisterList(MetaData collection, string tableOwner, string tableName,
        List<Dictionary<string, object>> list) => collection[Combine(tableOwner, tableName)] = list;

    private static Dictionary<string, object> FindFirst(
        MetaData collection, string tableOwner, string tableName, Predicate<Dictionary<string, object>> predicate) =>
        ((List<Dictionary<string, object>>)collection[Combine(tableOwner, tableName)]).FirstOrDefault(item => predicate(item));

    /// <summary>
    /// Resolves the column type based on the provided SQL type string. Determines the corresponding
    /// <see cref="ColumnType"/>, native database type, size, precision, and scale of the column.
    /// </summary>
    /// <param name="sqlType">The SQL type string to be analyzed.</param>
    /// <param name="columnType">Outputs the resolved <see cref="ColumnType"/> enumeration value.</param>
    /// <param name="nativeType">Outputs the native SQL type associated with the column.</param>
    /// <param name="size">Outputs the size of the column, if applicable.</param>
    /// <param name="precision">Outputs the precision of the column, if applicable.</param>
    /// <param name="scale">Outputs the scale of the column, if applicable.</param>
    private static void ResolveColumnType(string sqlType, out ColumnType columnType, out string nativeType,
                                          out short size, out byte precision, out byte scale)
    {
        size = precision = scale = 0;

        var lParen = sqlType.IndexOf('(');

        if (lParen > 0)
        {
            var rParen = sqlType.IndexOf(')');
            var sizeStr = sqlType[(lParen + 1)..rParen];
            var parts = Utility.Split(sizeStr, ',');
            
            nativeType = sqlType[..lParen].Trim().ToLowerInvariant();
            
            if (nativeType.StartsWith("numeric", StringComparison.OrdinalIgnoreCase) ||
                nativeType.StartsWith("decimal", StringComparison.OrdinalIgnoreCase))
            {
                precision = Utility.ToByte(parts[0]);
                if (parts.Length > 1) scale = Utility.ToByte(parts[1]);
            }
            else
                size = Utility.ToInt16(parts[0]);
        }
        else
            nativeType = sqlType.Trim().ToLowerInvariant();

        columnType = GetColumnType(nativeType);
    }

    /// <summary>
    /// Determines the corresponding <see cref="ColumnType"/> for a given SQLite data type.
    /// </summary>
    /// <remarks>
    /// This method does not really map SQLite data types to standard column types.
    /// It tries to recognize common data types and return their corresponding <see cref="ColumnType"/>.
    /// </remarks>
    /// <param name="sqliteType">The SQLite data type as a string.</param>
    /// <returns>The <see cref="ColumnType"/> that corresponds to the specified SQLite data type.</returns>
    private static ColumnType GetColumnType(string sqliteType) =>
        sqliteType switch
        {
            "bool" or "boolean" => ColumnType.Boolean,
            "tinyint" => ColumnType.TinyInt,
            "smallint" => ColumnType.SmallInt,
            "int" or "integer" => ColumnType.Integer,
            "bigint" => ColumnType.BigInt,
            "float" => ColumnType.SinglePrecision,
            "double" or "real" => ColumnType.DoublePrecision,
            "money" => ColumnType.Currency,
            "decimal" or "numeric" => ColumnType.Decimal,
            "date" => ColumnType.Date,
            "time" => ColumnType.Time,
            "datetime" or "timestamp" => ColumnType.DateTime,
            "char" or "character" => ColumnType.Char,
            "nchar" => ColumnType.NChar,
            "varchar" => ColumnType.VarChar,
            "nvarchar" => ColumnType.NVarChar,
            "text" or "clob" => ColumnType.Text,
            "ntext" or "nclob" => ColumnType.NText,
            "bit" => ColumnType.Bit,
            "blob" => ColumnType.Blob,
            _ => ColumnType.Unknown
        };

    /// <summary>
    /// Parses the input value into a strongly typed object based on the specified column type.
    /// </summary>
    /// <param name="value">The value to be parsed, which can be any object or null.</param>
    /// <param name="columnType">The type of column to determine the parsing logic for the value.</param>
    /// <returns>An object representing the parsed value, or <see cref="DBNull.Value"/>
    /// if parsing fails or the value is null.</returns>
    private static object ParseValue(object value, ColumnType columnType)
    {
        if (value == null || value == DBNull.Value) return null;
        
        return columnType switch
        {
            ColumnType.Boolean => value.ToString()!.ToLowerInvariant() switch
            {
                "0" or "false" => false,
                "1" or "true" => true,
                _ => DBNull.Value
            },
            ColumnType.TinyInt => Utility.IsNumeric(value, out var number) ? (sbyte)number : DBNull.Value,
            ColumnType.SmallInt => Utility.IsNumeric(value, out var number) ? (short)number : DBNull.Value,
            ColumnType.Integer => Utility.IsNumeric(value, out var number) ? (int)number : DBNull.Value,
            ColumnType.BigInt => Utility.IsNumeric(value, out var number) ? (long)number : DBNull.Value,
            ColumnType.SinglePrecision => Utility.IsNumeric(value, out var number) ? (float)number : DBNull.Value,
            ColumnType.DoublePrecision => Utility.IsNumeric(value, out var number) ? (double)number : DBNull.Value,
            ColumnType.Currency or ColumnType.Decimal => Utility.IsNumeric(value, out var number) ? number : DBNull.Value,
            ColumnType.Date or ColumnType.Time or ColumnType.DateTime => Utility.IsDate(value, out var date) ? date : DBNull.Value,
            ColumnType.Char or ColumnType.VarChar or ColumnType.Text => Utility.UnquotedStr(value),
            ColumnType.Bit => Utility.FromBitString(value.ToString()),
            _ => DBNull.Value
        };
    }

    /// <summary>
    /// Converts a SQLite foreign key rule string into a corresponding <see cref="ForeignKeyRule"/> enumeration value.
    /// </summary>
    /// <param name="sqlFkRule">The foreign key rule as represented in the SQLite metadata (e.g., "NO ACTION", "CASCADE").</param>
    /// <returns>A <see cref="ForeignKeyRule"/> enumeration value that corresponds to the specified SQLite foreign key rule.</returns>
    private static ForeignKeyRule ParseForeignKeyRule(string sqlFkRule) =>
        sqlFkRule switch
        {
            "NO ACTION" => ForeignKeyRule.None,
            "CASCADE" => ForeignKeyRule.Cascade,
            _ => ForeignKeyRule.Restrict
        };

    #endregion
}