using System.Collections.Generic;

namespace DbExport.Schema;

/// <summary>
/// Represents a collection of database tables.
/// </summary>
public class TableCollection : SchemaItemCollection<Table>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableCollection"/> class.
    /// </summary>
    public TableCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableCollection"/> class with the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the collection can contain.</param>
    public TableCollection(int capacity) : base(capacity) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableCollection"/> class with the specified collection of items.
    /// </summary>
    /// <param name="tables">The collection of items to initialize the collection with.</param>
    public TableCollection(IEnumerable<Table> tables) : base(tables) { }
}