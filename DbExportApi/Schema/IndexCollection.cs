using System.Collections.Generic;

namespace DbExport.Schema
{
    public class IndexCollection : SchemaItemCollection<Index>
    {
        public IndexCollection()
        {
        }

        public IndexCollection(int capacity)
            : base(capacity)
        {
        }

        public IndexCollection(IEnumerable<Index> items)
            : base(items)
        {
        }
    }
}
