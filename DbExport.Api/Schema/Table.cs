using System.Collections.Generic;
using System.Linq;

namespace DbExport.Schema;

/// <summary>
/// Represents a database table, which is a specialized <see cref="ColumnSet"/> containing columns,
/// constraints, and relationships to other tables.
/// </summary>
/// <remarks>
/// The Table class extends the ColumnSet class to provide additional functionality specific to
/// database tables, such as primary key generation, foreign key constraints, and referencing tables.
/// </remarks>
/// <param name="db">The database that owns the table.</param>
/// <param name="name">The name of the table.</param>
/// <param name="owner">The owner of the table.</param>
public class Table(Database db, string name, string owner) : ColumnSet(db, name)
{
    /// <summary>
    /// The owner of the table.
    /// </summary>
    public string Owner { get; } = owner;

    /// <summary>
    /// The primary key of the table.
    /// </summary>
    public PrimaryKey PrimaryKey { get; private set; }

    /// <summary>
    /// The indexes of the table.
    /// </summary>
    public IndexCollection Indexes { get; } = [];

    /// <summary>
    /// The foreign keys of the table.
    /// </summary>
    public ForeignKeyCollection ForeignKeys { get; } = [];

    /// <summary>
    /// The database that owns the table.
    /// </summary>
    public Database Database => (Database)Parent;

    /// <summary>
    /// Indicates whether the table has a primary key.
    /// </summary>
    public bool HasPrimaryKey => PrimaryKey?.Columns.Count > 0;

    /// <summary>
    /// Indicates whether the table has an index.
    /// </summary>
    public bool HasIndex => Indexes.Count > 0 && Indexes.Any(index => index.Columns.Count > 0);

    /// <summary>
    /// Indicates whether the table has a foreign key.
    /// </summary>
    public bool HasForeignKey => ForeignKeys.Count > 0 && ForeignKeys.Any(fk => fk.Columns.Count > 0);

    /// <summary>
    /// Gets the columns of the table that are not primary key columns.
    /// </summary>
    public ColumnCollection NonPKColumns => [..Columns.Where(column => !column.IsPKColumn)];

    /// <summary>
    /// Gets the columns of the table that are not foreign key columns.
    /// </summary>
    public ColumnCollection NonFKColumns => [..Columns.Where(column => !column.IsFKColumn)];

    /// <summary>
    /// Gets the columns of the table that are neither primary key nor foreign key columns.
    /// </summary>
    public ColumnCollection NonKeyColumns => [..Columns.Where(column => !column.IsKeyColumn)];

    /// <summary>
    /// Gets a collection of tables that are referenced by foreign keys in this table.
    /// </summary>
    public TableCollection ReferencedTables => [..ForeignKeys.Select(fk => fk.RelatedTable)];

    /// <summary>
    /// Gets a collection of tables that are referencing this table through foreign keys.
    /// </summary>
    public TableCollection ReferencingTables =>
        [..Database.Tables.Where(table => table.GetReferencingKey(this) != null)];

    public override string FullName => string.IsNullOrEmpty(Owner) ? Name : $"{Owner}.{Name}";

    /// <summary>
    /// Creates a primary key for the table using the specified name and column names.
    /// </summary>
    /// <param name="name">The name of the primary key to be created.</param>
    /// <param name="columnNames">A collection of column names that will be included in the primary key.</param>
    public void GeneratePrimaryKey(string name, IEnumerable<string> columnNames)
    {
        PrimaryKey = new PrimaryKey(this, name, columnNames);
    }

    /// <summary>
    /// Retrieves the foreign key in the current table that references the specified table.
    /// </summary>
    /// <param name="table">The table being referenced by the foreign key.</param>
    /// <returns>The foreign key that references the specified table, or null if no such key exists.</returns>
    public ForeignKey GetReferencingKey(Table table) =>
        ForeignKeys.FirstOrDefault(fk => Equals(table, fk.RelatedTable));

    /// <summary>
    /// Determines whether the table is an association table.
    /// An association table is identified as having more than one referenced table,
    /// and all its columns are either foreign key columns or generated columns.
    /// </summary>
    /// <returns>
    /// True if the table is an association table; otherwise, false.
    /// </returns>
    public bool IsAssociationTable() =>
        ReferencedTables.Count > 1 && Columns.All(column => column.IsFKColumn || column.IsGenerated);

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitTable(this);
    }
}