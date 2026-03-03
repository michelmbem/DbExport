using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DbExport.Schema;

namespace DbExport.Providers.SQLite;

/// <summary>
/// Provides schema information for SQLite databases. This class implements
/// the ISchemaProvider interface, enabling retrieval of database schema
/// metadata such as table names, column names, foreign key names, and associated metadata.
/// </summary>
public class SQLiteSchemaProvider : ISchemaProvider
{
    #region Fields
    
    /// <summary>
    /// Represents a data structure that holds information about the columns
    /// of database tables in the current schema context. This variable acts
    /// as a cache for table column metadata and is utilized to simplify
    /// metadata retrievals and avoid redundant database queries.
    /// </summary>
    private readonly MetaData tableColumns = [];

    /// <summary>
    /// Represents a data structure that caches metadata related to the indexes
    /// of database tables within the current schema context. This variable is used to
    /// store and retrieve index information such as uniqueness and origin, optimizing
    /// metadata queries and reducing redundant database calls.
    /// </summary>
    private readonly MetaData tableIndexes = [];

    /// <summary>
    /// Serves as a data structure to store information about the foreign keys
    /// of database tables within the current schema context. This variable is used
    /// to cache foreign key metadata, enabling efficient retrieval and minimizing
    /// redundant queries to the database.
    /// </summary>
    private readonly MetaData tableForeignKeys = [];

    /// <summary>
    /// Stores a mapping of table names to a boolean value indicating whether
    /// each table has an auto-increment column. This variable is used to cache
    /// the auto-increment metadata for tables, optimizing performance by reducing
    /// the need for repetitive queries to the database.
    /// </summary>
    private readonly Dictionary<string, bool> tableHasAutoIncrement = [];

    #endregion
    
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
        const string sql1 = """
                           SELECT schema, name
                           FROM pragma_table_list()
                           WHERE type = 'table'
                               AND name NOT LIKE 'sqlite_%'
                           """;

        const string sql2 = "SELECT name, sql FROM sqlite_master WHERE type = 'table'";

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list1 = helper.Query(sql1, SqlHelper.ToArrayList);
        var list2 = helper.Query(sql2, SqlHelper.ToArrayList);
        
        foreach (var item in list2)
        {
            tableHasAutoIncrement[item[0].ToString()!] =
                item[1].ToString()!.Contains("AUTOINCREMENT", StringComparison.OrdinalIgnoreCase);
        }

        return [..list1.Select(item => new NameOwnerPair(item[1].ToString(), item[0].ToString()))];
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

        var pkColumns = ((List<Dictionary<string, object>>)tableColumns[Combine(tableOwner, tableName)])
                        .Where(column => Convert.ToInt32(column["pk"]) > 0)
                        .OrderBy(column => Convert.ToInt32(column["pk"]))
                        .Select(column => column["name"].ToString())
                        .ToArray();

        if (pkColumns.Length > 0)
        {
            metadata["pk_name"] = $"pk_{tableName}";
            metadata["pk_columns"] = pkColumns;
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
        
        if (Convert.ToInt32(column["notnull"]) == 1)
            attributes |= ColumnAttributes.Required;

        if (Convert.ToInt32(column["pk"]) == 1 && tableHasAutoIncrement[tableName])
        {
            attributes |= ColumnAttributes.Identity;
            metadata["ident_seed"] = metadata["ident_incr"] = 1L;
        }

        metadata["attributes"] = attributes;

        return metadata;
    }

    public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)
    {
        var index = FindFirst(tableIndexes, tableOwner, tableName, item => indexName.Equals(item["name"]));
        MetaData metadata = new()
        {
            ["name"] = indexName,
            ["unique"] = Convert.ToInt32(index!["unique"]) == 1,
            ["primaryKey"] = index["origin"].Equals("pk")
        };
        
        const string sql = """
                           SELECT name
                           FROM pragma_index_info('{0}')
                           ORDER BY seqno
                           """;

        using var helper = new SqlHelper(ProviderName, ConnectionString);

        metadata["columns"] = helper.Query(string.Format(sql, indexName), SqlHelper.ToList)
                                    .Select(item => item.ToString())
                                    .ToArray();

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

    /// <summary>
    /// Combines the table owner and table name into a single string, separated by a period.
    /// </summary>
    /// <param name="tableOwner">The owner of the table, usually representing the schema or database user.</param>
    /// <param name="tableName">The name of the table for which the owner is being combined.</param>
    /// <returns>A string that combines the table owner and table name in the format "tableOwner.tableName".</returns>
    private static string Combine(string tableOwner, string tableName) => $"{tableOwner}.{tableName}";

    /// <summary>
    /// Registers a list of metadata associated with a specific table in the given collection.
    /// </summary>
    /// <param name="collection">The metadata collection where the table information will be registered.</param>
    /// <param name="tableOwner">The owner of the table for which metadata is being registered.</param>
    /// <param name="tableName">The name of the table for which metadata is being registered.</param>
    /// <param name="list">The list of metadata entries to be registered, represented as dictionaries of key-value pairs.</param>
    private static void RegisterList(MetaData collection, string tableOwner, string tableName,
                                     List<Dictionary<string, object>> list) => collection[Combine(tableOwner, tableName)] = list;

    /// <summary>
    /// Finds the first item in the specified collection that matches the given predicate for a specific table.
    /// </summary>
    /// <param name="collection">The metadata collection containing table-related data.</param>
    /// <param name="tableOwner">The owner of the table whose data is being queried.</param>
    /// <param name="tableName">The name of the table whose data is being queried.</param>
    /// <param name="predicate">The condition to match items in the collection.</param>
    /// <returns>A dictionary representing the first item in the collection that matches the predicate, or null if no match is found.</returns>
    private static Dictionary<string, object> FindFirst(
        MetaData collection, string tableOwner, string tableName, System.Predicate<Dictionary<string, object>> predicate) =>
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
    /// It first tries to match some common SQL types, then fallback in implementing
    /// an extended version of the SQLite data type affinity mechanism.
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
            "blob" or "" => ColumnType.Blob,
            not null when sqliteType.Contains("int") => ColumnType.BigInt,
            not null when sqliteType.Contains("floa") || sqliteType.Contains("doub") => ColumnType.DoublePrecision,
            not null when sqliteType.Contains("dec") || sqliteType.Contains("num") => ColumnType.Decimal,
            not null when sqliteType.Contains("char") || sqliteType.Contains("text") ||
                          sqliteType.Contains("clob") => ColumnType.Text,
            not null when sqliteType.Contains("date") || sqliteType.Contains("time") => ColumnType.DateTime,
            not null when sqliteType.Contains("interval") => ColumnType.Interval,
            not null when sqliteType.Contains("row") => ColumnType.RowVersion,
            not null when sqliteType.Contains("binary") || sqliteType.Contains("blob") ||
                          sqliteType.Contains("byte") => ColumnType.Blob,
            not null when sqliteType.Contains("uid") || sqliteType.Contains("uniqueid") => ColumnType.Guid,
            not null when sqliteType.Contains("xml") => ColumnType.Xml,
            not null when sqliteType.Contains("json") => ColumnType.Json,
            not null when sqliteType.Contains("geo") => ColumnType.Geometry,
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
            ColumnType.Char or ColumnType.NChar or ColumnType.VarChar or ColumnType.NVarChar or ColumnType.Text or
                ColumnType.NText => Utility.UnquotedStr(value),
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