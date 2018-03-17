using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DbExport.Schema;

namespace DbExport.Providers.MySqlClient
{
    public class MySqlSchemaProvider : ISchemaProvider
    {
        private readonly string connectionString;
        private readonly string databaseName;

        public MySqlSchemaProvider(string connectionString)
        {
            this.connectionString = connectionString;
            
            var properties = Utility.ParseConnectionString(connectionString);
            if (properties.ContainsKey("initial catalog"))
                databaseName = properties["initial catalog"];
            else if (properties.ContainsKey("database"))
                databaseName = properties["database"];
        }

        #region ISchemaProvider Members

        public string ProviderName
        {
            get { return "MySql.Data.MySqlClient"; }
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
            const string sql = @"SELECT
                                    TABLE_NAME
                                FROM
                                    INFORMATION_SCHEMA.TABLES
                                WHERE
                                    TABLE_SCHEMA = '{0}'
                                ORDER BY
                                    TABLE_NAME";

            var tableNames = new List<string>();

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object>>(string.Format(sql, DatabaseName), SqlHelper.FetchList);
                foreach (object item in list)
                    tableNames.Add(item.ToString());
            }

            return tableNames.ToArray();
        }

        public string[] GetColumnNames(string tableName)
        {
            const string sql = @"SELECT
                                    COLUMN_NAME
                                FROM
                                    INFORMATION_SCHEMA.COLUMNS
                                WHERE
                                    TABLE_SCHEMA = '{0}' AND
                                    TABLE_NAME = '{1}'";

            var columnNames = new List<string>();

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object>>(string.Format(sql, DatabaseName, tableName), SqlHelper.FetchList);
                foreach (object item in list)
                    columnNames.Add(item.ToString());
            }

            return columnNames.ToArray();
        }

        public string[] GetIndexNames(string tableName)
        {
            const string sql = @"SELECT
                                    DISTINCT INDEX_NAME
                                FROM
                                    INFORMATION_SCHEMA.STATISTICS
                                WHERE
                                    TABLE_SCHEMA = '{0}' AND
                                    TABLE_NAME = '{1}'
                                ORDER BY
                                    INDEX_NAME";

            var indexNames = new List<string>();

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object>>(string.Format(sql, DatabaseName, tableName), SqlHelper.FetchList);
                foreach (object item in list)
                    indexNames.Add(item.ToString());
            }

            return indexNames.ToArray();
        }

        public string[] GetFKNames(string tableName)
        {
            const string sql = @"SELECT
                                    CONSTRAINT_NAME
                                FROM
                                    INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                                WHERE
                                    CONSTRAINT_SCHEMA = '{0}' AND
                                    TABLE_NAME = '{1}'
                                ORDER BY
                                    CONSTRAINT_NAME";

            var fkNames = new List<string>();

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object>>(string.Format(sql, DatabaseName, tableName), SqlHelper.FetchList);
                foreach (object item in list)
                    fkNames.Add(item.ToString());
            }

            return fkNames.ToArray();
        }

        public Dictionary<string, object> GetTableMeta(string tableName)
        {
            const string sql = @"SELECT
                                    PK.CONSTRAINT_NAME,
                                    C.COLUMN_NAME
                                FROM
                                    INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK INNER JOIN
                                    INFORMATION_SCHEMA.KEY_COLUMN_USAGE C ON
                                        PK.TABLE_SCHEMA = C.TABLE_SCHEMA AND
                                        PK.TABLE_NAME = C.TABLE_NAME AND
                                        PK.CONSTRAINT_NAME = C.CONSTRAINT_NAME
                                WHERE
                                    PK.TABLE_SCHEMA = '{0}'AND
                                    PK.TABLE_NAME = '{1}' AND
                                    PK.CONSTRAINT_TYPE = 'PRIMARY KEY'
                                ORDER BY
                                    C.ORDINAL_POSITION";

            var metadata = new Dictionary<string, object>();
            metadata["name"] = tableName;
            metadata["owner"] = string.Empty;

            var pkColumns = new List<string>();
            var pkName = string.Empty;

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object[]>>(string.Format(sql, DatabaseName, tableName), SqlHelper.FetchIndexList);
                foreach (object[] values in list)
                {
                    pkName = values[0].ToString();
                    pkColumns.Add(values[1].ToString());
                }
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
            const string sql = @"SELECT
                                    DATA_TYPE,
                                    CHARACTER_MAXIMUM_LENGTH,
                                    NUMERIC_PRECISION,
                                    NUMERIC_SCALE,
                                    COLUMN_DEFAULT,
                                    COLUMN_COMMENT,
                                    IS_NULLABLE,
                                    CHARACTER_SET_NAME,
                                    EXTRA
                                FROM
                                    INFORMATION_SCHEMA.COLUMNS
                                WHERE
                                    TABLE_SCHEMA = '{0}' AND
                                    TABLE_NAME = '{1}' AND
                                    COLUMN_NAME = '{2}'";

            var metadata = new Dictionary<string, object>();
            metadata["name"] = columnName;

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var attributes = ColumnAttribute.None;
                var values = helper.Query<object[]>(string.Format(sql, DatabaseName, tableName, columnName), SqlHelper.FetchIndex);

                metadata["type"] = GetColumnType(values[0].ToString());
                metadata["nativeType"] = values[0].ToString();
                metadata["size"] = Utility.ToInt16(values[1]);
                metadata["precision"] = Utility.ToByte(values[2]);
                metadata["scale"] = Utility.ToByte(values[3]);
                metadata["defaultValue"] = Parse(Convert.ToString(values[4]), (ColumnType) metadata["type"]);
                metadata["description"] = Convert.ToString(values[5]);

                if (values[6].Equals("NO"))
                    attributes |= ColumnAttribute.Required;

                if (Regex.IsMatch(Convert.ToString(values[7]), "utf8", RegexOptions.IgnoreCase))
                    attributes |= ColumnAttribute.Unicode;

                if (values[8].Equals("auto_increment"))
                {
                    attributes |= ColumnAttribute.Identity;
                    metadata["ident_seed"] = metadata["ident_incr"] = 1L;
                }

                metadata["attributes"] = attributes;
            }

            return metadata;
        }

        public Dictionary<string, object> GetIndexMeta(string tableName, string indexName)
        {
            const string sql = @"SELECT
                                    A.NON_UNIQUE,
                                    A.COLUMN_NAME,
                                    B.CONSTRAINT_TYPE
                                FROM
                                    INFORMATION_SCHEMA.STATISTICS A LEFT OUTER JOIN
                                    INFORMATION_SCHEMA.TABLE_CONSTRAINTS B ON
                                        A.TABLE_SCHEMA = B.TABLE_SCHEMA AND
                                        A.TABLE_NAME = B.TABLE_NAME AND
                                        A.INDEX_NAME = B.CONSTRAINT_NAME
                                WHERE
                                    A.TABLE_SCHEMA = '{0}' AND
                                    A.TABLE_NAME = '{1}' AND
                                    A.INDEX_NAME = '{2}'
                                ORDER BY
                                    A.SEQ_IN_INDEX";

            var metadata = new Dictionary<string, object>();
            metadata["name"] = indexName;

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object[]>>(string.Format(sql, DatabaseName, tableName, indexName), SqlHelper.FetchIndexList);
                var indexColumns = new List<string>();
                var unique = false;
                var primaryKey = false;

                foreach (object[] values in list)
                {
                    unique = values[0].Equals(0);
                    indexColumns.Add(values[1].ToString());
                    primaryKey = values[2].Equals("PRIMARY KEY");
                }

                metadata["unique"] = unique;
                metadata["primaryKey"] = primaryKey;
                metadata["columns"] = indexColumns.ToArray();
            }

            return metadata;
        }

        public Dictionary<string, object> GetForeignKeyMeta(string tableName, string fkName)
        {
            const string sql = @"SELECT
                                    C.COLUMN_NAME,
                                    C.REFERENCED_COLUMN_NAME,
                                    C.REFERENCED_TABLE_NAME,
                                    FK.UPDATE_RULE,
                                    FK.DELETE_RULE
                                FROM
                                    INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS FK INNER JOIN
                                    INFORMATION_SCHEMA.KEY_COLUMN_USAGE C ON
                                    FK.CONSTRAINT_SCHEMA = C.CONSTRAINT_SCHEMA AND
                                    FK.TABLE_NAME = C.TABLE_NAME AND
                                    FK.CONSTRAINT_NAME = C.CONSTRAINT_NAME
                                WHERE
                                    FK.CONSTRAINT_SCHEMA = '{0}' AND
                                    FK.TABLE_NAME = '{1}' AND
                                    FK.CONSTRAINT_NAME = '{2}'
                                ORDER BY
                                    C.ORDINAL_POSITION";

            var metadata = new Dictionary<string, object>();
            var fkColumns = new List<string>();
            var relatedColumns = new List<string>();
            var relatedTable = string.Empty;
            var updateRule = ForeignKeyRule.None;
            var deleteRule = ForeignKeyRule.None;

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object[]>>(string.Format(sql, DatabaseName, tableName, fkName), SqlHelper.FetchIndexList);
                foreach (object[] values in list)
                {
                    fkColumns.Add(values[0].ToString());
                    relatedColumns.Add(values[1].ToString());
                    relatedTable = values[2].ToString();
                    updateRule = GetFKRule(values[3].ToString());
                    deleteRule = GetFKRule(values[4].ToString());
                }
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

        private static ColumnType GetColumnType(string mysqlType)
        {
            switch (mysqlType)
            {
                case "bool":
                case "tinyint":
                    return ColumnType.TinyInt;
                case "tinyint unsigned":
                case "year":
                    return ColumnType.UnsignedTinyInt;
                case "smallint":
                    return ColumnType.SmallInt;
                case "smallint unsigned":
                    return ColumnType.UnsignedSmallInt;
                case "int":
                case "integer":
                case "mediumint":
                    return ColumnType.Integer;
                case "int unsigned":
                case "integer unsigned":
                case "mediumint unsigned":
                    return ColumnType.UnsignedInt;
                case "bigint":
                case "serial":
                    return ColumnType.BigInt;
                case "bigint unsigned":
                    return ColumnType.UnsignedBigInt;
                case "float":
                    return ColumnType.SinglePrecision;
                case "double":
                case "real":
                    return ColumnType.DoublePrecision;
                case "money":
                case "smallmoney":
                    return ColumnType.Currency;
                case "dec":
                case "decimal":
                case "numeric":
                    return ColumnType.Decimal;
                case "date":
                    return ColumnType.Date;
                case "time":
                    return ColumnType.Time;
                case "datetime":
                    return ColumnType.DateTime;
                case "char":
                    return ColumnType.Char;
                case "varchar":
                case "enum":
                case "set":
                    return ColumnType.VarChar;
                case "text":
                case "tinytext":
                case "mediumtext":
                case "longtext":
                    return ColumnType.Text;
                case "bit":
                    return ColumnType.Bit;
                case "blob":
                case "tinyblob":
                case "mediumblob":
                case "longblob":
                    return ColumnType.Blob;
                case "timestamp":
                    return ColumnType.RowVersion;
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
                case ColumnType.TinyInt:
                    return Utility.IsNumeric(value) ? (object) Convert.ToSByte(value) : DBNull.Value;
                case ColumnType.UnsignedTinyInt:
                    return Utility.IsNumeric(value) ? (object) Convert.ToByte(value) : DBNull.Value;
                case ColumnType.SmallInt:
                    return Utility.IsNumeric(value) ? (object) Convert.ToInt16(value) : DBNull.Value;
                case ColumnType.UnsignedSmallInt:
                    return Utility.IsNumeric(value) ? (object) Convert.ToUInt16(value) : DBNull.Value;
                case ColumnType.Integer:
                    return Utility.IsNumeric(value) ? (object) Convert.ToInt32(value) : DBNull.Value;
                case ColumnType.UnsignedInt:
                    return Utility.IsNumeric(value) ? (object) Convert.ToUInt32(value) : DBNull.Value;
                case ColumnType.BigInt:
                    return Utility.IsNumeric(value) ? (object) Convert.ToInt64(value) : DBNull.Value;
                case ColumnType.UnsignedBigInt:
                    return Utility.IsNumeric(value) ? (object) Convert.ToUInt64(value) : DBNull.Value;
                case ColumnType.SinglePrecision:
                    return Utility.IsNumeric(value) ? (object) Convert.ToSingle(value) : DBNull.Value;
                case ColumnType.DoublePrecision:
                    return Utility.IsNumeric(value) ? (object) Convert.ToDouble(value) : DBNull.Value;
                case ColumnType.Currency:
                case ColumnType.Decimal:
                    return Utility.IsNumeric(value) ? (object) Convert.ToDecimal(value) : DBNull.Value;
                case ColumnType.Date:
                case ColumnType.Time:
                case ColumnType.DateTime:
                    return Utility.IsDate(value) ? (object) Convert.ToDateTime(value) : DBNull.Value;
                case ColumnType.Char:
                case ColumnType.VarChar:
                case ColumnType.Text:
                    return value;
                case ColumnType.Bit:
                    return Utility.FromBitString(value);
                default:
                    return DBNull.Value;
            }
        }

        private static ForeignKeyRule GetFKRule(string rule)
        {
            switch (rule)
            {
                case "RESTRICT":
                    return ForeignKeyRule.Restrict;
                case "CASCADE":
                    return ForeignKeyRule.Cascade;
                case "SET DEFAULT":
                    return ForeignKeyRule.SetDefault;
                case "SET NULL":
                    return ForeignKeyRule.SetNull;
                default:
                    return ForeignKeyRule.None;
            }
        }

        #endregion
    }
}
