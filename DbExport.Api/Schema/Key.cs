using System.Collections.Generic;
using System.Linq;

namespace DbExport.Schema;

/// <summary>
/// Represents an abstract base class for database keys, such as primary keys, foreign keys, and indexes.
/// Provides functionality to manage columns associated with the key and to compare key signatures.
/// </summary>
public abstract class Key : ColumnSet
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Key"/> class.
    /// </summary>
    /// <param name="table">The table that owns the key.</param>
    /// <param name="name">The name of the key.</param>
    /// <param name="columnNames">The names of the columns that make up the key.</param>
    protected Key(Table table, string name, IEnumerable<string> columnNames) : base(table, name)
    {
        foreach (var columnName in columnNames)
            Columns.Add(table.Columns[columnName]);
    }

    /// <summary>
    /// Gets the table that owns the key.
    /// </summary>
    public Table Table => (Table)Parent;

    /// <summary>
    /// Determines whether the current key matches the signature of another key.
    /// A signature match occurs when both keys have the same number of columns
    /// and the columns have identical names in the same order.
    /// </summary>
    /// <param name="other">The key to compare against the current key.</param>
    /// <returns>
    /// <c>true</c> if the current key matches the signature of the specified key; otherwise, <c>false</c>.
    /// </returns>
    public bool MatchesSignature(Key other) =>
        Columns.Count == other.Columns.Count && !Columns.Where((c, i) => c.Name != other.Columns[i].Name).Any();
}