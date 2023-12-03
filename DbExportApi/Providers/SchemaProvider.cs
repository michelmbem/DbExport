using System;
using System.Collections.Generic;
using DbExport.Schema;

namespace DbExport.Providers
{
    public static class SchemaProvider
    {
        public static ISchemaProvider GetProvider(string providerName, string connectionString)
        {
            switch (providerName)
            {
                case "System.Data.OleDb":
                    return new Access.AccessSchemaProvider(connectionString);
                case "System.Data.SqlClient":
                    return new SqlClient.SqlSchemaProvider(connectionString);
                case "Oracle.ManagedDataAccess.Client":
                    return new OracleClient.OracleSchemaProvider(connectionString);
                case "MySql.Data.MySqlClient":
                    return new MySqlClient.MySqlSchemaProvider(connectionString);
                case "Npgsql":
                    return new Npgsql.NpgsqlSchemaProvider(connectionString);
                case "System.Data.SQLite":
                    return new SQLite.SQLiteSchemaProvider(connectionString);
                default:
                    throw new ArgumentException("providerName");
            }
        }

        public static Database GetDatabase(ISchemaProvider provider)
        {
            var database = new Database(provider.DatabaseName, provider.ProviderName, provider.ConnectionString);
            var tableNames = provider.GetTableNames();

            foreach (string tableName in tableNames)
            {
                var table = GetTable(provider, database, tableName);
                database.Tables.Add(table);
            }

            return database;
        }

        public static Database GetDatabase(string providerName, string connectionString)
        {
            return GetDatabase(GetProvider(providerName, connectionString));
        }

        private static Table GetTable(ISchemaProvider provider, Database database, string tableName)
        {
            var metadata = provider.GetTableMeta(tableName);
            var table = new Table(database, tableName, (string) metadata["owner"]);

            var columnNames = provider.GetColumnNames(tableName);
            foreach (string columnName in columnNames)
            {
                var column = GetColumn(provider, table, columnName);
                table.Columns.Add(column);
            }

            var indexNames = provider.GetIndexNames(tableName);
            foreach (string indexName in indexNames)
            {
                var index = GetIndex(provider, table, indexName);
                table.Indexes.Add(index);
            }

            var fkNames = provider.GetFKNames(tableName);
            foreach (string fkName in fkNames)
            {
                var fk = GetForeignKey(provider, table, fkName);
                table.ForeignKeys.Add(fk);
            }

            if (metadata.ContainsKey("pk_name"))
                table.GeneratePrimaryKey((string) metadata["pk_name"], (IEnumerable<string>) metadata["pk_columns"]);

            return table;
        }

        private static Column GetColumn(ISchemaProvider provider, Table table, string columnName)
        {
            var metadata = provider.GetColumnMeta(table.Name, columnName);
            var column = new Column(table, columnName, (ColumnType) metadata["type"], (string) metadata["nativeType"],
                                    (short) metadata["size"], (byte) metadata["precision"], (byte) metadata["scale"],
                                    (ColumnAttribute) metadata["attributes"], metadata["defaultValue"],
                                    (string) metadata["description"]);

            if (metadata.ContainsKey("ident_seed"))
                column.MakeIdentity((long) metadata["ident_seed"], (long) metadata["ident_incr"]);

            return column;
        }

        private static Index GetIndex(ISchemaProvider provider, Table table, string indexName)
        {
            var metadata = provider.GetIndexMeta(table.Name, indexName);
            var index = new Index(table, indexName, (IEnumerable<string>) metadata["columns"],
                                  (bool) metadata["unique"], (bool) metadata["primaryKey"]);

            return index;
        }

        private static ForeignKey GetForeignKey(ISchemaProvider provider, Table table, string fkName)
        {
            var metadata = provider.GetForeignKeyMeta(table.Name, fkName);
            var fk = new ForeignKey(table, fkName, (IEnumerable<string>) metadata["columns"],
                                    (string) metadata["relatedTable"], (string[]) metadata["relatedColumns"],
                                    (ForeignKeyRule) metadata["updateRule"], (ForeignKeyRule) metadata["deleteRule"]);

            return fk;
        }
    }
}
