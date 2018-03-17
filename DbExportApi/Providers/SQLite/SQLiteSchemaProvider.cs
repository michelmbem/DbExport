using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DbExport.Schema;
using DbExport.Providers.SQLite.SqlParser;

namespace DbExport.Providers.SQLite
{
    public class SQLiteSchemaProvider : ISchemaProvider
    {
        private readonly string connectionString;
        private readonly string databaseName;
        private readonly Dictionary<string, AstNode> tableDefinitions;

        public SQLiteSchemaProvider(string connectionString)
        {
            this.connectionString = connectionString;
            
            var properties = Utility.ParseConnectionString(connectionString);
            var dbFilename = properties["data source"];
            databaseName = Path.GetFileNameWithoutExtension(dbFilename);

            tableDefinitions = new Dictionary<string, AstNode>();
            LoadTableDefinitions();
        }

        private void LoadTableDefinitions()
        {
            const string sql = @"SELECT name, sql FROM sqlite_master WHERE type = 'table'";

            using (var helper = new SqlHelper(ProviderName, ConnectionString))
            {
                var list = helper.Query<List<object[]>>(sql, SqlHelper.FetchIndexList);
                foreach (object[] values in list)
                {
                    var parser = new Parser(new Scanner(values[1].ToString()));
                    var tabelDef = parser.CreateTable();
                    tableDefinitions.Add(values[0].ToString(), tabelDef);
                }
            }
        }

        #region ISchemaProvider Members

        public string ProviderName
        {
            get { return "System.Data.SQLite"; }
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
            var tableNames = new string[tableDefinitions.Count];
            tableDefinitions.Keys.CopyTo(tableNames, 0);

            return tableNames;
        }

        public string[] GetColumnNames(string tableName)
        {
            var tableDef = tableDefinitions[tableName];
            var colSpecList = tableDef.ChildNodes[0];
            var colNames = new string[colSpecList.ChildNodes.Count];

            for (int i = 0; i < colNames.Length; ++i)
            {
                var colAttribs = colSpecList.ChildNodes[i].Data as Dictionary<string, object>;
                colNames[i] = colAttribs["COLUMN_NAME"].ToString();
            }

            return colNames;
        }

        public string[] GetIndexNames(string tableName)
        {
            return new string[] { }; // Note: Not relevant for now!
        }

        public string[] GetFKNames(string tableName)
        {
            var tableDef = tableDefinitions[tableName];
            var fkNames = new List<string>();

            foreach (AstNode node in tableDef.ChildNodes)
            {
                if (node.Kind == AstNodeKind.FKSPEC)
                {
                    var fkAttribs = (Dictionary<string, object>)node.Data;
                    fkNames.Add(fkAttribs["CONSTRAINT_NAME"].ToString());
                }
            }

            return fkNames.ToArray();
        }

        public Dictionary<string, object> GetTableMeta(string tableName)
        {
            var metadata = new Dictionary<string, object>();
            metadata["name"] = tableName;
            metadata["owner"] = string.Empty;

            var tableDef = tableDefinitions[tableName];
            var foundPK = false;

            foreach (AstNode node in tableDef.ChildNodes)
            {
                if (node.Kind == AstNodeKind.PKSPEC)
                {
                    foundPK = true;
                    metadata["pk_name"] = "PK_" + tableName;
                    metadata["pk_columns"] = ExtractColumnNames(node, 0);

                    break;
                }
            }

            if (!foundPK)
            {
                foreach (AstNode colSpec in tableDef.ChildNodes[0].ChildNodes)
                {
                    var colAttribs = (Dictionary<string, object>)colSpec.Data;
                    if (Convert.ToBoolean(colAttribs["PRIMARY_KEY"]))
                    {
                        metadata["pk_name"] = "PK_" + tableName;
                        metadata["pk_columns"] = new string[] { colAttribs["COLUMN_NAME"].ToString() };

                        break;
                    }
                }
            }

            return metadata;
        }

        public Dictionary<string, object> GetColumnMeta(string tableName, string columnName)
        {
            var metadata = new Dictionary<string, object>();
            metadata["name"] = columnName;
            
            var tableDef = tableDefinitions[tableName];
            foreach (AstNode colSpec in tableDef.ChildNodes[0].ChildNodes)
            {
                var colAttribs = (Dictionary<string, object>) colSpec.Data;
                if (colAttribs["COLUMN_NAME"].Equals(columnName))
                {
                    ColumnType columnType;
                    metadata["type"] = columnType = GetColumnType(colAttribs["TYPE_NAME"].ToString());
                    metadata["nativeType"] = colAttribs["TYPE_NAME"];
                    metadata["size"] = Utility.ToInt16(colAttribs["PRECISION"]);
                    metadata["precision"] = Utility.ToByte(colAttribs["PRECISION"]);
                    metadata["scale"] = Utility.ToByte(colAttribs["SCALE"]);
                    metadata["defaultValue"] = colAttribs["DEFAULT_VALUE"];
                    metadata["description"] = string.Empty;
                    
                    var attributes = ColumnAttribute.None;
                    if (Convert.ToBoolean(colAttribs["PRIMARY_KEY"]) ||
                        !Convert.ToBoolean(colAttribs["ALLOW_DBNULL"]))
                    {
                        attributes |= ColumnAttribute.Required;
                        if (Convert.ToBoolean(colAttribs["AUTO_INCREMENT"]) ||
                            (columnType == ColumnType.Integer && Convert.ToBoolean(colAttribs["UNIQUE"])))
                        {
                            attributes |= ColumnAttribute.Identity;
                            metadata["ident_seed"] = metadata["ident_incr"] = 1L;
                        }
                    }
                    metadata["attributes"] = attributes;

                    break;
                }
            }

            return metadata;
        }

        public Dictionary<string, object> GetIndexMeta(string tableName, string indexName)
        {
            return null; // Note: Ignored for the moment!
        }

        public Dictionary<string, object> GetForeignKeyMeta(string tableName, string fkName)
        {
            var metadata = new Dictionary<string, object>();


            var tableDef = tableDefinitions[tableName];

            foreach (AstNode fkSpec in tableDef.ChildNodes)
            {
                if (fkSpec.Kind != AstNodeKind.FKSPEC) continue;

                var fkAttribs = (Dictionary<string, object>) fkSpec.Data;
                if (fkAttribs["CONSTRAINT_NAME"].Equals(fkName))
                {
                    metadata["name"] = fkName;
                    metadata["columns"] = ExtractColumnNames(fkSpec, 0);
                    metadata["relatedTable"] = fkAttribs["TARGET_TABLE_NAME"];
                    metadata["relatedColumns"] = ExtractColumnNames(fkSpec, 1);
                    metadata["updateRule"] = fkAttribs["UPDATE_RULE"];
                    metadata["deleteRule"] = fkAttribs["DELETE_RULE"];

                    break;
                }
            }

            return metadata;
        }

        #endregion

        #region Utility

        private static string[] ExtractColumnNames(AstNode node, int index)
        {
            var columnList = node.ChildNodes[index];
            var columnNames = new string[columnList.ChildNodes.Count];
            
            for (int i = 0; i < columnNames.Length; ++i)
                columnNames[i] = columnList.ChildNodes[i].Data.ToString();

            return columnNames;
        }

        private static ColumnType GetColumnType(string sqliteType)
        {
            switch (sqliteType)
            {
                case "bit":
                    return ColumnType.Boolean;
                case "tinyint":
                    return ColumnType.UnsignedTinyInt;
                case "smallint":
                    return ColumnType.SmallInt;
                case "int":
                case "integer":
                    return ColumnType.Integer;
                case "bigint":
                    return ColumnType.BigInt;
                case "float":
                    return ColumnType.SinglePrecision;
                case "double":
                case "real":
                    return ColumnType.DoublePrecision;
                case "money":
                    return ColumnType.Currency;
                case "decimal":
                case "numeric":
                    return ColumnType.Decimal;
                case "datetime":
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
                case "blob":
                case "image":
                    return ColumnType.Blob;
                case "guid":
                    return ColumnType.Guid;
                default:
                    return ColumnType.Unknown;
            }
        }

        #endregion
    }
}
