using System.Linq;

namespace DbExport.Schema;

/// <summary>
/// Represents an abstract base class for a set of related columns within a database schema.
/// Provides functionality to manage the state of column checks and evaluate check conditions.
/// </summary>
public abstract class ColumnSet(SchemaItem parent, string name) :
    SchemaItem(parent, name), ICheckable
{
    /// <summary>
    /// Gets a collection of columns associated with the column set.
    /// </summary>
    public ColumnCollection Columns { get; } = [];

    public bool IsChecked { get; set; }

    /// <summary>
    /// Gets a value indicating whether all columns in the set are checked.
    /// </summary>
    public bool AllColumnsAreChecked => Columns.All(column => column.IsChecked);

    /// <summary>
    /// Gets a value indicating whether no column in the set is checked.
    /// </summary>
    public bool NoColumnIsChecked => Columns.All(column => !column.IsChecked);

    /// <summary>
    /// Gets a value indicating whether any column in the set is checked.
    /// </summary>
    public bool AnyColumnIsChecked => Columns.Any(column => column.IsChecked);

    /// <summary>
    /// Gets a value indicating whether any column in the set is unchecked.
    /// </summary>
    public bool AnyColumnIsUnchecked => Columns.Any(column => !column.IsChecked);
}