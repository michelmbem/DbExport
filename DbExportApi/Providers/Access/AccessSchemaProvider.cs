using System;
using System.Collections.Generic;
using System.IO;
using DbExport.Schema;

namespace DbExport.Providers.Access
{
    public class AccessSchemaProvider : ISchemaProvider
    {
        private readonly string connectionString;
        private readonly string databaseName;
        private readonly ADOX.Catalog catalog;

        public AccessSchemaProvider(string connectionString)
        {
            this.connectionString = connectionString;
            
            var properties = Utility.ParseConnectionString(connectionString);
            databaseName = Path.GetFileNameWithoutExtension(properties["data source"]);

            var connection = new ADODB.Connection();
            connection.Open(connectionString, null, null, 0);
            catalog = new ADOX.Catalog { ActiveConnection = connection };
        }

        #region ISchemaProvider Members

        public string ProviderName
        {
            get { return "System.Data.OleDb"; }
        }

        public string ConnectionString
        {
            get { return connectionString; }
        }

        public string DatabaseName
        {
            get { return databaseName; }
        }

        public string[] GetTableNames()
        {
            var tableNames = new List<string>();

            foreach (ADOX.Table table in catalog.Tables)
            {
                if (table.Type != "TABLE") continue;
                tableNames.Add(table.Name);
            }

            return tableNames.ToArray();
        }

        public string[] GetColumnNames(string tableName)
        {
            var columnNames = new List<string>();
            var table = catalog.Tables[tableName];

            foreach (ADOX.Column column in table.Columns)
                columnNames.Add(column.Name);

            return columnNames.ToArray();
        }

        public string[] GetIndexNames(string tableName)
        {
            var indexNames = new List<string>();
            var table = catalog.Tables[tableName];

            foreach (ADOX.Index index in table.Indexes)
                indexNames.Add(index.Name);

            return indexNames.ToArray();
        }

        public string[] GetFKNames(string tableName)
        {
            var fkNames = new List<string>();
            var table = catalog.Tables[tableName];

            foreach (ADOX.Key key in table.Keys)
            {
                if (key.Type != ADOX.KeyTypeEnum.adKeyForeign) continue;
                fkNames.Add(key.Name);
            }

            return fkNames.ToArray();
        }

        public Dictionary<string, object> GetTableMeta(string tableName)
        {
            var metadata = new Dictionary<string, object>();
            var table = catalog.Tables[tableName];

            metadata["name"] = tableName;
            metadata["owner"] = "Admin";
            
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

        public Dictionary<string, object> GetColumnMeta(string tableName, string columnName)
        {
            var metadata = new Dictionary<string, object>();
            var table = catalog.Tables[tableName];
            var column = table.Columns[columnName];
            ColumnType columnType;

            metadata["name"] = column.Name;
            metadata["type"] = columnType = GetColumnType(column.Type);
            metadata["nativeType"] = column.Type.ToString();
            metadata["size"] = (short) column.DefinedSize;
            metadata["precision"] = (byte) column.Precision;
            metadata["scale"] = column.NumericScale;
            metadata["defaultValue"] = DBNull.Value;
            metadata["description"] = string.Empty;

            if (!Utility.IsEmpty(column.Properties["Default"].Value))
                metadata["defaultValue"] = Parse(column.Properties["Default"].Value.ToString(), columnType);

            if (!Utility.IsEmpty(column.Properties["Description"].Value))
                metadata["description"] = column.Properties["Description"].Value.ToString();

            var attributes = ColumnAttribute.None;

            if (!((bool) column.Properties["Nullable"].Value))
                attributes |= ColumnAttribute.Required;

            if (((bool) column.Properties["Fixed Length"].Value))
                attributes |= ColumnAttribute.FixedLength;

            if (((bool) column.Properties["Autoincrement"].Value))
            {
                attributes |= ColumnAttribute.Identity;
                metadata["ident_seed"] = Convert.ToInt64(column.Properties["Seed"].Value);
                metadata["ident_incr"] = Convert.ToInt64(column.Properties["Increment"].Value);
            }

            metadata["attributes"] = attributes;

            return metadata;
        }

        public Dictionary<string, object> GetIndexMeta(string tableName, string indexName)
        {
            var metadata = new Dictionary<string, object>();
            var table = catalog.Tables[tableName];
            var index = table.Indexes[indexName];

            var columns = new string[index.Columns.Count];
            for (int i = 0; i < columns.Length; ++i)
                columns[i] = index.Columns[i].Name;

            metadata["name"] = indexName;
            metadata["unique"] = index.Unique;
            metadata["primaryKey"] = index.PrimaryKey;
            metadata["columns"] = columns;

            return metadata;
        }

        public Dictionary<string, object> GetForeignKeyMeta(string tableName, string fkName)
        {
            var metadata = new Dictionary<string, object>();
            var table = catalog.Tables[tableName];
            var key = table.Keys[fkName];

            var columns = new string[key.Columns.Count];
            var relatedColumns = new string[key.Columns.Count];

            for (int i = 0; i < columns.Length; ++i)
            {
                columns[i] = key.Columns[i].Name;
                relatedColumns[i] = key.Columns[i].RelatedColumn;
            }

            metadata["name"] = fkName;
            metadata["columns"] = columns;
            metadata["relatedTable"] = key.RelatedTable;
            metadata["relatedColumns"] = relatedColumns;
            metadata["updateRule"] = GetFKRule(key.UpdateRule);
            metadata["deleteRule"] = GetFKRule(key.DeleteRule);

            return metadata;
        }

        #endregion

        #region Utility

        private static ColumnType GetColumnType(ADOX.DataTypeEnum dataTypeEnum)
        {
            switch (dataTypeEnum)
            {
                case ADOX.DataTypeEnum.adBoolean:
                    return ColumnType.Boolean;
                case ADOX.DataTypeEnum.adUnsignedTinyInt:
                    return ColumnType.UnsignedTinyInt;
                case ADOX.DataTypeEnum.adSmallInt:
                    return ColumnType.SmallInt;
                case ADOX.DataTypeEnum.adInteger:
                    return ColumnType.Integer;
                case ADOX.DataTypeEnum.adSingle:
                    return ColumnType.SinglePrecision;
                case ADOX.DataTypeEnum.adDouble:
                    return ColumnType.DoublePrecision;
                case ADOX.DataTypeEnum.adCurrency:
                    return ColumnType.Currency;
                case ADOX.DataTypeEnum.adDecimal:
                case ADOX.DataTypeEnum.adNumeric:
                case ADOX.DataTypeEnum.adVarNumeric:
                    return ColumnType.Decimal;
                case ADOX.DataTypeEnum.adDate:
                case ADOX.DataTypeEnum.adDBTimeStamp:
                    return ColumnType.DateTime;
                case ADOX.DataTypeEnum.adVarChar:
                case ADOX.DataTypeEnum.adVarWChar:
                    return ColumnType.NVarChar;
                case ADOX.DataTypeEnum.adLongVarChar:
                case ADOX.DataTypeEnum.adLongVarWChar:
                    return ColumnType.NText;
                case ADOX.DataTypeEnum.adLongVarBinary:
                    return ColumnType.Blob;
                case ADOX.DataTypeEnum.adGUID:
                    return ColumnType.Guid;
                default:
                    return ColumnType.Unknown;
            }
        }

        private static object Parse(string value, ColumnType columnType)
        {
            if (Utility.IsEmpty(value) || value.ToUpper() == "NULL")
                return DBNull.Value;

            switch (columnType)
            {
                case ColumnType.Boolean:
                    switch (value.ToLower())
                    {
                        case "0":
                        case "false":
                            return false;
                        case "1":
                        case "true":
                            return true;
                        default:
                            return DBNull.Value;
                    }
                case ColumnType.UnsignedTinyInt:
                    return Utility.IsNumeric(value) ? (object) Convert.ToByte(value) : DBNull.Value;
                case ColumnType.SmallInt:
                    return Utility.IsNumeric(value) ? (object) Convert.ToInt16(value) : DBNull.Value;
                case ColumnType.Integer:
                    return Utility.IsNumeric(value) ? (object) Convert.ToInt32(value) : DBNull.Value;
                case ColumnType.SinglePrecision:
                    return Utility.IsNumeric(value) ? (object) Convert.ToSingle(value) : DBNull.Value;
                case ColumnType.DoublePrecision:
                    return Utility.IsNumeric(value) ? (object) Convert.ToDouble(value) : DBNull.Value;
                case ColumnType.Currency:
                case ColumnType.Decimal:
                    return Utility.IsNumeric(value) ? (object) Convert.ToDecimal(value) : DBNull.Value;
                case ColumnType.DateTime:
                    return Utility.IsDate(value) ? (object) Convert.ToDateTime(value) : DBNull.Value;
                case ColumnType.NVarChar:
                case ColumnType.NText:
                    return value;
                default:
                    return DBNull.Value;
            }
        }

        private static ForeignKeyRule GetFKRule(ADOX.RuleEnum ruleEnum)
        {
            switch (ruleEnum)
            {
                case ADOX.RuleEnum.adRICascade:
                    return ForeignKeyRule.Cascade;
                case ADOX.RuleEnum.adRISetNull:
                    return ForeignKeyRule.SetNull;
                case ADOX.RuleEnum.adRISetDefault:
                    return ForeignKeyRule.SetDefault;
                default:
                    return ForeignKeyRule.None;
            }
        }

        #endregion
    }
}
