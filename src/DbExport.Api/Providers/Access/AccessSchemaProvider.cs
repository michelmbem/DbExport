using System;
using System.IO;
using System.Linq;
using ADODB;
using ADOX;
using DbExport.Schema;
using Column = ADOX.Column;
using DataTypeEnum = ADOX.DataTypeEnum;
using Key = ADOX.Key;
using Table = ADOX.Table;

namespace DbExport.Providers.Access;

/// <summary>
/// Provides schema extraction and metadata retrieval functionalities for Microsoft Access databases.
/// Implements the <see cref="ISchemaProvider"/> interface specific to the Access database provider.
/// </summary>
public class AccessSchemaProvider : ISchemaProvider
{
    /// <summary>
    /// Represents an instance of the ADOX.Catalog used for manipulating and retrieving metadata
    /// from the schema of the connected Microsoft Access database.
    /// Provides access to tables, columns, indexes, and relationships within the database schema.
    /// Strongly tied to the connection established through the <see cref="AccessSchemaProvider"/>.
    /// </summary>
    private readonly Catalog catalog;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessSchemaProvider"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string used to connect to the Access database.</param>
    public AccessSchemaProvider(string connectionString)
    {
        ConnectionString = connectionString;
        
        var properties = Utility.ParseConnectionString(connectionString);
        DatabaseName = Path.GetFileNameWithoutExtension(properties["data source"]);

        var connection = new Connection();
        connection.Open(connectionString, Options: 0);
        catalog = new Catalog { ActiveConnection = connection };
    }

    #region ISchemaProvider Members

    /// <inheritdoc/>
    public string ProviderName => ProviderNames.ACCESS;

    /// <inheritdoc/>
    public string ConnectionString { get; }

    /// <inheritdoc/>
    public string DatabaseName { get; }

    /// <inheritdoc/>
    public NameOwnerPair[] GetTableNames() =>
        [..from Table table in catalog.Tables where table.Type == "TABLE" select new NameOwnerPair { Name = table.Name }];

    /// <inheritdoc/>
    public string[] GetColumnNames(string tableName, string tableOwner) =>
        [..from Column column in catalog.Tables[tableName].Columns select column.Name];

    /// <inheritdoc/>
    public string[] GetIndexNames(string tableName, string tableOwner) =>
        [..from ADOX.Index index in catalog.Tables[tableName].Indexes select index.Name];

    /// <inheritdoc/>
    public string[] GetForeignKeyNames(string tableName, string tableOwner) =>
        [..from Key key in catalog.Tables[tableName].Keys where key.Type == KeyTypeEnum.adKeyForeign select key.Name];

    /// <inheritdoc/>
    public MetaData GetTableMeta(string tableName, string tableOwner)
    {
        var table = catalog.Tables[tableName];
        MetaData metadata = new()
        {
            ["name"] = tableName,
            ["owner"] = "Admin"
        };
        
        foreach (ADOX.Index index in table.Indexes)
        {
            if (!index.PrimaryKey) continue;

            var pk = new string[index.Columns.Count];
            for (var i = 0; i < index.Columns.Count; ++i)
                pk[i] = index.Columns[i].Name;

            metadata["pk_columns"] = pk;
            metadata["pk_name"] = index.Name;
            break;
        }

        return metadata;
    }

    /// <inheritdoc/>
    public MetaData GetColumnMeta(string tableName, string tableOwner, string columnName)
    {
        var table = catalog.Tables[tableName];
        var column = table.Columns[columnName];
        MetaData metadata = new()
        {
            ["name"] = column.Name,
            ["type"] = GetColumnType(column.Type),
            ["nativeType"] = GetNativeTypeName(column.Type),
            ["size"] = (short)column.DefinedSize,
            ["precision"] = (byte)column.Precision,
            ["scale"] = column.NumericScale,
            ["defaultValue"] = DBNull.Value,
            ["description"] = string.Empty
        };

        var defaultValue = column.Properties["Default"].Value;
        if (!Utility.IsEmpty(defaultValue))
            metadata["defaultValue"] = Parse(defaultValue.ToString(), (ColumnType)metadata["type"]);

        var description = column.Properties["Description"].Value;
        if (!Utility.IsEmpty(description))
            metadata["description"] = description.ToString();

        var attributes = ColumnAttributes.None;

        if (!((bool)column.Properties["Nullable"].Value))
            attributes |= ColumnAttributes.Required;

        if (((bool)column.Properties["Fixed Length"].Value))
            attributes |= ColumnAttributes.FixedLength;

        if (((bool)column.Properties["Autoincrement"].Value))
        {
            attributes |= ColumnAttributes.Identity;
            metadata["ident_seed"] = Convert.ToInt64(column.Properties["Seed"].Value);
            metadata["ident_incr"] = Convert.ToInt64(column.Properties["Increment"].Value);
        }

        metadata["attributes"] = attributes;

        return metadata;
    }

    /// <inheritdoc/>
    public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)
    {
        var table = catalog.Tables[tableName];
        var index = table.Indexes[indexName];

        var columns = new string[index.Columns.Count];
        for (var i = 0; i < columns.Length; ++i)
            columns[i] = index.Columns[i].Name;

        return new MetaData
        {
            ["name"] = indexName,
            ["unique"] = index.Unique,
            ["primaryKey"] = index.PrimaryKey,
            ["columns"] = columns
        };
    }

    /// <inheritdoc/>
    public MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName)
    {
        var table = catalog.Tables[tableName];
        var key = table.Keys[fkName];

        var columns = new string[key.Columns.Count];
        var relatedColumns = new string[key.Columns.Count];

        for (var i = 0; i < columns.Length; ++i)
        {
            columns[i] = key.Columns[i].Name;
            relatedColumns[i] = key.Columns[i].RelatedColumn;
        }

        return new MetaData
        {
            ["name"] = fkName,
            ["columns"] = columns,
            ["relatedName"] = key.RelatedTable,
            ["relatedOwner"] = string.Empty,
            ["relatedColumns"] = relatedColumns,
            ["updateRule"] = GetFKRule(key.UpdateRule),
            ["deleteRule"] = GetFKRule(key.DeleteRule)
        };
    }

    #endregion

    #region Utility

    /// <summary>
    /// Converts an ADOX data type enumeration value into a corresponding <see cref="ColumnType"/> enumeration value.
    /// </summary>
    /// <param name="dataTypeEnum">The ADOX data type enumeration value to convert.</param>
    /// <returns>A <see cref="ColumnType"/> value that represents the equivalent column type.</returns>
    private static ColumnType GetColumnType(DataTypeEnum dataTypeEnum) =>
        dataTypeEnum switch
        {
            DataTypeEnum.adBoolean => ColumnType.Boolean,
            DataTypeEnum.adUnsignedTinyInt => ColumnType.UnsignedTinyInt,
            DataTypeEnum.adSmallInt => ColumnType.SmallInt,
            DataTypeEnum.adInteger => ColumnType.Integer,
            DataTypeEnum.adSingle => ColumnType.SinglePrecision,
            DataTypeEnum.adDouble => ColumnType.DoublePrecision,
            DataTypeEnum.adCurrency => ColumnType.Currency,
            DataTypeEnum.adDecimal or DataTypeEnum.adNumeric or
                DataTypeEnum.adVarNumeric => ColumnType.Decimal,
            DataTypeEnum.adDate or DataTypeEnum.adDBTime or
                DataTypeEnum.adDBTimeStamp => ColumnType.DateTime,
            DataTypeEnum.adVarChar or DataTypeEnum.adVarWChar => ColumnType.NVarChar,
            DataTypeEnum.adLongVarChar or DataTypeEnum.adLongVarWChar => ColumnType.NText,
            DataTypeEnum.adLongVarBinary => ColumnType.Blob,
            DataTypeEnum.adGUID => ColumnType.Guid,
            _ => ColumnType.Unknown,
        };

    /// <summary>
    /// Converts the specified ADOX data type enumeration value to its corresponding native type name.
    /// </summary>
    /// <param name="dataTypeEnum">The ADOX data type enumeration value representing the column data type.</param>
    /// <returns>A string representing the native type name for the specified ADOX data type.</returns>
    private static string GetNativeTypeName(DataTypeEnum dataTypeEnum) =>
        dataTypeEnum switch
        {
            DataTypeEnum.adBoolean => "bit",
            DataTypeEnum.adUnsignedTinyInt => "byte",
            DataTypeEnum.adSmallInt => "integer",
            DataTypeEnum.adInteger => "long",
            DataTypeEnum.adSingle => "single",
            DataTypeEnum.adDouble => "double",
            DataTypeEnum.adCurrency => "currency",
            DataTypeEnum.adDecimal or DataTypeEnum.adNumeric or
            DataTypeEnum.adVarNumeric => "decimal",
            DataTypeEnum.adDate or DataTypeEnum.adDBTime or
                DataTypeEnum.adDBTimeStamp => "datetime",
            DataTypeEnum.adVarChar or DataTypeEnum.adVarWChar or
                DataTypeEnum.adLongVarChar or DataTypeEnum.adLongVarWChar => "text",
            DataTypeEnum.adLongVarBinary => "oleobject",
            DataTypeEnum.adGUID => "uniqueidentifier",
            _ => "unknown",
        };

    /// <summary>
    /// Parses a string value into a strongly-typed object based on the specified column type.
    /// </summary>
    /// <param name="value">The string value to be parsed.</param>
    /// <param name="columnType">The type of the column that determines how the value is parsed.</param>
    /// <returns>An object representing the parsed value. Returns <see cref="DBNull.Value"/>
    /// if the value is empty, "NULL", or cannot be parsed according to the column type.</returns>
    private static object Parse(string value, ColumnType columnType)
    {
        if (Utility.IsEmpty(value) || "NULL".Equals(value, StringComparison.OrdinalIgnoreCase))
            return DBNull.Value;

        return columnType switch
        {
            ColumnType.Boolean => value.ToLower() switch
            {
                "0" or "false" => false,
                "1" or "true" => true,
                _ => DBNull.Value,
            },
            ColumnType.UnsignedTinyInt => Utility.IsNumeric(value, out var number) ? (byte)number : DBNull.Value,
            ColumnType.SmallInt => Utility.IsNumeric(value, out var number) ? (short)number : DBNull.Value,
            ColumnType.Integer => Utility.IsNumeric(value, out var number) ? (int)number : DBNull.Value,
            ColumnType.SinglePrecision => Utility.IsNumeric(value, out var number) ? (float)number : DBNull.Value,
            ColumnType.DoublePrecision => Utility.IsNumeric(value, out var number) ? (double)number : DBNull.Value,
            ColumnType.Currency or ColumnType.Decimal => Utility.IsNumeric(value, out var number) ? number : DBNull.Value,
            ColumnType.DateTime => Utility.IsDate(value, out var date) ? date : DBNull.Value,
            ColumnType.NVarChar or ColumnType.NText => value,
            _ => DBNull.Value,
        };
    }

    /// <summary>
    /// Converts an <see cref="ADOX.RuleEnum"/> value to its corresponding <see cref="ForeignKeyRule"/> enumeration value.
    /// </summary>
    /// <param name="ruleEnum">The <see cref="ADOX.RuleEnum"/> value representing the foreign key rule in an Access database.</param>
    /// <returns>A <see cref="ForeignKeyRule"/> enumeration value corresponding to the provided <paramref name="ruleEnum"/>.</returns>
    private static ForeignKeyRule GetFKRule(RuleEnum ruleEnum)
    {
        return ruleEnum switch
        {
            RuleEnum.adRICascade => ForeignKeyRule.Cascade,
            RuleEnum.adRISetNull => ForeignKeyRule.SetNull,
            RuleEnum.adRISetDefault => ForeignKeyRule.SetDefault,
            _ => ForeignKeyRule.None,
        };
    }

    #endregion
}
