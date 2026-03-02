using System;
using System.Collections.Generic;
using DbExport.Providers.Firebird;
using DbExport.Providers.MySqlClient;
using DbExport.Providers.Npgsql;
using DbExport.Providers.OracleClient;
using DbExport.Providers.SqlClient;
using DbExport.Providers.SQLite;
using DbExport.Schema;

namespace DbExport.Providers;

/// <summary>
/// A factory class that provides methods to create schema providers and retrieve database schemas
/// based on the specified provider name, connection string, and optional schema filter.
/// </summary>
public static class SchemaProvider
{
    /// <summary>
    /// Gets an instance of ISchemaProvider based on the provided provider name and connection string.
    /// </summary>
    /// <param name="providerName">The name of the database provider (e.g., "Microsoft.Data.SqlClient", "Oracle.ManagedDataAccess.Client").</param>
    /// <param name="connectionString">The connection string to connect to the database.</param>
    /// <returns>An instance of ISchemaProvider corresponding to the specified provider name.</returns>
    /// <exception cref="ArgumentException">Thrown when the provider name is not recognized.</exception>
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
            case ProviderNames.FIREBIRD:
                return new FirebirdSchemaProvider(connectionString);
            case ProviderNames.SQLITE:
                return new SQLiteSchemaProvider(connectionString);
            default:
                throw new ArgumentException(null, nameof(providerName));
        }
    }

    /// <summary>
    /// Extracts the database schema using the provided ISchemaProvider and optional schema filter,
    /// and constructs a Database object representing the schema.
    /// </summary>
    /// <param name="provider">An instance of ISchemaProvider to retrieve schema information from the database.</param>
    /// <param name="schema">A string representing the schema filter (e.g., a specific schema name).
    /// If null or whitespace, all schemas will be included.</param>
    /// <returns>A Database object representing the schema of the database as retrieved by the provider.</returns>
    public static Database GetDatabase(ISchemaProvider provider, string schema)
    {
        var database = new Database(provider.DatabaseName, provider.ProviderName, provider.ConnectionString);
        var allPairs = provider.GetTableNames();
        var filteredPairs = string.IsNullOrWhiteSpace(schema) ? allPairs : Array.FindAll(allPairs, IsInSchema);

        foreach (var (tableName, tableOwner) in filteredPairs)
        {
            var table = GetTable(provider, database, tableName, tableOwner);
            database.Tables.Add(table);
        }

        allPairs = provider.GetTypeNames();
        filteredPairs = string.IsNullOrWhiteSpace(schema) ? allPairs : Array.FindAll(allPairs, IsInSchema);

        foreach (var (typeName, typeOwner) in filteredPairs)
        {
            var dataType = GetDataType(provider, database, typeName, typeOwner);
            database.DataTypes.Add(dataType);
        }

        return database;
        
        bool IsInSchema(NameOwnerPair pair) =>
            schema.Equals(pair.Owner, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Extracts the database schema using the specified provider name, connection string, and optional schema filter,
    /// and constructs a Database object representing the schema.
    /// </summary>
    /// <param name="providerName">The name of the database provider
    /// (e.g., "Microsoft.Data.SqlClient", "Oracle.ManagedDataAccess.Client").</param>
    /// <param name="connectionString">The connection string to connect to the database.</param>
    /// <param name="schema">A string representing the schema filter (e.g., a specific schema name).
    /// If null or whitespace, all schemas will be included.</param>
    /// <returns>A Database object representing the schema of the database as retrieved by the provider.</returns>
    public static Database GetDatabase(string providerName, string connectionString, string schema) =>
        GetDatabase(GetProvider(providerName, connectionString), schema);

    /// <summary>
    /// Extracts the schema information for a specific table using the provided ISchemaProvider and constructs a Table object
    /// representing the table schema, including its columns, indexes, and foreign keys. The method retrieves metadata for the table
    /// and its components, and populates the Table object accordingly. If the table has a primary key, it generates the primary key
    /// using the metadata information.
    /// </summary>
    /// <param name="provider">The ISchemaProvider instance to retrieve schema information from the database.</param>
    /// <param name="database">The Database object to which the Table will belong.</param>
    /// <param name="tableName">The name of the table for which to retrieve schema information.</param>
    /// <param name="tableOwner">A string representing the owner of the table (e.g., schema name).
    /// This may be used to filter tables in databases that support multiple schemas.</param>
    /// <returns>A Table object representing the schema of the specified table, including its columns, indexes, and foreign keys.</returns>
    private static Table GetTable(ISchemaProvider provider, Database database, string tableName, string tableOwner)
    {
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

        var fkNames = provider.GetForeignKeyNames(tableName, tableOwner);
        foreach (var fkName in fkNames)
        {
            var fk = GetForeignKey(provider, table, fkName);
            table.ForeignKeys.Add(fk);
        }

        var metadata = provider.GetTableMeta(tableName, tableOwner);
        if (metadata.TryGetValue("pk_name", out var pkName))
            table.GeneratePrimaryKey((string)pkName, (IEnumerable<string>)metadata["pk_columns"]);

        return table;
    }

    /// <summary>
    /// Extracts the schema information for a specific column using the provided ISchemaProvider and constructs a Column object
    /// representing the column schema, including its data type, attributes, default value, and other properties. The method retrieves metadata for the column
    /// and populates the Column object accordingly. If the column is an identity column, it sets the identity properties based on the metadata information.
    /// </summary>
    /// <param name="provider">The ISchemaProvider instance to retrieve schema information from the database.</param>
    /// <param name="table">The Table object to which the Column will belong. This is used to establish the relationship between the column and its parent table.</param>
    /// <param name="columnName">The name of the column for which to retrieve schema information.</param>
    /// <returns>A Column object representing the schema of the specified column, including its data type, attributes, default value, and other properties.</returns>
    private static Column GetColumn(ISchemaProvider provider, Table table, string columnName)
    {
        var metadata = provider.GetColumnMeta(table.Name, table.Owner, columnName);
        var column = new Column(table, columnName, (ColumnType)metadata["type"], (string)metadata["nativeType"],
                                (short)metadata["size"], (byte)metadata["precision"], (byte)metadata["scale"],
                                (ColumnAttributes)metadata["attributes"], metadata["defaultValue"],
                                (string)metadata["description"]);

        if (metadata.TryGetValue("ident_seed", out var identSeed))
            column.MakeIdentity((long)identSeed, (long)metadata["ident_incr"]);

        return column;
    }

    /// <summary>
    /// Extracts the schema information for a specific index using the provided ISchemaProvider and constructs an Index object
    /// representing the index schema, including its columns, uniqueness, and whether it is a primary key. The method retrieves metadata for the index
    /// and populates the Index object accordingly. If the index is a primary key, it sets the primary key properties based on the metadata information.
    /// </summary>
    /// <param name="provider">The ISchemaProvider instance to retrieve schema information from the database.</param>
    /// <param name="table">The Table object to which the Index will belong. This is used to establish the relationship between the index and its parent table.</param>
    /// <param name="indexName">The name of the index for which to retrieve schema information.</param>
    /// <returns>An Index object representing the schema of the specified index, including its columns, uniqueness, and whether it is a primary key.</returns>
    private static Index GetIndex(ISchemaProvider provider, Table table, string indexName)
    {
        var metadata = provider.GetIndexMeta(table.Name, table.Owner, indexName);
        var index = new Index(table, indexName, (IEnumerable<string>)metadata["columns"],
                              (bool)metadata["unique"], (bool)metadata["primaryKey"]);

        return index;
    }

    /// <summary>
    /// Extracts the schema information for a specific foreign key using the provided ISchemaProvider and constructs a ForeignKey object
    /// representing the foreign key schema, including its related table, related columns, and update/delete rules.
    /// The method retrieves metadata for the foreign key and populates the ForeignKey object accordingly.
    /// The related table and columns are determined based on the metadata information,
    /// and the update/delete rules are set based on the foreign key rule values retrieved from the metadata.
    /// </summary>
    /// <param name="provider">The ISchemaProvider instance to retrieve schema information from the database.</param>
    /// <param name="table">The Table object to which the ForeignKey will belong.
    /// This is used to establish the relationship between the foreign key and its parent table.</param>
    /// <param name="fkName">The name of the foreign key for which to retrieve schema information.</param>
    /// <returns>A ForeignKey object representing the schema of the specified foreign key,
    /// including its related table, related columns, and update/delete rules.</returns>
    private static ForeignKey GetForeignKey(ISchemaProvider provider, Table table, string fkName)
    {
        var metadata = provider.GetForeignKeyMeta(table.Name, table.Owner, fkName);
        var fk = new ForeignKey(table, fkName, (IEnumerable<string>)metadata["columns"],
                                (string)metadata["relatedName"], (string)metadata["relatedOwner"],
                                (string[])metadata["relatedColumns"], (ForeignKeyRule)metadata["updateRule"],
                                (ForeignKeyRule)metadata["deleteRule"]);

        return fk;
    }

    /// <summary>
    /// Extracts the schema information for a specific data type using the provided ISchemaProvider and constructs a DataType object
    /// representing the data type schema, including its column type, native type, size, precision, scale, nullability, enumerated status,
    /// default value, and possible values. The method retrieves metadata for the data type and populates the DataType object accordingly.
    /// </summary>
    /// <param name="provider">The ISchemaProvider instance to retrieve schema information from the database.</param>
    /// <param name="database">The Database object to which the DataType will belong.
    /// This is used to establish the relationship between the data type and its parent database.</param>
    /// <param name="typeName">The name of the data type for which to retrieve schema information.</param>
    /// <param name="typeOwner">The owner of the data type (e.g., schema name). This may be used to filter data types in databases that support multiple schemas.</param>
    /// <returns>A DataType object representing the schema of the specified data type, including its column type, native type,
    /// character length, decimal precision and scale, nullability, enumerated status, default value, and possible values (for an enumerated type).</returns>
    private static DataType GetDataType(ISchemaProvider provider, Database database, string typeName, string typeOwner)
    {
        var metadata = provider.GetTypeMeta(typeName, typeOwner);
        var dataType = new DataType(database, typeName, typeOwner, (ColumnType)metadata["type"],
                                    (string)metadata["nativeType"], (short)metadata["size"],
                                    (byte)metadata["precision"], (byte)metadata["scale"],
                                    (bool)metadata["nullable"], (bool)metadata["enumerated"],
                                    metadata["defaultValue"], (IEnumerable<object>)metadata["possibleValues"]);
        
        return dataType;
    }
}