using System;
using System.Collections.Generic;

namespace DbExport.Schema
{
    public abstract class SchemaItemCollection<ItemType> : List<ItemType>
        where ItemType : SchemaItem
    {
        private readonly Dictionary<string, ItemType> dictionary = new Dictionary<string, ItemType>();

        protected SchemaItemCollection()
        {
        }

        protected SchemaItemCollection(int capacity)
            : base(capacity)
        {
        }

        protected SchemaItemCollection(IEnumerable<ItemType> items)
        {
            foreach (ItemType item in items)
                Add(item);
        }

        public ItemType this[string name]
        {
            get { return dictionary[name]; }
        }

        public new void Add(ItemType item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (dictionary.ContainsKey(item.Name))
                throw new ArgumentException("Item '" + item.Name + "' is already contained in the collection");
            
            base.Add(item);
            dictionary.Add(item.Name, item);
        }

        public bool Contains(string name)
        {
            return dictionary.ContainsKey(name);
        }

        public int IndexOf(string name)
        {
            return IndexOf(dictionary[name]);
        }

        public new bool Remove(ItemType item)
        {
            if (!base.Remove(item)) return false;
            return dictionary.Remove(item.Name);
        }

        public bool Remove(string name)
        {
            return Remove(dictionary[name]);
        }
    }
}
