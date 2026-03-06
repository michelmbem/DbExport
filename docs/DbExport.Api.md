# DbExport.Api Documentation

Auto-generated from XML doc comments in the DbExport.Api module.

## Index
- [ExportOptions](#exportoptions)
- [IVisitor](#ivisitor)
- [IVisitorAcceptor](#ivisitoracceptor)
- [Properties/AssemblyInfo](#properties/assemblyinfo)
- [Providers/Access/AccessSchemaBuilder](#providers/access/accessschemabuilder)
- [Providers/Access/AccessSchemaProvider](#providers/access/accessschemaprovider)
- [Providers/CodeGenerator](#providers/codegenerator)
- [Providers/Firebird/FirebirdCodeGenerator](#providers/firebird/firebirdcodegenerator)
- [Providers/Firebird/FirebirdOptions](#providers/firebird/firebirdoptions)
- [Providers/Firebird/FirebirdSchemaProvider](#providers/firebird/firebirdschemaprovider)
- [Providers/Firebird/FirebirdScriptExecutor](#providers/firebird/firebirdscriptexecutor)
- [Providers/ISchemaProvider](#providers/ischemaprovider)
- [Providers/IScriptExecutor](#providers/iscriptexecutor)
- [Providers/MetaData](#providers/metadata)
- [Providers/MySqlClient/MySqlCodeGenerator](#providers/mysqlclient/mysqlcodegenerator)
- [Providers/MySqlClient/MySqlOptions](#providers/mysqlclient/mysqloptions)
- [Providers/MySqlClient/MySqlSchemaProvider](#providers/mysqlclient/mysqlschemaprovider)
- [Providers/MySqlClient/MySqlScriptExecutor](#providers/mysqlclient/mysqlscriptexecutor)
- [Providers/NameOwnerPair](#providers/nameownerpair)
- [Providers/Npgsql/NpgsqlCodeGenerator](#providers/npgsql/npgsqlcodegenerator)
- [Providers/Npgsql/NpgsqlSchemaProvider](#providers/npgsql/npgsqlschemaprovider)
- [Providers/Npgsql/NpgsqlScriptExecutor](#providers/npgsql/npgsqlscriptexecutor)
- [Providers/OracleClient/OracleCodeGenerator](#providers/oracleclient/oraclecodegenerator)
- [Providers/OracleClient/OracleSchemaProvider](#providers/oracleclient/oracleschemaprovider)
- [Providers/ProviderNames](#providers/providernames)
- [Providers/SQLite/SQLiteCodeGenerator](#providers/sqlite/sqlitecodegenerator)
- [Providers/SQLite/SQLiteSchemaProvider](#providers/sqlite/sqliteschemaprovider)
- [Providers/SchemaProvider](#providers/schemaprovider)
- [Providers/SqlClient/SqlCodeGenerator](#providers/sqlclient/sqlcodegenerator)
- [Providers/SqlClient/SqlSchemaProvider](#providers/sqlclient/sqlschemaprovider)
- [Providers/SqlClient/SqlScripExecutor](#providers/sqlclient/sqlscripexecutor)
- [Schema/Column](#schema/column)
- [Schema/ColumnAttributes](#schema/columnattributes)
- [Schema/ColumnCollection](#schema/columncollection)
- [Schema/ColumnSet](#schema/columnset)
- [Schema/ColumnType](#schema/columntype)
- [Schema/DataType](#schema/datatype)
- [Schema/DataTypeCollection](#schema/datatypecollection)
- [Schema/Database](#schema/database)
- [Schema/ForeignKey](#schema/foreignkey)
- [Schema/ForeignKeyCollection](#schema/foreignkeycollection)
- [Schema/ForeignKeyRule](#schema/foreignkeyrule)
- [Schema/ICheckable](#schema/icheckable)
- [Schema/IDataItem](#schema/idataitem)
- [Schema/Index](#schema/index)
- [Schema/IndexCollection](#schema/indexcollection)
- [Schema/Key](#schema/key)
- [Schema/PrimaryKey](#schema/primarykey)
- [Schema/SchemaItem](#schema/schemaitem)
- [Schema/SchemaItemCollection](#schema/schemaitemcollection)
- [Schema/Table](#schema/table)
- [Schema/TableCollection](#schema/tablecollection)
- [Schema/TableExtensions](#schema/tableextensions)
- [SqlHelper](#sqlhelper)
- [Utility](#utility)

## ExportOptions

- `public enum ExportFlags`

  Flags to specify what aspects of the database to export. These can be combined using bitwise operations.

- `public class ExportOptions`

  Options for exporting a database, including what to export and any provider-specific settings.

- `public bool ExportSchema { get; set; }`

  Indicates whether the schema of the database should be exported as part of the export process. When set to true, the structural definitions of tables, views, and other schema components will be included in the export.

- `public bool ExportData { get; set; }`

  Determines whether the data within the tables of the database should be exported as part of the export process. When set to true, the contents of the tables, such as rows of data, will be included in the export.

- `public ExportFlags Flags { get; set; }`

  Specifies the flags that determine which components of a database should be exported during the export process. This property allows combining multiple values from the ExportFlags enumeration to customize the export behavior, such as including or excluding primary keys, foreign keys, indexes, defaults, or identity columns.

- `public dynamic ProviderSpecific { get; set; }`

  Allows specifying provider-specific settings to be used during the export process. This property can hold configuration options or parameters unique to a particular database provider, enabling customization of the export behavior for that provider.

- `public void SetFlag(ExportFlags flag, bool value) => Flags = value ? Flags | flag : Flags & ~flag;`

  Sets or clears a specific flag in the ExportFlags enumeration.

- `public bool HasFlag(ExportFlags flag) => Flags.HasFlag(flag);`

  Checks if a specific flag in the ExportFlags enumeration is set.


## IVisitor

- `public interface IVisitor`

  Defines the Visitor pattern for traversing the database schema. Each method corresponds to a specific schema element, allowing for operations to be performed on databases, tables, columns, keys, and indexes without modifying their classes. This design promotes separation of concerns and makes it easier to add new operations on the schema elements without changing their structure.


## IVisitorAcceptor

- `public interface IVisitorAcceptor`

  Defines an interface for accepting visitors, allowing external operations to be performed on implementing classes without modifying their structure. This is a key component of the Visitor design pattern, enabling separation of concerns and enhancing flexibility in extending functionality.


## Properties/AssemblyInfo


## Providers/Access/AccessSchemaBuilder

- `public class AccessSchemaBuilder(string connectionString) : IVisitor`

  Responsible for building and exporting database schema and data for Access databases. Implements the DbExport.IVisitor interface to support the visiting pattern for database schema elements. Handles schema creation, data export, foreign keys, indexes, identities, primary keys, and more based on configuration.

- `public ExportOptions ExportOptions { get; set; }`

  Gets or sets the export options that control the behavior of the schema and data export process.

- `public void VisitDatabase(Database database)`

  No XML summary provided.

- `public void VisitTable(Table table)`

  No XML summary provided.

- `public void VisitColumn(Column column)`

  No XML summary provided.

- `public void VisitPrimaryKey(PrimaryKey primaryKey)`

  No XML summary provided.

- `public void VisitIndex(Index index)`

  No XML summary provided.

- `public void VisitForeignKey(ForeignKey foreignKey)`

  No XML summary provided.


## Providers/Access/AccessSchemaProvider

- `public class AccessSchemaProvider : ISchemaProvider`

  Provides schema extraction and metadata retrieval functionalities for Microsoft Access databases. Implements the ISchemaProvider interface specific to the Access database provider.

- `public AccessSchemaProvider(string connectionString)`

  Initializes a new instance of the AccessSchemaProvider class.

- `public string ProviderName => ProviderNames.ACCESS;`

  No XML summary provided.

- `public string ConnectionString { get; }`

  No XML summary provided.

- `public string DatabaseName { get; }`

  No XML summary provided.

- `public NameOwnerPair[] GetTableNames()`

  No XML summary provided.

- `public string[] GetColumnNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetIndexNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetForeignKeyNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetTableMeta(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetColumnMeta(string tableName, string tableOwner, string columnName)`

  No XML summary provided.

- `public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)`

  No XML summary provided.

- `public MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName)`

  No XML summary provided.


## Providers/CodeGenerator

- `public abstract class CodeGenerator : IVisitor, IDisposable`

  Base class for code generators that produce SQL scripts for database schema and data export. This class implements the visitor pattern to traverse the database schema and generate appropriate SQL statements. Derived classes should override the visit methods to provide provider-specific SQL generation logic. The class also manages output writing and supports options for controlling the export process, such as whether to include schema, data, foreign keys, etc. The class implements IDisposable to allow for proper resource management of the output stream, especially when writing to files.

- `public static CodeGenerator Get(string providerName, TextWriter output) =>`

  Factory method to create an instance of a CodeGenerator subclass based on the provided database provider name.

- `public abstract string ProviderName { get; }`

  Gets the name of the database provider for which this code generator is designed to generate SQL scripts.

- `public ExportOptions ExportOptions { get; set; }`

  Gets or sets the export options that control the behavior of the code generation process, such as whether to include schema, data, foreign keys, identities, etc.

- `public TextWriter Output { get; }`

  Gets the TextWriter to which the generated SQL will be written. This property is initialized through the constructor and is used by the code generator to output the generated SQL statements.

- `public void Dispose()`

  No XML summary provided.

- `public virtual void VisitDatabase(Database database)`

  No XML summary provided.

- `public virtual void VisitTable(Table table)`

  No XML summary provided.

- `public virtual void VisitColumn(Column column)`

  No XML summary provided.

- `public virtual void VisitPrimaryKey(PrimaryKey primaryKey)`

  No XML summary provided.

- `public virtual void VisitIndex(Index index)`

  No XML summary provided.

- `public virtual void VisitForeignKey(ForeignKey foreignKey)`

  No XML summary provided.

- `public virtual void VisitDataType(DataType dataType) { }`

  No XML summary provided.


## Providers/Firebird/FirebirdCodeGenerator

- `public class FirebirdCodeGenerator : CodeGenerator`

  Generates SQL code specific to the Firebird database system. This class provides methods to process various database objects and options, translating them into Firebird-compatible SQL scripts.

- `public FirebirdCodeGenerator() { }`

  Initializes a new instance of the FirebirdCodeGenerator class.

- `public FirebirdCodeGenerator(TextWriter output) : base(output) { }`

  Initializes a new instance of the FirebirdCodeGenerator class with the specified TextWriter for output.

- `public FirebirdCodeGenerator(string path) : base(path) { }`

  Initializes a new instance of the FirebirdCodeGenerator class that writes output to a file at the specified path.

- `public FirebirdOptions FirebirdOptions => (FirebirdOptions)ExportOptions?.ProviderSpecific;`

  Represents configuration options specific to Firebird database generation.

- `public override string ProviderName => ProviderNames.FIREBIRD;`

  No XML summary provided.

- `public override void VisitDataType(DataType dataType)`

  No XML summary provided.


## Providers/Firebird/FirebirdOptions

- `public class FirebirdOptions`

  Represents configuration options for Firebird database operations.

- `public string DataDirectory { get; set; }`

  Gets or sets the file system path to the directory where Firebird database files will be created and stored. This property is essential for specifying the location of the database files during database operations.

- `public string DefaultCharSet { get; set; }`

  Gets or sets the default character set to be used for encoding text data.

- `public static int PageSize { get; set; } = 4096;`

  Gets or sets the page size for writing data to disk.

- `public static bool ForcedWrites { get; set; } = true;`

  Gets or sets a value indicating whether to force writes to disk.

- `public static bool Overwrite { get; set; }`

  Gets or sets a value indicating whether to overwrite existing files when exporting data.

- `public static string[] CharacterSets { get; } =`

  Gets a list of supported character sets for Firebird databases.

- `public string ToMarkdown() => $"""`

  Converts the FirebirdOptions properties and their current values into a Markdown table representation.


## Providers/Firebird/FirebirdSchemaProvider

- `public class FirebirdSchemaProvider : ISchemaProvider`

  Provides schema-related operations for Firebird databases, including retrieval of table names, column names, index names, foreign key names, and metadata. Implements the ISchemaProvider interface to support interaction with Firebird database schemas.

- `public FirebirdSchemaProvider(string connectionString)`

  Initializes a new instance of the FirebirdSchemaProvider class.

- `public string ProviderName => ProviderNames.FIREBIRD;`

  No XML summary provided.

- `public string ConnectionString { get; }`

  No XML summary provided.

- `public string DatabaseName { get; }`

  No XML summary provided.

- `public NameOwnerPair[] GetTableNames()`

  No XML summary provided.

- `public string[] GetColumnNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetIndexNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetForeignKeyNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetTableMeta(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetColumnMeta(string tableName, string tableOwner, string columnName)`

  No XML summary provided.

- `public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)`

  No XML summary provided.

- `public MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName)`

  No XML summary provided.

- `public NameOwnerPair[] GetTypeNames()`

  No XML summary provided.

- `public MetaData GetTypeMeta(string typeName, string typeOwner)`

  No XML summary provided.


## Providers/Firebird/FirebirdScriptExecutor

- `public partial class FirebirdScriptExecutor : IScriptExecutor`

  Provides functionality to execute Firebird SQL scripts against a Firebird database.

- `public void Execute(string connectionString, string script)`

  No XML summary provided.


## Providers/ISchemaProvider

- `public interface ISchemaProvider`

  A common interface for schema providers, which are responsible for retrieving database schema information such as table names, column names, index names, foreign key names, and their associated metadata. This interface abstracts the underlying database provider implementation, allowing for flexibility and extensibility in supporting different database systems.


## Providers/IScriptExecutor

- `public interface IScriptExecutor`

  Defines an abstraction for executing SQL scripts against a database connection.

- `public class SimpleScriptExecutor(string providerName) : IScriptExecutor`

  Provides a simple implementation of the IScriptExecutor interface that executes SQL scripts against a database connection using the SqlHelper class.

- `public void Execute(string connectionString, string script)`

  No XML summary provided.

- `public partial class BatchScriptExecutor(string providerName) : IScriptExecutor`

  An implementation of the IScriptExecutor interface that executes SQL scripts using batch processing when supported by the database provider.

- `public virtual void Execute(string connectionString, string script)`

  No XML summary provided.


## Providers/MetaData

- `public class MetaData : Dictionary<string, object>`

  Represents a collection of metadata key-value pairs. This class is used to store additional information about the database schema items such as tables, table columns and/or keys. The keys are case-insensitive, allowing for flexible access to the metadata values.

- `public MetaData() : base(StringComparer.OrdinalIgnoreCase) { }`

  Initializes a new instance of the MetaData class.


## Providers/MySqlClient/MySqlCodeGenerator

- `public class MySqlCodeGenerator : CodeGenerator`

  A code generator responsible for producing SQL scripts for MySQL databases.

- `public MySqlCodeGenerator() { }`

  Initializes a new instance of the MySqlCodeGenerator class.

- `public MySqlCodeGenerator(TextWriter output) : base(output) { }`

  Initializes a new instance of the MySqlCodeGenerator class with the specified TextWriter for output.

- `public MySqlCodeGenerator(string path) : base(path) { }`

  Initializes a new instance of the MySqlCodeGenerator class that writes output to a file at the specified path.

- `public MySqlOptions MySqlOptions => (MySqlOptions)ExportOptions?.ProviderSpecific;`

  Represents configuration options specific to MySQL database generation.

- `public override string ProviderName => ProviderNames.MYSQL;`

  No XML summary provided.


## Providers/MySqlClient/MySqlOptions

- `public sealed class CharacterSet(string name, string[] collations, string defaultCollation)`

  Represents a MySQL character set, including its name, supported collations, and default collation.

- `public string Name { get; } = name;`

  The name of the character set.

- `public string[] Collations { get; } = collations;`

  An array of supported collations for the character set.

- `public string DefaultCollation { get; } = defaultCollation;`

  The default collation for the character set.

- `public override bool Equals(object obj) => obj is CharacterSet other && Name == other.Name;`

  No XML summary provided.

- `public override int GetHashCode() => Name.GetHashCode();`

  No XML summary provided.

- `public override string ToString() => Name;`

  No XML summary provided.

- `public class MySqlOptions`

  Represents options specific to MySQL database generation.

- `public string StorageEngine { get; set; }`

  Gets or sets the storage engine to be used for the database.

- `public CharacterSet CharacterSet { get; set; }`

  Gets or sets the character set to be used for the database.

- `public string Collation { get; set; }`

  Gets or sets the collation to be used for the database.

- `public bool IsMariaDb { get; set; }`

  Gets or sets a value indicating whether to optimize SQL for MariaDB.

- `public static string[] StorageEngines { get; } =`

  Gets a list of supported storage engines for MySQL databases.

- `public static CharacterSet[] CharacterSets { get; } =`

  Gets a list of supported character sets for MySQL databases.

- `public string ToMarkdown() => $"""`

  Converts the current MySqlOptions instance into a Markdown-formatted table.


## Providers/MySqlClient/MySqlSchemaProvider

- `public partial class MySqlSchemaProvider : ISchemaProvider`

  Provides functionality to extract schema information from a MySQL database.

- `public MySqlSchemaProvider(string connectionString)`

  Represents a schema provider for interacting with MySQL databases. Provides methods to retrieve schema-related metadata, such as tables, columns, indices, and foreign keys. Implements the ISchemaProvider interface.

- `public string ProviderName => ProviderNames.MYSQL;`

  No XML summary provided.

- `public string ConnectionString { get; }`

  No XML summary provided.

- `public string DatabaseName { get; }`

  No XML summary provided.

- `public NameOwnerPair[] GetTableNames()`

  No XML summary provided.

- `public string[] GetColumnNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetIndexNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetForeignKeyNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetTableMeta(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetColumnMeta(string tableName, string tableOwner, string columnName)`

  No XML summary provided.

- `public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)`

  No XML summary provided.

- `public MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName)`

  No XML summary provided.

- `public NameOwnerPair[] GetTypeNames()`

  No XML summary provided.

- `public MetaData GetTypeMeta(string typeName, string typeOwner)`

  No XML summary provided.


## Providers/MySqlClient/MySqlScriptExecutor

- `public partial class MySqlScriptExecutor() : BatchScriptExecutor(ProviderNames.MYSQL)`

  Provides functionality for executing MySQL scripts, including management of database creation commands and connection string updates for the target database. Extends the functionality of the BatchScriptExecutor class for MySQL-specific use cases.

- `public override void Execute(string connectionString, string script)`

  No XML summary provided.


## Providers/NameOwnerPair

- `public sealed class NameOwnerPair`

  Represents a pair consisting of a name and an optional owner, commonly used to define database objects such as tables or types along with their associated schema or owner information.

- `public string Name { get; set; }`

  Gets or sets the name of the object.

- `public string Owner { get; set; }`

  Gets or sets the owner of the object, or null if not applicable.

- `public override bool Equals(object obj) =>`

  No XML summary provided.

- `public override int GetHashCode() => HashCode.Combine(Name, Owner);`

  No XML summary provided.

- `public override string ToString() => $"{Name}{(Owner != null ? $" ({Owner})" : "")}";`

  No XML summary provided.


## Providers/Npgsql/NpgsqlCodeGenerator

- `public class NpgsqlCodeGenerator : CodeGenerator`

  Provides functionality for generating database-specific code targeting Npgsql (PostgreSQL). This class extends the CodeGenerator base class and overrides certain methods to tailor code generation to the PostgreSQL database platform.

- `public NpgsqlCodeGenerator() { }`

  Initializes a new instance of the NpgsqlCodeGenerator class.

- `public NpgsqlCodeGenerator(TextWriter output) : base(output) { }`

  Initializes a new instance of the NpgsqlCodeGenerator class with the specified TextWriter for output.

- `public NpgsqlCodeGenerator(string path) : base(path) { }`

  Initializes a new instance of the NpgsqlCodeGenerator class that writes output to a file at the specified path.

- `public override string ProviderName => ProviderNames.POSTGRESQL;`

  No XML summary provided.

- `public override void VisitDataType(DataType dataType)`

  No XML summary provided.


## Providers/Npgsql/NpgsqlSchemaProvider

- `public partial class NpgsqlSchemaProvider : ISchemaProvider`

  Provides schema-related metadata for a Npgsql (PostgreSQL) database, allowing access to table, column, index, foreign key, and type information. This class implements the ISchemaProvider interface and serves as a provider for PostgreSQL database schemas.

- `public NpgsqlSchemaProvider(string connectionString)`

  Initializes a new instance of the NpgsqlSchemaProvider class.

- `public string ProviderName => ProviderNames.POSTGRESQL;`

  No XML summary provided.

- `public string ConnectionString { get; }`

  No XML summary provided.

- `public string DatabaseName { get; }`

  No XML summary provided.

- `public NameOwnerPair[] GetTableNames()`

  No XML summary provided.

- `public string[] GetColumnNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetIndexNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetForeignKeyNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetTableMeta(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetColumnMeta(string tableName, string tableOwner, string columnName)`

  No XML summary provided.

- `public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)`

  No XML summary provided.

- `public MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName)`

  No XML summary provided.

- `public NameOwnerPair[] GetTypeNames()`

  No XML summary provided.

- `public MetaData GetTypeMeta(string typeName, string typeOwner)`

  No XML summary provided.


## Providers/Npgsql/NpgsqlScriptExecutor

- `public partial class NpgsqlScriptExecutor() : BatchScriptExecutor(ProviderNames.POSTGRESQL)`

  Represents a script executor specifically designed for executing Npgsql (PostgreSQL) database scripts. This class extends the BatchScriptExecutor and overrides its behavior to handle PostgreSQL-specific use cases, such as 'CREATE DATABASE' commands and connection string adjustments for the target database.

- `public override void Execute(string connectionString, string script)`

  No XML summary provided.


## Providers/OracleClient/OracleCodeGenerator

- `public class OracleCodeGenerator : CodeGenerator`

  Generates Oracle-specific SQL code for database schema objects and related functionality.

- `public OracleCodeGenerator() { }`

  Initializes a new instance of the OracleCodeGenerator class.

- `public OracleCodeGenerator(TextWriter output) : base(output) { }`

  Initializes a new instance of the OracleCodeGenerator class with the specified TextWriter for output.

- `public OracleCodeGenerator(string path) : base(path) { }`

  Initializes a new instance of the OracleCodeGenerator class that writes output to a file at the specified path.

- `public override string ProviderName => ProviderNames.ORACLE;`

  No XML summary provided.


## Providers/OracleClient/OracleSchemaProvider

- `public class OracleSchemaProvider : ISchemaProvider`

  Provides schema metadata for Oracle databases by implementing the ISchemaProvider interface. This class allows retrieval of database objects such as tables, columns, indexes, and foreign keys, as well as their associated metadata.

- `public OracleSchemaProvider(string connectionString)`

  Initializes a new instance of the OracleSchemaProvider class.

- `public string ProviderName => ProviderNames.ORACLE;`

  No XML summary provided.

- `public string ConnectionString { get; }`

  No XML summary provided.

- `public string DatabaseName { get; }`

  No XML summary provided.

- `public NameOwnerPair[] GetTableNames()`

  No XML summary provided.

- `public string[] GetColumnNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetIndexNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetForeignKeyNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetTableMeta(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetColumnMeta(string tableName, string tableOwner, string columnName)`

  No XML summary provided.

- `public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)`

  No XML summary provided.

- `public MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName)`

  No XML summary provided.


## Providers/ProviderNames

- `public static class ProviderNames`

  A static class that contains constant string values representing the names of supported database providers. These names are typically used to identify the specific database provider when configuring database connections or performing database operations.

- `public const string ACCESS = "System.Data.OleDb";`

  A constant string representing the database provider name for Microsoft Access.

- `public const string SQLSERVER = "Microsoft.Data.SqlClient";`

  A constant string representing the database provider name for Microsoft SQL Server.

- `public const string ORACLE = "Oracle.ManagedDataAccess.Client";`

  A constant string representing the database provider name for Oracle using the Oracle Managed Data Access client.

- `public const string MYSQL = "MySqlConnector";`

  A constant string representing the database provider name for MySQL.

- `public const string POSTGRESQL = "Npgsql";`

  A constant string representing the database provider name for PostgreSQL.

- `public const string FIREBIRD = "FirebirdSql.Data.FirebirdClient";`

  A constant string representing the database provider name for Firebird.

- `public const string SQLITE = "System.Data.SQLite";`

  A constant string representing the database provider name for SQLite.


## Providers/SQLite/SQLiteCodeGenerator

- `public class SQLiteCodeGenerator : CodeGenerator`

  Generates SQLite-specific SQL scripts for database schema and data migrations. This class is designed to provide a SQLite-compatible implementation of the base CodeGenerator functionalities. It facilitates the generation of database schema definitions, constraints, and data migration scripts tailored for SQLite databases.

- `public SQLiteCodeGenerator() { }`

  Initializes a new instance of the SQLiteCodeGenerator class.

- `public SQLiteCodeGenerator(TextWriter output) : base(output) { }`

  Initializes a new instance of the SQLiteCodeGenerator class with the specified TextWriter for output.

- `public SQLiteCodeGenerator(string path) : base(path) { }`

  Initializes a new instance of the SQLiteCodeGenerator class that writes output to a file at the specified path.

- `public override string ProviderName => ProviderNames.SQLITE;`

  No XML summary provided.

- `public override void VisitColumn(Column column)`

  No XML summary provided.

- `public override void VisitPrimaryKey(PrimaryKey primaryKey)`

  No XML summary provided.

- `public override void VisitForeignKey(ForeignKey foreignKey)`

  No XML summary provided.


## Providers/SQLite/SQLiteSchemaProvider

- `public class SQLiteSchemaProvider : ISchemaProvider`

  Provides schema information for SQLite databases. This class implements the ISchemaProvider interface, enabling retrieval of database schema metadata such as table names, column names, foreign key names, and associated metadata.

- `public SQLiteSchemaProvider(string connectionString)`

  Initializes a new instance of the SQLiteSchemaProvider class.

- `public string ProviderName => ProviderNames.SQLITE;`

  No XML summary provided.

- `public string ConnectionString { get; }`

  No XML summary provided.

- `public string DatabaseName { get; }`

  No XML summary provided.

- `public NameOwnerPair[] GetTableNames()`

  No XML summary provided.

- `public string[] GetColumnNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetIndexNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetForeignKeyNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetTableMeta(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetColumnMeta(string tableName, string tableOwner, string columnName)`

  No XML summary provided.

- `public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)`

  No XML summary provided.

- `public MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName)`

  No XML summary provided.


## Providers/SchemaProvider

- `public static class SchemaProvider`

  A factory class that provides methods to create schema providers and retrieve database schemas based on the specified provider name, connection string, and optional schema filter.

- `public static ISchemaProvider GetProvider(string providerName, string connectionString)`

  Gets an instance of ISchemaProvider based on the provided provider name and connection string.

- `public static Database GetDatabase(ISchemaProvider provider, string schema)`

  Extracts the database schema using the provided ISchemaProvider and optional schema filter, and constructs a Database object representing the schema.

- `public static Database GetDatabase(string providerName, string connectionString, string schema) =>`

  Extracts the database schema using the specified provider name, connection string, and optional schema filter, and constructs a Database object representing the schema.


## Providers/SqlClient/SqlCodeGenerator

- `public class SqlCodeGenerator : CodeGenerator`

  Represents a code generator specifically designed for generating SQL Server-compatible scripts. Extends the CodeGenerator class to provide SQL Server-specific implementation details for database schema export and related functionality.

- `public SqlCodeGenerator() { }`

  Initializes a new instance of the SqlCodeGenerator class.

- `public SqlCodeGenerator(TextWriter output) : base(output) { }`

  Initializes a new instance of the SqlCodeGenerator class with the specified TextWriter for output.

- `public SqlCodeGenerator(string path) : base(path) { }`

  Initializes a new instance of the SqlCodeGenerator class that writes output to a file at the specified path.

- `public bool IsFileBased { get; set; }`

  Gets or sets a value indicating whether the SQL Server database is file-based. This is typically the case when using SQL Server Express LocalDB.

- `public override string ProviderName => ProviderNames.SQLSERVER;`

  No XML summary provided.

- `public override void VisitDataType(DataType dataType)`

  No XML summary provided.


## Providers/SqlClient/SqlSchemaProvider

- `public partial class SqlSchemaProvider : ISchemaProvider`

  No XML summary provided.

- `public SqlSchemaProvider(string connectionString)`

  Initializes a new instance of the SqlSchemaProvider class.

- `public string ProviderName => ProviderNames.SQLSERVER;`

  No XML summary provided.

- `public string ConnectionString { get; }`

  No XML summary provided.

- `public string DatabaseName { get; }`

  No XML summary provided.

- `public NameOwnerPair[] GetTableNames()`

  No XML summary provided.

- `public string[] GetColumnNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetIndexNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public string[] GetForeignKeyNames(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetTableMeta(string tableName, string tableOwner)`

  No XML summary provided.

- `public MetaData GetColumnMeta(string tableName, string tableOwner, string columnName)`

  No XML summary provided.

- `public MetaData GetIndexMeta(string tableName, string tableOwner, string indexName)`

  No XML summary provided.

- `public MetaData GetForeignKeyMeta(string tableName, string tableOwner, string fkName)`

  No XML summary provided.

- `public NameOwnerPair[] GetTypeNames()`

  No XML summary provided.

- `public MetaData GetTypeMeta(string typeName, string typeOwner)`

  No XML summary provided.


## Providers/SqlClient/SqlScripExecutor

- `public partial class SqlScripExecutor : IScriptExecutor`

  Represents an implementation of the IScriptExecutor interface for executing SQL scripts against a SQL Server database using a given connection string.

- `public void Execute(string connectionString, string script)`

  No XML summary provided.


## Schema/Column

- `public class Column(`

  Represents a column in a database table with metadata and attributes that provide detailed information about its configuration and behavior within the schema.

- `public ColumnType ColumnType { get; } = type;`

  Gets the type of the column, which is defined by the ColumnType enumeration.

- `public string NativeType { get; } = nativeType;`

  Gets the native type of the column as a string.

- `public short Size { get; } = size;`

  Gets the size (or character length) of the column's data.'

- `public byte Precision { get; } = precision;`

  Gets the precision of the column, primarily used for numeric types.

- `public byte Scale { get; } = scale;`

  Gets the scale (number of decimal places) of numeric types.

- `public ColumnAttributes Attributes { get; private set; } = attributes | GetAttributesFromType(type);`

  Gets the attributes that describe the column's behavior and constraints.'

- `public object DefaultValue { get; } = defaultValue;`

  Gets the default value defined for the column if no value is provided during insertion.

- `public string Description { get; } = description;`

  Gets the description or documentation of the column, often used for metadata purposes.

- `public long IdentitySeed { get; private set; }`

  Gets or sets the seed value for the identity column.

- `public long IdentityIncrement { get; private set; }`

  Gets or sets the increment value for the identity column.

- `public Table Table => (Table)Parent;`

  Gets the table to which this column belongs.

- `public DataType DataType =>`

  Gets the definition of the column's data type if it's user-defined and available in imported schema.

- `public bool IsRequired => Attributes.HasFlag(ColumnAttributes.Required);`

  Gets a value indicating whether the column is nullable.

- `public bool IsComputed => Attributes.HasFlag(ColumnAttributes.Computed);`

  Gets a value indicating whether the column is a computed column.

- `public bool IsIdentity => Attributes.HasFlag(ColumnAttributes.Identity);`

  Gets a value indicating whether the column is an identity column.

- `public bool IsGenerated => IsComputed || IsIdentity || ColumnType == ColumnType.RowVersion;`

  Gets a value indicating whether the column is a row version column.

- `public bool IsPKColumn => Attributes.HasFlag(ColumnAttributes.PKColumn);`

  Gets a value indicating whether the column is a primary key column.

- `public bool IsFKColumn => Attributes.HasFlag(ColumnAttributes.FKColumn);`

  Gets a value indicating whether the column is a foreign key column.

- `public bool IsKeyColumn => IsPKColumn || IsFKColumn;`

  Gets a value indicating whether the column is a key column.

- `public bool IsIndexColumn => Attributes.HasFlag(ColumnAttributes.IXColumn);`

  Gets a value indicating whether the column is an index column.

- `public bool IsNumeric => Attributes.HasFlag(ColumnAttributes.Numeric);`

  Gets a value indicating whether the column is of a numeric type.

- `public bool IsAlphabetic => Attributes.HasFlag(ColumnAttributes.Alphabetic);`

  Gets a value indicating whether the column is of an alphabetic type.

- `public bool IsFixedLength => Attributes.HasFlag(ColumnAttributes.FixedLength);`

  Gets a value indicating whether the column is fixed-length.

- `public bool IsUnsigned => Attributes.HasFlag(ColumnAttributes.Unsigned);`

  Gets a value indicating whether the column is of an unsigned integer type.

- `public bool IsUnicode => Attributes.HasFlag(ColumnAttributes.Unicode);`

  Gets a value indicating whether the column is of a Unicode type.

- `public bool IsIntegral => IsNumeric && IsFixedLength;`

  Gets a value indicating whether the column is of an integral type.

- `public bool IsNatural => IsIntegral && IsUnsigned;`

  Gets a value indicating whether the column is of a natural integer type, i.e., an unsigned integer

- `public bool IsTemporal => Attributes.HasFlag(ColumnAttributes.Temporal);`

  Gets a value indicating whether the column is of a temporal type.

- `public bool IsBinary => Attributes.HasFlag(ColumnAttributes.Binary);`

  Gets a value indicating whether the column is of a binary type.

- `public bool IsChecked { get; set; }`

  No XML summary provided.

- `public void SetAttribute(ColumnAttributes attribute)`

  Sets the attribute of the column.

- `public void MakeIdentity(long seed, long increment)`

  Configures the column as an identity column with a specified seed and increment.

- `public override void AcceptVisitor(IVisitor visitor)`

  No XML summary provided.


## Schema/ColumnAttributes

- `public enum ColumnAttributes`

  Specifies a set of attributes that can be associated with a database column.


## Schema/ColumnCollection

- `public class ColumnCollection : SchemaItemCollection<Column>`

  Represents a collection of database columns.

- `public ColumnCollection() { }`

  Initializes a new instance of the ColumnCollection class.

- `public ColumnCollection(int capacity) : base(capacity) { }`

  Initializes a new instance of the ColumnCollection class with the specified initial capacity.

- `public ColumnCollection(IEnumerable<Column> columns) : base(columns) { }`

  Initializes a new instance of the ColumnCollection class with the specified collection of items.


## Schema/ColumnSet

- `public abstract class ColumnSet(SchemaItem parent, string name) :`

  Represents an abstract base class for a set of related columns within a database schema. Provides functionality to manage the state of column checks and evaluate check conditions.

- `public ColumnCollection Columns { get; } = [];`

  Gets a collection of columns associated with the column set.

- `public bool IsChecked { get; set; }`

  No XML summary provided.

- `public bool AllColumnsAreChecked => Columns.All(column => column.IsChecked);`

  Gets a value indicating whether all columns in the set are checked.

- `public bool NoColumnIsChecked => Columns.All(column => !column.IsChecked);`

  Gets a value indicating whether no column in the set is checked.

- `public bool AnyColumnIsChecked => Columns.Any(column => column.IsChecked);`

  Gets a value indicating whether any column in the set is checked.

- `public bool AnyColumnIsUnchecked => Columns.Any(column => !column.IsChecked);`

  Gets a value indicating whether any column in the set is unchecked.


## Schema/ColumnType

- `public enum ColumnType`

  Represents the types of columns that can be used in a database schema. This enumeration provides a comprehensive list of supported data types including numeric, textual, date/time, binary, and user-defined types.


## Schema/DataType

- `public class DataType(`

  Represents a user-defined database data type with associated metadata such as size, precision, scale, and other characteristics.

- `public string Owner { get; } = owner;`

  The owner of the data type.

- `public ColumnType ColumnType { get; } = type;`

  The column type of the data type.

- `public string NativeType { get; } = nativeType;`

  The native database-specific type definition.

- `public short Size { get; } = size;`

  The size (or character length) of the data type.

- `public byte Precision { get; } = precision;`

  The decimal precision of the data type.

- `public byte Scale { get; } = scale;`

  The decimal scale of the data type.

- `public bool IsNullable { get; } = nullable;`

  Indicates whether the data type is nullable.

- `public bool IsEnumerated { get; } = enumerated;`

  Indicates whether the data type is an enumerated (enum) type.

- `public object DefaultValue { get; } = defaultValue;`

  The default value of the data type, if any.

- `public ImmutableHashSet<object> PossibleValues { get; } = ImmutableHashSet.CreateRange(possibleValues);`

  A collection of possible values for the data type, if it is enumerated.

- `public Database Database => (Database)Parent;`

  The database that owns the data type.

- `public bool IsRequired => !IsNullable;`

  Gets a value indicating whether the data type is required.

- `public bool IsChecked { get; set; }`

  No XML summary provided.

- `public override string FullName => string.IsNullOrEmpty(Owner) ? Name : $"{Owner}.{Name}";`

  No XML summary provided.

- `public override void AcceptVisitor(IVisitor visitor)`

  No XML summary provided.


## Schema/DataTypeCollection

- `public class DataTypeCollection : SchemaItemCollection<DataType>`

  Represents a collection of database data types.

- `public DataTypeCollection() { }`

  Initializes a new instance of the DataTypeCollection class.

- `public DataTypeCollection(int capacity) : base(capacity) { }`

  Initializes a new instance of the DataTypeCollection class with the specified initial capacity.

- `public DataTypeCollection(IEnumerable<DataType> dataTypes) : base(dataTypes) { }`

  Initializes a new instance of the DataTypeCollection class with the specified collection of items.


## Schema/Database

- `public class Database(string name, string providerName, string connectionString) : SchemaItem(null, name)`

  Represents a database within the schema, containing metadata about its provider, connection information, data types, and tables. It serves as the root schema item for database-related operations.

- `public string ProviderName { get; } = providerName;`

  The name of the database provider used to connect to the database.

- `public string ConnectionString { get; } = connectionString;`

  The connection string used to connect to the database.

- `public DataTypeCollection DataTypes { get; } = [];`

  The collection of data types defined in the database.

- `public TableCollection Tables { get; } = [];`

  The collection of tables in the database.

- `public override void AcceptVisitor(IVisitor visitor)`

  No XML summary provided.


## Schema/ForeignKey

- `public class ForeignKey : Key`

  Represents a foreign key constraint in a database schema. This class provides details about the referenced table, the columns involved, and the actions to take on update or delete.

- `public ForeignKey(Table table, string name, IEnumerable<string> columnNames,`

  Initializes a new instance of the ForeignKey class.

- `public string RelatedTableName { get; }`

  Gets the name of the referenced table.

- `public string RelatedTableOwner { get; }`

  Gets the owner of the referenced table.

- `public string RelatedTableFullName => string.IsNullOrEmpty(RelatedTableOwner)`

  Gets the fully qualified name of the referenced table.

- `public string[] RelatedColumnNames { get; }`

  Gets the names of the columns in the referenced table.

- `public ForeignKeyRule UpdateRule { get; }`

  Gets the action to take on update.

- `public ForeignKeyRule DeleteRule { get; }`

  Gets the action to take on delete.

- `public Table RelatedTable =>`

  Gets a reference to the referenced table if loaded in the imported database schema, otherwise null.

- `public Column GetRelatedColumn(int i) => RelatedTable?.Columns[RelatedColumnNames[i]];`

  Retrieves the related column based on the specified index in the foreign key relationship.

- `public override void AcceptVisitor(IVisitor visitor)`

  No XML summary provided.


## Schema/ForeignKeyCollection

- `public class ForeignKeyCollection : SchemaItemCollection<ForeignKey>`

  Represents a collection of database foreign keys.

- `public ForeignKeyCollection() { }`

  Initializes a new instance of the ForeignKeyCollection class.

- `public ForeignKeyCollection(int capacity) : base(capacity) { }`

  Initializes a new instance of the ForeignKeyCollection class with the specified initial capacity.

- `public ForeignKeyCollection(IEnumerable<ForeignKey> foreignKeys) : base(foreignKeys) { }`

  Initializes a new instance of the ForeignKeyCollection class with the specified collection of items.


## Schema/ForeignKeyRule

- `public enum ForeignKeyRule`

  Defines the actions to be taken when a foreign key constraint is violated or when referenced data is modified or deleted.


## Schema/ICheckable

- `public interface ICheckable`

  Represents an item that can be checked or unchecked.


## Schema/IDataItem

- `public interface IDataItem`

  Represents a data item that encapsulates schema-related properties and metadata for a database column or type definition.


## Schema/Index

- `public class Index : Key`

  Represents a database index associated with a table. An index is used to enhance the performance of database queries by providing quick access to rows in a table based on the values of one or more columns.

- `public Index(Table table, string name, IEnumerable<string> columnNames, bool unique, bool primaryKey) :`

  Initializes a new instance of the Index class.

- `public bool IsUnique { get; }`

  Gets a value indicating whether the index is unique.

- `public bool IsPrimaryKey { get; }`

  Gets a value indicating whether the index is a primary key.

- `public bool MatchesPrimaryKey => Table.HasPrimaryKey && MatchesSignature(Table.PrimaryKey);`

  Gets a value indicating whether the index matches a primary key constraint.

- `public bool MatchesForeignKey => Table.HasForeignKey && Table.ForeignKeys.Any(MatchesSignature);`

  Gets a value indicating whether the index matches a foreign key constraint.

- `public bool MatchesKey => MatchesPrimaryKey || MatchesForeignKey;`

  Gets a value indicating whether the index matches a primary or foreign key constraint.

- `public override void AcceptVisitor(IVisitor visitor)`

  No XML summary provided.


## Schema/IndexCollection

- `public class IndexCollection : SchemaItemCollection<Index>`

  Represents a collection of database indexes.

- `public IndexCollection() { }`

  Initializes a new instance of the IndexCollection class.

- `public IndexCollection(int capacity) : base(capacity) { }`

  Initializes a new instance of the IndexCollection class with the specified initial capacity.

- `public IndexCollection(IEnumerable<Index> indexes) : base(indexes) { }`

  Initializes a new instance of the IndexCollection class with the specified collection of items.


## Schema/Key

- `public abstract class Key : ColumnSet`

  Represents an abstract base class for database keys, such as primary keys, foreign keys, and indexes. Provides functionality to manage columns associated with the key and to compare key signatures.

- `public Table Table => (Table)Parent;`

  Gets the table that owns the key.

- `public bool MatchesSignature(Key other) =>`

  Determines whether the current key matches the signature of another key. A signature match occurs when both keys have the same number of columns and the columns have identical names in the same order.


## Schema/PrimaryKey

- `public class PrimaryKey : Key`

  Represents the primary key of a database table. A primary key uniquely identifies each record in the table and enforces entity integrity within the database schema.

- `public PrimaryKey(Table table, string name, IEnumerable<string> columnNames) :`

  Initializes a new instance of the PrimaryKey class.

- `public bool IsComputed => Columns.Count == 1 && Columns[0].IsComputed;`

  Gets a value indicating whether the primary key is computed.

- `public bool IsIdentity => Columns.Count == 1 && Columns[0].IsIdentity;`

  Gets a value indicating whether the primary key is an identity column.

- `public override void AcceptVisitor(IVisitor visitor)`

  No XML summary provided.


## Schema/SchemaItem

- `public abstract class SchemaItem : IVisitorAcceptor`

  Represents a base class for all schema items.

- `public SchemaItem Parent { get; }`

  Gets a reference to the parent schema item.

- `public string Name { get; }`

  Gets the name of the schema item.

- `public virtual string FullName => Name;`

  Gets the fully qualified name of the schema item, which may include the owner or other contextual information, depending on the implementation of the derived class.

- `public abstract void AcceptVisitor(IVisitor visitor);`

  No XML summary provided.

- `public override string ToString() => $"{GetType().Name}[{FullName}]";`

  No XML summary provided.


## Schema/SchemaItemCollection

- `public abstract class SchemaItemCollection<TItem> : List<TItem> where TItem : SchemaItem`

  Represents a collection of strongly typed schema items, providing functionality for indexing, addition, removal, and lookup based on item names. This collection is intended to manage schema items that inherit from the SchemaItem class.

- `public TItem this[string name] => dictionary[name];`

  Provides indexed access to schema items in the collection by their unique names. The indexer performs a lookup in the internal dictionary to retrieve the schema item that matches the given name.

- `public bool TryGetValue(string name, out TItem item) => dictionary.TryGetValue(name, out item);`

  Attempts to retrieve the schema item associated with the specified name from the collection.

- `public new void Add(TItem item)`

  Adds the specified item to the collection.

- `public new void AddRange(IEnumerable<TItem> items)`

  Adds a range of items to the collection.

- `public bool Contains(string name) => dictionary.ContainsKey(name);`

  Determines whether the collection contains an item with the specified name.

- `public int IndexOf(string name) => IndexOf(dictionary[name]);`

  Returns the zero-based index of the schema item with the specified name within the collection.

- `public new bool Remove(TItem item) => base.Remove(item) && dictionary.Remove(item.FullName);`

  Removes the specified schema item from the collection.

- `public bool Remove(string name) => Remove(dictionary[name]);`

  Removes a schema item from the collection by its name.

- `public new void RemoveRange(int index, int count)`

  Removes a range of schema items from the collection starting at the specified index.

- `public new void Clear()`

  Removes all items from the SchemaItemCollection and clears the internal dictionary used for name-based lookups.


## Schema/Table

- `public class Table(Database db, string name, string owner) : ColumnSet(db, name)`

  Represents a database table, which is a specialized ColumnSet containing columns, constraints, and relationships to other tables.

- `public string Owner { get; } = owner;`

  The owner of the table.

- `public PrimaryKey PrimaryKey { get; private set; }`

  The primary key of the table.

- `public IndexCollection Indexes { get; } = [];`

  The indexes of the table.

- `public ForeignKeyCollection ForeignKeys { get; } = [];`

  The foreign keys of the table.

- `public Database Database => (Database)Parent;`

  The database that owns the table.

- `public bool HasPrimaryKey => PrimaryKey?.Columns.Count > 0;`

  Indicates whether the table has a primary key.

- `public bool HasIndex => Indexes.Count > 0 && Indexes.Any(index => index.Columns.Count > 0);`

  Indicates whether the table has an index.

- `public bool HasForeignKey => ForeignKeys.Count > 0 && ForeignKeys.Any(fk => fk.Columns.Count > 0);`

  Indicates whether the table has a foreign key.

- `public ColumnCollection NonPKColumns => [..Columns.Where(column => !column.IsPKColumn)];`

  Gets the columns of the table that are not primary key columns.

- `public ColumnCollection NonFKColumns => [..Columns.Where(column => !column.IsFKColumn)];`

  Gets the columns of the table that are not foreign key columns.

- `public ColumnCollection NonKeyColumns => [..Columns.Where(column => !column.IsKeyColumn)];`

  Gets the columns of the table that are neither primary key nor foreign key columns.

- `public TableCollection ReferencedTables => [..ForeignKeys.Select(fk => fk.RelatedTable)];`

  Gets a collection of tables that are referenced by foreign keys in this table.

- `public TableCollection ReferencingTables =>`

  Gets a collection of tables that are referencing this table through foreign keys.

- `public override string FullName => string.IsNullOrEmpty(Owner) ? Name : $"{Owner}.{Name}";`

  No XML summary provided.

- `public void GeneratePrimaryKey(string name, IEnumerable<string> columnNames)`

  Creates a primary key for the table using the specified name and column names.

- `public ForeignKey GetReferencingKey(Table table) =>`

  Retrieves the foreign key in the current table that references the specified table.

- `public bool IsAssociationTable() =>`

  Determines whether the table is an association table. An association table is identified as having more than one referenced table, and all its columns are either foreign key columns or generated columns.

- `public override void AcceptVisitor(IVisitor visitor)`

  No XML summary provided.


## Schema/TableCollection

- `public class TableCollection : SchemaItemCollection<Table>`

  Represents a collection of database tables.

- `public TableCollection() { }`

  Initializes a new instance of the TableCollection class.

- `public TableCollection(int capacity) : base(capacity) { }`

  Initializes a new instance of the TableCollection class with the specified initial capacity.

- `public TableCollection(IEnumerable<Table> tables) : base(tables) { }`

  Initializes a new instance of the TableCollection class with the specified collection of items.


## Schema/TableExtensions

- `public enum QueryOptions`

  Specifies options for customizing the behavior of query generation in database operations. These options determine how tables and columns are processed and formatted during SQL generation.

- `public static class TableExtensions`

  Contains extension methods for performing various database operations on a Table object. These operations include generating SQL statements (SELECT, INSERT, UPDATE, DELETE), executing queries, and managing batch operations.

- `public static string GenerateSelect(this Table table, QueryOptions options)`

  Generates a SQL SELECT statement for the specified table based on the provided query options.

- `public static string GenerateSelect(this Table table, Key key, QueryOptions options)`

  Generates a SQL SELECT statement for the specified table, filtering results based on the given key and applying the provided query options.

- `public static string GenerateInsert(this Table table, string providerName, QueryOptions options)`

  Generates a SQL INSERT statement for the specified table based on the provided query options.

- `public static string GenerateUpdate(this Table table, string providerName, QueryOptions options)`

  Generates a SQL UPDATE statement for the specified table, including the columns to update and the primary key for filtering.

- `public static string GenerateDelete(this Table table, string providerName, QueryOptions options)`

  Generates a SQL DELETE statement for the specified table using the given provider and query options.

- `public static DbDataReader OpenReader(this Table table, QueryOptions options)`

  Opens a data reader for the specified table using the provided query options.

- `public static void CopyTo(this Table table, DbConnection targetConnection, QueryOptions sourceOptions,`

  Copies data from the specified table to the target database connection using the provided query options.

- `public static TRowSet Select<TRowSet>(this Table table, Func<DbDataReader, TRowSet> extractor)`

  Executes a SQL SELECT query on the specified table and processes the result set using the provided extractor function.

- `public static List<TRow> Select<TRow>(this Table table) where TRow : class, new() =>`

  Returns a list of entities of the specified type, mapped from the rows in the table.

- `public static TRowSet Select<TRowSet, TKey>(this Table table, Key key, TKey keyValue,`

  Executes a SQL SELECT query for the specified table and key and maps the result set to a custom type using the provided key binder and data extraction function.

- `public static List<TRow> Select<TRow>(this Table table, Key key, params object[] keyValues)`

  Retrieves a list of entities of type  from the specified table based on the provided key and key values.

- `public static bool Insert<TRow>(this Table table, TRow rowValue, Action<DbCommand, TRow> rowBinder)`

  Inserts a new row into the specified table using the provided row value and a custom row binder action.

- `public static bool Insert<TRow>(this Table table, TRow rowValue) where TRow : class, new() =>`

  Inserts a row into the specified table using the provided row data and a default row binder.

- `public static bool Insert(this Table table, params object[] rowValues) =>`

  Inserts a new row into the specified table using the provided values.

- `public static bool Update<TRow>(this Table table, TRow rowValue, Action<DbCommand, TRow> rowBinder)`

  Executes an update operation on the specified table using the given row value and a binding action to populate parameter values.

- `public static bool Update<TRow>(this Table table, TRow rowValue) where TRow : class, new() =>`

  Updates the specified table with the provided row data.

- `public static bool Delete<TKey>(this Table table, TKey keyValue, Action<DbCommand, TKey> keyBinder)`

  Deletes a record from the specified table based on the provided key value and binds the key using the given key binder.

- `public static bool Delete<TKey>(this Table table, TKey keyValue) where TKey : class, new() =>`

  Deletes a record from the specified table using the provided key value and binds the key to the using a default key binder.

- `public static bool Delete(this Table table, params object[] keyValues) =>`

  Deletes a row from the specified table using the provided key values to identify the target row.

- `public static bool InsertBatch<TRow>(this Table table, IEnumerable<TRow> rowValues,`

  Inserts a batch of rows into the specified table using the provided binding logic to map rows to database parameters.

- `public static bool InsertBatch<TRow>(this Table table, IEnumerable<TRow> rowValues) where TRow : class, new() =>`

  Inserts a batch of rows into the specified table using the provided row values and a default row binding implementation.

- `public static bool UpdateBatch<TRow>(this Table table, IEnumerable<TRow> rowValues,`

  Updates a batch of rows in the specified table using the provided row values and a function to bind parameter values to the command.

- `public static bool UpdateBatch<TRow>(this Table table, IEnumerable<TRow> rowValues) where TRow : class, new() =>`

  Updates a batch of rows in the specified table using the provided collection of row values and a default entity-to-command binder.

- `public static bool DeleteBatch<TKey>(this Table table, IEnumerable<TKey> keyValues,`

  Executes a batch delete operation for the specified table based on a collection of key values.

- `public static bool DeleteBatch<TKey>(this Table table, IEnumerable<TKey> keyValues) where TKey : class, new() =>`

  Deletes a batch of records from the specified table based on the provided key values.


## SqlHelper

- `public sealed class SqlHelper : IDisposable`

  A helper class for executing SQL queries and scripts against a database. It provides methods for querying data, executing non-query commands, and executing SQL scripts with support for different database providers. The class implements IDisposable to ensure proper disposal of database connections when necessary.

- `public SqlHelper(DbConnection connection)`

  Initializes a new instance of the SqlHelper class with the specified database connection.

- `public SqlHelper(string providerName, string connectionString) :`

  Initializes a new instance of the SqlHelper class with the specified provider name and connection string.

- `public SqlHelper(Database database) : this(Utility.GetConnection(database)) { }`

  Initializes a new instance of the SqlHelper class with the specified Database object.

- `public string ProviderName { get; }`

  Gets the name of the database provider being used by the SqlHelper instance.

- `public void Dispose()`

  No XML summary provided.

- `public TResult Query<TResult>(string sql, Func<DbDataReader, TResult> extractor)`

  Executes the specified SQL query and uses the provided extractor function to process the results from the data reader.

- `public TResult Query<TSource, TResult>(`

  Executes a SQL query with the specified parameters, binds them to the command, and extracts the result using the provided extractor function.

- `public object QueryScalar(string sql)`

  Executes the specified SQL query and returns the value of the first column of the first row in the result set.

- `public object QueryScalar<TSource>(string sql, TSource paramSource, Action<DbCommand, TSource> binder)`

  Executes the specified SQL query with parameters, using a custom binder to bind the parameters, and returns a single scalar value resulting from the query.

- `public int Execute(string sql)`

  Executes the specified SQL command and returns the number of rows affected by the command.

- `public int Execute<TSource>(string sql, TSource paramSource, Action<DbCommand, TSource> binder)`

  Executes a parameterized SQL command using the specified parameter source and binder.

- `public int ExecuteBatch<TSource>(string sql, IEnumerable<TSource> paramSources, Action<DbCommand, TSource> binder)`

  Executes a batch of SQL commands using a provided SQL statement and a collection of parameters.

- `public int ExecuteBatch(string sql, DbDataReader dataReader)`

  Executes a batch of SQL statements using the provided SQL command template and a data reader as the source for parameters.

- `public static void ExecuteScript(string providerName, string connectionString, string script)`

  Executes the specified SQL script against the database using the provided provider name and connection string.

- `public static IScriptExecutor GetScripExecutor(string providerName) =>`

  Gets the script executor that matches the given provider name.

- `public static object[] ToArray(DbDataReader dataReader)`

  Extracts the values of the columns from the first row of the data reader and returns them as an array of objects.

- `public static Dictionary<string, object> ToDictionary(DbDataReader dataReader)`

  Extracts the values of the columns from the first row of the data reader and returns them as a dictionary, where the keys are the column names and the values are the corresponding column values.

- `public static TEntity ToEntity<TEntity>(DbDataReader dataReader) where TEntity : class, new()`

  Converts the current row of the specified DbDataReader into an instance of the specified entity type.

- `public static List<object[]> ToArrayList(DbDataReader dataReader)`

  Extracts the values of the columns from all rows of the data reader and returns them as a list of object arrays, where each object array represents a row of data, and the elements of the array correspond to the column values in that row.

- `public static List<Dictionary<string, object>> ToDictionaryList(DbDataReader dataReader)`

  Extracts the values of the columns from all rows of the data reader and returns them as a list of dictionaries, where each dictionary represents a row of data, and the keys of the dictionary are the column names, and the values are the corresponding column values for that row.

- `public static List<TEntity> ToEntityList<TEntity>(DbDataReader dataReader) where TEntity : class, new()`

  Converts the data retrieved by a DbDataReader into a list of entities of the specified type.

- `public static List<object> ToList(DbDataReader dataReader)`

  Extracts the values of the first column from all rows of the data reader and returns them as a list of objects, where each object in the list corresponds to the value of the first column for a row of data from the data reader.

- `public static void FromArray(DbCommand command, object[] values)`

  Populates the parameters of a database command with the specified array of values.

- `public static void FromDictionary(DbCommand command, Dictionary<string, object> values)`

  Populates the parameters of a database command with values from the provided dictionary.

- `public static void FromEntity<TEntity>(DbCommand command, TEntity entity) where TEntity : class`

  Sets the parameter values of the specified DbCommand using the property values of the provided entity.

- `public static IReadOnlyList<string> Extract(string sql)`

  Extracts parameter names from the specified SQL string by identifying and parsing parameter placeholders. Handles SQL syntax rules such as skipping string literals, quoted identifiers, and comments.


## Utility

- `public static partial class Utility`

  A collection of helper methods and properties for database operations, string manipulation, and type conversion within the application.

- `public static Encoding Encoding { get; set; } = Encoding.UTF8;`

  The default encoding used for reading and writing data within the application. This can be set to a different encoding if needed, but defaults to UTF-8 for broad compatibility with various data sources and formats.

- `public static void RegisterDbProviderFactories()`

  Registers database provider factories for supported database types, enabling ADO.NET support for those providers within the application.

- `public static DbConnection GetConnection(string providerName, string connectionString)`

  Creates and returns a database connection using the specified provider name and connection string.

- `public static DbConnection GetConnection(Database database) =>`

  Creates and returns a database connection using the specified database configuration.

- `public static Dictionary<string, string> ParseConnectionString(string connectionString)`

  Parses a connection string into a dictionary of key-value pairs.

- `public static string TransformConnectionString(string connectionString, Func<string, string, string> transformer)`

  Transforms the specified connection string by applying a provided transformation function to each property value.

- `public static string SanitizeConnectionString(string connectionString) =>`

  Returns a sanitized version of the specified connection string with sensitive information, such as passwords, masked to prevent exposure.

- `public static string Escape(string name, string providerName) =>`

  Escapes the specified identifier name according to the syntax conventions of the given database provider.

- `public static string ToParameterName(string columnName, string providerName) =>`

  Converts a column name into a parameter name formatted according to the conventions of the specified database provider.

- `public static Dictionary<string, TValue> EmptyDictionary<TValue>() => new(StringComparer.OrdinalIgnoreCase);`

  Creates and returns an empty dictionary with string keys that uses a case-insensitive string comparer.

- `public static string[] Split(string input, char separator) =>`

  Splits the specified string into an array of substrings based on the provided separator character, removing empty entries and trimming whitespace from each substring.

- `public static bool IsEmpty(object value) =>`

  Determines whether the specified object is considered empty.

- `public static bool IsBoolean(object value) =>`

  Checks if the specified object can be interpreted as a boolean value.

- `public static bool IsNumeric(object value, out decimal converted) =>`

  Determines whether the specified object can be interpreted as a numeric value.

- `public static bool IsDate(object value, out DateTime converted) =>`

  Determines whether the specified object can be interpreted as a date/time value.

- `public static byte ToByte(object value) =>`

  Converts the specified object to its equivalent byte value, if possible.

- `public static short ToInt16(object value) =>`

  Converts the specified object to its equivalent 16-bit signed integer using the current culture's formatting conventions.

- `public static string QuotedStr(object value, char quote = '\'') =>`

  Formats the specified value as a string enclosed in the specified quote character, escaping any existing quotes within the value.

- `public static string UnquotedStr(object value, char quote = '\'') =>`

  Removes surrounding quote characters from the specified string representation and replaces any consecutive quote characters with a single instance.

- `public static string BinToHex(byte[] bytes) =>`

  Converts a byte array to its hexadecimal string representation.

- `public static string ToBitString(byte[] bytes) =>`

  Converts an array of bytes to a single binary string representation, where each byte is represented as an 8-character binary value.

- `public static byte[] FromBitString(string value)`

  Converts a binary string representation into an array of bytes.

- `public static string ToBaseN(byte b, byte n)`

  Converts the specified byte value to its string representation in the given base.

- `public static byte FromBaseN(string value, byte n) =>`

  Converts a string representation of a number in a specified base back to its byte value.


