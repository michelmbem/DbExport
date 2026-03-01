using System.Collections.Generic;

namespace DbExport.Schema;

/// <summary>
/// Represents a collection of database foreign keys.
/// </summary>
public class ForeignKeyCollection : SchemaItemCollection<ForeignKey>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForeignKeyCollection"/> class.
    /// </summary>
    public ForeignKeyCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForeignKeyCollection"/> class with the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the collection can contain.</param>
    public ForeignKeyCollection(int capacity) : base(capacity) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForeignKeyCollection"/> class with the specified collection of items.
    /// </summary>
    /// <param name="foreignKeys">The collection of items to initialize the collection with.</param>
    public ForeignKeyCollection(IEnumerable<ForeignKey> foreignKeys) : base(foreignKeys) { }
}