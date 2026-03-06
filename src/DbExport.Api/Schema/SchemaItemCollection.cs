using System;
using System.Collections.Generic;

namespace DbExport.Schema;

/// <summary>
/// Represents a collection of strongly typed schema items, providing functionality for indexing,
/// addition, removal, and lookup based on item names. This collection is intended to manage
/// schema items that inherit from the <see cref="SchemaItem"/> class.
/// </summary>
/// <typeparam name="TItem">The specific type of schema items contained in the collection.</typeparam>
public abstract class SchemaItemCollection<TItem> : List<TItem> where TItem : SchemaItem
{
    /// <summary>
    /// Represents a private dictionary that serves as a lookup mechanism for storing and retrieving
    /// schema items within the <see cref="SchemaItemCollection{TItem}"/>. The dictionary
    /// maps item names to their corresponding instances to enable efficient access.
    /// </summary>
    private readonly Dictionary<string, TItem> dictionary = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaItemCollection{TItem}"/> class.
    /// </summary>
    protected SchemaItemCollection() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaItemCollection{TItem}"/> class with the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial number of elements that the collection can contain.</param>
    protected SchemaItemCollection(int capacity) : base(capacity) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaItemCollection{TItem}"/> class with the specified collection of items.
    /// </summary>
    /// <param name="items">The collection of items to initialize the collection with.</param>
    protected SchemaItemCollection(IEnumerable<TItem> items)
    {
        AddRange(items);
    }

    /// <summary>
    /// Provides indexed access to schema items in the collection by their unique names.
    /// The indexer performs a lookup in the internal dictionary to retrieve the schema item
    /// that matches the given name.
    /// </summary>
    /// <param name="name">The unique name of the schema item to retrieve.</param>
    /// <returns>The schema item associated with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if the specified name does not exist in the collection.
    /// </exception>
    public TItem this[string name] => dictionary[name];

    /// <summary>
    /// Attempts to retrieve the schema item associated with the specified name from the collection.
    /// </summary>
    /// <param name="name">The name of the schema item to find in the collection.</param>
    /// <param name="item">When this method returns, contains the schema item associated with the specified name,
    /// if the name is found, or null if the name is not found. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the collection contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public bool TryGetValue(string name, out TItem item) => dictionary.TryGetValue(name, out item);

    /// <summary>
    /// Adds the specified item to the collection.
    /// </summary>
    /// <param name="item">The item to add to the collection. Must not be null and must have a unique full name.</param>
    /// <exception cref="ArgumentException">Thrown when an item with the same full name already exists in the collection.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the provided item is null.</exception>
    public new void Add(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (dictionary.ContainsKey(item.FullName))
            throw new ArgumentException($"Item '{item.FullName}' is already present in the collection");
            
        base.Add(item);
        dictionary.Add(item.FullName, item);
    }

    /// <summary>
    /// Adds a range of items to the collection.
    /// </summary>
    /// <param name="items">An enumerable collection of items to add to the collection. Each item must not be null and must have a unique full name.</param>
    /// <exception cref="ArgumentException">Thrown when one or more items in the collection have full names that already exist in the collection.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the provided collection or any item in it is null.</exception>
    public new void AddRange(IEnumerable<TItem> items)
    {
        foreach (var item in items)
            Add(item);
    }

    /// <summary>
    /// Determines whether the collection contains an item with the specified name.
    /// </summary>
    /// <param name="name">The name of the schema item to locate in the collection.</param>
    /// <returns>
    /// <c>true</c> if an item with the specified name is found in the collection; otherwise, <c>false</c>.
    /// </returns>
    public bool Contains(string name) => dictionary.ContainsKey(name);

    /// <summary>
    /// Returns the zero-based index of the schema item with the specified name within the collection.
    /// </summary>
    /// <param name="name">The name of the schema item to locate in the collection.</param>
    /// <returns>
    /// The zero-based index of the schema item if found in the collection; otherwise, -1.
    /// </returns>
    public int IndexOf(string name) => IndexOf(dictionary[name]);

    /// <summary>
    /// Removes the specified schema item from the collection.
    /// </summary>
    /// <param name="item">
    /// The schema item to remove from the collection. This item must exist within the collection.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified schema item was successfully removed from the collection;
    /// otherwise, <c>false</c>.
    /// </returns>
    public new bool Remove(TItem item) => base.Remove(item) && dictionary.Remove(item.FullName);

    /// <summary>
    /// Removes a schema item from the collection by its name.
    /// </summary>
    /// <param name="name">
    /// The name of the schema item to remove from the collection. The item must exist within the collection.
    /// </param>
    /// <returns>
    /// <c>true</c> if the schema item with the specified name was successfully removed from the collection;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool Remove(string name) => Remove(dictionary[name]);

    /// <summary>
    /// Removes a range of schema items from the collection starting at the specified index.
    /// </summary>
    /// <param name="index">
    /// The zero-based starting index of the range of elements to remove. Must be within the bounds of the collection.
    /// </param>
    /// <param name="count">
    /// The number of items to remove starting from the specified index. Must be non-negative and not exceed the available items from the index.
    /// </param>
    public new void RemoveRange(int index, int count)
    {
        for (var i = index; i < index + count; i++)
            Remove(this[i]);
    }

    /// <summary>
    /// Removes all items from the <see cref="SchemaItemCollection{TItem}"/> and clears the internal dictionary
    /// used for name-based lookups.
    /// </summary>
    public new void Clear()
    {
        base.Clear();
        dictionary.Clear();
    }
}