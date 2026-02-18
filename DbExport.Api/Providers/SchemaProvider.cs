using System;
using System.Collections.Generic;
using DbExport.Providers.MySqlClient;
using DbExport.Providers.Npgsql;
using DbExport.Providers.OracleClient;
using DbExport.Providers.SqlClient;
using DbExport.Providers.SQLite;
using DbExport.Schema;

namespace DbExport.Providers;

public static class SchemaProvider
{
    public static ISchemaProvider GetProvider(string providerName, string connectionString)
    {
        switch (providerName)
        {
#if WINDOWS
            case ProviderNames.ACCESS:
                return new Access.AccessSchemaProvider(connectionString);
#endif
            case ProviderNames.SQLSERVER:
                return new SqlSchemaProvider(connectionString);
            case ProviderNames.ORACLE:
                return new OracleSchemaProvider(connectionString);
            case ProviderNames.MYSQL:
                return new MySqlSchemaProvider(connectionString);
            case ProviderNames.POSTGRESQL:
                return new NpgsqlSchemaProvider(connectionString);
            case ProviderNames.SQLITE:
                return new SQLiteSchemaProvider(connectionString);
            default:
                throw new ArgumentException(null, nameof(providerName));
        }
    }

    public static Database GetDatabase(ISchemaProvider provider, string schema)
    {
        var database = new Database(provider.DatabaseName, provider.ProviderName, provider.ConnectionString);
        var tablePairs = provider.GetTableNames();
        var filteredTablePairs = string.IsNullOrWhiteSpace(schema)
            ? tablePairs
            : Array.FindAll(tablePairs, pair => pair.Item2 == schema);

        foreach (var (tableName, tableOwner) in filteredTablePairs)
        {
            var table = GetTable(provider, database, tableName, tableOwner);
            database.Tables.Add(table);
        }

        return database;
    }

    public static Database GetDatabase(string providerName, string connectionString, string schema) =>
        GetDatabase(GetProvider(providerName, connectionString), schema);

    private static Table GetTable(ISchemaProvider provider, Database database, string tableName, string tableOwner)
    {
        var metadata = provider.GetTableMeta(tableName, tableOwner);
        var table = new Table(database, tableName, tableOwner);

        var columnNames = provider.GetColumnNames(tableName, tableOwner);
        foreach (var columnName in columnNames)
        {
            var column = GetColumn(provider, table, columnName);
            table.Columns.Add(column);
        }

        var indexNames = provider.GetIndexNames(tableName, tableOwner);
        foreach (var indexName in indexNames)
        {
            var index = GetIndex(provider, table, indexName);
            table.Indexes.Add(index);
        }

        var fkNames = provider.GetFKNames(tableName, tableOwner);
        foreach (var fkName in fkNames)
        {
            var fk = GetForeignKey(provider, table, fkName);
            table.ForeignKeys.Add(fk);
        }

        if (metadata.TryGetValue("pk_name", out var pkName))
            table.GeneratePrimaryKey((string)pkName, (IEnumerable<string>)metadata["pk_columns"]);

        return table;
    }

    private static Column GetColumn(ISchemaProvider provider, Table table, string columnName)
    {
        var metadata = provider.GetColumnMeta(table.Name, table.Owner, columnName);
        var column = new Column(table, columnName, (ColumnType)metadata["type"], (string)metadata["nativeType"],
                                (short)metadata["size"], (byte)metadata["precision"], (byte)metadata["scale"],
                                (ColumnAttribute)metadata["attributes"], metadata["defaultValue"],
                                (string)metadata["description"]);

        if (metadata.TryGetValue("ident_seed", out var identSeed))
            column.MakeIdentity((long)identSeed, (long)metadata["ident_incr"]);

        return column;
    }

    private static Index GetIndex(ISchemaProvider provider, Table table, string indexName)
    {
        var metadata = provider.GetIndexMeta(table.Name, table.Owner, indexName);
        var index = new Index(table, indexName, (IEnumerable<string>)metadata["columns"],
                              (bool)metadata["unique"], (bool)metadata["primaryKey"]);

        return index;
    }

    private static ForeignKey GetForeignKey(ISchemaProvider provider, Table table, string fkName)
    {
        var metadata = provider.GetForeignKeyMeta(table.Name, table.Owner, fkName);
        var fk = new ForeignKey(table, fkName, (IEnumerable<string>)metadata["columns"],
                                (string)metadata["relatedTable"], (string[])metadata["relatedColumns"],
                                (ForeignKeyRule)metadata["updateRule"], (ForeignKeyRule)metadata["deleteRule"]);

        return fk;
    }
}