using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DbExport.Providers.SQLite.SqlParser;
using DbExport.Schema;

namespace DbExport.Providers.SQLite;

/// <summary>
/// Provides schema information for SQLite databases. This class implements
/// the ISchemaProvider interface, enabling retrieval of database schema
/// metadata such as table names, column names, foreign key names, and associated metadata.
/// </summary>
public class SQLiteSchemaProvider : ISchemaProvider
{
    /// <summary>
    /// A dictionary containing the definitions of tables in the SQLite database schema.
    /// The keys represent the names of the tables, and the values are instances of
    /// <see cref="AstNode"/> that describe the table structure, including metadata,
    /// column definitions, and constraints such as primary keys, foreign keys, and indexes.
    /// </summary>
    private readonly Dictionary<string, AstNode> tableDefinitions;

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

        tableDefinitions = [];
        LoadTableDefinitions();
    }

    #region ISchemaProvider Members

    public string ProviderName => ProviderNames.SQLITE;

    public string ConnectionString { get; }

    public string DatabaseName { get; }

    public NameOwnerPair[] GetTableNames() =>
        [..tableDefinitions.Keys.Select(name => new NameOwnerPair(name))];

    public string[] GetColumnNames(string tableName, string tableOwner)
    {
        var tableDef = tableDefinitions[tableName];
        var colSpecList = tableDef.Children[0];
        var colNames = new string[colSpecList.Children.Count];

        for (var i = 0; i < colNames.Length; ++i)
        {
            var colAttribs = (MetaData)colSpecList.Children[i].Data;
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
                select (MetaData)node.Data into fkAttribs
                select fkAttribs["CONSTRAINT_NAME"].ToString())];
    }

    public MetaData GetTableMeta(string tableName, string tableOwner)
    {
        MetaData metadata = new()
        {
            ["name"] = tableName,
            ["owner"] = string.Empty
        };

        var tableDef = tableDefinitions[tableName];
        var pkNode = tableDef.Children.FirstOrDefault(node => node.Kind == AstNodeKind.PKSPEC);

        if (pkNode == null)
        {
            var colAttribs = tableDef.Children[0].Children
                                     .Select(colSpec => (MetaData)colSpec.Data)
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

    public MetaData GetColumnMeta(string tableName, string tableOwner, string columnName)
    {
        var metadata = new MetaData
        {
            ["name"] = columnName
        };

        var tableDef = tableDefinitions[tableName];
        var colAttribs = tableDef.Children[0].Children
                                 .Select(colSpec => (MetaData)colSpec.Data)
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

    public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)
        => null; // Note: Ignored for the moment!

    public MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName)
    {
        var metadata = new MetaData();
        var tableDef = tableDefinitions[tableName];

        foreach (var fkSpec in tableDef.Children)
        {
            if (fkSpec.Kind != AstNodeKind.FKSPEC) continue;

            var fkAttribs = (MetaData) fkSpec.Data;
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

    /// <summary>
    /// Extracts the column names from the specified AST (Abstract Syntax Tree) node at the given child index.
    /// </summary>
    /// <param name="node">The root AST node containing the column definition data.</param>
    /// <param name="index">The index of the child node where column definitions are located.</param>
    /// <returns>An array of column names extracted from the specified node.</returns>
    private static string[] ExtractColumnNames(AstNode node, int index)
    {
        var columnList = node.Children[index];
        var columnNames = new string[columnList.Children.Count];
            
        for (var i = 0; i < columnNames.Length; ++i)
            columnNames[i] = columnList.Children[i].Data.ToString();

        return columnNames;
    }

    /// <summary>
    /// Determines the corresponding <see cref="ColumnType"/> for a given SQLite data type.
    /// </summary>
    /// <remarks>
    /// This method does not really map SQLite data types to standard column types.
    /// It tries to recognize common data types and return their corresponding <see cref="ColumnType"/>.
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
            "char" or "character" or "nchar" => ColumnType.Char,
            "varchar" or "nvarchar" => ColumnType.VarChar,
            "text" or "ntext" or "clob" or "nclob" => ColumnType.Text,
            "bit" => ColumnType.Bit,
            "blob" => ColumnType.Blob,
            _ => ColumnType.Unknown
        };

    /// <summary>
    /// Loads the definitions of all tables in the connected SQLite database into memory.
    /// The table definitions are obtained by extracting and parsing their DDL from the sqlite_master table.
    /// </summary>
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