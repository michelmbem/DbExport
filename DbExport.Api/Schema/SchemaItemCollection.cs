using System;
using System.Collections.Generic;

namespace DbExport.Schema;

public abstract class SchemaItemCollection<TItem> : List<TItem> where TItem : SchemaItem
{
    private readonly Dictionary<string, TItem> dictionary = [];

    protected SchemaItemCollection() { }

    protected SchemaItemCollection(int capacity) : base(capacity) { }

    protected SchemaItemCollection(IEnumerable<TItem> items)
    {
        AddRange(items);
    }

    public TItem this[string name] => dictionary[name];

    public bool TryGetValue(string name, out TItem item) => dictionary.TryGetValue(name, out item);

    public new void Add(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (dictionary.ContainsKey(item.FullName))
            throw new ArgumentException($"Item '{item.FullName}' is already present in the collection");
            
        base.Add(item);
        dictionary.Add(item.FullName, item);
    }
    
    public new void AddRange(IEnumerable<TItem> items)
    {
        foreach (var item in items)
            Add(item);
    }

    public bool Contains(string name) => dictionary.ContainsKey(name);

    public int IndexOf(string name) => IndexOf(dictionary[name]);

    public new bool Remove(TItem item) => base.Remove(item) && dictionary.Remove(item.FullName);

    public bool Remove(string name) => Remove(dictionary[name]);
    
    public new void RemoveRange(int index, int count)
    {
        for (var i = index; i < index + count; i++)
            Remove(this[i]);
    }

    public new void Clear()
    {
        base.Clear();
        dictionary.Clear();
    }
}