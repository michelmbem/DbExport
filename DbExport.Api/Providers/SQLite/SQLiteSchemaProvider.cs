using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DbExport.Providers.SQLite.SqlParser;
using DbExport.Schema;

namespace DbExport.Providers.SQLite;

public class SQLiteSchemaProvider : ISchemaProvider
{
    private readonly Dictionary<string, AstNode> tableDefinitions;

    public SQLiteSchemaProvider(string connectionString)
    {
        ConnectionString = connectionString;
            
        var properties = Utility.ParseConnectionString(connectionString);
        var dbFilename = properties["data source"];
        DatabaseName = Path.GetFileNameWithoutExtension(dbFilename);

        tableDefinitions = [];
        LoadTableDefinitions();
    }

    #region ISchemaProvider Members

    public string ProviderName => ProviderNames.SQLITE;

    public string ConnectionString { get; }

    public string DatabaseName { get; }

    public (string, string)[] GetTableNames() =>
        tableDefinitions.Keys.Select(name => (name, string.Empty)).ToArray();

    public string[] GetColumnNames(string tableName, string tableOwner)
    {
        var tableDef = tableDefinitions[tableName];
        var colSpecList = tableDef.Children[0];
        var colNames = new string[colSpecList.Children.Count];

        for (var i = 0; i < colNames.Length; ++i)
        {
            var colAttribs = (Dictionary<string, object>)colSpecList.Children[i].Data;
            colNames[i] = colAttribs["COLUMN_NAME"].ToString();
        }

        return colNames;
    }

    public string[] GetIndexNames(string tableName, string tableOwner) => []; // Note: Not relevant for now!

    public string[] GetFKNames(string tableName, string tableOwner)
    {
        var tableDef = tableDefinitions[tableName];

        return [..(from node in tableDef.Children
                where node.Kind == AstNodeKind.FKSPEC
                select (Dictionary<string, object>)node.Data into fkAttribs
                select fkAttribs["CONSTRAINT_NAME"].ToString())];
    }

    public Dictionary<string, object> GetTableMeta(string tableName, string tableOwner)
    {
        Dictionary<string, object> metadata = new()
        {
            ["name"] = tableName,
            ["owner"] = string.Empty
        };

        var tableDef = tableDefinitions[tableName];
        var pkNode = tableDef.Children.FirstOrDefault(node => node.Kind == AstNodeKind.PKSPEC);

        if (pkNode == null)
        {
            var colAttribs = tableDef.Children[0].Children
                                     .Select(colSpec => (Dictionary<string, object>)colSpec.Data)
                                     .FirstOrDefault(colAttribs => Convert.ToBoolean(colAttribs["PRIMARY_KEY"]));
            if (colAttribs == null) return metadata;
            
            metadata["pk_name"] = $"PK_{tableName}";
            metadata["pk_columns"] = (string[])[colAttribs["COLUMN_NAME"].ToString()];
        }
        else
        {
            metadata["pk_name"] = $"PK_{tableName}";
            metadata["pk_columns"] = ExtractColumnNames(pkNode, 0);
        }

        return metadata;
    }

    public Dictionary<string, object> GetColumnMeta(string tableName, string tableOwner, string columnName)
    {
        var metadata = new Dictionary<string, object>
        {
            ["name"] = columnName
        };

        var tableDef = tableDefinitions[tableName];
        var colAttribs = tableDef.Children[0].Children
                                 .Select(colSpec => (Dictionary<string, object>)colSpec.Data)
                                 .FirstOrDefault(colAttribs => colAttribs["COLUMN_NAME"].Equals(columnName));
        
        if (colAttribs == null) return metadata;
        
        ColumnType columnType;
        metadata["type"] = columnType = GetColumnType(colAttribs["TYPE_NAME"].ToString());
        metadata["nativeType"] = colAttribs["TYPE_NAME"];
        metadata["size"] = Utility.ToInt16(colAttribs["PRECISION"]);
        metadata["precision"] = Utility.ToByte(colAttribs["PRECISION"]);
        metadata["scale"] = Utility.ToByte(colAttribs["SCALE"]);
        metadata["defaultValue"] = colAttribs["DEFAULT_VALUE"];
        metadata["description"] = string.Empty;
                    
        var attributes = ColumnAttributes.None;
        if (Convert.ToBoolean(colAttribs["PRIMARY_KEY"]) ||
            !Convert.ToBoolean(colAttribs["ALLOW_DBNULL"]))
        {
            attributes |= ColumnAttributes.Required;
            if (Convert.ToBoolean(colAttribs["AUTO_INCREMENT"]) ||
                (columnType == ColumnType.Integer && Convert.ToBoolean(colAttribs["UNIQUE"])))
            {
                attributes |= ColumnAttributes.Identity;
                metadata["ident_seed"] = metadata["ident_incr"] = 1L;
            }
        }
        metadata["attributes"] = attributes;

        return metadata;
    }

    public Dictionary<string, object> GetIndexMeta(string tableName, string tableOwner, string indexName)
        => null; // Note: Ignored for the moment!

    public Dictionary<string, object> GetForeignKeyMeta(string tableName, string tableOwner, string fkName)
    {
        var metadata = new Dictionary<string, object>();
        var tableDef = tableDefinitions[tableName];

        foreach (var fkSpec in tableDef.Children)
        {
            if (fkSpec.Kind != AstNodeKind.FKSPEC) continue;

            var fkAttribs = (Dictionary<string, object>) fkSpec.Data;
            if (!fkAttribs["CONSTRAINT_NAME"].Equals(fkName)) continue;
            
            metadata["name"] = fkName;
            metadata["columns"] = ExtractColumnNames(fkSpec, 0);
            metadata["relatedName"] = fkAttribs["TARGET_TABLE_NAME"];
            metadata["relatedOwner"] = string.Empty;
            metadata["relatedColumns"] = ExtractColumnNames(fkSpec, 1);
            metadata["updateRule"] = fkAttribs["UPDATE_RULE"];
            metadata["deleteRule"] = fkAttribs["DELETE_RULE"];
            break;
        }

        return metadata;
    }

    #endregion

    #region Utility

    private static string[] ExtractColumnNames(AstNode node, int index)
    {
        var columnList = node.Children[index];
        var columnNames = new string[columnList.Children.Count];
            
        for (var i = 0; i < columnNames.Length; ++i)
            columnNames[i] = columnList.Children[i].Data.ToString();

        return columnNames;
    }

    private static ColumnType GetColumnType(string sqliteType) =>
        sqliteType switch
        {
            "bit" => ColumnType.Boolean,
            "tinyint" => ColumnType.UnsignedTinyInt,
            "smallint" => ColumnType.SmallInt,
            "int" or "integer" => ColumnType.Integer,
            "bigint" => ColumnType.BigInt,
            "float" => ColumnType.SinglePrecision,
            "double" or "real" => ColumnType.DoublePrecision,
            "money" => ColumnType.Currency,
            "decimal" or "numeric" => ColumnType.Decimal,
            "datetime" => ColumnType.DateTime,
            "char" => ColumnType.Char,
            "nchar" => ColumnType.NChar,
            "varchar" => ColumnType.VarChar,
            "nvarchar" => ColumnType.NVarChar,
            "text" => ColumnType.Text,
            "ntext" => ColumnType.NText,
            "blob" or "image" => ColumnType.Blob,
            "guid" => ColumnType.Guid,
            _ => ColumnType.Unknown
        };

    private void LoadTableDefinitions()
    {
        const string sql = "SELECT name, sql FROM sqlite_master WHERE type = 'table'";

        using var helper = new SqlHelper(ProviderName, ConnectionString);
        var list = helper.Query(sql, SqlHelper.ToArrayList);
        
        foreach (var values in list)
        {
            var parser = new Parser(new Scanner(values[1].ToString()));
            var tableDef = parser.CreateTable();
            tableDefinitions.Add(values[0].ToString()!, tableDef);
        }
    }

    #endregion
}