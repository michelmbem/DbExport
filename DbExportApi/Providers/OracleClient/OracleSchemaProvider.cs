using System;
using System.Collections.Generic;
using DbExport.Schema;

namespace DbExport.Providers.OracleClient
{
    public class OracleSchemaProvider : ISchemaProvider
    {
        private readonly string connectionString;
        private readonly string databaseName;

        public OracleSchemaProvider(string connectionString)
        {
            this.connectionString = connectionString;
            
            var properties = Utility.ParseConnectionString(connectionString);
            if (properties.ContainsKey("user id"))
                databaseName = properties["user id"];
            else if (properties.ContainsKey("uid"))
                databaseName = properties["uid"];
        }

        #region ISchemaProvider Members

        public string ProviderName
        {
            get { return "System.Data.OracleClient"; }
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
	                                USER_TABLES
                                WHERE
                                    TEMPORARY = 'N'
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
	                                USER_TAB_COLUMNS
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
            const string sql = @"SELECT
                                    INDEX_NAME
                                FROM
                                    USER_INDEXES
                                WHERE
                                    TABLE_NAME = '{0}'";

            var indexNames = new List<string>();

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object>>(string.Format(sql, tableName), SqlHelper.FetchList);
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
	                                USER_CONSTRAINTS
                                WHERE
	                                TABLE_NAME = '{0}'
	                                AND CONSTRAINT_TYPE = 'R'";

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
            const string sql = @"SELECT
                                    C.CONSTRAINT_NAME,
                                    K.COLUMN_NAME
                                FROM
                                    USER_CONSTRAINTS C,
                                    USER_CONS_COLUMNS K
                                WHERE
                                    C.TABLE_NAME = K.TABLE_NAME
                                    AND C.CONSTRAINT_NAME = K.CONSTRAINT_NAME
                                    AND C.TABLE_NAME = '{0}'
                                    AND C.CONSTRAINT_TYPE = 'P'
                                ORDER BY
                                    K.POSITION";

            var metadata = new Dictionary<string, object>();
            metadata["name"] = tableName;
            metadata["owner"] = string.Empty;

            var pkColumns = new List<string>();
            var pkName = string.Empty;

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
               var list = helper.Query<List<object[]>>(string.Format(sql, tableName), SqlHelper.FetchIndexList);
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
	                                DATA_LENGTH,
	                                DATA_PRECISION,
	                                DATA_SCALE,
	                                DATA_DEFAULT,
	                                NULLABLE
                                FROM
	                                USER_TAB_COLUMNS
                                WHERE
	                                TABLE_NAME =  '{0}'
	                                AND COLUMN_NAME = '{1}'";

            var metadata = new Dictionary<string, object>();
            metadata["name"] = columnName;
            metadata["description"] = string.Empty;

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                ColumnType columnType;
                var attributes = ColumnAttribute.None;
                var values = helper.Query<object[]>(string.Format(sql, tableName, columnName), SqlHelper.FetchIndex);

                metadata["nativeType"] = values[0].ToString();
                metadata["size"] = Utility.ToInt16(values[1]);
                metadata["precision"] = Utility.ToByte(values[2]);
                metadata["scale"] = Utility.ToByte(values[3]);
                metadata["type"] = columnType = GetColumnType((string) metadata["nativeType"],
                                                              (byte) metadata["precision"],
                                                              (byte) metadata["scale"]);
                metadata["defaultValue"] = Parse(Convert.ToString(values[4]), columnType);
                
                if (values[5].Equals("N"))
                    attributes |= ColumnAttribute.Required;
                
                metadata["attributes"] =  attributes;
            }

            return metadata;
        }

        public Dictionary<string, object> GetIndexMeta(string tableName, string indexName)
        {
            const string sql1 = @"SELECT
	                                COLUMN_NAME,
                                    DESCEND
                                FROM
	                                USER_IND_COLUMNS
                                WHERE
	                                TABLE_NAME =  '{0}'
	                                AND INDEX_NAME = '{1}'";

            const string sql2 = @"SELECT
                                    UNIQUENESS
                                FROM
                                    USER_INDEXES
                                WHERE
                                    TABLE_NAME = '{0}'
                                    AND INDEX_NAME = '{1}'";

            const string sql3 = @"SELECT
                                    COUNT(*)
                                FROM
                                    USER_CONSTRAINTS
                                WHERE
                                    TABLE_NAME = '{0}'
                                    AND CONSTRAINT_NAME = '{1}'
                                    AND CONSTRAINT_TYPE = 'P'";

            var metadata = new Dictionary<string, object>();
            metadata["name"] = indexName;
            
            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object[]>>(string.Format(sql1, tableName, indexName), SqlHelper.FetchIndexList);
                var indexColumns = new List<string>();

                foreach (object[] values in list)
                    indexColumns.Add(values[0].ToString());

                metadata["columns"] = indexColumns.ToArray();
                metadata["unique"] = helper.QueryScalar(string.Format(sql2, tableName, indexName)).Equals("UNIQUE");
                metadata["primaryKey"] = helper.QueryScalar(string.Format(sql3, tableName, indexName)).Equals(1);
            }

            return metadata;
        }

        public Dictionary<string, object> GetForeignKeyMeta(string tableName, string fkName)
        {
            const string sql = @"SELECT
                                    FK.COLUMN_NAME FK_Column,
                                    PK.COLUMN_NAME PK_Column,
                                    PT.TABLE_NAME PK_Table,
	                                C.DELETE_RULE
                                FROM
                                    USER_CONSTRAINTS C,
                                    USER_CONS_COLUMNS FK,
                                    USER_CONSTRAINTS PT,
                                    USER_CONS_COLUMNS PK
                                WHERE
	                                C.TABLE_NAME = FK.TABLE_NAME
                                    AND C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
                                    AND C.R_CONSTRAINT_NAME = PT.CONSTRAINT_NAME
                                    AND PT.CONSTRAINT_TYPE = 'P'
                                    AND PT.TABLE_NAME = PK.TABLE_NAME
                                    AND PT.CONSTRAINT_NAME = PK.CONSTRAINT_NAME
                                    AND FK.POSITION = PK.POSITION
                                    AND C.TABLE_NAME = '{0}'
	                                AND C.CONSTRAINT_NAME = '{1}'
                                ORDER BY
	                                FK.POSITION";

            var metadata = new Dictionary<string, object>();
            var fkColumns = new List<string>();
            var relatedColumns = new List<string>();
            var relatedTable = string.Empty;
            var deleteRule = ForeignKeyRule.None;

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object[]>>(string.Format(sql, tableName, fkName), SqlHelper.FetchIndexList);
                foreach (object[] values in list)
                {
                    fkColumns.Add(values[0].ToString());
                    relatedColumns.Add(values[1].ToString());
                    relatedTable = values[2].ToString();
                    deleteRule = values[3].Equals("NO ACTION") ? ForeignKeyRule.None : ForeignKeyRule.Cascade;
                }
            }

            metadata["name"] = fkName;
            metadata["columns"] = fkColumns.ToArray();
            metadata["relatedTable"] = relatedTable;
            metadata["relatedColumns"] = relatedColumns.ToArray();
            metadata["updateRule"] = ForeignKeyRule.None;
            metadata["deleteRule"] = deleteRule;

            return metadata;
        }

        #endregion

        #region Utility

        private static ColumnType GetColumnType(string oracleType, byte precision, byte scale)
        {
            switch (oracleType)
            {
                case "SIMPLE_INTEGER":
                    return ColumnType.Integer;
                case "BINARY_FLOAT":
                    return ColumnType.SinglePrecision;
                case "BINARY_DOUBLE":
                    return ColumnType.DoublePrecision;
                case "NUMBER":
                    if (scale == 0)
                    {
                        if (precision < 3) return ColumnType.TinyInt;
                        if (precision < 5) return ColumnType.SmallInt;
                        if (precision < 10) return ColumnType.Integer;
                        if (precision < 19) return ColumnType.BigInt;
                    }
                    return ColumnType.Decimal;
                case "DATE":
                    return ColumnType.DateTime;
                case "CHAR":
                    return ColumnType.Char;
                case "NCHAR":
                    return ColumnType.NChar;
                case "VARCHAR2":
                    return ColumnType.VarChar;
                case "NVARCHAR2":
                    return ColumnType.NVarChar;
                case "CLOB":
                case "LONG":
                    return ColumnType.Text;
                case "NCLOB":
                    return ColumnType.NText;
                case "BLOB":
                case "RAW":
                case "LONG RAW":
                    return ColumnType.Blob;
                case "BFILE":
                    return ColumnType.File;
                case "XMLType":
                    return ColumnType.Xml;
                default:
                    return oracleType.StartsWith("TIMESTAMP") ? ColumnType.DateTime : ColumnType.Unknown;
            }
        }

        private static object Parse(string value, ColumnType columnType)
        {
            if (Utility.IsEmpty(value) || value.ToUpper() == "NULL")
                return DBNull.Value;

            switch (columnType)
            {
                case ColumnType.TinyInt:
                    return Utility.IsNumeric(value) ? (object) Convert.ToSByte(value) : DBNull.Value;
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

        #endregion
    }
}
