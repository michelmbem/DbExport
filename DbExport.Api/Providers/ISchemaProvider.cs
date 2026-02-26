namespace DbExport.Providers;

/// <summary>
/// A common interface for schema providers, which are responsible for retrieving database schema information
/// such as table names, column names, index names, foreign key names, and their associated metadata.
/// This interface abstracts the underlying database provider implementation,
/// allowing for flexibility and extensibility in supporting different database systems.
/// </summary>
public interface ISchemaProvider
{
    /// <summary>
    /// Gets the name of the database provider.
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Gets the connection string used to connect to the database.
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// Gets the name of the database for which the schema information is being retrieved.
    /// </summary>
    string DatabaseName { get; }

    /// <summary>
    /// Extracts the names of all tables in the database, along with their respective owners.
    /// </summary>
    /// <returns>An array of <see cref="NameOwnerPair"/> objects, each containing the name and owner of a table.</returns>
    NameOwnerPair[] GetTableNames();

    /// <summary>
    /// Extracts the names of all columns for a specified table and its owner.
    /// </summary>
    /// <param name="tableName">The name of the table for which to retrieve column names.</param>
    /// <param name="tableOwner">The owner of the table for which to retrieve column names.</param>
    /// <returns>An array of column names for the specified table and owner.</returns>
    string[] GetColumnNames(string tableName, string tableOwner);

    /// <summary>
    /// Extracts the names of all indexes for a specified table and its owner.
    /// </summary>
    /// <param name="tableName">The name of the table for which to retrieve index names.</param>
    /// <param name="tableOwner">The owner of the table for which to retrieve index names.</param>
    /// <returns>An array of index names for the specified table and owner.</returns>
    string[] GetIndexNames(string tableName, string tableOwner);

    /// <summary>
    /// Extracts the names of all foreign keys for a specified table and its owner.
    /// </summary>
    /// <param name="tableName">That name of the table for which to retrieve foreign key names.</param>
    /// <param name="tableOwner">The owner of the table for which to retrieve foreign key names.</param>
    /// <returns>An array of foreign key names for the specified table and owner.</returns>
    string[] GetFKNames(string tableName, string tableOwner);

    /// <summary>
    /// Extracts the metadata for a specified table and its owner, including information such as column data types,
    /// index definitions, foreign key relationships, and other relevant schema details.
    /// </summary>
    /// <param name="tableName">The name of the table for which to retrieve metadata.</param>
    /// <param name="tableOwner">The owner of the table for which to retrieve metadata.</param>
    /// <returns>A <see cref="MetaData"/> object containing the metadata for the specified table and owner.</returns>
    MetaData GetTableMeta(string tableName, string tableOwner);

    /// <summary>
    /// Extracts the metadata for a specified column within a table and its owner, including information such as data type,
    /// nullability, default values, and other relevant schema details specific to the column.
    /// </summary>
    /// <param name="tableName">The name of the table containing the column for which to retrieve metadata.</param>
    /// <param name="tableOwner">The owner of the table containing the column for which to retrieve metadata.</param>
    /// <param name="columnName">The name of the column for which to retrieve metadata.</param>
    /// <returns>A <see cref="MetaData"/> object containing the metadata for the specified column, table, and owner.</returns>
    MetaData GetColumnMeta(string tableName, string tableOwner, string columnName);

    /// <summary>
    /// Extracts the metadata for a specified index within a table and its owner, including information such as index type,
    /// indexed columns, uniqueness, and other relevant schema details specific to the index.
    /// </summary>
    /// <param name="tableName">The name of the table containing the index for which to retrieve metadata.</param>
    /// <param name="tableOwner">The owner of the table containing the index for which to retrieve metadata.</param>
    /// <param name="indexName">The name of the index for which to retrieve metadata.</param>
    /// <returns>A <see cref="MetaData"/> object containing the metadata for the specified index, table, and owner.</returns>
    MetaData GetIndexMeta(string tableName, string tableOwner, string indexName);

    /// <summary>
    /// Extracts the metadata for a specified foreign key within a table and its owner, including information such as referenced table and columns,
    /// foreign key constraints, and other relevant schema details specific to the foreign key relationship.
    /// </summary>
    /// <param name="tableName">The name of the table containing the foreign key for which to retrieve metadata.</param>
    /// <param name="tableOwner">The owner of the table containing the foreign key for which to retrieve metadata.</param>
    /// <param name="fkName">The name of the foreign key for which to retrieve metadata.</param>
    /// <returns>A <see cref="MetaData"/> object containing the metadata for the specified foreign key, table, and owner.</returns>
    MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName);

    /// <summary>
    /// Extracts the names of all types (e.g., user-defined types, enums, etc.) in the database, along with their respective owners.
    /// This method is optional and may not be implemented by all database providers, as not all databases support user-defined types or similar constructs.
    /// </summary>
    /// <returns>An array of <see cref="NameOwnerPair"/> objects, each containing the name and owner of a type in the database.
    /// If the database does not support types or if this method is not implemented, it may return an empty array.</returns>
    NameOwnerPair[] GetTypeNames() => [];

    /// <summary>
    /// Extracts the metadata for a specified type and its owner, including information such as type definition, underlying data type,
    /// allowed values (for enums), and other relevant schema details specific to the type.
    /// </summary>
    /// <param name="typeName">The name of the type for which to retrieve metadata.</param>
    /// <param name="typeOwner">The owner of the type for which to retrieve metadata.</param>
    /// <returns>A <see cref="MetaData"/> object containing the metadata for the specified type and owner.
    /// If the database does not support types or if this method is not implemented, it may return an empty <see cref="MetaData"/> object.</returns>
    MetaData GetTypeMeta(string typeName, string typeOwner) => [];
}