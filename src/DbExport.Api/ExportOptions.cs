using System;

namespace DbExport;

/// <summary>
/// Flags to specify what aspects of the database to export. These can be combined using bitwise operations.
/// </summary>
[Flags]
public enum ExportFlags
{
    /// <summary>
    /// Represents no export operation. When this flag is set, no database schema or data will be exported.
    /// Typically used for scenarios where export functionality is disabled or not required.
    /// </summary>
    ExportNothing = 0,

    /// <summary>
    /// Represents the export operation for primary key definitions. When this flag is set,
    /// the primary keys of database tables will be included in the export output.
    /// Commonly used to ensure that table structures retain their unique key constraints.
    /// </summary>
    ExportPrimaryKeys = 1,

    /// <summary>
    /// Specifies that foreign key constraints should be included during the export operation.
    /// When this flag is set, all relationships defined by foreign keys in the database schema will be exported.
    /// This is useful for preserving data integrity and enforcing referential rules in the exported schema.
    /// </summary>
    ExportForeignKeys = 2,

    /// <summary>
    /// Represents an operation to export database indexes. When this flag is set,
    /// all defined indexes in the database schema will be included in the export process.
    /// This is typically used to preserve and recreate index structures in the target environment.
    /// </summary>
    ExportIndexes = 4,

    /// <summary>
    /// Specifies that default values for database objects, such as columns or data types,
    /// should be included in the export operation. When this flag is enabled, default
    /// constraints or definitions are generated as part of the database schema.
    /// </summary>
    ExportDefaults = 8,

    /// <summary>
    /// Represents the export of identity columns from the database schema. When this flag is set, identity columns
    /// (columns with auto-increment or similar behavior) are included during the export process.
    /// Useful for scenarios where retaining identity column definitions is necessary for database migration or replication.
    /// </summary>
    ExportIdentities = 16
}

/// <summary>
/// Options for exporting a database, including what to export and any provider-specific settings.
/// </summary>
public class ExportOptions
{
    /// <summary>
    /// Indicates whether the schema of the database should be exported as part of the export process.
    /// When set to true, the structural definitions of tables, views, and other schema components
    /// will be included in the export.
    /// </summary>
    public bool ExportSchema { get; set; }

    /// <summary>
    /// Determines whether the data within the tables of the database should be exported as part of the export process.
    /// When set to true, the contents of the tables, such as rows of data, will be included in the export.
    /// </summary>
    public bool ExportData { get; set; }

    /// <summary>
    /// Specifies the flags that determine which components of a database should be exported during the
    /// export process. This property allows combining multiple values from the <see cref="ExportFlags"/>
    /// enumeration to customize the export behavior, such as including or excluding primary keys,
    /// foreign keys, indexes, defaults, or identity columns.
    /// </summary>
    public ExportFlags Flags { get; set; }

    /// <summary>
    /// Allows specifying provider-specific settings to be used during the export process.
    /// This property can hold configuration options or parameters unique to a particular database provider,
    /// enabling customization of the export behavior for that provider.
    /// </summary>
    public dynamic ProviderSpecific { get; set; }

    /// <summary>
    /// Sets or clears a specific flag in the <see cref="ExportFlags"/> enumeration.
    /// </summary>
    /// <param name="flag">The flag to set or clear.</param>
    /// <param name="value">
    /// A boolean value indicating whether to set or clear the specified flag.
    /// If true, the flag will be set; if false, the flag will be cleared.
    /// </param>
    public void SetFlag(ExportFlags flag, bool value) => Flags = value ? Flags | flag : Flags & ~flag;

    /// <summary>
    /// Checks if a specific flag in the <see cref="ExportFlags"/> enumeration is set.
    /// </summary>
    /// <param name="flag">The flag to check for in the current <see cref="ExportFlags"/> value.</param>
    /// <returns>
    /// A boolean value indicating whether the specified flag is set.
    /// Returns true if the flag is set; otherwise, false.
    /// </returns>
    public bool HasFlag(ExportFlags flag) => Flags.HasFlag(flag);
}