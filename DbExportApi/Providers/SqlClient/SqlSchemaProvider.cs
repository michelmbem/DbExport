using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DbExport.Schema;

namespace DbExport.Providers.SqlClient
{
    public class SqlSchemaProvider : ISchemaProvider
    {
        private readonly string connectionString;
        private readonly string databaseName;


        public SqlSchemaProvider(string connectionString)
        {
            this.connectionString = connectionString;
            
            var properties = Utility.ParseConnectionString(connectionString);
            if (properties.ContainsKey("initial catalog"))
                databaseName = properties["initial catalog"];
            else if (properties.ContainsKey("database"))
                databaseName = properties["database"];
            else if (properties.ContainsKey("attachdbfilename"))
            {
                var dbFilename = properties["attachdbfilename"];
                databaseName = Path.GetFileNameWithoutExtension(dbFilename);
            }
        }

        #region ISchemaProvider Members

        public string ProviderName
        {
            get { return "System.Data.SqlClient"; }
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
	                                TABLE_TYPE = 'BASE TABLE'
	                                AND OBJECTPROPERTY(OBJECT_ID(TABLE_NAME), 'IsMsShipped') = 0
                                ORDER BY
	                                TABLE_NAME";

            var tableNames = new List<string>();

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object>>(sql, SqlHelper.FetchList);
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
	                                TABLE_NAME = '{0}'";

            var columnNames = new List<string>();

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object>>(string.Format(sql, tableName), SqlHelper.FetchList);
                foreach (object item in list)
                    columnNames.Add(item.ToString());
            }

            return columnNames.ToArray();
        }

        public string[] GetIndexNames(string tableName)
        {
            var indexNames = new List<string>();

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object[]>>("EXEC sp_helpindex '" + tableName + "'", SqlHelper.FetchIndexList);
                
                foreach (object[] values in list)
                    indexNames.Add(values[0].ToString());
            }

            return indexNames.ToArray();
        }

        public string[] GetFKNames(string tableName)
        {
            const string sql = @"SELECT
	                                CONSTRAINT_NAME
                                FROM
	                                INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                WHERE
	                                TABLE_NAME = '{0}'
	                                AND CONSTRAINT_TYPE = 'FOREIGN KEY'";

            var fkNames = new List<string>();

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object>>(string.Format(sql, tableName), SqlHelper.FetchList);
                foreach (object item in list)
                    fkNames.Add(item.ToString());
            }

            return fkNames.ToArray();
        }

        public Dictionary<string, object> GetTableMeta(string tableName)
        {
            const string sql1 = @"SELECT
	                                TABLE_SCHEMA
                                FROM
	                                INFORMATION_SCHEMA.TABLES
                                WHERE
	                                TABLE_NAME =  '{0}'";

            const string sql2 = @"SELECT
	                                T.CONSTRAINT_NAME,
	                                K.COLUMN_NAME
                                FROM
	                                INFORMATION_SCHEMA.TABLE_CONSTRAINTS T
	                                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE K
	                                ON T.CONSTRAINT_NAME = K.CONSTRAINT_NAME
                                WHERE
	                                T.CONSTRAINT_TYPE = 'PRIMARY KEY'
	                                AND T.TABLE_NAME = '{0}'
                                ORDER BY
	                                K.ORDINAL_POSITION";

            var metadata = new Dictionary<string, object>();
            var pkColumns = new List<string>();
            var pkName = string.Empty;

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                metadata["name"] = tableName;
                metadata["owner"] = helper.QueryScalar(string.Format(sql1, tableName));

                var list = helper.Query<List<object[]>>(string.Format(sql2, tableName), SqlHelper.FetchIndexList);
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
            const string sql1 = @"SELECT
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
	                                AND COLUMN_NAME = '{1}'";

            const string sql2 = @"SELECT
                                    ex.value
                                FROM
                                    sys.columns c
                                    LEFT OUTER JOIN sys.extended_properties ex
                                    ON ex.major_id = c.object_id
                                    AND ex.minor_id = c.column_id
                                    AND ex.name = 'MS_Description'
                                WHERE
                                    OBJECT_NAME(c.object_id) = '{0}'
                                    AND c.name = '{1}'";

            var metadata = new Dictionary<string, object>();
            metadata["name"] = columnName;
            metadata["defaultValue"] = string.Empty;

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                ColumnType columnType;
                var attributes = ColumnAttribute.None;
                var values = helper.Query<object[]>(string.Format(sql1, tableName, columnName), SqlHelper.FetchIndex);
                
                metadata["type"] = columnType = GetColumnType(values[0].ToString());
                metadata["nativeType"] = values[0].ToString();
                metadata["size"] = Utility.ToInt16(values[1]);
                metadata["precision"] = Utility.ToByte(values[2]);
                metadata["scale"] = Utility.ToByte(values[3]);

                if (values[4] != DBNull.Value)
                {
                    var defaultValue = values[5].ToString();
                    metadata["defaultValue"] = Parse(defaultValue.Substring(2, defaultValue.Length - 2), columnType);
                }

                if (values[5].Equals("NO"))
                    attributes |= ColumnAttribute.Required;

                if (values[6].Equals(1))
                    attributes |= ColumnAttribute.Identity;

                if (values[7].Equals(1))
                    attributes |= ColumnAttribute.Computed;

                metadata["attributes"] = attributes;
                metadata["description"] = Convert.ToString(helper.QueryScalar(string.Format(sql2, tableName, columnName)));

                if ((attributes & ColumnAttribute.Identity) != ColumnAttribute.None)
                {
                    metadata["ident_seed"] = Convert.ToInt64(helper.QueryScalar("SELECT IDENT_SEED('" + tableName + "')"));
                    metadata["ident_incr"] = Convert.ToInt64(helper.QueryScalar("SELECT IDENT_INCR('" + tableName + "')"));
                }
            }

            return metadata;
        }

        public Dictionary<string, object> GetIndexMeta(string tableName, string indexName)
        {
            var metadata = new Dictionary<string, object>();
            var indexDescription = string.Empty;
            var indexKeys = string.Empty;

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object[]>>("EXEC sp_helpindex '" + tableName + "'", SqlHelper.FetchIndexList);
                foreach (object[] values in list)
                {
                    if (!values[0].Equals(indexName)) continue;
                    indexDescription = values[1].ToString();
                    indexKeys = values[2].ToString();
                    break;
                }
            }

            metadata["name"] = indexName;
            metadata["unique"] = Regex.IsMatch(indexDescription, @"\bunique\b");
            metadata["primaryKey"] = Regex.IsMatch(indexDescription, @"\bprimary key\b");
            metadata["columns"] = Regex.Split(indexKeys, @"\s*\,\s*");

            return metadata;
        }

        public Dictionary<string, object> GetForeignKeyMeta(string tableName, string fkName)
        {
            const string sql = @"SELECT
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
	                                AND TC1.CONSTRAINT_NAME = '{1}'
                                ORDER BY
	                                KCU1.ORDINAL_POSITION";

            var metadata = new Dictionary<string, object>();
            var fkColumns = new List<string>();
            var relatedColumns = new List<string>();
            var relatedTable = string.Empty;
            var updateRule = ForeignKeyRule.None;
            var deleteRule = ForeignKeyRule.None;

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object[]>>(string.Format(sql, tableName, fkName), SqlHelper.FetchIndexList);
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

        private static ColumnType GetColumnType(string sqlType)
        {
            switch (sqlType)
            {
                case "bit":
                    return ColumnType.Boolean;
                case "tinyint":
                    return ColumnType.UnsignedTinyInt;
                case "smallint":
                    return ColumnType.SmallInt;
                case "int":
                    return ColumnType.Integer;
               case "bigint":
                    return ColumnType.BigInt;
                case "real":
                    return ColumnType.SinglePrecision;
                case "float":
                    return ColumnType.DoublePrecision;
                case "money":
                case "smallmoney":
                    return ColumnType.Currency;
                case "decimal":
                case "numeric":
                    return ColumnType.Decimal;
                case "datetime":
                case "smalldatetime":
                    return ColumnType.DateTime;
                case "char":
                    return ColumnType.Char;
                case "nchar":
                    return ColumnType.NChar;
                case "varchar":
                    return ColumnType.VarChar;
                case "nvarchar":
                    return ColumnType.NVarChar;
                case "text":
                    return ColumnType.Text;
                case "ntext":
                    return ColumnType.NText;
                case "binary":
                case "varbinary":
                case "image":
                    return ColumnType.Blob;
                case "uniqueidentifier":
                    return ColumnType.Guid;
                case "rowversion":
                case "timestamp":
                    return ColumnType.RowVersion;
                case "xml":
                    return ColumnType.Xml;
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
                        case "1":
                            return true;
                        case "0":
                            return false;
                        default:
                            return DBNull.Value;
                    }
                case ColumnType.UnsignedTinyInt:
                    return Utility.IsNumeric(value) ? (object) Convert.ToByte(value) : DBNull.Value;
                case ColumnType.SmallInt:
                    return Utility.IsNumeric(value) ? (object) Convert.ToInt16(value) : DBNull.Value;
                case ColumnType.Integer:
                    return Utility.IsNumeric(value) ? (object) Convert.ToInt32(value) : DBNull.Value;
                case ColumnType.BigInt:
                    return Utility.IsNumeric(value) ? (object) Convert.ToInt64(value) : DBNull.Value;
                case ColumnType.SinglePrecision:
                    return Utility.IsNumeric(value) ? (object) Convert.ToSingle(value) : DBNull.Value;
                case ColumnType.DoublePrecision:
                    return Utility.IsNumeric(value) ? (object) Convert.ToDouble(value) : DBNull.Value;
                case ColumnType.Currency:
                case ColumnType.Decimal:
                    return Utility.IsNumeric(value) ? (object) Convert.ToDecimal(value) : DBNull.Value;
                case ColumnType.DateTime:
                    return Utility.IsDate(value) ? (object) Convert.ToDateTime(value) : DBNull.Value;
                case ColumnType.Char:
                case ColumnType.NChar:
                case ColumnType.VarChar:
                case ColumnType.NVarChar:
                case ColumnType.Text:
                case ColumnType.NText:
                    return value;
                default:
                    return DBNull.Value;
            }
        }

        private static ForeignKeyRule GetFKRule(string rule)
        {
            switch (rule)
            {
                case "CASCADE":
                    return ForeignKeyRule.Cascade;
                default:
                    return ForeignKeyRule.None;
            }
        }

        #endregion
    }
}