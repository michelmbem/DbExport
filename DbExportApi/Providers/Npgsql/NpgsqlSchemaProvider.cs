using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DbExport.Schema;

namespace DbExport.Providers.Npgsql
{
    public class NpgsqlSchemaProvider : ISchemaProvider
    {
        private readonly string connectionString;
        private readonly string databaseName;

        public NpgsqlSchemaProvider(string connectionString)
        {
            this.connectionString = connectionString;
            
            var properties = Utility.ParseConnectionString(connectionString);
            databaseName = properties["database"];
        }

        #region ISchemaProvider Members

        public string ProviderName
        {
            get { return "Npgsql"; }
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
	                                TABLE_SCHEMA = 'public'
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
	                                TABLE_SCHEMA = 'public'
	                                AND TABLE_NAME = '{0}'
                                ORDER BY
	                                ORDINAL_POSITION";

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
	                                INDEXNAME
                                FROM
	                                PG_CATALOG.PG_INDEXES
                                WHERE
	                                SCHEMANAME = 'public'
	                                AND TABLENAME = '{0}'";

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
	                                INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                WHERE
	                                CONSTRAINT_TYPE = 'FOREIGN KEY'
	                                AND TABLE_SCHEMA = 'public'
	                                AND TABLE_NAME = '{0}'
                                ORDER BY
	                                CONSTRAINT_NAME";

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
	                                PK.CONSTRAINT_NAME,
	                                C.COLUMN_NAME
                                FROM
	                                INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK
	                                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE C
	                                ON PK.TABLE_SCHEMA = C.TABLE_SCHEMA
	                                AND PK.TABLE_NAME = C.TABLE_NAME
	                                AND PK.CONSTRAINT_NAME = C.CONSTRAINT_NAME
                                WHERE
	                                PK.CONSTRAINT_TYPE = 'PRIMARY KEY'
	                                AND PK.TABLE_SCHEMA = 'public'
	                                AND PK.TABLE_NAME = '{0}'
                                ORDER BY
	                                C.ORDINAL_POSITION";

            var metadata = new Dictionary<string, object>();
            metadata["name"] = tableName;
            metadata["owner"] = "public";

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
	                                CHARACTER_MAXIMUM_LENGTH,
	                                NUMERIC_PRECISION,
	                                NUMERIC_SCALE,
	                                COLUMN_DEFAULT,
	                                CHARACTER_SET_NAME,
	                                IS_NULLABLE,
	                                IS_IDENTITY,
	                                IDENTITY_START,
	                                IDENTITY_INCREMENT,
	                                IS_GENERATED,
	                                GENERATION_EXPRESSION
                                FROM
	                                INFORMATION_SCHEMA.COLUMNS
                                WHERE
	                                TABLE_SCHEMA = 'public'
	                                AND TABLE_NAME = '{0}'
	                                AND COLUMN_NAME = '{1}'";

            var metadata = new Dictionary<string, object>();
            metadata["name"] = columnName;
            metadata["description"] = string.Empty;

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                ColumnType columnType;
                var attributes = ColumnAttribute.None;
                var values = helper.Query<object[]>(string.Format(sql, tableName, columnName), SqlHelper.FetchIndex);
                
                metadata["type"] = columnType = GetColumnType(values[0].ToString());
                metadata["nativeType"] = values[0].ToString();
                metadata["size"] = Utility.ToInt16(values[1]);
                metadata["precision"] = Utility.ToByte(values[2]);
                metadata["scale"] = Utility.ToByte(values[3]);
                metadata["defaultValue"] = Parse(Convert.ToString(values[4]), columnType);

                if (Regex.IsMatch(Convert.ToString(values[5]), "utf8", RegexOptions.IgnoreCase))
                    attributes |= ColumnAttribute.Unicode;

                if (values[6].Equals("NO"))
                    attributes |= ColumnAttribute.Required;

                if (values[7].Equals("YES"))
                {
                    attributes |= ColumnAttribute.Identity;
                    metadata["ident_seed"] = Convert.ToString(values[8]);
                    metadata["ident_incr"] = Convert.ToString(values[9]);
                }
                else if (values[0].Equals("serial"))
                {
                    attributes |= ColumnAttribute.Identity;
                    metadata["ident_seed"] = metadata["ident_incr"] = 1L;
                }

                if (values[10].Equals("YES"))
                {
                    attributes |= ColumnAttribute.Computed;
                    metadata["expression"] = Convert.ToString(values[11]);
                }

                metadata["attributes"] = attributes;
            }

            return metadata;
        }

        public Dictionary<string, object> GetIndexMeta(string tableName, string indexName)
        {
            const string sql1 = @"SELECT
	                                INDEXDEF
                                FROM
	                                PG_CATALOG.PG_INDEXES
                                WHERE
	                                SCHEMANAME = 'public'
	                                AND TABLENAME = '{0}'
	                                AND INDEXNAME = '{1}'";

            const string sql2 = @"SELECT
                                    COUNT(*)
                                FROM
                                    INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                WHERE
                                    TABLE_SCHEMA = 'public'
                                    AND TABLE_NAME = '{0}'
                                    AND CONSTRAINT_NAME = '{1}'
                                    AND CONSTRAINT_TYPE = 'PRIMARY KEY'";

            var metadata = new Dictionary<string, object>();
            metadata["name"] = indexName;

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var def = helper.QueryScalar(string.Format(sql1, tableName, indexName)).ToString();
                var lparen = def.IndexOf('(');
                var rparent = def.LastIndexOf(')');
                var columnNames = def.Substring(lparen + 1, rparent - lparen - 1);

                metadata["unique"] = def.StartsWith("CREATE UNIQUE INDEX");
                metadata["columns"] = Regex.Split(columnNames, @"\s*\,\s*");
                metadata["primaryKey"] = helper.QueryScalar(string.Format(sql2, tableName, indexName)).Equals(1);
            }

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
                                        ON RC.CONSTRAINT_SCHEMA = TC1.CONSTRAINT_SCHEMA
                                        AND RC.CONSTRAINT_NAME = TC1.CONSTRAINT_NAME
                                    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU1
                                        ON TC1.CONSTRAINT_SCHEMA = KCU1.CONSTRAINT_SCHEMA
                                        AND TC1.CONSTRAINT_NAME = KCU1.CONSTRAINT_NAME
                                    JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC2
                                        ON RC.CONSTRAINT_SCHEMA = TC2.CONSTRAINT_SCHEMA
                                        AND RC.UNIQUE_CONSTRAINT_NAME = TC2.CONSTRAINT_NAME
                                    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU2
                                        ON TC2.CONSTRAINT_SCHEMA = KCU2.CONSTRAINT_SCHEMA
                                        AND TC2.CONSTRAINT_NAME = KCU2.CONSTRAINT_NAME
                                        AND KCU1.ORDINAL_POSITION = KCU2.ORDINAL_POSITION
                                WHERE
                                    TC1.TABLE_SCHEMA = 'public'
                                    AND TC1.TABLE_NAME = '{0}'
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

        private static ColumnType GetColumnType(string mysqlType)
        {
            switch (mysqlType)
            {
                case "bool":
                case "boolean":
                    return ColumnType.Boolean;
                case "int2":
                case "smallint":
                    return ColumnType.SmallInt;
                case "int":
                case "int4":
                case "integer":
                    return ColumnType.Integer;
                case "int8":
                case "bigint":
                case "serial":
                    return ColumnType.BigInt;
                case "real":
                case "float4":
                    return ColumnType.SinglePrecision;
                case "double precision":
                case "float8":
                    return ColumnType.DoublePrecision;
                case "money":
                    return ColumnType.Currency;
                case "numeric":
                    return ColumnType.Decimal;
                case "date":
                    return ColumnType.Date;
                case "time":
                    return ColumnType.Time;
                case "datetime":
                    return ColumnType.DateTime;
                case "interval":
                    return ColumnType.Interval;
                case "char":
                case "character":
                case "bpchar":
                    return ColumnType.Char;
                case "varchar":
                case "character varying":
                    return ColumnType.VarChar;
                case "text":
                    return ColumnType.Text;
                case "bit":
                case "bit varying":
                case "varbit":
                    return ColumnType.Bit;
                case "bytea":
                    return ColumnType.Blob;
                default:
                    return mysqlType.StartsWith("timestamp") ? ColumnType.DateTime : ColumnType.Unknown;
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
                        case "n":
                        case "no":
                        case "f":
                        case "false":
                            return false;
                        case "1":
                        case "y":
                        case "yes":
                        case "t":
                        case "true":
                            return true;
                        default:
                            return DBNull.Value;
                    }
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
