using System.Collections.Generic;

namespace DbExport.Schema;

/// <summary>
/// Represents a collection of database data types.
/// </summary>
public class DataTypeCollection : SchemaItemCollection<DataType>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataTypeCollection"/> class.
    /// </summary>
    public DataTypeCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTypeCollection"/> class with the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the collection can contain.</param>
    public DataTypeCollection(int capacity) : base(capacity) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTypeCollection"/> class with the specified collection of items.
    /// </summary>
    /// <param name="dataTypes">The collection of items to initialize the collection with.</param>
    public DataTypeCollection(IEnumerable<DataType> dataTypes) : base(dataTypes) { }
}