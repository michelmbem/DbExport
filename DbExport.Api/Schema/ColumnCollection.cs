using System.Collections.Generic;

namespace DbExport.Schema;

/// <summary>
/// Represents a collection of database columns.
/// </summary>
public class ColumnCollection : SchemaItemCollection<Column>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnCollection"/> class.
    /// </summary>
    public ColumnCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnCollection"/> class with the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the collection can contain.</param>
    public ColumnCollection(int capacity) : base(capacity) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnCollection"/> class with the specified collection of items.
    /// </summary>
    /// <param name="columns">The collection of items to initialize the collection with.</param>
    public ColumnCollection(IEnumerable<Column> columns) : base(columns) { }
}