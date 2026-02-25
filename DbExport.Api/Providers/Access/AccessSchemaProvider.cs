using DbExport.Schema;
using System;
using System.Collections.Generic;
using System.IO;

namespace DbExport.Providers.Access;

public class AccessSchemaProvider : ISchemaProvider
{
    private readonly ADOX.Catalog catalog;

    public AccessSchemaProvider(string connectionString)
    {
        ConnectionString = connectionString;
        
        var properties = Utility.ParseConnectionString(connectionString);
        DatabaseName = Path.GetFileNameWithoutExtension(properties["data source"]);

        var connection = new ADODB.Connection();
        connection.Open(connectionString, Options: 0);
        catalog = new ADOX.Catalog { ActiveConnection = connection };
    }

    #region ISchemaProvider Members

    public string ProviderName => ProviderNames.ACCESS;

    public string ConnectionString { get; }

    public string DatabaseName { get; }

    public (string, string)[] GetTableNames()
    {
        List<(string, string)> tablePairs = [];

        foreach (ADOX.Table table in catalog.Tables)
        {
            if (table.Type != "TABLE") continue;
            tablePairs.Add((table.Name, string.Empty));
        }

        return [..tablePairs];
    }

    public string[] GetColumnNames(string tableName, string tableOwner)
    {
        List<string> columnNames = [];
        var table = catalog.Tables[tableName];

        foreach (ADOX.Column column in table.Columns)
            columnNames.Add(column.Name);

        return [..columnNames];
    }

    public string[] GetIndexNames(string tableName, string tableOwner)
    {
        List<string> indexNames = [];
        var table = catalog.Tables[tableName];

        foreach (ADOX.Index index in table.Indexes)
            indexNames.Add(index.Name);

        return [..indexNames];
    }

    public string[] GetFKNames(string tableName, string tableOwner)
    {
        List<string> fkNames = [];
        var table = catalog.Tables[tableName];

        foreach (ADOX.Key key in table.Keys)
        {
            if (key.Type != ADOX.KeyTypeEnum.adKeyForeign) continue;
            fkNames.Add(key.Name);
        }

        return [..fkNames];
    }

    public Dictionary<string, object> GetTableMeta(string tableName, string tableOwner)
    {
        var table = catalog.Tables[tableName];
        Dictionary<string, object> metadata = new()
        {
            ["name"] = tableName,
            ["owner"] = "Admin"
        };
        
        foreach (ADOX.Index index in table.Indexes)
        {
            if (!index.PrimaryKey) continue;

            var pk = new string[index.Columns.Count];
            for (int i = 0; i < index.Columns.Count; ++i)
                pk[i] = index.Columns[i].Name;

            metadata["pk_columns"] = pk;
            metadata["pk_name"] = index.Name;
            break;
        }

        return metadata;
    }

    public Dictionary<string, object> GetColumnMeta(string tableName, string tableOwner, string columnName)
    {
        var table = catalog.Tables[tableName];
        var column = table.Columns[columnName];
        Dictionary<string, object> metadata = new()
        {
            ["name"] = column.Name,
            ["type"] = GetColumnType(column.Type),
            ["nativeType"] = column.Type.ToString(),
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

    public Dictionary<string, object> GetIndexMeta(string tableName, string tableOwner, string indexName)
    {
        var table = catalog.Tables[tableName];
        var index = table.Indexes[indexName];

        var columns = new string[index.Columns.Count];
        for (int i = 0; i < columns.Length; ++i)
            columns[i] = index.Columns[i].Name;

        return new Dictionary<string, object>
        {
            ["name"] = indexName,
            ["unique"] = index.Unique,
            ["primaryKey"] = index.PrimaryKey,
            ["columns"] = columns
        };
    }

    public Dictionary<string, object> GetForeignKeyMeta(string tableName, string tableOwner, string fkName)
    {
        var table = catalog.Tables[tableName];
        var key = table.Keys[fkName];

        var columns = new string[key.Columns.Count];
        var relatedColumns = new string[key.Columns.Count];

        for (int i = 0; i < columns.Length; ++i)
        {
            columns[i] = key.Columns[i].Name;
            relatedColumns[i] = key.Columns[i].RelatedColumn;
        }

        return new Dictionary<string, object>
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

    private static ColumnType GetColumnType(ADOX.DataTypeEnum dataTypeEnum)
    {
        return dataTypeEnum switch
        {
            ADOX.DataTypeEnum.adBoolean => ColumnType.Boolean,
            ADOX.DataTypeEnum.adUnsignedTinyInt => ColumnType.UnsignedTinyInt,
            ADOX.DataTypeEnum.adSmallInt => ColumnType.SmallInt,
            ADOX.DataTypeEnum.adInteger => ColumnType.Integer,
            ADOX.DataTypeEnum.adSingle => ColumnType.SinglePrecision,
            ADOX.DataTypeEnum.adDouble => ColumnType.DoublePrecision,
            ADOX.DataTypeEnum.adCurrency => ColumnType.Currency,
            ADOX.DataTypeEnum.adDecimal or ADOX.DataTypeEnum.adNumeric or
            ADOX.DataTypeEnum.adVarNumeric => ColumnType.Decimal,
            ADOX.DataTypeEnum.adDate or ADOX.DataTypeEnum.adDBTime or
            ADOX.DataTypeEnum.adDBTimeStamp => ColumnType.DateTime,
            ADOX.DataTypeEnum.adVarChar or ADOX.DataTypeEnum.adVarWChar => ColumnType.NVarChar,
            ADOX.DataTypeEnum.adLongVarChar or ADOX.DataTypeEnum.adLongVarWChar => ColumnType.NText,
            ADOX.DataTypeEnum.adLongVarBinary => ColumnType.Blob,
            ADOX.DataTypeEnum.adGUID => ColumnType.Guid,
            _ => ColumnType.Unknown,
        };
    }

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

    private static ForeignKeyRule GetFKRule(ADOX.RuleEnum ruleEnum)
    {
        return ruleEnum switch
        {
            ADOX.RuleEnum.adRICascade => ForeignKeyRule.Cascade,
            ADOX.RuleEnum.adRISetNull => ForeignKeyRule.SetNull,
            ADOX.RuleEnum.adRISetDefault => ForeignKeyRule.SetDefault,
            _ => ForeignKeyRule.None,
        };
    }

    #endregion
}
