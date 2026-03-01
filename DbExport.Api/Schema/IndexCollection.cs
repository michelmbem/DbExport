using System.Collections.Generic;

namespace DbExport.Schema;

/// <summary>
/// Represents a collection of database indexes.
/// </summary>
public class IndexCollection : SchemaItemCollection<Index>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexCollection"/> class.
    /// </summary>
    public IndexCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexCollection"/> class with the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the collection can contain.</param>
    public IndexCollection(int capacity) : base(capacity) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexCollection"/> class with the specified collection of items.
    /// </summary>
    /// <param name="indexes">The collection of items to initialize the collection with.</param>
    public IndexCollection(IEnumerable<Index> indexes) : base(indexes) { }
}