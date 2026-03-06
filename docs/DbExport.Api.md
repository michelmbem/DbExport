# DbExport.Api Documentation

Auto-generated from XML documentation comments in the DbExport.Api module.

## Index
- [Namespace `DbExport`](#namespace-dbexport)
- [`ExportFlags`](#type-dbexport-exportflags)
- [`ExportOptions`](#type-dbexport-exportoptions)
- [`IVisitor`](#type-dbexport-ivisitor)
- [`IVisitorAcceptor`](#type-dbexport-ivisitoracceptor)
- [`SqlHelper`](#type-dbexport-sqlhelper)
- [`Utility`](#type-dbexport-utility)
- [Namespace `DbExport.Providers`](#namespace-dbexport-providers)
- [`CodeGenerator`](#type-dbexport-providers-codegenerator)
- [`ISchemaProvider`](#type-dbexport-providers-ischemaprovider)
- [`IScriptExecutor`](#type-dbexport-providers-iscriptexecutor)
- [`SimpleScriptExecutor`](#type-dbexport-providers-simplescriptexecutor)
- [`BatchScriptExecutor`](#type-dbexport-providers-batchscriptexecutor)
- [`MetaData`](#type-dbexport-providers-metadata)
- [`NameOwnerPair`](#type-dbexport-providers-nameownerpair)
- [`ProviderNames`](#type-dbexport-providers-providernames)
- [`SchemaProvider`](#type-dbexport-providers-schemaprovider)
- [Namespace `DbExport.Providers.Firebird`](#namespace-dbexport-providers-firebird)
- [`FirebirdCodeGenerator`](#type-dbexport-providers-firebird-firebirdcodegenerator)
- [`FirebirdOptions`](#type-dbexport-providers-firebird-firebirdoptions)
- [`FirebirdSchemaProvider`](#type-dbexport-providers-firebird-firebirdschemaprovider)
- [`FirebirdScriptExecutor`](#type-dbexport-providers-firebird-firebirdscriptexecutor)
- [Namespace `DbExport.Providers.MySqlClient`](#namespace-dbexport-providers-mysqlclient)
- [`MySqlCodeGenerator`](#type-dbexport-providers-mysqlclient-mysqlcodegenerator)
- [`CharacterSet`](#type-dbexport-providers-mysqlclient-characterset)
- [`MySqlOptions`](#type-dbexport-providers-mysqlclient-mysqloptions)
- [`MySqlSchemaProvider`](#type-dbexport-providers-mysqlclient-mysqlschemaprovider)
- [`MySqlScriptExecutor`](#type-dbexport-providers-mysqlclient-mysqlscriptexecutor)
- [Namespace `DbExport.Providers.Npgsql`](#namespace-dbexport-providers-npgsql)
- [`NpgsqlCodeGenerator`](#type-dbexport-providers-npgsql-npgsqlcodegenerator)
- [`NpgsqlSchemaProvider`](#type-dbexport-providers-npgsql-npgsqlschemaprovider)
- [`NpgsqlScriptExecutor`](#type-dbexport-providers-npgsql-npgsqlscriptexecutor)
- [Namespace `DbExport.Providers.OracleClient`](#namespace-dbexport-providers-oracleclient)
- [`OracleCodeGenerator`](#type-dbexport-providers-oracleclient-oraclecodegenerator)
- [`OracleSchemaProvider`](#type-dbexport-providers-oracleclient-oracleschemaprovider)
- [Namespace `DbExport.Providers.SqlClient`](#namespace-dbexport-providers-sqlclient)
- [`SqlCodeGenerator`](#type-dbexport-providers-sqlclient-sqlcodegenerator)
- [`SqlSchemaProvider`](#type-dbexport-providers-sqlclient-sqlschemaprovider)
- [`SqlScripExecutor`](#type-dbexport-providers-sqlclient-sqlscripexecutor)
- [Namespace `DbExport.Providers.SQLite`](#namespace-dbexport-providers-sqlite)
- [`SQLiteCodeGenerator`](#type-dbexport-providers-sqlite-sqlitecodegenerator)
- [`SQLiteSchemaProvider`](#type-dbexport-providers-sqlite-sqliteschemaprovider)
- [Namespace `DbExport.Schema`](#namespace-dbexport-schema)
- [`Column`](#type-dbexport-schema-column)
- [`ColumnAttributes`](#type-dbexport-schema-columnattributes)
- [`ColumnCollection`](#type-dbexport-schema-columncollection)
- [`ColumnSet`](#type-dbexport-schema-columnset)
- [`ColumnType`](#type-dbexport-schema-columntype)
- [`Database`](#type-dbexport-schema-database)
- [`DataType`](#type-dbexport-schema-datatype)
- [`DataTypeCollection`](#type-dbexport-schema-datatypecollection)
- [`ForeignKey`](#type-dbexport-schema-foreignkey)
- [`ForeignKeyCollection`](#type-dbexport-schema-foreignkeycollection)
- [`ForeignKeyRule`](#type-dbexport-schema-foreignkeyrule)
- [`ICheckable`](#type-dbexport-schema-icheckable)
- [`IDataItem`](#type-dbexport-schema-idataitem)
- [`Index`](#type-dbexport-schema-index)
- [`IndexCollection`](#type-dbexport-schema-indexcollection)
- [`Key`](#type-dbexport-schema-key)
- [`PrimaryKey`](#type-dbexport-schema-primarykey)
- [`SchemaItem`](#type-dbexport-schema-schemaitem)
- [`SchemaItemCollection`1`](#type-dbexport-schema-schemaitemcollection-1)
- [`Table`](#type-dbexport-schema-table)
- [`TableCollection`](#type-dbexport-schema-tablecollection)
- [`QueryOptions`](#type-dbexport-schema-queryoptions)
- [`TableExtensions`](#type-dbexport-schema-tableextensions)
- [Namespace `DbExport.SqlHelper`](#namespace-dbexport-sqlhelper)
- [`ParameterParser`](#type-dbexport-sqlhelper-parameterparser)
- [Namespace `System.Text.RegularExpressions.Generated`](#namespace-system-text-regularexpressions-generated)
- [`CreateDbRegex_0`](#type-system-text-regularexpressions-generated-createdbregex-0)
- [`DelimiterRegex_1`](#type-system-text-regularexpressions-generated-delimiterregex-1)
- [`UserTypeRegex_2`](#type-system-text-regularexpressions-generated-usertyperegex-2)
- [`Utf8Regex_3`](#type-system-text-regularexpressions-generated-utf8regex-3)
- [`CreateDbRegex_4`](#type-system-text-regularexpressions-generated-createdbregex-4)
- [`CommaRegex_5`](#type-system-text-regularexpressions-generated-commaregex-5)
- [`CreateDbRegex_6`](#type-system-text-regularexpressions-generated-createdbregex-6)
- [`UniqueRegex_7`](#type-system-text-regularexpressions-generated-uniqueregex-7)
- [`PrimaryKeyRegex_8`](#type-system-text-regularexpressions-generated-primarykeyregex-8)
- [`DelimiterRegex_9`](#type-system-text-regularexpressions-generated-delimiterregex-9)
- [`CreateDbRegex_10`](#type-system-text-regularexpressions-generated-createdbregex-10)
- [`CreateTypeRegex_11`](#type-system-text-regularexpressions-generated-createtyperegex-11)
- [`PasswordRegex_12`](#type-system-text-regularexpressions-generated-passwordregex-12)
- [`Utilities`](#type-system-text-regularexpressions-generated-utilities)
- [Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_0`](#namespace-system-text-regularexpressions-generated-createdbregex-0)
- [`RunnerFactory`](#type-system-text-regularexpressions-generated-createdbregex-0-runnerfactory)
- [Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_0.RunnerFactory`](#namespace-system-text-regularexpressions-generated-createdbregex-0-runnerfactory)
- [`Runner`](#type-system-text-regularexpressions-generated-createdbregex-0-runnerfactory-runner)
- [Namespace `System.Text.RegularExpressions.Generated.DelimiterRegex_1`](#namespace-system-text-regularexpressions-generated-delimiterregex-1)
- [`RunnerFactory`](#type-system-text-regularexpressions-generated-delimiterregex-1-runnerfactory)
- [Namespace `System.Text.RegularExpressions.Generated.DelimiterRegex_1.RunnerFactory`](#namespace-system-text-regularexpressions-generated-delimiterregex-1-runnerfactory)
- [`Runner`](#type-system-text-regularexpressions-generated-delimiterregex-1-runnerfactory-runner)
- [Namespace `System.Text.RegularExpressions.Generated.UserTypeRegex_2`](#namespace-system-text-regularexpressions-generated-usertyperegex-2)
- [`RunnerFactory`](#type-system-text-regularexpressions-generated-usertyperegex-2-runnerfactory)
- [Namespace `System.Text.RegularExpressions.Generated.UserTypeRegex_2.RunnerFactory`](#namespace-system-text-regularexpressions-generated-usertyperegex-2-runnerfactory)
- [`Runner`](#type-system-text-regularexpressions-generated-usertyperegex-2-runnerfactory-runner)
- [Namespace `System.Text.RegularExpressions.Generated.Utf8Regex_3`](#namespace-system-text-regularexpressions-generated-utf8regex-3)
- [`RunnerFactory`](#type-system-text-regularexpressions-generated-utf8regex-3-runnerfactory)
- [Namespace `System.Text.RegularExpressions.Generated.Utf8Regex_3.RunnerFactory`](#namespace-system-text-regularexpressions-generated-utf8regex-3-runnerfactory)
- [`Runner`](#type-system-text-regularexpressions-generated-utf8regex-3-runnerfactory-runner)
- [Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_4`](#namespace-system-text-regularexpressions-generated-createdbregex-4)
- [`RunnerFactory`](#type-system-text-regularexpressions-generated-createdbregex-4-runnerfactory)
- [Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_4.RunnerFactory`](#namespace-system-text-regularexpressions-generated-createdbregex-4-runnerfactory)
- [`Runner`](#type-system-text-regularexpressions-generated-createdbregex-4-runnerfactory-runner)
- [Namespace `System.Text.RegularExpressions.Generated.CommaRegex_5`](#namespace-system-text-regularexpressions-generated-commaregex-5)
- [`RunnerFactory`](#type-system-text-regularexpressions-generated-commaregex-5-runnerfactory)
- [Namespace `System.Text.RegularExpressions.Generated.CommaRegex_5.RunnerFactory`](#namespace-system-text-regularexpressions-generated-commaregex-5-runnerfactory)
- [`Runner`](#type-system-text-regularexpressions-generated-commaregex-5-runnerfactory-runner)
- [Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_6`](#namespace-system-text-regularexpressions-generated-createdbregex-6)
- [`RunnerFactory`](#type-system-text-regularexpressions-generated-createdbregex-6-runnerfactory)
- [Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_6.RunnerFactory`](#namespace-system-text-regularexpressions-generated-createdbregex-6-runnerfactory)
- [`Runner`](#type-system-text-regularexpressions-generated-createdbregex-6-runnerfactory-runner)
- [Namespace `System.Text.RegularExpressions.Generated.UniqueRegex_7`](#namespace-system-text-regularexpressions-generated-uniqueregex-7)
- [`RunnerFactory`](#type-system-text-regularexpressions-generated-uniqueregex-7-runnerfactory)
- [Namespace `System.Text.RegularExpressions.Generated.UniqueRegex_7.RunnerFactory`](#namespace-system-text-regularexpressions-generated-uniqueregex-7-runnerfactory)
- [`Runner`](#type-system-text-regularexpressions-generated-uniqueregex-7-runnerfactory-runner)
- [Namespace `System.Text.RegularExpressions.Generated.PrimaryKeyRegex_8`](#namespace-system-text-regularexpressions-generated-primarykeyregex-8)
- [`RunnerFactory`](#type-system-text-regularexpressions-generated-primarykeyregex-8-runnerfactory)
- [Namespace `System.Text.RegularExpressions.Generated.PrimaryKeyRegex_8.RunnerFactory`](#namespace-system-text-regularexpressions-generated-primarykeyregex-8-runnerfactory)
- [`Runner`](#type-system-text-regularexpressions-generated-primarykeyregex-8-runnerfactory-runner)
- [Namespace `System.Text.RegularExpressions.Generated.DelimiterRegex_9`](#namespace-system-text-regularexpressions-generated-delimiterregex-9)
- [`RunnerFactory`](#type-system-text-regularexpressions-generated-delimiterregex-9-runnerfactory)
- [Namespace `System.Text.RegularExpressions.Generated.DelimiterRegex_9.RunnerFactory`](#namespace-system-text-regularexpressions-generated-delimiterregex-9-runnerfactory)
- [`Runner`](#type-system-text-regularexpressions-generated-delimiterregex-9-runnerfactory-runner)
- [Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_10`](#namespace-system-text-regularexpressions-generated-createdbregex-10)
- [`RunnerFactory`](#type-system-text-regularexpressions-generated-createdbregex-10-runnerfactory)
- [Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_10.RunnerFactory`](#namespace-system-text-regularexpressions-generated-createdbregex-10-runnerfactory)
- [`Runner`](#type-system-text-regularexpressions-generated-createdbregex-10-runnerfactory-runner)
- [Namespace `System.Text.RegularExpressions.Generated.CreateTypeRegex_11`](#namespace-system-text-regularexpressions-generated-createtyperegex-11)
- [`RunnerFactory`](#type-system-text-regularexpressions-generated-createtyperegex-11-runnerfactory)
- [Namespace `System.Text.RegularExpressions.Generated.CreateTypeRegex_11.RunnerFactory`](#namespace-system-text-regularexpressions-generated-createtyperegex-11-runnerfactory)
- [`Runner`](#type-system-text-regularexpressions-generated-createtyperegex-11-runnerfactory-runner)
- [Namespace `System.Text.RegularExpressions.Generated.PasswordRegex_12`](#namespace-system-text-regularexpressions-generated-passwordregex-12)
- [`RunnerFactory`](#type-system-text-regularexpressions-generated-passwordregex-12-runnerfactory)
- [Namespace `System.Text.RegularExpressions.Generated.PasswordRegex_12.RunnerFactory`](#namespace-system-text-regularexpressions-generated-passwordregex-12-runnerfactory)
- [`Runner`](#type-system-text-regularexpressions-generated-passwordregex-12-runnerfactory-runner)

## Namespace `DbExport`

### Type `ExportFlags` (type)

Flags to specify what aspects of the database to export. These can be combined using bitwise operations.

#### Fields

- `ExportNothing`

  Represents no export operation. When this flag is set, no database schema or data will be exported. Typically used for scenarios where export functionality is disabled or not required.

- `ExportPrimaryKeys`

  Represents the export operation for primary key definitions. When this flag is set, the primary keys of database tables will be included in the export output. Commonly used to ensure that table structures retain their unique key constraints.

- `ExportForeignKeys`

  Specifies that foreign key constraints should be included during the export operation. When this flag is set, all relationships defined by foreign keys in the database schema will be exported. This is useful for preserving data integrity and enforcing referential rules in the exported schema.

- `ExportIndexes`

  Represents an operation to export database indexes. When this flag is set, all defined indexes in the database schema will be included in the export process. This is typically used to preserve and recreate index structures in the target environment.

- `ExportDefaults`

  Specifies that default values for database objects, such as columns or data types, should be included in the export operation. When this flag is enabled, default constraints or definitions are generated as part of the database schema.

- `ExportIdentities`

  Represents the export of identity columns from the database schema. When this flag is set, identity columns (columns with auto-increment or similar behavior) are included during the export process. Useful for scenarios where retaining identity column definitions is necessary for database migration or replication.

### Type `ExportOptions` (type)

Options for exporting a database, including what to export and any provider-specific settings.

#### Properties

- `ExportSchema`

  Indicates whether the schema of the database should be exported as part of the export process. When set to true, the structural definitions of tables, views, and other schema components will be included in the export.

- `ExportData`

  Determines whether the data within the tables of the database should be exported as part of the export process. When set to true, the contents of the tables, such as rows of data, will be included in the export.

- `Flags`

  Specifies the flags that determine which components of a database should be exported during the export process. This property allows combining multiple values from the DbExport.ExportFlags enumeration to customize the export behavior, such as including or excluding primary keys, foreign keys, indexes, defaults, or identity columns.

- `ProviderSpecific`

  Allows specifying provider-specific settings to be used during the export process. This property can hold configuration options or parameters unique to a particular database provider, enabling customization of the export behavior for that provider.

#### Methods

- `SetFlag(DbExport.ExportFlags,System.Boolean)`

  Sets or clears a specific flag in the DbExport.ExportFlags enumeration.

  Parameters:
  - `flag`: The flag to set or clear.
  - `value`: A boolean value indicating whether to set or clear the specified flag. If true, the flag will be set; if false, the flag will be cleared.

- `HasFlag(DbExport.ExportFlags)`

  Checks if a specific flag in the DbExport.ExportFlags enumeration is set.

  Parameters:
  - `flag`: The flag to check for in the current DbExport.ExportFlags value.

  Returns: A boolean value indicating whether the specified flag is set. Returns true if the flag is set; otherwise, false.

### Type `IVisitor` (type)

Defines the Visitor pattern for traversing the database schema. Each method corresponds to a specific schema element, allowing for operations to be performed on databases, tables, columns, keys, and indexes without modifying their classes. This design promotes separation of concerns and makes it easier to add new operations on the schema elements without changing their structure.

#### Methods

- `VisitDatabase(DbExport.Schema.Database)`

  Visits a Database object, allowing the visitor to perform operations on the database schema. This method is the entry point for traversing the database structure.

  Parameters:
  - `database`: The Database object to be visited.

- `VisitTable(DbExport.Schema.Table)`

  Visits a Table object, allowing the visitor to perform operations on the table schema.

  Parameters:
  - `table`: The Table object to be visited.

- `VisitColumn(DbExport.Schema.Column)`

  Visits a Column object, allowing the visitor to perform operations on the column schema.

  Parameters:
  - `column`: The Column object to be visited.

- `VisitPrimaryKey(DbExport.Schema.PrimaryKey)`

  Visits a PrimaryKey object, allowing the visitor to perform operations on the primary key schema.

  Parameters:
  - `primaryKey`: The PrimaryKey object to be visited.

- `VisitIndex(DbExport.Schema.Index)`

  Visits an Index object, allowing the visitor to perform operations on the index schema.

  Parameters:
  - `index`: The Index object to be visited.

- `VisitForeignKey(DbExport.Schema.ForeignKey)`

  Visits a ForeignKey object, allowing the visitor to perform operations on the foreign key schema.

  Parameters:
  - `foreignKey`: The ForeignKey object to be visited.

- `VisitDataType(DbExport.Schema.DataType)`

  Visits a DataType object, allowing the visitor to perform operations on the data type schema.

  Parameters:
  - `dataType`: The DataType object to be visited.

### Type `IVisitorAcceptor` (type)

Defines an interface for accepting visitors, allowing external operations to be performed on implementing classes without modifying their structure. This is a key component of the Visitor design pattern, enabling separation of concerns and enhancing flexibility in extending functionality.

#### Methods

- `AcceptVisitor(DbExport.IVisitor)`

  Accepts a visitor, allowing it to perform operations on the implementing class. The visitor will typically have specific methods for handling different types of elements in the object structure.

  Parameters:
  - `visitor`: The visitor instance that will perform operations on the implementing class.

### Type `SqlHelper` (type)

A helper class for executing SQL queries and scripts against a database. It provides methods for querying data, executing non-query commands, and executing SQL scripts with support for different database providers. The class implements IDisposable to ensure proper disposal of database connections when necessary.

#### Fields

- `PROPERTY_FLAGS`

  Defines a set of binding flags used for property access within the SqlHelper class. Includes flags for public, instance-level, and case-insensitive member access.

- `GET_PROPERTY_FLAGS`

  Represents a combination of binding flags used to access and modify properties within the SqlHelper class. Includes public, instance-level, case-insensitive member access, and property-setting capabilities.

- `SET_PROPERTY_FLAGS`

  Defines a combination of binding flags used to set property values on objects, including public, instance-level, and case-insensitive member access. Extends the base property flags by adding support for property value setting.

- `connection`

  Represents the database connection used by the SqlHelper instance to communicate with the database. This connection is essential for executing queries, commands, and scripts. The connection must be properly initialized and associated with a compatible database provider.

- `disposeConnection`

  Indicates whether the SqlHelper instance is responsible for disposing of the database connection. When set to true, the connection will be disposed upon disposing the SqlHelper instance.

#### Properties

- `ProviderName`

  Gets the name of the database provider being used by the SqlHelper instance.

#### Constructors

- `SqlHelper(System.Data.Common.DbConnection)`

  Initializes a new instance of the SqlHelper class with the specified database connection.

  Parameters:
  - `connection`: The database connection to be used by the SqlHelper instance.

- `SqlHelper(System.String,System.String)`

  Initializes a new instance of the SqlHelper class with the specified provider name and connection string.

  Parameters:
  - `providerName`: The name of the database provider.
  - `connectionString`: The connection string for the database connection.

- `SqlHelper(DbExport.Schema.Database)`

  Initializes a new instance of the SqlHelper class with the specified Database object.

  Parameters:
  - `database`: The Database object containing the provider name and connection string for the database connection.

#### Methods

- `Dispose`

  No XML summary provided.

- `Dispose(System.Boolean)`

  Releases the unmanaged resources used by the SqlHelper instance and optionally releases the managed resources.

  Parameters:
  - `disposing`: No XML description provided.

- `Query``1(System.String,System.Func{System.Data.Common.DbDataReader,``0})`

  Executes the specified SQL query and uses the provided extractor function to process the results from the data reader.

  Type Parameters:
  - `TResult`: The type of the result returned by the extractor function.

  Parameters:
  - `sql`: The SQL query to be executed.
  - `extractor`: The function that processes the data reader and returns a result of type TResult.

  Returns: A result of type TResult obtained by processing the data reader with the extractor function.

- `Query``2(System.String,``0,System.Action{System.Data.Common.DbCommand,``0},System.Func{System.Data.Common.DbDataReader,``1})`

  Executes a SQL query with the specified parameters, binds them to the command, and extracts the result using the provided extractor function.

  Type Parameters:
  - `TSource`: The type of the parameter source object.
  - `TResult`: The type of the result to be returned.

  Parameters:
  - `sql`: The SQL query to be executed.
  - `paramSource`: The source object containing parameters to be bound to the query.
  - `binder`: An action that binds parameters from the source object to the database command.
  - `extractor`: A function that processes the data reader and extracts the desired result.

  Returns: The result extracted from the data reader based on the extractor function.

- `QueryScalar(System.String)`

  Executes the specified SQL query and returns the value of the first column of the first row in the result set.

  Parameters:
  - `sql`: The SQL query to be executed.

  Returns: The value of the first column of the first row in the result set, or null if the result set is empty.

- `QueryScalar``1(System.String,``0,System.Action{System.Data.Common.DbCommand,``0})`

  Executes the specified SQL query with parameters, using a custom binder to bind the parameters, and returns a single scalar value resulting from the query.

  Type Parameters:
  - `TSource`: The type of the parameter source.

  Parameters:
  - `sql`: The SQL query to be executed.
  - `paramSource`: The object containing the parameter values to be used in the SQL query.
  - `binder`: An action that binds the parameter values to the SQL command.

  Returns: The scalar value returned from the execution of the SQL query.

- `Execute(System.String)`

  Executes the specified SQL command and returns the number of rows affected by the command.

  Parameters:
  - `sql`: The SQL command to be executed.

  Returns: The number of rows affected by the command.

- `Execute``1(System.String,``0,System.Action{System.Data.Common.DbCommand,``0})`

  Executes a parameterized SQL command using the specified parameter source and binder.

  Type Parameters:
  - `TSource`: The type of the parameter source object.

  Parameters:
  - `sql`: The SQL command to execute.
  - `paramSource`: The source object providing parameter values for the command.
  - `binder`: The action to bind the parameter values from the source object to the command.

  Returns: The number of rows affected by the command execution.

- `ExecuteBatch``1(System.String,System.Collections.Generic.IEnumerable{``0},System.Action{System.Data.Common.DbCommand,``0})`

  Executes a batch of SQL commands using a provided SQL statement and a collection of parameters.

  Type Parameters:
  - `TSource`: The type of the parameter source used for binding.

  Parameters:
  - `sql`: The SQL command to be executed for each item in the collection.
  - `paramSources`: The collection of parameter sources, where each item represents the parameters for a single execution of the SQL command.
  - `binder`: A delegate that binds the parameters from a parameter source to a database command.

  Returns: The total number of rows affected by all executed commands in the batch.

- `ExecuteBatch(System.String,System.Data.Common.DbDataReader)`

  Executes a batch of SQL statements using the provided SQL command template and a data reader as the source for parameters.

  Parameters:
  - `sql`: The SQL command template to be executed for each row of data read from the data reader.
  - `dataReader`: The data reader containing the rows of data to be processed in the batch.

  Returns: The total number of rows affected by executing the batch.

- `PrepareCommand(System.Data.Common.DbCommand,System.String)`

  Configures the specified System.Data.Common.DbCommand with the provided SQL statement, sets its command text, and prepares parameters for execution.

  Parameters:
  - `command`: The System.Data.Common.DbCommand to be configured.
  - `sql`: The SQL query or command text to be assigned to the command.

- `ExecuteScript(System.String,System.String,System.String)`

  Executes the specified SQL script against the database using the provided provider name and connection string.

  Parameters:
  - `providerName`: The name of the database provider.
  - `connectionString`: The connection string for the database connection.
  - `script`: A string containing the SQL script to be executed.

- `GetScripExecutor(System.String)`

  Gets the script executor that matches the given provider name.

  Parameters:
  - `providerName`: The name of the database provider.

  Returns: An instance of a class that implements the DbExport.Providers.IScriptExecutor interface.

- `ToArray(System.Data.Common.DbDataReader)`

  Extracts the values of the columns from the first row of the data reader and returns them as an array of objects.

  Parameters:
  - `dataReader`: The DbDataReader from which to extract the column values.

  Returns: An array of objects containing the values of the columns from the first row of the data reader, or null if the data reader is empty.

- `ToDictionary(System.Data.Common.DbDataReader)`

  Extracts the values of the columns from the first row of the data reader and returns them as a dictionary, where the keys are the column names and the values are the corresponding column values.

  Parameters:
  - `dataReader`: The DbDataReader from which to extract the column values.

  Returns: A dictionary containing the column names as keys and the corresponding column values as values from the first row of the data reader, or null if the data reader is empty.

- `ToEntity``1(System.Data.Common.DbDataReader)`

  Converts the current row of the specified System.Data.Common.DbDataReader into an instance of the specified entity type.

  Type Parameters:
  - `TEntity`: The type of the entity to create. Must be a reference type with a parameterless constructor.

  Parameters:
  - `dataReader`: The System.Data.Common.DbDataReader positioned on the row to be converted.

  Returns: An instance of TEntity populated with values from the current row of the dataReader, or null if no rows are available to read.

- `ToArrayList(System.Data.Common.DbDataReader)`

  Extracts the values of the columns from all rows of the data reader and returns them as a list of object arrays, where each object array represents a row of data, and the elements of the array correspond to the column values in that row.

  Parameters:
  - `dataReader`: The DbDataReader from which to extract the column values.

  Returns: A list of object arrays, where each object array contains the column values for a row of data from the data reader.

- `ToDictionaryList(System.Data.Common.DbDataReader)`

  Extracts the values of the columns from all rows of the data reader and returns them as a list of dictionaries, where each dictionary represents a row of data, and the keys of the dictionary are the column names, and the values are the corresponding column values for that row.

  Parameters:
  - `dataReader`: The DbDataReader from which to extract the column values.

  Returns: A list of dictionaries, where each dictionary contains the column names as keys and the corresponding column values as values for a row of data from the data reader.

- `ToEntityList``1(System.Data.Common.DbDataReader)`

  Converts the data retrieved by a DbDataReader into a list of entities of the specified type.

  Type Parameters:
  - `TEntity`: The type of the entity to convert each row into. This type must be a class and have a parameterless constructor.

  Parameters:
  - `dataReader`: The DbDataReader containing the data to be converted.

  Returns: A list of entities of type TEntity, with each entity representing a row from the DbDataReader.

- `ToList(System.Data.Common.DbDataReader)`

  Extracts the values of the first column from all rows of the data reader and returns them as a list of objects, where each object in the list corresponds to the value of the first column for a row of data from the data reader.

  Parameters:
  - `dataReader`: The DbDataReader from which to extract the column values.

  Returns: A list of objects containing the values of the first column for each row of data from the data reader.

- `FromArray(System.Data.Common.DbCommand,System.Object[])`

  Populates the parameters of a database command with the specified array of values.

  Parameters:
  - `command`: The database command whose parameters will be populated.
  - `values`: An array of values to assign to the command's parameters.

- `FromDictionary(System.Data.Common.DbCommand,System.Collections.Generic.Dictionary{System.String,System.Object})`

  Populates the parameters of a database command with values from the provided dictionary.

  Parameters:
  - `command`: The database command whose parameters will be populated.
  - `values`: A dictionary containing parameter names as keys and their corresponding values.

- `FromEntity``1(System.Data.Common.DbCommand,``0)`

  Sets the parameter values of the specified System.Data.Common.DbCommand using the property values of the provided entity.

  Type Parameters:
  - `TEntity`: The type of the entity, which must be a reference type.

  Parameters:
  - `command`: The database command whose parameters will be updated.
  - `entity`: The entity from which the parameter values are retrieved.

- `FromDataReader(System.Data.Common.DbCommand,System.Data.Common.DbDataReader)`

  Populates the parameters of a given database command using the current row of the provided data reader.

  Parameters:
  - `command`: The database command whose parameters will be populated.
  - `dataReader`: The data reader containing the source data for the command's parameters.

### Type `Utility` (type)

A collection of helper methods and properties for database operations, string manipulation, and type conversion within the application.

#### Fields

- `CI`

  The default System.Globalization.CultureInfo instance used throughout the utility for culture-specific operations such as string comparisons, parsing, and formatting. This is set to System.Globalization.CultureInfo.InvariantCulture to ensure consistent behavior across different cultures and locales.

#### Properties

- `Encoding`

  The default encoding used for reading and writing data within the application. This can be set to a different encoding if needed, but defaults to UTF-8 for broad compatibility with various data sources and formats.

#### Methods

- `RegisterDbProviderFactories`

  Registers database provider factories for supported database types, enabling ADO.NET support for those providers within the application.

- `GetConnection(System.String,System.String)`

  Creates and returns a database connection using the specified provider name and connection string.

  Parameters:
  - `providerName`: The name of the database provider to use for creating the connection. This must match a registered provider name.
  - `connectionString`: The connection string used to establish the connection to the database. It must be a valid connection string for the specified provider.

  Returns: A DbConnection object that represents the established connection to the database.

  Throws:
  - `System.InvalidOperationException`: Thrown if a connection cannot be created for the specified provider name.

- `GetConnection(DbExport.Schema.Database)`

  Creates and returns a database connection using the specified database configuration.

  Parameters:
  - `database`: The database object containing the provider name and connection string used to establish the connection. Cannot be null.

  Returns: A DbConnection instance representing the established connection to the database.

- `ParseConnectionString(System.String)`

  Parses a connection string into a dictionary of key-value pairs.

  Parameters:
  - `connectionString`: The connection string to be parsed, formatted as key-value pairs separated by semicolons.

  Returns: A dictionary containing the parsed key-value pairs from the connection string. Each key corresponds to a setting name, and each value corresponds to the setting's value.

- `TransformConnectionString(System.String,System.Func{System.String,System.String,System.String})`

  Transforms the specified connection string by applying a provided transformation function to each property value.

  Parameters:
  - `connectionString`: The connection string to be transformed, formatted as a semicolon-separated list of key-value pairs.
  - `transformer`: A function that takes a property key and its corresponding value, and returns the transformed value for that property.

  Returns: A new connection string with the transformed property values, formatted as a semicolon-separated list.

- `SanitizeConnectionString(System.String)`

  Returns a sanitized version of the specified connection string with sensitive information, such as passwords, masked to prevent exposure.

  Parameters:
  - `connectionString`: The connection string to be sanitized. May contain sensitive information that should be masked before logging or displaying.

  Returns: A connection string in which password values are replaced with asterisks of the same length as the original value.

- `Escape(System.String,System.String)`

  Escapes the specified identifier name according to the syntax conventions of the given database provider.

  Parameters:
  - `name`: The identifier name to be escaped, such as a table or column name.
  - `providerName`: The name of the database provider that determines the escaping format to use.

  Returns: A string containing the escaped identifier, formatted according to the requirements of the specified database provider.

- `ToParameterName(System.String,System.String)`

  Converts a column name into a parameter name formatted according to the conventions of the specified database provider.

  Parameters:
  - `columnName`: The name of the column to be converted into a parameter name.
  - `providerName`: The name of the database provider that determines the parameter name format.

  Returns: A string representing the formatted parameter name suitable for the specified database provider.

- `EmptyDictionary``1`

  Creates and returns an empty dictionary with string keys that uses a case-insensitive string comparer.

  Type Parameters:
  - `TValue`: The type of the values stored in the dictionary.

  Returns: An empty dictionary with string keys and values of type TValue. The dictionary uses a case-insensitive comparer for its keys.

- `Split(System.String,System.Char)`

  Splits the specified string into an array of substrings based on the provided separator character, removing empty entries and trimming whitespace from each substring.

  Parameters:
  - `input`: The string to be split into substrings. Cannot be null.
  - `separator`: The character that delimits the substrings in the input string.

  Returns: An array of strings containing the substrings from the input string. The array will be empty if the input string is null, empty, or contains only separator characters.

- `IsEmpty(System.Object)`

  Determines whether the specified object is considered empty.

  Parameters:
  - `value`: The object to evaluate for emptiness. This can be null, a string, or an array.

  Returns: Returns true if the object is null, DBNull, an empty string, or an empty array; otherwise, returns false.

- `IsBoolean(System.Object)`

  Checks if the specified object can be interpreted as a boolean value.

  Parameters:
  - `value`: The object to check for boolean representation.

  Returns: true if the object can be interpreted as a boolean value; otherwise, false.

- `IsNumeric(System.Object,System.Decimal@)`

  Determines whether the specified object can be interpreted as a numeric value.

  Parameters:
  - `value`: The object to evaluate for numeric representation.
  - `converted`: When this method returns, contains the decimal representation of the value if conversion is successful; otherwise, it is set to zero.

  Returns: true if the value can be successfully converted to a numeric type; otherwise, false.

- `IsDate(System.Object,System.DateTime@)`

  Determines whether the specified object can be interpreted as a date/time value.

  Parameters:
  - `value`: The object to evaluate for date/time representation.
  - `converted`: When this method returns, contains the DateTime representation of the value if conversion is successful; otherwise, it is set to DateTime.Min.

  Returns: true if the value can be successfully converted to a date/time type; otherwise, false.

- `ToByte(System.Object)`

  Converts the specified object to its equivalent byte value, if possible.

  Parameters:
  - `value`: The object to convert.

  Returns: The byte value of the specified object if the conversion succeeds; otherwise, 0.

- `ToInt16(System.Object)`

  Converts the specified object to its equivalent 16-bit signed integer using the current culture's formatting conventions.

  Parameters:
  - `value`: The object to convert.

  Returns: A 16-bit signed integer equivalent to the numeric value contained in the specified object, or zero if the conversion is unsuccessful.

- `QuotedStr(System.Object,System.Char)`

  Formats the specified value as a string enclosed in the specified quote character, escaping any existing quotes within the value.

  Parameters:
  - `value`: The value to be formatted as a quoted string. This can be any object that can be converted to a string.
  - `quote`: The character used to enclose the value. The default is a single quote (').

  Returns: A string that represents the value enclosed in the specified quote character, with any existing quotes in the value escaped.

- `UnquotedStr(System.Object,System.Char)`

  Removes surrounding quote characters from the specified string representation and replaces any consecutive quote characters with a single instance.

  Parameters:
  - `value`: The object to process. The object's string representation must contain surrounding quote characters. Cannot be null.
  - `quote`: The character used to denote quotes in the string. Defaults to a single quote (').

  Returns: A string with the surrounding quote characters removed and any consecutive quote characters replaced with a single instance. Returns an empty string if the input is null or does not contain surrounding quotes.

- `BinToHex(System.Byte[])`

  Converts a byte array to its hexadecimal string representation.

  Parameters:
  - `bytes`: The array of bytes to convert. Cannot be null.

  Returns: A string containing the hexadecimal values of the input bytes, with each byte represented as a two-digit lowercase hexadecimal number.

- `ToBitString(System.Byte[])`

  Converts an array of bytes to a single binary string representation, where each byte is represented as an 8-character binary value.

  Parameters:
  - `bytes`: The array of bytes to convert. Cannot be null.

  Returns: A string containing the binary representation of the input byte array, with each byte's binary value concatenated in order.

- `FromBitString(System.String)`

  Converts a binary string representation into an array of bytes.

  Parameters:
  - `value`: The binary string to convert, which must consist of '0' and '1' characters. The string will be padded with leading zeros to ensure its length is a multiple of 8.

  Returns: An array of bytes representing the binary string. Each byte corresponds to 8 bits from the input string.

- `ToBaseN(System.Byte,System.Byte)`

  Converts the specified byte value to its string representation in the given base.

  Parameters:
  - `b`: The byte value to convert.
  - `n`: The base for the conversion. Must be between 2 and 36, inclusive.

  Returns: A string representing the byte value in the specified base. Returns "0" if the input value is zero.

- `FromBaseN(System.String,System.Byte)`

  Converts a string representation of a number in a specified base back to its byte value.

  Parameters:
  - `value`: The string representation of the number to convert. It should consist of valid digits for the specified base.
  - `n`: The base of the number system used in the input string. Must be between 2 and 36, inclusive.

  Returns: A byte value corresponding to the input string interpreted as a number in the specified base. Returns 0 if the input string is empty or invalid.

- `Digit2Char(System.Int32)`

  Converts a numeric digit or hexadecimal value to its corresponding character representation.

  Parameters:
  - `i`: The integer value to convert. Must be in the range 0 to 15, where values from 0 to 9 are mapped to '0'–'9', and values from 10 to 15 are mapped to 'A'–'F'.

  Returns: A character representing the digit or hexadecimal value corresponding to the input integer.

- `Char2Digit(System.Char)`

  Converts a character representing a digit or hexadecimal value back to its numeric integer representation.

  Parameters:
  - `c`: The character to convert. Valid inputs are '0'–'9' for values 0 to 9, and 'A'–'F' (case-insensitive) for values 10 to 15. Any character outside these ranges will be processed based on its ASCII value, which may lead to undefined behavior.

  Returns: An integer representing the numeric value of the input character. For '0'–'9', returns 0 to 9; for 'A'–'F', returns 10 to 15; for other characters, returns a value based on their ASCII code.

- `PasswordRegex`

  No XML summary provided.

## Namespace `DbExport.Providers`

### Type `CodeGenerator` (type)

Base class for code generators that produce SQL scripts for database schema and data export. This class implements the visitor pattern to traverse the database schema and generate appropriate SQL statements. Derived classes should override the visit methods to provide provider-specific SQL generation logic. The class also manages output writing and supports options for controlling the export process, such as whether to include schema, data, foreign keys, etc. The class implements IDisposable to allow for proper resource management of the output stream, especially when writing to files.

#### Fields

- `closeOutput`

  Indicates whether the output stream should be closed when the CodeGenerator instance is disposed. This ensures proper resource management, especially when the output stream is created internally and not supplied by the caller.

- `indentation`

  Represents the current indentation level used for formatting output. This value determines the number of tab characters written before each line to maintain proper hierarchical formatting in the generated code or text output.

- `textColumn`

  Tracks the current horizontal position (column) of the text being written to the output stream. Used to manage indentation and formatting during code generation, ensuring proper alignment and structured output.

#### Properties

- `ProviderName`

  Gets the name of the database provider for which this code generator is designed to generate SQL scripts.

- `ExportOptions`

  Gets or sets the export options that control the behavior of the code generation process, such as whether to include schema, data, foreign keys, identities, etc.

- `Output`

  Gets the TextWriter to which the generated SQL will be written. This property is initialized through the constructor and is used by the code generator to output the generated SQL statements.

- `SupportsDbCreation`

  Gets a value indicating whether this code generator supports generating a CREATE DATABASE statement as part of the export process.

- `GeneratesRowVersion`

  Gets a value indicating whether this code generator generates row version columns as part of the data export process.

- `RequireInlineConstraints`

  Gets a value indicating whether this code generator requires foreign key constraints to be included inline within the CREATE TABLE statements, as opposed to being generated as separate ALTER TABLE statements after the tables are created.

#### Constructors

- `CodeGenerator(System.IO.TextWriter)`

  Initializes a new instance of the CodeGenerator class with the specified TextWriter for output.

  Parameters:
  - `output`: The TextWriter to which the generated SQL will be written. Must not be null.

- `CodeGenerator`

  Initializes a new instance of the CodeGenerator class that writes output to the console.

- `CodeGenerator(System.String)`

  Initializes a new instance of the CodeGenerator class that writes output to a file at the specified path.

  Parameters:
  - `path`: The file path where the generated SQL will be written. The file will be created if it does not exist, or appended to if it does. Must not be null or empty.

#### Methods

- `Get(System.String,System.IO.TextWriter)`

  Factory method to create an instance of a CodeGenerator subclass based on the provided database provider name.

  Parameters:
  - `providerName`: The name of the database provider for which to create a code generator. Supported values include: "Microsoft.Data.SqlClient" for SQL Server, "Oracle.ManagedDataAccess.Client" for Oracle, "MySqlConnector" for MySQL, "Npgsql" for PostgreSQL, "FirebirdSql.Data.FirebirdClient" for Firebird, and "System.Data.SQLite" for SQLite. Must not be null or empty.
  - `output`: The TextWriter to which the generated SQL will be written. Must not be null.

  Returns: A CodeGenerator instance specific to the given provider name, initialized with the provided output TextWriter.

  Throws:
  - `System.ArgumentException`: When the providerName is not recognized as a supported database provider.

- `Dispose`

  No XML summary provided.

- `Dispose(System.Boolean)`

  Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

  Parameters:
  - `disposing`: A value indicating whether the method has been called directly or indirectly by a user's code. If true, both managed and unmanaged resources can be disposed; if false, only unmanaged resources should be released.

- `VisitDatabase(DbExport.Schema.Database)`

  No XML summary provided.

- `VisitTable(DbExport.Schema.Table)`

  No XML summary provided.

- `VisitColumn(DbExport.Schema.Column)`

  No XML summary provided.

- `VisitPrimaryKey(DbExport.Schema.PrimaryKey)`

  No XML summary provided.

- `VisitIndex(DbExport.Schema.Index)`

  No XML summary provided.

- `VisitForeignKey(DbExport.Schema.ForeignKey)`

  No XML summary provided.

- `VisitDataType(DbExport.Schema.DataType)`

  No XML summary provided.

- `Escape(System.String)`

  Escapes the given name (e.g., table name, column name) according to the syntax rules of the target database provider.

  Parameters:
  - `name`: The name to be escaped. This could be a table name, column name, or any identifier that may require escaping to avoid conflicts with reserved keywords or special characters.

  Returns: A string representing the escaped name, formatted according to the conventions of the target database provider.

- `GetTypeName(DbExport.Schema.Column)`

  Gets the SQL type name for the given column, taking into account the column's data type and any provider-specific type mappings.

  Parameters:
  - `column`: The column for which to determine the SQL type name. The method will consider the column's ColumnType and, if it is a user-defined type, its associated DataType to determine the appropriate SQL type name.

  Returns: A string representing the SQL type name for the column, formatted according to the conventions of the target database provider.

- `GetTypeName(DbExport.Schema.IDataItem)`

  Gets the SQL type name for the given data item, which could be a column or a user-defined data type.

  Parameters:
  - `item`: The data item for which to determine the SQL type name. This could be a column or a user-defined data type. The method will use the properties of the data item, such as ColumnType, Size, Precision, and Scale, to determine the appropriate SQL type name, potentially using provider-specific type mappings and formatting rules.

  Returns: A string representing the SQL type name for the data item, formatted according to the conventions of the target database provider.

- `GetTypeReference(DbExport.Schema.DataType)`

  Gets the SQL type reference for a user-defined data type, which may involve referencing the data type by name or using a specific syntax depending on the target database provider.

  Parameters:
  - `dataType`: The user-defined data type for which to get the SQL type reference. This method is called when a column has a ColumnType of UserDefined, and the column's DataType property is not null. The method will determine how to reference this user-defined data type in the generated SQL, which may involve using the data type's name or a specific syntax depending on the conventions of the target database provider.

  Returns: A string representing the SQL type reference for the user-defined data type, formatted according to the conventions of the target database provider.

- `GetKeyName(DbExport.Schema.Key)`

  Gets the name to be used for a key (such as an index or foreign key constraint) in the generated SQL.

  Parameters:
  - `key`: The key for which to get the name. This method is called when generating SQL for indexes and foreign key constraints, and it determines how to name these keys in the generated SQL. The default implementation returns the escaped name of the key, but derived classes can override this method to provide different naming conventions or to include additional information in the key name as needed by the target database provider.

  Returns: A string representing the name to be used for the key in the generated SQL, formatted according to the conventions of the target database provider.

- `Format(System.Object,DbExport.Schema.ColumnType)`

  Formats a value for inclusion in a SQL statement, taking into account the value's type and the corresponding column type.

  Parameters:
  - `value`: The value to be formatted for inclusion in a SQL statement. This could be a default value for a column, a value being inserted into a table, or any other value that needs to be represented as a literal in the generated SQL. The method will determine how to format this value based on its type and the specified column type, ensuring that it is correctly represented in the SQL syntax for the target database provider.
  - `columnType`: The column type that corresponds to the value being formatted. This information is used to determine how to format the value, such as whether to quote it, how to format dates and times, how to represent binary data, etc., according to the conventions of the target database provider.

  Returns: A string representing the formatted value, ready to be included as a literal in a SQL statement, formatted according to the conventions of the target database provider.

- `WriteComment(System.String,System.Object[])`

  Writes a comment line to the output, formatted according to the conventions of the target database provider.

  Parameters:
  - `format`: A format string that describes the content of the comment. This string can include placeholders for arguments, which will be replaced by the corresponding values in the args parameter.
  - `args`: The arguments to be formatted into the comment string. These values will replace the placeholders in the format string, allowing for dynamic content to be included in the comment based on the context of the code generation process, such as database name, generation timestamp, author, etc.

- `WriteDelimiter`

  Writes a statement delimiter (such as a semicolon) to the output, according to the syntax rules of the target database provider.

- `WriteDbCreationDirective(DbExport.Schema.Database)`

  Writes a CREATE DATABASE statement for the given database, according to the syntax rules of the target database provider.

  Parameters:
  - `database`: The database for which to write the CREATE DATABASE statement. This method will generate the appropriate SQL to create the database, including the database name and any necessary syntax according to the conventions of the target database provider. This method is called if the code generator supports database creation and if the export options indicate that the schema should be exported.

- `WriteIdentitySpecification(DbExport.Schema.Column)`

  Writes the identity specification for the specified column.

  Parameters:
  - `column`: The column for which the identity specification is being written.

- `WriteTableCreationSuffix(DbExport.Schema.Table)`

  Writes any additional SQL syntax that should be included at the end of a CREATE TABLE statement for the given table.

  Parameters:
  - `table`: The table for which to write the table creation suffix. This method can be overridden by derived classes to include additional syntax after the closing parenthesis of a CREATE TABLE statement, such as table options, storage engine specifications, or other provider-specific syntax that should be included when creating a table.

- `WriteDataMigrationPrefix`

  Writes any necessary SQL statements or directives that should be included before the data migration (INSERT statements) for the tables.

- `WriteDataMigrationSuffix`

  Writes any necessary SQL statements or directives that should be included after the data migration (INSERT statements) for the tables.

- `WriteUpdateRule(DbExport.Schema.ForeignKeyRule)`

  Writes the syntax for the ON UPDATE clause of a foreign key constraint, based on the specified update rule.

  Parameters:
  - `updateRule`: The foreign key rule that specifies the action to be taken when a referenced row is updated. This method will generate the appropriate SQL syntax for the ON UPDATE clause of a foreign key constraint, based on the value of the updateRule parameter, which can indicate actions such as CASCADE, SET NULL, SET DEFAULT, etc., according to the conventions of the target database provider.

- `WriteDeleteRule(DbExport.Schema.ForeignKeyRule)`

  Writes the syntax for the ON DELETE clause of a foreign key constraint, based on the specified delete rule.

  Parameters:
  - `deleteRule`: The foreign key rule that specifies the action to be taken when a referenced row is deleted. This method will generate the appropriate SQL syntax for the ON DELETE clause of a foreign key constraint, based on the value of the deleteRule parameter, which can indicate actions such as CASCADE, SET NULL, SET DEFAULT, etc., according to the conventions of the target database provider.

- `GetForeignKeyRuleText(DbExport.Schema.ForeignKeyRule)`

  Gets the text representation of a foreign key rule (such as ON UPDATE or ON DELETE actions) based on the specified rule.

  Parameters:
  - `rule`: The foreign key rule for which to get the text representation.

  Returns: A string representing the text of the foreign key rule, such as "CASCADE", "SET NULL", "RESTRICT", etc., formatted according to the conventions of the target database provider.

- `WriteInsertDirective(DbExport.Schema.Table,System.Data.Common.DbDataReader)`

  Writes an INSERT statement for the given table and data reader, generating the appropriate SQL syntax to insert a row of data into the table.

  Parameters:
  - `table`: The table into which the data will be inserted. This method will generate an INSERT statement that targets this table, including the table name and the columns for which data will be inserted.
  - `dr`: A DbDataReader that contains the data to be inserted into the table. This method will read values from this data reader to generate the VALUES clause of the INSERT statement, formatting each value according to its type and the corresponding column type, and ensuring that the generated SQL correctly represents the data for insertion into the target database.

- `Indent`

  Increases the indentation level for the generated SQL output. This method is typically called when entering a new block of SQL statements, such as after a CREATE TABLE statement, to ensure that the generated SQL is properly indented for readability. The indentation level is managed internally, and the Write methods will use this indentation level to prefix lines with the appropriate number of tabs or spaces according to the conventions of the target database provider. The Unindent method should be called when exiting a block to decrease the indentation level accordingly.

- `Unindent`

  Decreases the current indentation level by one, ensuring it does not fall below zero.

- `Write(System.Char)`

  Writes a single character to the output, respecting the current indentation level.

  Parameters:
  - `c`: The character to write. Carriage return characters are ignored.

- `Write(System.String)`

  Writes the specified string to the underlying output.

  Parameters:
  - `s`: The string to write. Must not be null.

- `Write(System.String,System.Object[])`

  Writes a formatted string to the output stream.

  Parameters:
  - `format`: A composite format string.
  - `values`: An array of objects to format and write to the output stream.

- `WriteLine`

  Writes a new line to the current output stream.

- `WriteLine(System.String)`

  Writes a string followed by a line terminator to the output.

  Parameters:
  - `s`: The string to write. If null, only a line terminator is written.

- `WriteLine(System.String,System.Object[])`

  Writes a formatted line, followed by a line termination string, to the output.

  Parameters:
  - `format`: The composite format string. Cannot be null or empty.
  - `values`: An array of objects to format using the specified format string. Can be null if no formatting is required.

- `BytesToHexString(System.Object)`

  Converts a byte array into a hexadecimal string representation prefixed with "0x".

  Parameters:
  - `value`: The input value, expected to be an object containing a byte array.

  Returns: A string representing the hexadecimal representation of the byte array, prefixed with "0x", or "''" if the array is empty.

- `IsSelected(DbExport.Schema.Index)`

  Determines whether the specified index is selected based on specific criteria, including whether it is checked, does not match a key, has at least one column, and all of its columns are checked.

  Parameters:
  - `index`: The index to evaluate for selection. Must not be null.

  Returns: Returns true if the index is checked, does not match a primary or foreign key, has columns, and all the columns are checked; otherwise, returns false.

- `IsSelected(DbExport.Schema.ForeignKey)`

  Determines whether the specified foreign key is selected for processing based on its properties and related table state.

  Parameters:
  - `fk`: The foreign key to evaluate. Must not be null.

  Returns: Returns `true` if the foreign key is checked, all its columns are checked, and its related table is also checked; otherwise, returns `false`.

### Type `ISchemaProvider` (type)

A common interface for schema providers, which are responsible for retrieving database schema information such as table names, column names, index names, foreign key names, and their associated metadata. This interface abstracts the underlying database provider implementation, allowing for flexibility and extensibility in supporting different database systems.

#### Properties

- `ProviderName`

  Gets the name of the database provider.

- `ConnectionString`

  Gets the connection string used to connect to the database.

- `DatabaseName`

  Gets the name of the database for which the schema information is being retrieved.

#### Methods

- `GetTableNames`

  Extracts the names of all tables in the database, along with their respective owners.

  Returns: An array of DbExport.Providers.NameOwnerPair objects, each containing the name and owner of a table.

- `GetColumnNames(System.String,System.String)`

  Extracts the names of all columns for a specified table and its owner.

  Parameters:
  - `tableName`: The name of the table for which to retrieve column names.
  - `tableOwner`: The owner of the table for which to retrieve column names.

  Returns: An array of column names for the specified table and owner.

- `GetIndexNames(System.String,System.String)`

  Extracts the names of all indexes for a specified table and its owner.

  Parameters:
  - `tableName`: The name of the table for which to retrieve index names.
  - `tableOwner`: The owner of the table for which to retrieve index names.

  Returns: An array of index names for the specified table and owner.

- `GetForeignKeyNames(System.String,System.String)`

  Extracts the names of all foreign keys for a specified table and its owner.

  Parameters:
  - `tableName`: That name of the table for which to retrieve foreign key names.
  - `tableOwner`: The owner of the table for which to retrieve foreign key names.

  Returns: An array of foreign key names for the specified table and owner.

- `GetTableMeta(System.String,System.String)`

  Extracts the metadata for a specified table and its owner, including information such as column data types, index definitions, foreign key relationships, and other relevant schema details.

  Parameters:
  - `tableName`: The name of the table for which to retrieve metadata.
  - `tableOwner`: The owner of the table for which to retrieve metadata.

  Returns: A DbExport.Providers.MetaData object containing the metadata for the specified table and owner.

- `GetColumnMeta(System.String,System.String,System.String)`

  Extracts the metadata for a specified column within a table and its owner, including information such as data type, nullability, default values, and other relevant schema details specific to the column.

  Parameters:
  - `tableName`: The name of the table containing the column for which to retrieve metadata.
  - `tableOwner`: The owner of the table containing the column for which to retrieve metadata.
  - `columnName`: The name of the column for which to retrieve metadata.

  Returns: A DbExport.Providers.MetaData object containing the metadata for the specified column, table, and owner.

- `GetIndexMeta(System.String,System.String,System.String)`

  Extracts the metadata for a specified index within a table and its owner, including information such as index type, indexed columns, uniqueness, and other relevant schema details specific to the index.

  Parameters:
  - `tableName`: The name of the table containing the index for which to retrieve metadata.
  - `tableOwner`: The owner of the table containing the index for which to retrieve metadata.
  - `indexName`: The name of the index for which to retrieve metadata.

  Returns: A DbExport.Providers.MetaData object containing the metadata for the specified index, table, and owner.

- `GetForeignKeyMeta(System.String,System.String,System.String)`

  Extracts the metadata for a specified foreign key within a table and its owner, including information such as referenced table and columns, foreign key constraints, and other relevant schema details specific to the foreign key relationship.

  Parameters:
  - `tableName`: The name of the table containing the foreign key for which to retrieve metadata.
  - `tableOwner`: The owner of the table containing the foreign key for which to retrieve metadata.
  - `fkName`: The name of the foreign key for which to retrieve metadata.

  Returns: A DbExport.Providers.MetaData object containing the metadata for the specified foreign key, table, and owner.

- `GetTypeNames`

  Extracts the names of all types (e.g., user-defined types, enums, etc.) in the database, along with their respective owners. This method is optional and may not be implemented by all database providers, as not all databases support user-defined types or similar constructs.

  Returns: An array of DbExport.Providers.NameOwnerPair objects, each containing the name and owner of a type in the database. If the database does not support types or if this method is not implemented, it may return an empty array.

- `GetTypeMeta(System.String,System.String)`

  Extracts the metadata for a specified type and its owner, including information such as type definition, underlying data type, allowed values (for enums), and other relevant schema details specific to the type.

  Parameters:
  - `typeName`: The name of the type for which to retrieve metadata.
  - `typeOwner`: The owner of the type for which to retrieve metadata.

  Returns: A DbExport.Providers.MetaData object containing the metadata for the specified type and owner. If the database does not support types or if this method is not implemented, it may return an empty DbExport.Providers.MetaData object.

### Type `IScriptExecutor` (type)

Defines an abstraction for executing SQL scripts against a database connection.

#### Methods

- `Execute(System.String,System.String)`

  Executes a SQL script against the specified database connection.

  Parameters:
  - `connectionString`: The connection string used to connect to the database.
  - `script`: The SQL script to be executed.

### Type `SimpleScriptExecutor` (type)

Provides a simple implementation of the DbExport.Providers.IScriptExecutor interface that executes SQL scripts against a database connection using the DbExport.SqlHelper class.

#### Constructors

- `SimpleScriptExecutor(System.String)`

  Provides a simple implementation of the DbExport.Providers.IScriptExecutor interface that executes SQL scripts against a database connection using the DbExport.SqlHelper class.

  Parameters:
  - `providerName`: The name of the database provider used to establish the connection.

### Type `BatchScriptExecutor` (type)

An implementation of the DbExport.Providers.IScriptExecutor interface that executes SQL scripts using batch processing when supported by the database provider.

#### Constructors

- `BatchScriptExecutor(System.String)`

  An implementation of the DbExport.Providers.IScriptExecutor interface that executes SQL scripts using batch processing when supported by the database provider.

  Parameters:
  - `providerName`: The name of the database provider used to establish the connection.

#### Methods

- `DelimiterRegex`

  No XML summary provided.

### Type `MetaData` (type)

Represents a collection of metadata key-value pairs. This class is used to store additional information about the database schema items such as tables, table columns and/or keys. The keys are case-insensitive, allowing for flexible access to the metadata values.

#### Constructors

- `MetaData`

  Initializes a new instance of the DbExport.Providers.MetaData class.

### Type `NameOwnerPair` (type)

Represents a pair consisting of a name and an optional owner, commonly used to define database objects such as tables or types along with their associated schema or owner information.

#### Properties

- `Name`

  Gets or sets the name of the object.

- `Owner`

  Gets or sets the owner of the object, or null if not applicable.

#### Methods

- `Equals(System.Object)`

  No XML summary provided.

- `GetHashCode`

  No XML summary provided.

- `ToString`

  No XML summary provided.

### Type `ProviderNames` (type)

A static class that contains constant string values representing the names of supported database providers. These names are typically used to identify the specific database provider when configuring database connections or performing database operations.

#### Fields

- `ACCESS`

  A constant string representing the database provider name for Microsoft Access.

- `SQLSERVER`

  A constant string representing the database provider name for Microsoft SQL Server.

- `ORACLE`

  A constant string representing the database provider name for Oracle using the Oracle Managed Data Access client.

- `MYSQL`

  A constant string representing the database provider name for MySQL.

- `POSTGRESQL`

  A constant string representing the database provider name for PostgreSQL.

- `FIREBIRD`

  A constant string representing the database provider name for Firebird.

- `SQLITE`

  A constant string representing the database provider name for SQLite.

### Type `SchemaProvider` (type)

A factory class that provides methods to create schema providers and retrieve database schemas based on the specified provider name, connection string, and optional schema filter.

#### Methods

- `GetProvider(System.String,System.String)`

  Gets an instance of ISchemaProvider based on the provided provider name and connection string.

  Parameters:
  - `providerName`: The name of the database provider (e.g., "Microsoft.Data.SqlClient", "Oracle.ManagedDataAccess.Client").
  - `connectionString`: The connection string to connect to the database.

  Returns: An instance of ISchemaProvider corresponding to the specified provider name.

  Throws:
  - `System.ArgumentException`: Thrown when the provider name is not recognized.

- `GetDatabase(DbExport.Providers.ISchemaProvider,System.String)`

  Extracts the database schema using the provided ISchemaProvider and optional schema filter, and constructs a Database object representing the schema.

  Parameters:
  - `provider`: An instance of ISchemaProvider to retrieve schema information from the database.
  - `schema`: A string representing the schema filter (e.g., a specific schema name). If null or whitespace, all schemas will be included.

  Returns: A Database object representing the schema of the database as retrieved by the provider.

- `GetDatabase(System.String,System.String,System.String)`

  Extracts the database schema using the specified provider name, connection string, and optional schema filter, and constructs a Database object representing the schema.

  Parameters:
  - `providerName`: The name of the database provider (e.g., "Microsoft.Data.SqlClient", "Oracle.ManagedDataAccess.Client").
  - `connectionString`: The connection string to connect to the database.
  - `schema`: A string representing the schema filter (e.g., a specific schema name). If null or whitespace, all schemas will be included.

  Returns: A Database object representing the schema of the database as retrieved by the provider.

- `GetTable(DbExport.Providers.ISchemaProvider,DbExport.Schema.Database,System.String,System.String)`

  Extracts the schema information for a specific table using the provided ISchemaProvider and constructs a Table object representing the table schema, including its columns, indexes, and foreign keys. The method retrieves metadata for the table and its components, and populates the Table object accordingly. If the table has a primary key, it generates the primary key using the metadata information.

  Parameters:
  - `provider`: The ISchemaProvider instance to retrieve schema information from the database.
  - `database`: The Database object to which the Table will belong.
  - `tableName`: The name of the table for which to retrieve schema information.
  - `tableOwner`: A string representing the owner of the table (e.g., schema name). This may be used to filter tables in databases that support multiple schemas.

  Returns: A Table object representing the schema of the specified table, including its columns, indexes, and foreign keys.

- `GetColumn(DbExport.Providers.ISchemaProvider,DbExport.Schema.Table,System.String)`

  Extracts the schema information for a specific column using the provided ISchemaProvider and constructs a Column object representing the column schema, including its data type, attributes, default value, and other properties. The method retrieves metadata for the column and populates the Column object accordingly. If the column is an identity column, it sets the identity properties based on the metadata information.

  Parameters:
  - `provider`: The ISchemaProvider instance to retrieve schema information from the database.
  - `table`: The Table object to which the Column will belong. This is used to establish the relationship between the column and its parent table.
  - `columnName`: The name of the column for which to retrieve schema information.

  Returns: A Column object representing the schema of the specified column, including its data type, attributes, default value, and other properties.

- `GetIndex(DbExport.Providers.ISchemaProvider,DbExport.Schema.Table,System.String)`

  Extracts the schema information for a specific index using the provided ISchemaProvider and constructs an Index object representing the index schema, including its columns, uniqueness, and whether it is a primary key. The method retrieves metadata for the index and populates the Index object accordingly. If the index is a primary key, it sets the primary key properties based on the metadata information.

  Parameters:
  - `provider`: The ISchemaProvider instance to retrieve schema information from the database.
  - `table`: The Table object to which the Index will belong. This is used to establish the relationship between the index and its parent table.
  - `indexName`: The name of the index for which to retrieve schema information.

  Returns: An Index object representing the schema of the specified index, including its columns, uniqueness, and whether it is a primary key.

- `GetForeignKey(DbExport.Providers.ISchemaProvider,DbExport.Schema.Table,System.String)`

  Extracts the schema information for a specific foreign key using the provided ISchemaProvider and constructs a ForeignKey object representing the foreign key schema, including its related table, related columns, and update/delete rules. The method retrieves metadata for the foreign key and populates the ForeignKey object accordingly. The related table and columns are determined based on the metadata information, and the update/delete rules are set based on the foreign key rule values retrieved from the metadata.

  Parameters:
  - `provider`: The ISchemaProvider instance to retrieve schema information from the database.
  - `table`: The Table object to which the ForeignKey will belong. This is used to establish the relationship between the foreign key and its parent table.
  - `fkName`: The name of the foreign key for which to retrieve schema information.

  Returns: A ForeignKey object representing the schema of the specified foreign key, including its related table, related columns, and update/delete rules.

- `GetDataType(DbExport.Providers.ISchemaProvider,DbExport.Schema.Database,System.String,System.String)`

  Extracts the schema information for a specific data type using the provided ISchemaProvider and constructs a DataType object representing the data type schema, including its column type, native type, size, precision, scale, nullability, enumerated status, default value, and possible values. The method retrieves metadata for the data type and populates the DataType object accordingly.

  Parameters:
  - `provider`: The ISchemaProvider instance to retrieve schema information from the database.
  - `database`: The Database object to which the DataType will belong. This is used to establish the relationship between the data type and its parent database.
  - `typeName`: The name of the data type for which to retrieve schema information.
  - `typeOwner`: The owner of the data type (e.g., schema name). This may be used to filter data types in databases that support multiple schemas.

  Returns: A DataType object representing the schema of the specified data type, including its column type, native type, character length, decimal precision and scale, nullability, enumerated status, default value, and possible values (for an enumerated type).

## Namespace `DbExport.Providers.Firebird`

### Type `FirebirdCodeGenerator` (type)

Generates SQL code specific to the Firebird database system. This class provides methods to process various database objects and options, translating them into Firebird-compatible SQL scripts.

#### Properties

- `FirebirdOptions`

  Represents configuration options specific to Firebird database generation.

#### Constructors

- `FirebirdCodeGenerator`

  Initializes a new instance of the FirebirdCodeGenerator class.

- `FirebirdCodeGenerator(System.IO.TextWriter)`

  Initializes a new instance of the FirebirdCodeGenerator class with the specified TextWriter for output.

  Parameters:
  - `output`: The TextWriter to which the generated SQL will be written. Must not be null.

- `FirebirdCodeGenerator(System.String)`

  Initializes a new instance of the FirebirdCodeGenerator class that writes output to a file at the specified path.

  Parameters:
  - `path`: The file path where the generated SQL will be written. Must not be null or empty.

#### Methods

- `FormatText(System.String)`

  Formats the given text input by splitting it into UTF-8 encoded chunks and escaping single quotes.

  Parameters:
  - `input`: The text input to be formatted.

  Returns: The formatted text as a single concatenated string of UTF-8 encoded chunks.

### Type `FirebirdOptions` (type)

Represents configuration options for Firebird database operations.

#### Properties

- `DataDirectory`

  Gets or sets the file system path to the directory where Firebird database files will be created and stored. This property is essential for specifying the location of the database files during database operations.

- `DefaultCharSet`

  Gets or sets the default character set to be used for encoding text data.

- `PageSize`

  Gets or sets the page size for writing data to disk.

- `ForcedWrites`

  Gets or sets a value indicating whether to force writes to disk.

- `Overwrite`

  Gets or sets a value indicating whether to overwrite existing files when exporting data.

- `CharacterSets`

  Gets a list of supported character sets for Firebird databases.

#### Methods

- `ToMarkdown`

  Converts the FirebirdOptions properties and their current values into a Markdown table representation.

  Returns: A string containing a Markdown-formatted table with the FirebirdOptions properties and their values.

### Type `FirebirdSchemaProvider` (type)

Provides schema-related operations for Firebird databases, including retrieval of table names, column names, index names, foreign key names, and metadata. Implements the DbExport.Providers.ISchemaProvider interface to support interaction with Firebird database schemas.

#### Constructors

- `FirebirdSchemaProvider(System.String)`

  Initializes a new instance of the DbExport.Providers.Firebird.FirebirdSchemaProvider class.

  Parameters:
  - `connectionString`: The connection string used to connect to the database.

#### Methods

- `ResolveColumnType(System.Int32,System.Nullable{System.Int32},System.Int32)`

  Resolves the column type based on the Firebird field type, subtype, and scale.

  Parameters:
  - `fbType`: The Firebird field type identifier.
  - `subType`: The Firebird field subtype identifier, if applicable.
  - `scale`: The scale value associated with the field.

  Returns: The resolved DbExport.Schema.ColumnType corresponding to the specified parameters.

- `GetNativeTypeName(System.Int32,System.Nullable{System.Int32},System.Byte)`

  Resolves the native Firebird type name based on the specified field type, subtype, and scale.

  Parameters:
  - `fbType`: The Firebird field type identifier.
  - `subType`: The Firebird field subtype identifier, or null if not applicable.
  - `scale`: The scale of the field, indicating decimal places for numeric types.

  Returns: A string representing the native Firebird type name that corresponds to the provided type, subtype, and scale.

- `GetFKRule(System.String)`

  Converts the provided foreign key rule string into a corresponding DbExport.Schema.ForeignKeyRule enumeration value.

  Parameters:
  - `rule`: The foreign key rule as a string, typically provided by the database.

  Returns: A DbExport.Schema.ForeignKeyRule value that corresponds to the input rule. If the rule is null or does not match any known values, the method returns DbExport.Schema.ForeignKeyRule.None.

### Type `FirebirdScriptExecutor` (type)

Provides functionality to execute Firebird SQL scripts against a Firebird database.

#### Methods

- `CreateDbRegex`

  No XML summary provided.

## Namespace `DbExport.Providers.MySqlClient`

### Type `MySqlCodeGenerator` (type)

A code generator responsible for producing SQL scripts for MySQL databases.

#### Properties

- `MySqlOptions`

  Represents configuration options specific to MySQL database generation.

#### Constructors

- `MySqlCodeGenerator`

  Initializes a new instance of the MySqlCodeGenerator class.

- `MySqlCodeGenerator(System.IO.TextWriter)`

  Initializes a new instance of the MySqlCodeGenerator class with the specified TextWriter for output.

  Parameters:
  - `output`: The TextWriter to which the generated SQL will be written. Must not be null.

- `MySqlCodeGenerator(System.String)`

  Initializes a new instance of the MySqlCodeGenerator class that writes output to a file at the specified path.

  Parameters:
  - `path`: The file path where the generated SQL will be written. Must not be null or empty.

### Type `CharacterSet` (type)

Represents a MySQL character set, including its name, supported collations, and default collation.

#### Properties

- `Name`

  The name of the character set.

- `Collations`

  An array of supported collations for the character set.

- `DefaultCollation`

  The default collation for the character set.

#### Constructors

- `CharacterSet(System.String,System.String[],System.String)`

  Represents a MySQL character set, including its name, supported collations, and default collation.

  Parameters:
  - `name`: The name of the character set.
  - `collations`: An array of supported collations for the character set.
  - `defaultCollation`: The default collation for the character set.

### Type `MySqlOptions` (type)

Represents options specific to MySQL database generation.

#### Properties

- `StorageEngine`

  Gets or sets the storage engine to be used for the database.

- `CharacterSet`

  Gets or sets the character set to be used for the database.

- `Collation`

  Gets or sets the collation to be used for the database.

- `IsMariaDb`

  Gets or sets a value indicating whether to optimize SQL for MariaDB.

- `StorageEngines`

  Gets a list of supported storage engines for MySQL databases.

- `CharacterSets`

  Gets a list of supported character sets for MySQL databases.

#### Methods

- `ToMarkdown`

  Converts the current MySqlOptions instance into a Markdown-formatted table.

  Returns: A string representation of the MySqlOptions instance in Markdown table format. The table includes details such as Storage Engine, Character Set, Collation, and the optimization flag for MariaDB.

### Type `MySqlSchemaProvider` (type)

Provides functionality to extract schema information from a MySQL database.

#### Constructors

- `MySqlSchemaProvider(System.String)`

  Represents a schema provider for interacting with MySQL databases. Provides methods to retrieve schema-related metadata, such as tables, columns, indices, and foreign keys. Implements the DbExport.Providers.ISchemaProvider interface.

  Parameters:
  - `connectionString`: The connection string used to connect to the MySQL database.

#### Methods

- `GetColumnType(System.String)`

  Determines the corresponding DbExport.Schema.ColumnType for the given MySQL data type. Maps MySQL-specific type definitions to standard column type enumerations.

  Parameters:
  - `mysqlType`: The MySQL data type as a string.

  Returns: The mapped DbExport.Schema.ColumnType that corresponds to the given MySQL data type.

- `Parse(System.String,DbExport.Schema.ColumnType)`

  Parses a string value into an appropriate object based on the specified column type. Handles different data types such as numeric types, date/time types, and string types. Returns System.DBNull.Value if the value is empty, null, or invalid for the specified type.

  Parameters:
  - `value`: The string value to parse.
  - `columnType`: The column type that determines how the value should be parsed.

  Returns: The parsed object corresponding to the specified column type, or System.DBNull.Value if the value cannot be converted.

- `GetFKRule(System.String)`

  Maps a string representation of a foreign key constraint rule to its corresponding DbExport.Schema.ForeignKeyRule enum value. Used to interpret foreign key rules such as "CASCADE" or "SET NULL", as retrieved from the database metadata.

  Parameters:
  - `rule`: A string representing the foreign key rule from the database metadata.

  Returns: A DbExport.Schema.ForeignKeyRule enum value corresponding to the provided string representation. If no match is found, returns DbExport.Schema.ForeignKeyRule.None.

- `UserTypeRegex`

  No XML summary provided.

- `Utf8Regex`

  No XML summary provided.

### Type `MySqlScriptExecutor` (type)

Provides functionality for executing MySQL scripts, including management of database creation commands and connection string updates for the target database. Extends the functionality of the BatchScriptExecutor class for MySQL-specific use cases.

#### Constructors

- `MySqlScriptExecutor`

  Provides functionality for executing MySQL scripts, including management of database creation commands and connection string updates for the target database. Extends the functionality of the BatchScriptExecutor class for MySQL-specific use cases.

#### Methods

- `Unescape(System.String)`

  Removes enclosing backticks from a database or table name if present.

  Parameters:
  - `name`: The name of the database or table, which may be enclosed in backticks.

  Returns: The unescaped name without backticks, or the original name if backticks are not present.

- `CreateDbRegex`

  No XML summary provided.

## Namespace `DbExport.Providers.Npgsql`

### Type `NpgsqlCodeGenerator` (type)

Provides functionality for generating database-specific code targeting Npgsql (PostgreSQL). This class extends the DbExport.Providers.CodeGenerator base class and overrides certain methods to tailor code generation to the PostgreSQL database platform.

#### Constructors

- `NpgsqlCodeGenerator`

  Initializes a new instance of the NpgsqlCodeGenerator class.

- `NpgsqlCodeGenerator(System.IO.TextWriter)`

  Initializes a new instance of the NpgsqlCodeGenerator class with the specified TextWriter for output.

  Parameters:
  - `output`: The TextWriter to which the generated SQL will be written. Must not be null.

- `NpgsqlCodeGenerator(System.String)`

  Initializes a new instance of the NpgsqlCodeGenerator class that writes output to a file at the specified path.

  Parameters:
  - `path`: The file path where the generated SQL will be written. Must not be null or empty.

### Type `NpgsqlSchemaProvider` (type)

Provides schema-related metadata for a Npgsql (PostgreSQL) database, allowing access to table, column, index, foreign key, and type information. This class implements the DbExport.Providers.ISchemaProvider interface and serves as a provider for PostgreSQL database schemas.

#### Constructors

- `NpgsqlSchemaProvider(System.String)`

  Initializes a new instance of the DbExport.Providers.Npgsql.NpgsqlSchemaProvider class.

  Parameters:
  - `connectionString`: The connection string to use for database access.

#### Methods

- `GetColumnType(System.String)`

  Maps a PostgreSQL native data type to the corresponding DbExport.Schema.ColumnType enumeration value.

  Parameters:
  - `npgsqlType`: The native PostgreSQL type as a string.

  Returns: The corresponding DbExport.Schema.ColumnType for the specified PostgreSQL type.

- `Parse(System.String,DbExport.Schema.ColumnType)`

  Parses a string value into an object of the appropriate type based on the specified column type.

  Parameters:
  - `value`: The string representation of the value to parse.
  - `columnType`: The target column type used to determine the type of the parsed value.

  Returns: Returns the parsed value as an object of the respective type, or System.DBNull.Value if parsing fails or the value is null or "NULL".

- `GetFKRule(System.String)`

  Converts a string representation of a foreign key rule into its corresponding DbExport.Schema.ForeignKeyRule enum value.

  Parameters:
  - `rule`: The string representation of the foreign key rule. Possible values include "RESTRICT", "CASCADE", "SET DEFAULT", "SET NULL", or other values.

  Returns: The corresponding DbExport.Schema.ForeignKeyRule enum value. If the input does not match any predefined rules, DbExport.Schema.ForeignKeyRule.None is returned.

- `CommaRegex`

  No XML summary provided.

- `Utf8Regex`

  No XML summary provided.

### Type `NpgsqlScriptExecutor` (type)

Represents a script executor specifically designed for executing Npgsql (PostgreSQL) database scripts. This class extends the DbExport.Providers.BatchScriptExecutor and overrides its behavior to handle PostgreSQL-specific use cases, such as 'CREATE DATABASE' commands and connection string adjustments for the target database.

#### Constructors

- `NpgsqlScriptExecutor`

  Represents a script executor specifically designed for executing Npgsql (PostgreSQL) database scripts. This class extends the DbExport.Providers.BatchScriptExecutor and overrides its behavior to handle PostgreSQL-specific use cases, such as 'CREATE DATABASE' commands and connection string adjustments for the target database.

#### Methods

- `CreateDbRegex`

  No XML summary provided.

## Namespace `DbExport.Providers.OracleClient`

### Type `OracleCodeGenerator` (type)

Generates Oracle-specific SQL code for database schema objects and related functionality.

#### Constructors

- `OracleCodeGenerator`

  Initializes a new instance of the OracleCodeGenerator class.

- `OracleCodeGenerator(System.IO.TextWriter)`

  Initializes a new instance of the OracleCodeGenerator class with the specified TextWriter for output.

  Parameters:
  - `output`: The TextWriter to which the generated SQL will be written. Must not be null.

- `OracleCodeGenerator(System.String)`

  Initializes a new instance of the OracleCodeGenerator class that writes output to a file at the specified path.

  Parameters:
  - `path`: The file path where the generated SQL will be written. Must not be null or empty.

### Type `OracleSchemaProvider` (type)

Provides schema metadata for Oracle databases by implementing the ISchemaProvider interface. This class allows retrieval of database objects such as tables, columns, indexes, and foreign keys, as well as their associated metadata.

#### Constructors

- `OracleSchemaProvider(System.String)`

  Initializes a new instance of the DbExport.Providers.OracleClient.OracleSchemaProvider class.

  Parameters:
  - `connectionString`: The connection string used to connect to the Oracle database.

#### Methods

- `GetColumnType(System.String,System.Byte,System.Byte)`

  Determines the corresponding DbExport.Schema.ColumnType for the provided Oracle database type based on its type name, precision, and scale.

  Parameters:
  - `oracleType`: The name of the Oracle database type (e.g., "NUMBER", "CHAR").
  - `precision`: The precision of the Oracle type, used for numeric types.
  - `scale`: The scale of the Oracle type, used for numeric types.

  Returns: The DbExport.Schema.ColumnType representing the equivalent column type for the specified Oracle type.

- `Parse(System.String,DbExport.Schema.ColumnType)`

  Parses a specified string value into an object of the specified database column type.

  Parameters:
  - `value`: The string value to parse.
  - `columnType`: The column type that determines the target data type of the parsed value.

  Returns: The parsed object of the requested column type, or System.DBNull.Value if the value is null, empty, or cannot be converted to the specified column type.

## Namespace `DbExport.Providers.SqlClient`

### Type `SqlCodeGenerator` (type)

Represents a code generator specifically designed for generating SQL Server-compatible scripts. Extends the DbExport.Providers.CodeGenerator class to provide SQL Server-specific implementation details for database schema export and related functionality.

#### Properties

- `IsFileBased`

  Gets or sets a value indicating whether the SQL Server database is file-based. This is typically the case when using SQL Server Express LocalDB.

#### Constructors

- `SqlCodeGenerator`

  Initializes a new instance of the DbExport.Providers.SqlClient.SqlCodeGenerator class.

- `SqlCodeGenerator(System.IO.TextWriter)`

  Initializes a new instance of the DbExport.Providers.SqlClient.SqlCodeGenerator class with the specified TextWriter for output.

  Parameters:
  - `output`: The TextWriter to which the generated SQL will be written. Must not be null.

- `SqlCodeGenerator(System.String)`

  Initializes a new instance of the DbExport.Providers.SqlClient.SqlCodeGenerator class that writes output to a file at the specified path.

  Parameters:
  - `path`: The file path where the generated SQL will be written. Must not be null or empty.

### Type `SqlSchemaProvider` (type)

No XML summary provided.

#### Constructors

- `SqlSchemaProvider(System.String)`

  Initializes a new instance of the DbExport.Providers.SqlClient.SqlSchemaProvider class.

  Parameters:
  - `connectionString`: The connection string used to connect to the database.

#### Methods

- `GetColumnType(System.String)`

  Converts a given SQL type name to its equivalent DbExport.Schema.ColumnType enumeration value.

  Parameters:
  - `sqlType`: The name of the SQL type to be converted.

  Returns: A DbExport.Schema.ColumnType value that corresponds to the given SQL type.

- `Parse(System.String,DbExport.Schema.ColumnType)`

  Parses a given string value into the appropriate object type based on the specified column type.

  Parameters:
  - `value`: The string value to be parsed.
  - `columnType`: The column type that determines how the value is interpreted.

  Returns: An object representing the parsed value. Returns System.DBNull.Value if the value is invalid or cannot be parsed for the specified column type.

- `GetFKRule(System.String)`

  Determines the DbExport.Schema.ForeignKeyRule based on the provided rule string.

  Parameters:
  - `rule`: The string representing the foreign key rule (e.g., "CASCADE").

  Returns: A DbExport.Schema.ForeignKeyRule enumeration value corresponding to the input rule.

- `UniqueRegex`

  No XML summary provided.

- `PrimaryKeyRegex`

  No XML summary provided.

- `CommaRegex`

  No XML summary provided.

### Type `SqlScripExecutor` (type)

Represents an implementation of the DbExport.Providers.IScriptExecutor interface for executing SQL scripts against a SQL Server database using a given connection string.

#### Methods

- `Unescape(System.String)`

  Removes surrounding square brackets from the input string if they exist.

  Parameters:
  - `name`: The string to unescape, potentially surrounded by square brackets.

  Returns: The input string without surrounding square brackets, or the original string if no brackets are present.

- `DelimiterRegex`

  No XML summary provided.

- `CreateDbRegex`

  No XML summary provided.

- `CreateTypeRegex`

  No XML summary provided.

## Namespace `DbExport.Providers.SQLite`

### Type `SQLiteCodeGenerator` (type)

Generates SQLite-specific SQL scripts for database schema and data migrations. This class is designed to provide a SQLite-compatible implementation of the base DbExport.Providers.CodeGenerator functionalities. It facilitates the generation of database schema definitions, constraints, and data migration scripts tailored for SQLite databases.

#### Constructors

- `SQLiteCodeGenerator`

  Initializes a new instance of the SQLiteCodeGenerator class.

- `SQLiteCodeGenerator(System.IO.TextWriter)`

  Initializes a new instance of the SQLiteCodeGenerator class with the specified TextWriter for output.

  Parameters:
  - `output`: The TextWriter to which the generated SQL will be written. Must not be null.

- `SQLiteCodeGenerator(System.String)`

  Initializes a new instance of the SQLiteCodeGenerator class that writes output to a file at the specified path.

  Parameters:
  - `path`: The file path where the generated SQL will be written. Must not be null or empty.

### Type `SQLiteSchemaProvider` (type)

Provides schema information for SQLite databases. This class implements the ISchemaProvider interface, enabling retrieval of database schema metadata such as table names, column names, foreign key names, and associated metadata.

#### Fields

- `tableColumns`

  Represents a data structure that holds information about the columns of database tables in the current schema context. This variable acts as a cache for table column metadata and is utilized to simplify metadata retrievals and avoid redundant database queries.

- `tableIndexes`

  Represents a data structure that caches metadata related to the indexes of database tables within the current schema context. This variable is used to store and retrieve index information such as uniqueness and origin, optimizing metadata queries and reducing redundant database calls.

- `tableForeignKeys`

  Serves as a data structure to store information about the foreign keys of database tables within the current schema context. This variable is used to cache foreign key metadata, enabling efficient retrieval and minimizing redundant queries to the database.

- `tableHasAutoIncrement`

  Stores a mapping of table names to a boolean value indicating whether each table has an auto-increment column. This variable is used to cache the auto-increment metadata for tables, optimizing performance by reducing the need for repetitive queries to the database.

#### Constructors

- `SQLiteSchemaProvider(System.String)`

  Initializes a new instance of the DbExport.Providers.SQLite.SQLiteSchemaProvider class.

  Parameters:
  - `connectionString`: The connection string used to connect to the SQLite database.

#### Methods

- `Combine(System.String,System.String)`

  Combines the table owner and table name into a single string, separated by a period.

  Parameters:
  - `tableOwner`: The owner of the table, usually representing the schema or database user.
  - `tableName`: The name of the table for which the owner is being combined.

  Returns: A string that combines the table owner and table name in the format "tableOwner.tableName".

- `RegisterList(DbExport.Providers.MetaData,System.String,System.String,System.Collections.Generic.List{System.Collections.Generic.Dictionary{System.String,System.Object}})`

  Registers a list of metadata associated with a specific table in the given collection.

  Parameters:
  - `collection`: The metadata collection where the table information will be registered.
  - `tableOwner`: The owner of the table for which metadata is being registered.
  - `tableName`: The name of the table for which metadata is being registered.
  - `list`: The list of metadata entries to be registered, represented as dictionaries of key-value pairs.

- `FindFirst(DbExport.Providers.MetaData,System.String,System.String,System.Predicate{System.Collections.Generic.Dictionary{System.String,System.Object}})`

  Finds the first item in the specified collection that matches the given predicate for a specific table.

  Parameters:
  - `collection`: The metadata collection containing table-related data.
  - `tableOwner`: The owner of the table whose data is being queried.
  - `tableName`: The name of the table whose data is being queried.
  - `predicate`: The condition to match items in the collection.

  Returns: A dictionary representing the first item in the collection that matches the predicate, or null if no match is found.

- `ResolveColumnType(System.String,DbExport.Schema.ColumnType@,System.String@,System.Int16@,System.Byte@,System.Byte@)`

  Resolves the column type based on the provided SQL type string. Determines the corresponding DbExport.Schema.ColumnType, native database type, size, precision, and scale of the column.

  Parameters:
  - `sqlType`: The SQL type string to be analyzed.
  - `columnType`: Outputs the resolved DbExport.Schema.ColumnType enumeration value.
  - `nativeType`: Outputs the native SQL type associated with the column.
  - `size`: Outputs the size of the column, if applicable.
  - `precision`: Outputs the precision of the column, if applicable.
  - `scale`: Outputs the scale of the column, if applicable.

- `GetColumnType(System.String)`

  Determines the corresponding DbExport.Schema.ColumnType for a given SQLite data type.

  Parameters:
  - `sqliteType`: The SQLite data type as a string.

  Returns: The DbExport.Schema.ColumnType that corresponds to the specified SQLite data type.

- `ParseValue(System.Object,DbExport.Schema.ColumnType)`

  Parses the input value into a strongly typed object based on the specified column type.

  Parameters:
  - `value`: The value to be parsed, which can be any object or null.
  - `columnType`: The type of column to determine the parsing logic for the value.

  Returns: An object representing the parsed value, or System.DBNull.Value if parsing fails or the value is null.

- `ParseForeignKeyRule(System.String)`

  Converts a SQLite foreign key rule string into a corresponding DbExport.Schema.ForeignKeyRule enumeration value.

  Parameters:
  - `sqlFkRule`: The foreign key rule as represented in the SQLite metadata (e.g., "NO ACTION", "CASCADE").

  Returns: A DbExport.Schema.ForeignKeyRule enumeration value that corresponds to the specified SQLite foreign key rule.

## Namespace `DbExport.Schema`

### Type `Column` (type)

Represents a column in a database table with metadata and attributes that provide detailed information about its configuration and behavior within the schema.

#### Properties

- `ColumnType`

  Gets the type of the column, which is defined by the `ColumnType` enumeration.

- `NativeType`

  Gets the native type of the column as a string.

- `Size`

  Gets the size (or character length) of the column's data.'

- `Precision`

  Gets the precision of the column, primarily used for numeric types.

- `Scale`

  Gets the scale (number of decimal places) of numeric types.

- `Attributes`

  Gets the attributes that describe the column's behavior and constraints.'

- `DefaultValue`

  Gets the default value defined for the column if no value is provided during insertion.

- `Description`

  Gets the description or documentation of the column, often used for metadata purposes.

- `IdentitySeed`

  Gets or sets the seed value for the identity column.

- `IdentityIncrement`

  Gets or sets the increment value for the identity column.

- `Table`

  Gets the table to which this column belongs.

- `DataType`

  Gets the definition of the column's data type if it's user-defined and available in imported schema.

- `IsRequired`

  Gets a value indicating whether the column is nullable.

- `IsComputed`

  Gets a value indicating whether the column is a computed column.

- `IsIdentity`

  Gets a value indicating whether the column is an identity column.

- `IsGenerated`

  Gets a value indicating whether the column is a row version column.

- `IsPKColumn`

  Gets a value indicating whether the column is a primary key column.

- `IsFKColumn`

  Gets a value indicating whether the column is a foreign key column.

- `IsKeyColumn`

  Gets a value indicating whether the column is a key column.

- `IsIndexColumn`

  Gets a value indicating whether the column is an index column.

- `IsNumeric`

  Gets a value indicating whether the column is of a numeric type.

- `IsAlphabetic`

  Gets a value indicating whether the column is of an alphabetic type.

- `IsFixedLength`

  Gets a value indicating whether the column is fixed-length.

- `IsUnsigned`

  Gets a value indicating whether the column is of an unsigned integer type.

- `IsUnicode`

  Gets a value indicating whether the column is of a Unicode type.

- `IsIntegral`

  Gets a value indicating whether the column is of an integral type.

- `IsNatural`

  Gets a value indicating whether the column is of a natural integer type, i.e., an unsigned integer

- `IsTemporal`

  Gets a value indicating whether the column is of a temporal type.

- `IsBinary`

  Gets a value indicating whether the column is of a binary type.

#### Constructors

- `Column(DbExport.Schema.Table,System.String,DbExport.Schema.ColumnType,System.String,System.Int16,System.Byte,System.Byte,DbExport.Schema.ColumnAttributes,System.Object,System.String)`

  Represents a column in a database table with metadata and attributes that provide detailed information about its configuration and behavior within the schema.

  Parameters:
  - `table`: The table to which this column belongs.
  - `name`: The name of the column within the table.
  - `type`: The logical type of the column, defined by `ColumnType`.
  - `nativeType`: The database-specific type of the column as a string.
  - `size`: The maximum size of the column's data in bytes or characters.
  - `precision`: The precision of the column, primarily used for numeric types.
  - `scale`: The scale (number of decimal places) of numeric types.
  - `attributes`: The attributes that describe the column's behavior and constraints.
  - `defaultValue`: The default value defined for the column if no value is provided during insertion.
  - `description`: The description or documentation of the column, often used for metadata purposes.

#### Methods

- `SetAttribute(DbExport.Schema.ColumnAttributes)`

  Sets the attribute of the column.

  Parameters:
  - `attribute`: The attribute to set.

- `MakeIdentity(System.Int64,System.Int64)`

  Configures the column as an identity column with a specified seed and increment.

  Parameters:
  - `seed`: The initial value of the identity column.
  - `increment`: The step value used to increment the identity column.

- `GetAttributesFromType(DbExport.Schema.ColumnType)`

  Retrieves the attributes associated with the specified column type.

  Parameters:
  - `type`: The type of the column for which attributes are being retrieved.

  Returns: The attributes derived from the specified column type.

### Type `ColumnAttributes` (type)

Specifies a set of attributes that can be associated with a database column.

#### Fields

- `None`

  Indicates that no specific attributes are associated with the column.

- `Required`

  Indicates that the column is required and must have a value.

- `Computed`

  Indicates that the column's value is computed by the database.

- `Identity`

  Indicates that the column is an identity column.

- `PKColumn`

  Indicates that the column is part of the primary key of a table.

- `FKColumn`

  Indicates that the column is a foreign key in the database schema.

- `IXColumn`

  Indicates that the column is part of an index.

- `Numeric`

  Indicates that the column holds numeric data.

- `Alphabetic`

  Indicates that the column contains alphabetic or textual data.

- `FixedLength`

  Specifies that the column has a fixed length.

- `Unsigned`

  Specifies that the column represents an unsigned numeric value.

- `Unicode`

  Specifies that the column supports or is encoded using Unicode characters.

- `Temporal`

  Indicates that the column is associated with temporal data or is used to track changes over time.

- `Binary`

  Indicates that the column stores binary data.

### Type `ColumnCollection` (type)

Represents a collection of database columns.

#### Constructors

- `ColumnCollection`

  Initializes a new instance of the DbExport.Schema.ColumnCollection class.

- `ColumnCollection(System.Int32)`

  Initializes a new instance of the DbExport.Schema.ColumnCollection class with the specified initial capacity.

  Parameters:
  - `capacity`: The initial number of elements that the collection can contain.

- `ColumnCollection(System.Collections.Generic.IEnumerable{DbExport.Schema.Column})`

  Initializes a new instance of the DbExport.Schema.ColumnCollection class with the specified collection of items.

  Parameters:
  - `columns`: The collection of items to initialize the collection with.

### Type `ColumnSet` (type)

Represents an abstract base class for a set of related columns within a database schema. Provides functionality to manage the state of column checks and evaluate check conditions.

#### Properties

- `Columns`

  Gets a collection of columns associated with the column set.

- `AllColumnsAreChecked`

  Gets a value indicating whether all columns in the set are checked.

- `NoColumnIsChecked`

  Gets a value indicating whether no column in the set is checked.

- `AnyColumnIsChecked`

  Gets a value indicating whether any column in the set is checked.

- `AnyColumnIsUnchecked`

  Gets a value indicating whether any column in the set is unchecked.

#### Constructors

- `ColumnSet(DbExport.Schema.SchemaItem,System.String)`

  Represents an abstract base class for a set of related columns within a database schema. Provides functionality to manage the state of column checks and evaluate check conditions.

### Type `ColumnType` (type)

Represents the types of columns that can be used in a database schema. This enumeration provides a comprehensive list of supported data types including numeric, textual, date/time, binary, and user-defined types.

#### Fields

- `Unknown`

  Represents an unknown or unspecified column data type. This value is used as a placeholder when the actual data type cannot be determined or is not provided.

- `Boolean`

  Represents a Boolean data type, typically used to store true or false values.

- `TinyInt`

  Represents a tiny integer data type typically used to store very small integers. This data type commonly has a storage size of 1 byte and can store values within a limited range, often from 0 to 255 for unsigned or -128 to 127 for signed.

- `UnsignedTinyInt`

  Represents an 8-bit unsigned integer column type. This type is used to store non-negative integer values ranging from 0 to 255.

- `SmallInt`

  Represents a 16-bit signed integer column type in the database schema. Used for storing smaller numeric values within the range of -32,768 to 32,767.

- `UnsignedSmallInt`

  Represents an unsigned small integer column type in a database schema. This type is used for storing non-negative integer values with a smaller range compared to standard integers.

- `Integer`

  Represents a column data type that stores integer values in a database schema. This type is used for whole numbers without fractional or decimal components.

- `UnsignedInt`

  Represents an unsigned 32-bit integer column data type. This type is used when the column stores non-negative whole numbers that fall within the range of an unsigned 32-bit integer.

- `BigInt`

  Represents a column data type for large integer values in a database schema. Typically used to store 64-bit signed integer data, allowing for a wide range of numeric values.

- `UnsignedBigInt`

  Represents a 64-bit unsigned integer column data type. This type is used for storing large non-negative whole numbers that require a greater range than 32-bit integers can provide.

- `SinglePrecision`

  Represents a single-precision floating-point column data type. Typically used to store approximate numeric values with a 32-bit floating-point format, suitable for scenarios where reduced precision is acceptable for saving storage space.

- `DoublePrecision`

  Represents a double-precision floating-point numeric column type. This value is typically used for storing high-precision numerical data requiring 64 bits of storage, adhering to the IEEE 754 standard.

- `Currency`

  Represents a column data type for monetary or currency values. This type is commonly used to store precise financial data.

- `Decimal`

  Represents a column data type designed for high-precision fixed-point numeric values. This type is typically used for financial and monetary calculations where accuracy is critical.

- `Date`

  Represents a date column data type. This value is used for columns that store calendar dates without time components.

- `Time`

  Represents a column data type that stores time values without a date component. Typically used to define fields containing time-of-day information.

- `DateTime`

  Represents a column data type used to store both date and time information. This value is commonly used for data that combines calendar dates with specific timestamps.

- `Interval`

  Represents a time-based interval or duration, typically used to store a span of time in a database column. This value is suited for scenarios requiring precise measurements of time elapsed between events or date/time calculations.

- `Char`

  Represents a fixed-length character column data type. Used for storing text of a specific length, typically for performance optimization and ensuring consistent data size for string values.

- `NChar`

  Represents a fixed-length Unicode character column in a database. Used to store text data with a predefined length and support for multilingual characters.

- `VarChar`

  Represents a variable-length character column data type. This type is typically used to store text data of variable length within a defined maximum size.

- `NVarChar`

  Represents a variable-length, Unicode character data type. This type is used for storing textual data that may include multilingual characters, supporting a wide range of character sets using the Unicode standard.

- `Text`

  Represents a column data type used for storing large amounts of textual data. This type is suitable for cases where text values of varying and potentially significant length need to be handled.

- `NText`

  Represents a column data type used to store large Unicode text data. This type is typically employed for columns designed to handle text strings that exceed standard size limitations.

- `Bit`

  Represents a column data type used to store binary or logical bit values. This type is commonly used for fields that store boolean-like states or flag information.

- `Blob`

  Represents a binary large object (BLOB) column data type, typically used for storing variable-length binary data such as images, files, or multimedia.

- `File`

  Represents a column that stores file data or file-related information. This type is typically used when the column contains references to file paths, binary file data, or metadata about files.

- `Xml`

  Represents a column data type designed to store XML data. This value is used for columns that require structured or semi-structured data stored in XML format, often used for interoperability or hierarchical data representation.

- `Json`

  Represents a column data type used to store JSON-encoded data. This type is suitable for columns containing structured or semi-structured data in JSON format.

- `Geometry`

  Represents a spatial data type used to store geometric or geographical information, such as points, lines, and polygons. This type is typically used for mapping and spatial analysis.

- `Guid`

  Represents a column data type used for storing globally unique identifiers (GUIDs). Typically used to store unique keys or identifiers across distributed systems.

- `RowVersion`

  Represents a column data type used for tracking and managing versioning of rows in a database table. Typically utilized for concurrency control to detect changes to data rows.

- `UserDefined`

  Represents a user-defined column data type. This value is used when the column type is specified explicitly by the user, often for custom or database-specific data types that are not covered by standard types.

### Type `Database` (type)

Represents a database within the schema, containing metadata about its provider, connection information, data types, and tables. It serves as the root schema item for database-related operations.

#### Properties

- `ProviderName`

  The name of the database provider used to connect to the database.

- `ConnectionString`

  The connection string used to connect to the database.

- `DataTypes`

  The collection of data types defined in the database.

- `Tables`

  The collection of tables in the database.

#### Constructors

- `Database(System.String,System.String,System.String)`

  Represents a database within the schema, containing metadata about its provider, connection information, data types, and tables. It serves as the root schema item for database-related operations.

  Parameters:
  - `name`: The name of the database.
  - `providerName`: The name of the database provider used to connect to the database.
  - `connectionString`: The connection string used to connect to the database.

### Type `DataType` (type)

Represents a user-defined database data type with associated metadata such as size, precision, scale, and other characteristics.

#### Properties

- `Owner`

  The owner of the data type.

- `ColumnType`

  The column type of the data type.

- `NativeType`

  The native database-specific type definition.

- `Size`

  The size (or character length) of the data type.

- `Precision`

  The decimal precision of the data type.

- `Scale`

  The decimal scale of the data type.

- `IsNullable`

  Indicates whether the data type is nullable.

- `IsEnumerated`

  Indicates whether the data type is an enumerated (enum) type.

- `DefaultValue`

  The default value of the data type, if any.

- `PossibleValues`

  A collection of possible values for the data type, if it is enumerated.

- `Database`

  The database that owns the data type.

- `IsRequired`

  Gets a value indicating whether the data type is required.

#### Constructors

- `DataType(DbExport.Schema.Database,System.String,System.String,DbExport.Schema.ColumnType,System.String,System.Int16,System.Byte,System.Byte,System.Boolean,System.Boolean,System.Object,System.Collections.Generic.IEnumerable{System.Object})`

  Represents a user-defined database data type with associated metadata such as size, precision, scale, and other characteristics.

  Parameters:
  - `database`: The parent `Database` object associated with the data type.
  - `name`: The name of the data type.
  - `owner`: The owning schema or namespace of the data type.
  - `type`: The column type, specified as an enum of `ColumnType`.
  - `nativeType`: The native database-specific type definition.
  - `size`: The size of the data type, typically applicable for text or binary data.
  - `precision`: The precision value, applicable for numeric types.
  - `scale`: The scale value, applicable for numeric types with fractional parts.
  - `nullable`: Indicates whether the data type is nullable.
  - `enumerated`: Indicates whether the data type is an enumerated (enum) type.
  - `defaultValue`: The default value of the data type, if any.
  - `possibleValues`: A collection of possible values for the data type, if it is enumerated.

### Type `DataTypeCollection` (type)

Represents a collection of database data types.

#### Constructors

- `DataTypeCollection`

  Initializes a new instance of the DbExport.Schema.DataTypeCollection class.

- `DataTypeCollection(System.Int32)`

  Initializes a new instance of the DbExport.Schema.DataTypeCollection class with the specified initial capacity.

  Parameters:
  - `capacity`: The initial number of elements that the collection can contain.

- `DataTypeCollection(System.Collections.Generic.IEnumerable{DbExport.Schema.DataType})`

  Initializes a new instance of the DbExport.Schema.DataTypeCollection class with the specified collection of items.

  Parameters:
  - `dataTypes`: The collection of items to initialize the collection with.

### Type `ForeignKey` (type)

Represents a foreign key constraint in a database schema. This class provides details about the referenced table, the columns involved, and the actions to take on update or delete.

#### Properties

- `RelatedTableName`

  Gets the name of the referenced table.

- `RelatedTableOwner`

  Gets the owner of the referenced table.

- `RelatedTableFullName`

  Gets the fully qualified name of the referenced table.

- `RelatedColumnNames`

  Gets the names of the columns in the referenced table.

- `UpdateRule`

  Gets the action to take on update.

- `DeleteRule`

  Gets the action to take on delete.

- `RelatedTable`

  Gets a reference to the referenced table if loaded in the imported database schema, otherwise null.

#### Constructors

- `ForeignKey(DbExport.Schema.Table,System.String,System.Collections.Generic.IEnumerable{System.String},System.String,System.String,System.String[],DbExport.Schema.ForeignKeyRule,DbExport.Schema.ForeignKeyRule)`

  Initializes a new instance of the DbExport.Schema.ForeignKey class.

  Parameters:
  - `table`: The table that owns the foreign key.
  - `name`: The name of the foreign key constraint.
  - `columnNames`: The names of the columns that make up the foreign key.
  - `relatedName`: The name of the referenced table.
  - `relatedOwner`: The owner of the referenced table.
  - `relatedColumns`: The names of the columns in the referenced table.
  - `updateRule`: The action to take on update.
  - `deleteRule`: The action to take on delete.

#### Methods

- `GetRelatedColumn(System.Int32)`

  Retrieves the related column based on the specified index in the foreign key relationship.

  Parameters:
  - `i`: The zero-based index of the related column in the foreign key relationship.

  Returns: The DbExport.Schema.Column object corresponding to the specified index, or null if the related table or column is not found.

### Type `ForeignKeyCollection` (type)

Represents a collection of database foreign keys.

#### Constructors

- `ForeignKeyCollection`

  Initializes a new instance of the DbExport.Schema.ForeignKeyCollection class.

- `ForeignKeyCollection(System.Int32)`

  Initializes a new instance of the DbExport.Schema.ForeignKeyCollection class with the specified initial capacity.

  Parameters:
  - `capacity`: The initial number of elements that the collection can contain.

- `ForeignKeyCollection(System.Collections.Generic.IEnumerable{DbExport.Schema.ForeignKey})`

  Initializes a new instance of the DbExport.Schema.ForeignKeyCollection class with the specified collection of items.

  Parameters:
  - `foreignKeys`: The collection of items to initialize the collection with.

### Type `ForeignKeyRule` (type)

Defines the actions to be taken when a foreign key constraint is violated or when referenced data is modified or deleted.

#### Fields

- `None`

  No action is taken.

- `Restrict`

  Specifies that the action is restricted, meaning the operation causing the foreign key constraint violation cannot proceed.

- `Cascade`

  Specifies that the action is cascaded, meaning that the operation causing the foreign key constraint violation will be propagated to the referenced table.

- `SetNull`

  Specifies that the action is set null, meaning that the foreign key column value will be set to NULL.

- `SetDefault`

  Specifies that the action is set default, meaning that the foreign key column value will be set to the default value of the referenced column.

### Type `ICheckable` (type)

Represents an item that can be checked or unchecked.

#### Properties

- `IsChecked`

  Gets or sets a value indicating whether the item is checked.

### Type `IDataItem` (type)

Represents a data item that encapsulates schema-related properties and metadata for a database column or type definition.

#### Properties

- `ColumnType`

  Gets the name of the data item.

- `NativeType`

  Gets the native type of the data item.

- `Size`

  Gets the size (or character length) of the data item.

- `Precision`

  Gets the decimal precision of the data item.

- `Scale`

  Gets the decimal scale of the data item.

- `IsRequired`

  Gets a value indicating whether the data item is nullable.

- `DefaultValue`

  Gets the default value of the data item.

#### Methods

- `GetFullTypeName(DbExport.Schema.IDataItem,System.Boolean)`

  Generates the full type name representation for the provided data item.

  Parameters:
  - `item`: The data item for which the type name needs to be generated.
  - `native`: Determines whether to use the native database type or map to a generic type representation. Defaults to true.

  Returns: The full type name, including size, precision, and scale if applicable.

### Type `Index` (type)

Represents a database index associated with a table. An index is used to enhance the performance of database queries by providing quick access to rows in a table based on the values of one or more columns.

#### Properties

- `IsUnique`

  Gets a value indicating whether the index is unique.

- `IsPrimaryKey`

  Gets a value indicating whether the index is a primary key.

- `MatchesPrimaryKey`

  Gets a value indicating whether the index matches a primary key constraint.

- `MatchesForeignKey`

  Gets a value indicating whether the index matches a foreign key constraint.

- `MatchesKey`

  Gets a value indicating whether the index matches a primary or foreign key constraint.

#### Constructors

- `Index(DbExport.Schema.Table,System.String,System.Collections.Generic.IEnumerable{System.String},System.Boolean,System.Boolean)`

  Initializes a new instance of the DbExport.Schema.Index class.

  Parameters:
  - `table`: The table that owns the index.
  - `name`: The name of the index.
  - `columnNames`: The names of the columns that make up the index.
  - `unique`: Whether the index is unique.
  - `primaryKey`: Whether the index is a primary key.

### Type `IndexCollection` (type)

Represents a collection of database indexes.

#### Constructors

- `IndexCollection`

  Initializes a new instance of the DbExport.Schema.IndexCollection class.

- `IndexCollection(System.Int32)`

  Initializes a new instance of the DbExport.Schema.IndexCollection class with the specified initial capacity.

  Parameters:
  - `capacity`: The initial number of elements that the collection can contain.

- `IndexCollection(System.Collections.Generic.IEnumerable{DbExport.Schema.Index})`

  Initializes a new instance of the DbExport.Schema.IndexCollection class with the specified collection of items.

  Parameters:
  - `indexes`: The collection of items to initialize the collection with.

### Type `Key` (type)

Represents an abstract base class for database keys, such as primary keys, foreign keys, and indexes. Provides functionality to manage columns associated with the key and to compare key signatures.

#### Properties

- `Table`

  Gets the table that owns the key.

#### Constructors

- `Key(DbExport.Schema.Table,System.String,System.Collections.Generic.IEnumerable{System.String})`

  Initializes a new instance of the DbExport.Schema.Key class.

  Parameters:
  - `table`: The table that owns the key.
  - `name`: The name of the key.
  - `columnNames`: The names of the columns that make up the key.

#### Methods

- `MatchesSignature(DbExport.Schema.Key)`

  Determines whether the current key matches the signature of another key. A signature match occurs when both keys have the same number of columns and the columns have identical names in the same order.

  Parameters:
  - `other`: The key to compare against the current key.

  Returns: `true` if the current key matches the signature of the specified key; otherwise, `false`.

### Type `PrimaryKey` (type)

Represents the primary key of a database table. A primary key uniquely identifies each record in the table and enforces entity integrity within the database schema.

#### Properties

- `IsComputed`

  Gets a value indicating whether the primary key is computed.

- `IsIdentity`

  Gets a value indicating whether the primary key is an identity column.

#### Constructors

- `PrimaryKey(DbExport.Schema.Table,System.String,System.Collections.Generic.IEnumerable{System.String})`

  Initializes a new instance of the DbExport.Schema.PrimaryKey class.

  Parameters:
  - `table`: The table that owns the primary key.
  - `name`: The name of the primary key constraint.
  - `columnNames`: The names of the columns that make up the primary key.

### Type `SchemaItem` (type)

Represents a base class for all schema items.

#### Properties

- `Parent`

  Gets a reference to the parent schema item.

- `Name`

  Gets the name of the schema item.

- `FullName`

  Gets the fully qualified name of the schema item, which may include the owner or other contextual information, depending on the implementation of the derived class.

#### Constructors

- `SchemaItem(DbExport.Schema.SchemaItem,System.String)`

  Initializes a new instance of the DbExport.Schema.SchemaItem class.

  Parameters:
  - `parent`: A reference to the parent schema item.
  - `name`: The name of the schema item. Should not be null or whitespace.

### Type `SchemaItemCollection`1` (type)

Represents a collection of strongly typed schema items, providing functionality for indexing, addition, removal, and lookup based on item names. This collection is intended to manage schema items that inherit from the DbExport.Schema.SchemaItem class.

Type Parameters:
- `TItem`: The specific type of schema items contained in the collection.

#### Fields

- `dictionary`

  Represents a private dictionary that serves as a lookup mechanism for storing and retrieving schema items within the DbExport.Schema.SchemaItemCollection`1. The dictionary maps item names to their corresponding instances to enable efficient access.

#### Properties

- `Item(System.String)`

  Provides indexed access to schema items in the collection by their unique names. The indexer performs a lookup in the internal dictionary to retrieve the schema item that matches the given name.

  Parameters:
  - `name`: The unique name of the schema item to retrieve.

  Returns: The schema item associated with the specified name.

  Throws:
  - `System.Collections.Generic.KeyNotFoundException`: Thrown if the specified name does not exist in the collection.

#### Constructors

- `SchemaItemCollection`1`

  Initializes a new instance of the DbExport.Schema.SchemaItemCollection`1 class.

- `SchemaItemCollection`1(System.Int32)`

  Initializes a new instance of the DbExport.Schema.SchemaItemCollection`1 class with the specified initial capacity.

  Parameters:
  - `capacity`: The initial number of elements that the collection can contain.

- `SchemaItemCollection`1(System.Collections.Generic.IEnumerable{`0})`

  Initializes a new instance of the DbExport.Schema.SchemaItemCollection`1 class with the specified collection of items.

  Parameters:
  - `items`: The collection of items to initialize the collection with.

#### Methods

- `TryGetValue(System.String,`0@)`

  Attempts to retrieve the schema item associated with the specified name from the collection.

  Parameters:
  - `name`: The name of the schema item to find in the collection.
  - `item`: When this method returns, contains the schema item associated with the specified name, if the name is found, or null if the name is not found. This parameter is passed uninitialized.

  Returns: `true` if the collection contains an item with the specified name; otherwise, `false`.

- `Add(`0)`

  Adds the specified item to the collection.

  Parameters:
  - `item`: The item to add to the collection. Must not be null and must have a unique full name.

  Throws:
  - `System.ArgumentException`: Thrown when an item with the same full name already exists in the collection.
  - `System.ArgumentNullException`: Thrown when the provided item is null.

- `AddRange(System.Collections.Generic.IEnumerable{`0})`

  Adds a range of items to the collection.

  Parameters:
  - `items`: An enumerable collection of items to add to the collection. Each item must not be null and must have a unique full name.

  Throws:
  - `System.ArgumentException`: Thrown when one or more items in the collection have full names that already exist in the collection.
  - `System.ArgumentNullException`: Thrown when the provided collection or any item in it is null.

- `Contains(System.String)`

  Determines whether the collection contains an item with the specified name.

  Parameters:
  - `name`: The name of the schema item to locate in the collection.

  Returns: `true` if an item with the specified name is found in the collection; otherwise, `false`.

- `IndexOf(System.String)`

  Returns the zero-based index of the schema item with the specified name within the collection.

  Parameters:
  - `name`: The name of the schema item to locate in the collection.

  Returns: The zero-based index of the schema item if found in the collection; otherwise, -1.

- `Remove(`0)`

  Removes the specified schema item from the collection.

  Parameters:
  - `item`: The schema item to remove from the collection. This item must exist within the collection.

  Returns: `true` if the specified schema item was successfully removed from the collection; otherwise, `false`.

- `Remove(System.String)`

  Removes a schema item from the collection by its name.

  Parameters:
  - `name`: The name of the schema item to remove from the collection. The item must exist within the collection.

  Returns: `true` if the schema item with the specified name was successfully removed from the collection; otherwise, `false`.

- `RemoveRange(System.Int32,System.Int32)`

  Removes a range of schema items from the collection starting at the specified index.

  Parameters:
  - `index`: The zero-based starting index of the range of elements to remove. Must be within the bounds of the collection.
  - `count`: The number of items to remove starting from the specified index. Must be non-negative and not exceed the available items from the index.

- `Clear`

  Removes all items from the DbExport.Schema.SchemaItemCollection`1 and clears the internal dictionary used for name-based lookups.

### Type `Table` (type)

Represents a database table, which is a specialized DbExport.Schema.ColumnSet containing columns, constraints, and relationships to other tables.

#### Properties

- `Owner`

  The owner of the table.

- `PrimaryKey`

  The primary key of the table.

- `Indexes`

  The indexes of the table.

- `ForeignKeys`

  The foreign keys of the table.

- `Database`

  The database that owns the table.

- `HasPrimaryKey`

  Indicates whether the table has a primary key.

- `HasIndex`

  Indicates whether the table has an index.

- `HasForeignKey`

  Indicates whether the table has a foreign key.

- `NonPKColumns`

  Gets the columns of the table that are not primary key columns.

- `NonFKColumns`

  Gets the columns of the table that are not foreign key columns.

- `NonKeyColumns`

  Gets the columns of the table that are neither primary key nor foreign key columns.

- `ReferencedTables`

  Gets a collection of tables that are referenced by foreign keys in this table.

- `ReferencingTables`

  Gets a collection of tables that are referencing this table through foreign keys.

#### Constructors

- `Table(DbExport.Schema.Database,System.String,System.String)`

  Represents a database table, which is a specialized DbExport.Schema.ColumnSet containing columns, constraints, and relationships to other tables.

  Parameters:
  - `db`: The database that owns the table.
  - `name`: The name of the table.
  - `owner`: The owner of the table.

#### Methods

- `GeneratePrimaryKey(System.String,System.Collections.Generic.IEnumerable{System.String})`

  Creates a primary key for the table using the specified name and column names.

  Parameters:
  - `name`: The name of the primary key to be created.
  - `columnNames`: A collection of column names that will be included in the primary key.

- `GetReferencingKey(DbExport.Schema.Table)`

  Retrieves the foreign key in the current table that references the specified table.

  Parameters:
  - `table`: The table being referenced by the foreign key.

  Returns: The foreign key that references the specified table, or null if no such key exists.

- `IsAssociationTable`

  Determines whether the table is an association table. An association table is identified as having more than one referenced table, and all its columns are either foreign key columns or generated columns.

  Returns: True if the table is an association table; otherwise, false.

### Type `TableCollection` (type)

Represents a collection of database tables.

#### Constructors

- `TableCollection`

  Initializes a new instance of the DbExport.Schema.TableCollection class.

- `TableCollection(System.Int32)`

  Initializes a new instance of the DbExport.Schema.TableCollection class with the specified initial capacity.

  Parameters:
  - `capacity`: The initial number of elements that the collection can contain.

- `TableCollection(System.Collections.Generic.IEnumerable{DbExport.Schema.Table})`

  Initializes a new instance of the DbExport.Schema.TableCollection class with the specified collection of items.

  Parameters:
  - `tables`: The collection of items to initialize the collection with.

### Type `QueryOptions` (type)

Specifies options for customizing the behavior of query generation in database operations. These options determine how tables and columns are processed and formatted during SQL generation.

#### Fields

- `None`

  Indicates that no query options are applied. This is the default value, meaning that all applicable table columns and default behaviors are included during SQL generation without exclusions or modifications.

- `SkipIdentity`

  Indicates that identity columns are excluded during query generation. This option prevents the inclusion of columns that are marked as identity fields, typically used for automatically generated values in database tables.

- `SkipComputed`

  Specifies that computed columns are excluded during query generation. This option removes columns with calculated values, such as those defined with SQL expressions, from the generated SQL statement.

- `SkipRowVersion`

  Indicates that columns marked with the RowVersion property should be excluded from the query generation process. This option is used to prevent inclusion of RowVersion columns, which are typically used for concurrency tracking and may not be necessary in certain queries or export scenarios.

- `SkipGenerated`

  Represents a combination of query options that excludes identity, computed, and row version columns during SQL generation. This option is used to skip all columns that are automatically generated by the database, ensuring they are omitted from the resulting query.

- `QualifyTableName`

  Specifies that fully qualified table names, including schema names, should be used during SQL generation. This ensures that table references are explicit, which can help avoid ambiguities when working with multiple schemas or databases.

- `All`

  Represents a combination of all available query options. This includes skipping identity, computed, and row version columns during query generation, as well as qualifying table names with schema information, ensuring comprehensive customization of the query output.

### Type `TableExtensions` (type)

Contains extension methods for performing various database operations on a DbExport.Schema.Table object. These operations include generating SQL statements (SELECT, INSERT, UPDATE, DELETE), executing queries, and managing batch operations.

#### Methods

- `GenerateSelect(DbExport.Schema.Table,DbExport.Schema.QueryOptions)`

  Generates a SQL SELECT statement for the specified table based on the provided query options.

  Parameters:
  - `table`: The table for which the SELECT statement will be generated.
  - `options`: The query options that define which columns to include and how the table name should be qualified.

  Returns: A string containing the generated SQL SELECT statement.

- `GenerateSelect(DbExport.Schema.Table,DbExport.Schema.Key,DbExport.Schema.QueryOptions)`

  Generates a SQL SELECT statement for the specified table, filtering results based on the given key and applying the provided query options.

  Parameters:
  - `table`: The table for which the SELECT statement will be generated.
  - `key`: The key defining the filter criteria for the SELECT statement.
  - `options`: The query options that specify how the statement should be generated, including conditions and table name qualification.

  Returns: A string containing the generated SQL SELECT statement with the applied key filter.

- `GenerateInsert(DbExport.Schema.Table,System.String,DbExport.Schema.QueryOptions)`

  Generates a SQL INSERT statement for the specified table based on the provided query options.

  Parameters:
  - `table`: The table for which the INSERT statement will be generated.
  - `providerName`: The name of the database provider used to format the SQL syntax.
  - `options`: The query options that determine which columns to include and whether to qualify the table name.

  Returns: A string containing the generated SQL INSERT statement.

- `GenerateUpdate(DbExport.Schema.Table,System.String,DbExport.Schema.QueryOptions)`

  Generates a SQL UPDATE statement for the specified table, including the columns to update and the primary key for filtering.

  Parameters:
  - `table`: The table for which the UPDATE statement will be generated.
  - `providerName`: The name of the database provider being used, which determines SQL syntax specifics.
  - `options`: The query options that define which columns to include and whether to qualify the table name.

  Returns: A string containing the generated SQL UPDATE statement.

- `GenerateDelete(DbExport.Schema.Table,System.String,DbExport.Schema.QueryOptions)`

  Generates a SQL DELETE statement for the specified table using the given provider and query options.

  Parameters:
  - `table`: The table for which the DELETE statement will be generated.
  - `providerName`: The name of the database provider, used to ensure compatibility when generating the SQL statement.
  - `options`: The query options that specify how the table name should be qualified and determine whether certain table features should be skipped.

  Returns: A string containing the generated SQL DELETE statement.

- `IsMatch(DbExport.Schema.Column,DbExport.Schema.QueryOptions)`

  Determines whether a column matches the specified query options.

  Parameters:
  - `c`: The column to evaluate against the query options.
  - `o`: The query options that define the criteria for a column to match.

  Returns: A boolean value indicating whether the column meets the specified options.

- `AppendNameOf(System.Text.StringBuilder,DbExport.Schema.Table,System.String,DbExport.Schema.QueryOptions)`

  Appends a fully qualified or unqualified table name to the provided System.Text.StringBuilder instance based on the specified query options.

  Parameters:
  - `sb`: The System.Text.StringBuilder instance to which the table name will be appended.
  - `table`: The DbExport.Schema.Table whose name will be appended.
  - `providerName`: The name of the database provider, used to escape identifiers.
  - `options`: The query options that determine whether the table name should be qualified with its owner.

  Returns: The System.Text.StringBuilder instance with the appended table name.

- `AppendFilterBy(System.Text.StringBuilder,DbExport.Schema.Key,System.String)`

  Appends a SQL WHERE clause to the provided StringBuilder instance using the specified key columns and provider-specific syntax for a database.

  Parameters:
  - `sb`: The StringBuilder instance to which the WHERE clause will be appended.
  - `key`: The key object containing the columns to be included in the filter.
  - `providerName`: The name of the database provider to ensure provider-specific escaping and parameter naming conventions.

  Returns: The updated StringBuilder instance containing the appended WHERE clause.

- `OpenReader(DbExport.Schema.Table,DbExport.Schema.QueryOptions)`

  Opens a data reader for the specified table using the provided query options.

  Parameters:
  - `table`: The table for which the data reader will be opened.
  - `options`: The query options that define how the SELECT statement should be generated.

  Returns: A System.Data.Common.DbDataReader instance to read the rows retrieved from the specified table. The connection will automatically close when the reader is disposed.

- `CopyTo(DbExport.Schema.Table,System.Data.Common.DbConnection,DbExport.Schema.QueryOptions,DbExport.Schema.QueryOptions)`

  Copies data from the specified table to the target database connection using the provided query options.

  Parameters:
  - `table`: The table from which data will be copied.
  - `targetConnection`: The target database connection where the data will be inserted.
  - `sourceOptions`: The query options that define how data is read from the source table.
  - `targetOptions`: The query options that define how data is inserted into the target database.

- `Select``1(DbExport.Schema.Table,System.Func{System.Data.Common.DbDataReader,``0})`

  Executes a SQL SELECT query on the specified table and processes the result set using the provided extractor function.

  Type Parameters:
  - `TRowSet`: The type of the result set that will be created from the query.

  Parameters:
  - `table`: The table on which the SELECT query will be executed.
  - `extractor`: A function that processes the data reader and extracts the result set.

  Returns: The result set created by the extractor function based on the query output.

- `Select``1(DbExport.Schema.Table)`

  Returns a list of entities of the specified type, mapped from the rows in the table.

  Type Parameters:
  - `TRow`: The type of entity to which each row in the table will be mapped. Must be a class with a parameterless constructor.

  Parameters:
  - `table`: The table from which data will be selected and mapped to entities.

  Returns: A list of entities of the specified type, representing the data from the table.

- `Select``2(DbExport.Schema.Table,DbExport.Schema.Key,``1,System.Action{System.Data.Common.DbCommand,``1},System.Func{System.Data.Common.DbDataReader,``0})`

  Executes a SQL SELECT query for the specified table and key and maps the result set to a custom type using the provided key binder and data extraction function.

  Type Parameters:
  - `TRowSet`: The type into which the query results will be mapped.
  - `TKey`: The type of the key value used for query filtering.

  Parameters:
  - `table`: The table for which the SELECT query will be executed.
  - `key`: The key used to filter the query results.
  - `keyValue`: The value of the key to use with the SELECT query.
  - `keyBinder`: A function that binds the key value to the database command parameters.
  - `extractor`: A function that extracts data from the query results and maps it to the specified type.

  Returns: An instance of TRowSet containing the extracted query results.

- `Select``1(DbExport.Schema.Table,DbExport.Schema.Key,System.Object[])`

  Retrieves a list of entities of type TRow from the specified table based on the provided key and key values.

  Type Parameters:
  - `TRow`: The entity type of the rows to be retrieved. Must be a class with a parameterless constructor.

  Parameters:
  - `table`: The table from which to select rows.
  - `key`: The key that defines the criteria used for selection.
  - `keyValues`: The values of the key to be used as the selection criteria.

  Returns: A list of entities of type TRow representing the selected rows.

- `Insert``1(DbExport.Schema.Table,``0,System.Action{System.Data.Common.DbCommand,``0})`

  Inserts a new row into the specified table using the provided row value and a custom row binder action.

  Type Parameters:
  - `TRow`: The type of the row object to be inserted.

  Parameters:
  - `table`: The table where the row will be inserted.
  - `rowValue`: The value of the row to insert.
  - `rowBinder`: A delegate that binds the row value to a database command.

  Returns: A boolean indicating whether the insertion was successful.

- `Insert``1(DbExport.Schema.Table,``0)`

  Inserts a row into the specified table using the provided row data and a default row binder.

  Type Parameters:
  - `TRow`: The type of the row data to be inserted, which must be a class with a parameterless constructor.

  Parameters:
  - `table`: The table into which the row will be inserted.
  - `rowValue`: The data for the row to be inserted.

  Returns: A boolean indicating whether the row was successfully inserted.

- `Insert(DbExport.Schema.Table,System.Object[])`

  Inserts a new row into the specified table using the provided values.

  Parameters:
  - `table`: The table where the data will be inserted.
  - `rowValues`: An array of values representing the data for the row to be inserted.

  Returns: A boolean value indicating whether the insert operation was successful.

- `Update``1(DbExport.Schema.Table,``0,System.Action{System.Data.Common.DbCommand,``0})`

  Executes an update operation on the specified table using the given row value and a binding action to populate parameter values.

  Type Parameters:
  - `TRow`: The type of the row value being updated.

  Parameters:
  - `table`: The table on which the update operation will be executed.
  - `rowValue`: The row data containing the values to be updated.
  - `rowBinder`: An action that binds the provided row data to the parameters of the database command.

  Returns: A boolean value indicating whether the update operation affected one or more rows.

- `Update``1(DbExport.Schema.Table,``0)`

  Updates the specified table with the provided row data.

  Type Parameters:
  - `TRow`: The type of the row data. Must be a reference type with a parameterless constructor.

  Parameters:
  - `table`: The table to be updated.
  - `rowValue`: The object containing the data to update the table with.

  Returns: A boolean indicating whether the update operation was successful.

- `Delete``1(DbExport.Schema.Table,``0,System.Action{System.Data.Common.DbCommand,``0})`

  Deletes a record from the specified table based on the provided key value and binds the key using the given key binder.

  Type Parameters:
  - `TKey`: The type of the key used to identify the record to delete.

  Parameters:
  - `table`: The table from which the record will be deleted.
  - `keyValue`: The key value identifying the record to delete.
  - `keyBinder`: A delegate that binds the key value to a database command.

  Returns: A boolean indicating whether the record was successfully deleted.

- `Delete``1(DbExport.Schema.Table,``0)`

  Deletes a record from the specified table using the provided key value and binds the key to the using a default key binder.

  Type Parameters:
  - `TKey`: The type of the key value used to identify the record to delete.

  Parameters:
  - `table`: The table from which the record will be deleted.
  - `keyValue`: The key value of the record to be deleted.

  Returns: A boolean value indicating whether the deletion was successful.

- `Delete(DbExport.Schema.Table,System.Object[])`

  Deletes a row from the specified table using the provided key values to identify the target row.

  Parameters:
  - `table`: The table from which the row will be deleted.
  - `keyValues`: An array of key values used to locate the row to delete.

  Returns: A boolean value indicating whether the deletion was successful.

- `InsertBatch``1(DbExport.Schema.Table,System.Collections.Generic.IEnumerable{``0},System.Action{System.Data.Common.DbCommand,``0})`

  Inserts a batch of rows into the specified table using the provided binding logic to map rows to database parameters.

  Type Parameters:
  - `TRow`: The type of the data rows to be inserted.

  Parameters:
  - `table`: The target table into which the rows will be inserted.
  - `rowValues`: The collection of data rows to insert into the table.
  - `rowBinder`: The callback function responsible for binding each row to the database command parameters.

  Returns: A boolean value indicating whether the batch insert operation affected any rows.

- `InsertBatch``1(DbExport.Schema.Table,System.Collections.Generic.IEnumerable{``0})`

  Inserts a batch of rows into the specified table using the provided row values and a default row binding implementation.

  Type Parameters:
  - `TRow`: The type of the rows being inserted. Must be a class and have a parameterless constructor.

  Parameters:
  - `table`: The table into which the rows will be inserted.
  - `rowValues`: The collection of row values to be inserted into the table.

  Returns: True if the batch insert operation succeeds; otherwise, false.

- `UpdateBatch``1(DbExport.Schema.Table,System.Collections.Generic.IEnumerable{``0},System.Action{System.Data.Common.DbCommand,``0})`

  Updates a batch of rows in the specified table using the provided row values and a function to bind parameter values to the command.

  Type Parameters:
  - `TRow`: The type of the objects representing the rows to be updated.

  Parameters:
  - `table`: The table where the rows will be updated.
  - `rowValues`: The collection of row values to update the table with.
  - `rowBinder`: The function that binds parameter values to the database command object for each row.

  Returns: A boolean value indicating whether the batch update affected any rows.

- `UpdateBatch``1(DbExport.Schema.Table,System.Collections.Generic.IEnumerable{``0})`

  Updates a batch of rows in the specified table using the provided collection of row values and a default entity-to-command binder.

  Type Parameters:
  - `TRow`: The type of the rows in the collection, which must be a reference type with a parameterless constructor.

  Parameters:
  - `table`: The table where the batch update will be applied.
  - `rowValues`: The collection of row values to be updated in the table.

  Returns: True if the batch update is successful; otherwise, false.

- `DeleteBatch``1(DbExport.Schema.Table,System.Collections.Generic.IEnumerable{``0},System.Action{System.Data.Common.DbCommand,``0})`

  Executes a batch delete operation for the specified table based on a collection of key values.

  Type Parameters:
  - `TKey`: The type of the key values used to identify the records to delete.

  Parameters:
  - `table`: The table from which records will be deleted.
  - `keyValues`: The collection of key values representing the records to delete.
  - `keyBinder`: A callback action to bind the key value to the database command parameters.

  Returns: A boolean value indicating whether the operation affected any records.

- `DeleteBatch``1(DbExport.Schema.Table,System.Collections.Generic.IEnumerable{``0})`

  Deletes a batch of records from the specified table based on the provided key values.

  Type Parameters:
  - `TKey`: The type of the key used to identify records for deletion.

  Parameters:
  - `table`: The table from which the records will be deleted.
  - `keyValues`: A collection of key values identifying the records to be deleted.

  Returns: A boolean indicating whether the operation was successful.

## Namespace `DbExport.SqlHelper`

### Type `ParameterParser` (type)

Provides methods for parsing SQL queries and extracting parameter names. This utility is used to identify and process parameter placeholders within a given SQL string while handling edge cases such as string literals, quoted identifiers, and comments.

#### Methods

- `Extract(System.String)`

  Extracts parameter names from the specified SQL string by identifying and parsing parameter placeholders. Handles SQL syntax rules such as skipping string literals, quoted identifiers, and comments.

  Parameters:
  - `sql`: The SQL string to parse for parameter placeholders.

  Returns: A read-only list of unique parameter names found in the SQL string.

- `IsParamPrefix(System.Char)`

  Determines whether the specified character is a valid prefix for a SQL parameter.

  Parameters:
  - `c`: The character to evaluate as a potential parameter prefix.

  Returns: True if the character is a valid parameter prefix; otherwise, false.

- `IsParamChar(System.Char)`

  Determines whether the specified character is valid as part of a SQL parameter name.

  Parameters:
  - `c`: The character to evaluate.

  Returns: True if the character is a letter, digit, or underscore; otherwise, false.

- `SkipSingleQuote(System.String,System.Int32,System.Int32)`

  Skips over a single-quoted string literal in a SQL string, starting at the given index. Handles escaped single quotes by advancing the index past them.

  Parameters:
  - `sql`: The SQL string being processed.
  - `i`: The current index within the SQL string where the single quote starts.
  - `limit`: The length of the SQL string, used as a boundary for the processing loop.

  Returns: The index immediately following the closing single quote, or the last index of the string if no matching quote is found.

- `SkipDoubleQuote(System.String,System.Int32,System.Int32)`

  Skips over a quoted identifier in the SQL string, starting at the specified index. Advances the index to the position after the closing double-quote or to the end of the string if no closing double-quote is found.

  Parameters:
  - `sql`: The SQL string being parsed.
  - `i`: The current position in the SQL string where the quoted identifier begins, including the opening double-quote.
  - `limit`: The maximum index to evaluate within the SQL string.

  Returns: The index of the closing double-quote or the index of the last evaluable character if the closing double-quote is not found.

- `SkipLineComment(System.String,System.Int32,System.Int32)`

  Skips a line comment within the SQL string, advancing the index to the end of the comment. Line comments are identified as sequences starting with "--" and ending at the next newline character or the end of the string.

  Parameters:
  - `sql`: The SQL string containing the line comment to skip.
  - `i`: The current index in the SQL string, positioned at the start of the line comment.
  - `limit`: The length of the SQL string to determine the upper bounds for processing.

  Returns: The updated index positioned at the end of the line comment or the end of the string, whichever comes first.

- `SkipBlockComment(System.String,System.Int32,System.Int32)`

  Skips a block comment in the SQL string and returns the updated index after the block comment ends. This method is called when a block comment is detected and ensures that parsing continues from the correct position after the comment.

  Parameters:
  - `sql`: The SQL string being parsed.
  - `i`: The current index in the SQL string where the block comment starts.
  - `limit`: The length of the SQL string, used as the upper boundary for parsing.

  Returns: The index immediately after the end of the block comment, or the last index of the SQL string if the comment is unclosed.

## Namespace `System.Text.RegularExpressions.Generated`

### Type `CreateDbRegex_0` (type)

Custom System.Text.RegularExpressions.Regex-derived type for the CreateDbRegex method.

#### Fields

- `Instance`

  Cached, thread-safe singleton instance.

#### Constructors

- `CreateDbRegex_0`

  Initializes the instance.

### Type `DelimiterRegex_1` (type)

Custom System.Text.RegularExpressions.Regex-derived type for the DelimiterRegex method.

#### Fields

- `Instance`

  Cached, thread-safe singleton instance.

#### Constructors

- `DelimiterRegex_1`

  Initializes the instance.

### Type `UserTypeRegex_2` (type)

Custom System.Text.RegularExpressions.Regex-derived type for the UserTypeRegex method.

#### Fields

- `Instance`

  Cached, thread-safe singleton instance.

#### Constructors

- `UserTypeRegex_2`

  Initializes the instance.

### Type `Utf8Regex_3` (type)

Custom System.Text.RegularExpressions.Regex-derived type for the Utf8Regex method.

#### Fields

- `Instance`

  Cached, thread-safe singleton instance.

#### Constructors

- `Utf8Regex_3`

  Initializes the instance.

### Type `CreateDbRegex_4` (type)

Custom System.Text.RegularExpressions.Regex-derived type for the CreateDbRegex method.

#### Fields

- `Instance`

  Cached, thread-safe singleton instance.

#### Constructors

- `CreateDbRegex_4`

  Initializes the instance.

### Type `CommaRegex_5` (type)

Custom System.Text.RegularExpressions.Regex-derived type for the CommaRegex method.

#### Fields

- `Instance`

  Cached, thread-safe singleton instance.

#### Constructors

- `CommaRegex_5`

  Initializes the instance.

### Type `CreateDbRegex_6` (type)

Custom System.Text.RegularExpressions.Regex-derived type for the CreateDbRegex method.

#### Fields

- `Instance`

  Cached, thread-safe singleton instance.

#### Constructors

- `CreateDbRegex_6`

  Initializes the instance.

### Type `UniqueRegex_7` (type)

Custom System.Text.RegularExpressions.Regex-derived type for the UniqueRegex method.

#### Fields

- `Instance`

  Cached, thread-safe singleton instance.

#### Constructors

- `UniqueRegex_7`

  Initializes the instance.

### Type `PrimaryKeyRegex_8` (type)

Custom System.Text.RegularExpressions.Regex-derived type for the PrimaryKeyRegex method.

#### Fields

- `Instance`

  Cached, thread-safe singleton instance.

#### Constructors

- `PrimaryKeyRegex_8`

  Initializes the instance.

### Type `DelimiterRegex_9` (type)

Custom System.Text.RegularExpressions.Regex-derived type for the DelimiterRegex method.

#### Fields

- `Instance`

  Cached, thread-safe singleton instance.

#### Constructors

- `DelimiterRegex_9`

  Initializes the instance.

### Type `CreateDbRegex_10` (type)

Custom System.Text.RegularExpressions.Regex-derived type for the CreateDbRegex method.

#### Fields

- `Instance`

  Cached, thread-safe singleton instance.

#### Constructors

- `CreateDbRegex_10`

  Initializes the instance.

### Type `CreateTypeRegex_11` (type)

Custom System.Text.RegularExpressions.Regex-derived type for the CreateTypeRegex method.

#### Fields

- `Instance`

  Cached, thread-safe singleton instance.

#### Constructors

- `CreateTypeRegex_11`

  Initializes the instance.

### Type `PasswordRegex_12` (type)

Custom System.Text.RegularExpressions.Regex-derived type for the PasswordRegex method.

#### Fields

- `Instance`

  Cached, thread-safe singleton instance.

#### Constructors

- `PasswordRegex_12`

  Initializes the instance.

### Type `Utilities` (type)

Helper methods used by generated System.Text.RegularExpressions.Regex-derived implementations.

#### Fields

- `s_defaultTimeout`

  Default timeout value set in System.AppContext, or System.Text.RegularExpressions.Regex.InfiniteMatchTimeout if none was set.

- `s_hasTimeout`

  Whether System.Text.RegularExpressions.Generated.Utilities.s_defaultTimeout is non-infinite.

- `WordCategoriesMask`

  Provides a mask of Unicode categories that combine to form [\w].

- `s_indexOfAnyStrings_OrdinalIgnoreCase_21A67AE61D9FA8FB1383A27A7C869F98EABAB8FE6CC6E8623DBE6F2E8E80913C`

  Supports searching for the specified strings.

- `s_indexOfAnyStrings_OrdinalIgnoreCase_44ABEE92F74F079CED68BEA6C00F04F60BAEF113EBEDDFF15724049297685A23`

  Supports searching for the specified strings.

- `s_indexOfString_993F6C5A9B3E9856C185C6E593F2739E6B0B201BACB3493C1EEFA8D46F147512`

  Supports searching for the string "primary key".

- `s_indexOfString_create_OrdinalIgnoreCase`

  Supports searching for the string "create".

- `s_indexOfString_unique_Ordinal`

  Supports searching for the string "unique".

- `s_indexOfString_utf8_OrdinalIgnoreCase`

  Supports searching for the string "utf8".

#### Properties

- `WordCharBitmap`

  Gets a bitmap for whether each character 0 through 127 is in [\w]

#### Methods

- `IsBoundaryWordChar(System.Char)`

  Determines whether the specified index is a boundary word character.

- `IsPostWordCharBoundary(System.ReadOnlySpan{System.Char},System.Int32)`

  Determines whether the specified index is a boundary.

- `IsPreWordCharBoundary(System.ReadOnlySpan{System.Char},System.Int32)`

  Determines whether the specified index is a boundary.

- `IsWordChar(System.Char)`

  Determines whether the character is part of the [\w] set.

- `StackPush(System.Int32[]@,System.Int32@,System.Int32)`

  Pushes 1 value onto the backtracking stack.

## Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_0`

### Type `RunnerFactory` (type)

Provides a factory for creating System.Text.RegularExpressions.RegexRunner instances to be used by methods on System.Text.RegularExpressions.Regex.

#### Methods

- `CreateInstance`

  Creates an instance of a System.Text.RegularExpressions.RegexRunner used by methods on System.Text.RegularExpressions.Regex.

## Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_0.RunnerFactory`

### Type `Runner` (type)

Provides the runner that contains the custom logic implementing the specified regular expression.

#### Methods

- `Scan(System.ReadOnlySpan{System.Char})`

  Scan the inputSpan starting from base.runtextstart for the next match.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

- `TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})`

  Search inputSpan starting from base.runtextpos for the next location a match could possibly start.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if a possible match was found; false if no more matches are possible.

- `TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})`

  Determine whether inputSpan at base.runtextpos is a match for the regular expression.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if the regular expression matches at the current position; otherwise, false.

## Namespace `System.Text.RegularExpressions.Generated.DelimiterRegex_1`

### Type `RunnerFactory` (type)

Provides a factory for creating System.Text.RegularExpressions.RegexRunner instances to be used by methods on System.Text.RegularExpressions.Regex.

#### Methods

- `CreateInstance`

  Creates an instance of a System.Text.RegularExpressions.RegexRunner used by methods on System.Text.RegularExpressions.Regex.

## Namespace `System.Text.RegularExpressions.Generated.DelimiterRegex_1.RunnerFactory`

### Type `Runner` (type)

Provides the runner that contains the custom logic implementing the specified regular expression.

#### Methods

- `Scan(System.ReadOnlySpan{System.Char})`

  Scan the inputSpan starting from base.runtextstart for the next match.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

- `TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})`

  Search inputSpan starting from base.runtextpos for the next location a match could possibly start.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if a possible match was found; false if no more matches are possible.

- `TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})`

  Determine whether inputSpan at base.runtextpos is a match for the regular expression.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if the regular expression matches at the current position; otherwise, false.

## Namespace `System.Text.RegularExpressions.Generated.UserTypeRegex_2`

### Type `RunnerFactory` (type)

Provides a factory for creating System.Text.RegularExpressions.RegexRunner instances to be used by methods on System.Text.RegularExpressions.Regex.

#### Methods

- `CreateInstance`

  Creates an instance of a System.Text.RegularExpressions.RegexRunner used by methods on System.Text.RegularExpressions.Regex.

## Namespace `System.Text.RegularExpressions.Generated.UserTypeRegex_2.RunnerFactory`

### Type `Runner` (type)

Provides the runner that contains the custom logic implementing the specified regular expression.

#### Methods

- `Scan(System.ReadOnlySpan{System.Char})`

  Scan the inputSpan starting from base.runtextstart for the next match.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

- `TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})`

  Search inputSpan starting from base.runtextpos for the next location a match could possibly start.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if a possible match was found; false if no more matches are possible.

- `TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})`

  Determine whether inputSpan at base.runtextpos is a match for the regular expression.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if the regular expression matches at the current position; otherwise, false.

## Namespace `System.Text.RegularExpressions.Generated.Utf8Regex_3`

### Type `RunnerFactory` (type)

Provides a factory for creating System.Text.RegularExpressions.RegexRunner instances to be used by methods on System.Text.RegularExpressions.Regex.

#### Methods

- `CreateInstance`

  Creates an instance of a System.Text.RegularExpressions.RegexRunner used by methods on System.Text.RegularExpressions.Regex.

## Namespace `System.Text.RegularExpressions.Generated.Utf8Regex_3.RunnerFactory`

### Type `Runner` (type)

Provides the runner that contains the custom logic implementing the specified regular expression.

#### Methods

- `Scan(System.ReadOnlySpan{System.Char})`

  Scan the inputSpan starting from base.runtextstart for the next match.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

- `TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})`

  Search inputSpan starting from base.runtextpos for the next location a match could possibly start.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if a possible match was found; false if no more matches are possible.

- `TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})`

  Determine whether inputSpan at base.runtextpos is a match for the regular expression.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if the regular expression matches at the current position; otherwise, false.

## Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_4`

### Type `RunnerFactory` (type)

Provides a factory for creating System.Text.RegularExpressions.RegexRunner instances to be used by methods on System.Text.RegularExpressions.Regex.

#### Methods

- `CreateInstance`

  Creates an instance of a System.Text.RegularExpressions.RegexRunner used by methods on System.Text.RegularExpressions.Regex.

## Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_4.RunnerFactory`

### Type `Runner` (type)

Provides the runner that contains the custom logic implementing the specified regular expression.

#### Methods

- `Scan(System.ReadOnlySpan{System.Char})`

  Scan the inputSpan starting from base.runtextstart for the next match.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

- `TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})`

  Search inputSpan starting from base.runtextpos for the next location a match could possibly start.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if a possible match was found; false if no more matches are possible.

- `TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})`

  Determine whether inputSpan at base.runtextpos is a match for the regular expression.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if the regular expression matches at the current position; otherwise, false.

## Namespace `System.Text.RegularExpressions.Generated.CommaRegex_5`

### Type `RunnerFactory` (type)

Provides a factory for creating System.Text.RegularExpressions.RegexRunner instances to be used by methods on System.Text.RegularExpressions.Regex.

#### Methods

- `CreateInstance`

  Creates an instance of a System.Text.RegularExpressions.RegexRunner used by methods on System.Text.RegularExpressions.Regex.

## Namespace `System.Text.RegularExpressions.Generated.CommaRegex_5.RunnerFactory`

### Type `Runner` (type)

Provides the runner that contains the custom logic implementing the specified regular expression.

#### Methods

- `Scan(System.ReadOnlySpan{System.Char})`

  Scan the inputSpan starting from base.runtextstart for the next match.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

- `TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})`

  Search inputSpan starting from base.runtextpos for the next location a match could possibly start.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if a possible match was found; false if no more matches are possible.

- `TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})`

  Determine whether inputSpan at base.runtextpos is a match for the regular expression.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if the regular expression matches at the current position; otherwise, false.

## Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_6`

### Type `RunnerFactory` (type)

Provides a factory for creating System.Text.RegularExpressions.RegexRunner instances to be used by methods on System.Text.RegularExpressions.Regex.

#### Methods

- `CreateInstance`

  Creates an instance of a System.Text.RegularExpressions.RegexRunner used by methods on System.Text.RegularExpressions.Regex.

## Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_6.RunnerFactory`

### Type `Runner` (type)

Provides the runner that contains the custom logic implementing the specified regular expression.

#### Methods

- `Scan(System.ReadOnlySpan{System.Char})`

  Scan the inputSpan starting from base.runtextstart for the next match.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

- `TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})`

  Search inputSpan starting from base.runtextpos for the next location a match could possibly start.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if a possible match was found; false if no more matches are possible.

- `TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})`

  Determine whether inputSpan at base.runtextpos is a match for the regular expression.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if the regular expression matches at the current position; otherwise, false.

## Namespace `System.Text.RegularExpressions.Generated.UniqueRegex_7`

### Type `RunnerFactory` (type)

Provides a factory for creating System.Text.RegularExpressions.RegexRunner instances to be used by methods on System.Text.RegularExpressions.Regex.

#### Methods

- `CreateInstance`

  Creates an instance of a System.Text.RegularExpressions.RegexRunner used by methods on System.Text.RegularExpressions.Regex.

## Namespace `System.Text.RegularExpressions.Generated.UniqueRegex_7.RunnerFactory`

### Type `Runner` (type)

Provides the runner that contains the custom logic implementing the specified regular expression.

#### Methods

- `Scan(System.ReadOnlySpan{System.Char})`

  Scan the inputSpan starting from base.runtextstart for the next match.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

- `TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})`

  Search inputSpan starting from base.runtextpos for the next location a match could possibly start.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if a possible match was found; false if no more matches are possible.

- `TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})`

  Determine whether inputSpan at base.runtextpos is a match for the regular expression.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if the regular expression matches at the current position; otherwise, false.

## Namespace `System.Text.RegularExpressions.Generated.PrimaryKeyRegex_8`

### Type `RunnerFactory` (type)

Provides a factory for creating System.Text.RegularExpressions.RegexRunner instances to be used by methods on System.Text.RegularExpressions.Regex.

#### Methods

- `CreateInstance`

  Creates an instance of a System.Text.RegularExpressions.RegexRunner used by methods on System.Text.RegularExpressions.Regex.

## Namespace `System.Text.RegularExpressions.Generated.PrimaryKeyRegex_8.RunnerFactory`

### Type `Runner` (type)

Provides the runner that contains the custom logic implementing the specified regular expression.

#### Methods

- `Scan(System.ReadOnlySpan{System.Char})`

  Scan the inputSpan starting from base.runtextstart for the next match.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

- `TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})`

  Search inputSpan starting from base.runtextpos for the next location a match could possibly start.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if a possible match was found; false if no more matches are possible.

- `TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})`

  Determine whether inputSpan at base.runtextpos is a match for the regular expression.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if the regular expression matches at the current position; otherwise, false.

## Namespace `System.Text.RegularExpressions.Generated.DelimiterRegex_9`

### Type `RunnerFactory` (type)

Provides a factory for creating System.Text.RegularExpressions.RegexRunner instances to be used by methods on System.Text.RegularExpressions.Regex.

#### Methods

- `CreateInstance`

  Creates an instance of a System.Text.RegularExpressions.RegexRunner used by methods on System.Text.RegularExpressions.Regex.

## Namespace `System.Text.RegularExpressions.Generated.DelimiterRegex_9.RunnerFactory`

### Type `Runner` (type)

Provides the runner that contains the custom logic implementing the specified regular expression.

#### Methods

- `Scan(System.ReadOnlySpan{System.Char})`

  Scan the inputSpan starting from base.runtextstart for the next match.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

- `TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})`

  Search inputSpan starting from base.runtextpos for the next location a match could possibly start.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if a possible match was found; false if no more matches are possible.

- `TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})`

  Determine whether inputSpan at base.runtextpos is a match for the regular expression.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if the regular expression matches at the current position; otherwise, false.

## Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_10`

### Type `RunnerFactory` (type)

Provides a factory for creating System.Text.RegularExpressions.RegexRunner instances to be used by methods on System.Text.RegularExpressions.Regex.

#### Methods

- `CreateInstance`

  Creates an instance of a System.Text.RegularExpressions.RegexRunner used by methods on System.Text.RegularExpressions.Regex.

## Namespace `System.Text.RegularExpressions.Generated.CreateDbRegex_10.RunnerFactory`

### Type `Runner` (type)

Provides the runner that contains the custom logic implementing the specified regular expression.

#### Methods

- `Scan(System.ReadOnlySpan{System.Char})`

  Scan the inputSpan starting from base.runtextstart for the next match.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

- `TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})`

  Search inputSpan starting from base.runtextpos for the next location a match could possibly start.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if a possible match was found; false if no more matches are possible.

- `TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})`

  Determine whether inputSpan at base.runtextpos is a match for the regular expression.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if the regular expression matches at the current position; otherwise, false.

## Namespace `System.Text.RegularExpressions.Generated.CreateTypeRegex_11`

### Type `RunnerFactory` (type)

Provides a factory for creating System.Text.RegularExpressions.RegexRunner instances to be used by methods on System.Text.RegularExpressions.Regex.

#### Methods

- `CreateInstance`

  Creates an instance of a System.Text.RegularExpressions.RegexRunner used by methods on System.Text.RegularExpressions.Regex.

## Namespace `System.Text.RegularExpressions.Generated.CreateTypeRegex_11.RunnerFactory`

### Type `Runner` (type)

Provides the runner that contains the custom logic implementing the specified regular expression.

#### Methods

- `Scan(System.ReadOnlySpan{System.Char})`

  Scan the inputSpan starting from base.runtextstart for the next match.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

- `TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})`

  Search inputSpan starting from base.runtextpos for the next location a match could possibly start.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if a possible match was found; false if no more matches are possible.

- `TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})`

  Determine whether inputSpan at base.runtextpos is a match for the regular expression.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if the regular expression matches at the current position; otherwise, false.

## Namespace `System.Text.RegularExpressions.Generated.PasswordRegex_12`

### Type `RunnerFactory` (type)

Provides a factory for creating System.Text.RegularExpressions.RegexRunner instances to be used by methods on System.Text.RegularExpressions.Regex.

#### Methods

- `CreateInstance`

  Creates an instance of a System.Text.RegularExpressions.RegexRunner used by methods on System.Text.RegularExpressions.Regex.

## Namespace `System.Text.RegularExpressions.Generated.PasswordRegex_12.RunnerFactory`

### Type `Runner` (type)

Provides the runner that contains the custom logic implementing the specified regular expression.

#### Methods

- `Scan(System.ReadOnlySpan{System.Char})`

  Scan the inputSpan starting from base.runtextstart for the next match.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

- `TryFindNextPossibleStartingPosition(System.ReadOnlySpan{System.Char})`

  Search inputSpan starting from base.runtextpos for the next location a match could possibly start.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if a possible match was found; false if no more matches are possible.

- `TryMatchAtCurrentPosition(System.ReadOnlySpan{System.Char})`

  Determine whether inputSpan at base.runtextpos is a match for the regular expression.

  Parameters:
  - `inputSpan`: The text being scanned by the regular expression.

  Returns: true if the regular expression matches at the current position; otherwise, false.

