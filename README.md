# DbExport

DbExport is a desktop database migration wizard built with .NET and Avalonia.
It helps you migrate schema and/or data between different database engines.

## Features

- Guided 7-step migration workflow
- Schema and data migration support
- Selective export options (primary keys, foreign keys, indexes, defaults, identity columns)
- Object-level selection before script generation
- SQL script generation and execution from the app
- Save generated scripts as `.sql`

## Library Module (Mini-ORM)

`DbExport.Api` can also be used as a lightweight mini-ORM/data-access helper.

- Provider-agnostic ADO.NET helper (`SqlHelper`) for query, scalar, execute, and script execution
- Automatic SQL parameter discovery from SQL text (`@name`, `:name`, etc.)
- Multiple binders for command parameters (`FromArray`, `FromDictionary`, `FromEntity`)
- Data reader extractors for single-row and multi-row mapping (array, dictionary, entity)
- `TableExtensions` helpers to generate provider-specific SQL (`GenerateSelect`, `GenerateInsert`, `GenerateUpdate`, `GenerateDelete`)
- Lightweight CRUD helpers on schema tables (`Select`, `Insert`, `Update`, `Delete`) with batch variants (`InsertBatch`, `UpdateBatch`, `DeleteBatch`)
- Cross-database copy helper (`CopyTo`) to move table data between providers

### Examples

`SqlHelper` - query + scalar:

```csharp
using var helper = new SqlHelper(providerName, connectionString);
var users = helper.Query("SELECT Id, Name FROM Users", SqlHelper.ToDictionaryList);
var totalUsers = helper.QueryScalar("SELECT COUNT(*) FROM Users");
```

`SqlHelper` - parameterized query:

```csharp
var user = helper.Query(
    "SELECT Id, Name, Email FROM Users WHERE Id = @Id",
    new { Id = 42 },
    (cmd, p) => cmd.Parameters["Id"].Value = p.Id,
    SqlHelper.ToDictionary);
```

`SqlHelper` - execute + batch insert with entity binding:

```csharp
public sealed class UserRow
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

var inserted = helper.Execute(
    "INSERT INTO Users (Id, Name, Email) VALUES (@Id, @Name, @Email)",
    new UserRow { Id = 1, Name = "Ada", Email = "ada@example.com" },
    SqlHelper.FromEntity);

var batchInserted = helper.ExecuteBatch(
    "INSERT INTO Users (Id, Name, Email) VALUES (@Id, @Name, @Email)",
    new[]
    {
        new UserRow { Id = 2, Name = "Linus", Email = "linus@example.com" },
        new UserRow { Id = 3, Name = "Grace", Email = "grace@example.com" }
    },
    SqlHelper.FromEntity);
```

`TableExtensions` - schema-driven CRUD:

```csharp
using DbExport.Providers;
using DbExport.Schema;

var db = SchemaProvider.GetDatabase(providerName, connectionString, "dbo");
var usersTable = db.Tables["dbo.Users"]; // FullName: "<schema>.<table>"

var allUsers = usersTable.Select<UserRow>();
var added = usersTable.Insert(new UserRow { Id = 10, Name = "Marie", Email = "marie@example.com" });
var updated = usersTable.Update(new UserRow { Id = 10, Name = "Marie Curie", Email = "marie@example.com" });
var deleted = usersTable.Delete(10); // single-column PK
```

`TableExtensions` - copy rows to another provider:

```csharp
var sourceDb = SchemaProvider.GetDatabase(sourceProvider, sourceConnectionString, "public");
var sourceUsersTable = sourceDb.Tables["public.users"];

using var targetConnection = Utility.GetConnection(targetProvider, targetConnectionString);
targetConnection.Open();

sourceUsersTable.CopyTo(
    targetConnection,
    QueryOptions.QualifyTableName,
    QueryOptions.All);
```

## Supported Databases

- Microsoft SQL Server
- Oracle Database
- MySQL
- PostgreSQL
- Firebird
- SQLite
- Microsoft Access (Windows only)
- SQL Server LocalDB (Windows only)

## Platform Notes

- Windows target: `net10.0-windows`
- Linux/macOS target: `net10.0`
- Access support is Windows-only (OLE DB/COM interop)

## Requirements

- .NET 10 SDK
- Access to source and target databases
- Sufficient DB permissions for schema/data migration

## Getting Started

```bash
dotnet restore
dotnet build
dotnet run --project DbExport.Gui
```

## Running Tests

```bash
dotnet test
```

## Publishing

```bash
dotnet publish -c Release -r win-x64
dotnet publish -c Release -r linux-x64
dotnet publish -c Release -r osx-arm64
```

Artifacts are emitted under `Bin/Release/`.

## Screenshots

<!--
  Replace titles, descriptions, and image paths below.
  Suggested location for images: docs/screenshots/
-->

### 1. Source Database Setup
Brief description of what this screen shows.

![Source Database Setup](docs/screenshots/source-database-setup.png)

### 2. Migration Options
Brief description of what this screen shows.

![Migration Options](docs/screenshots/migration-options.png)

### 3. Items Selection
Brief description of what this screen shows.

![Items Selection](docs/screenshots/items-selection.png)

### 4. Generated SQL Script
Brief description of what this screen shows.

![Generated SQL Script](docs/screenshots/generated-sql-script.png)

## Solution Structure

- `DbExport.Gui`: Avalonia desktop app
- `DbExport.Api`: Core migration logic and providers
- `DbExport.Tests`: Unit tests

## Logging

Logs are written to the OS local app data folder:

- `DbExport/app.log`

## License

Apache 2.0. See [LICENSE](LICENSE).
