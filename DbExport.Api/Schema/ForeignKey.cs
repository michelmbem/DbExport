using System.Collections.Generic;

namespace DbExport.Schema;

/// <summary>
/// Represents a foreign key constraint in a database schema. This class provides
/// details about the referenced table, the columns involved, and the actions
/// to take on update or delete.
/// </summary>
public class ForeignKey : Key
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForeignKey"/> class.
    /// </summary>
    /// <param name="table">The table that owns the foreign key.</param>
    /// <param name="name">The name of the foreign key constraint.</param>
    /// <param name="columnNames">The names of the columns that make up the foreign key.</param>
    /// <param name="relatedName">The name of the referenced table.</param>
    /// <param name="relatedOwner">The owner of the referenced table.</param>
    /// <param name="relatedColumns">The names of the columns in the referenced table.</param>
    /// <param name="updateRule">The action to take on update.</param>
    /// <param name="deleteRule">The action to take on delete.</param>
    public ForeignKey(Table table, string name, IEnumerable<string> columnNames,
                      string relatedName, string relatedOwner, string[] relatedColumns,
                      ForeignKeyRule updateRule, ForeignKeyRule deleteRule) :
        base(table, name, columnNames)
    {
        RelatedTableName = relatedName;
        RelatedTableOwner = relatedOwner;
        RelatedColumnNames = relatedColumns;
        UpdateRule = updateRule;
        DeleteRule = deleteRule;

        foreach (var column in Columns)
            column.SetAttribute(ColumnAttributes.FKColumn);
    }

    /// <summary>
    /// Gets the name of the referenced table.
    /// </summary>
    public string RelatedTableName { get; }

    /// <summary>
    /// Gets the owner of the referenced table.
    /// </summary>
    public string RelatedTableOwner { get; }

    /// <summary>
    /// Gets the fully qualified name of the referenced table.
    /// </summary>
    public string RelatedTableFullName => string.IsNullOrEmpty(RelatedTableOwner)
        ? RelatedTableName
        : $"{RelatedTableOwner}.{RelatedTableName}";

    /// <summary>
    /// Gets the names of the columns in the referenced table.
    /// </summary>
    public string[] RelatedColumnNames { get; }

    /// <summary>
    /// Gets the action to take on update.
    /// </summary>
    public ForeignKeyRule UpdateRule { get; }

    /// <summary>
    /// Gets the action to take on delete.
    /// </summary>
    public ForeignKeyRule DeleteRule { get; }

    /// <summary>
    /// Gets a reference to the referenced table if loaded in the imported database schema, otherwise null.
    /// </summary>
    public Table RelatedTable =>
        Table.Database.Tables.TryGetValue(RelatedTableFullName, out var related) ? related : null;

    /// <summary>
    /// Retrieves the related column based on the specified index in the foreign key relationship.
    /// </summary>
    /// <param name="i">The zero-based index of the related column in the foreign key relationship.</param>
    /// <returns>The <see cref="Column"/> object corresponding to the specified index, or null if the related table or column is not found.</returns>
    public Column GetRelatedColumn(int i) => RelatedTable?.Columns[RelatedColumnNames[i]];

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitForeignKey(this);
    }
}