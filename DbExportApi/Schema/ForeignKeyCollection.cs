using System.Collections.Generic;

namespace DbExport.Schema
{
    public class ForeignKeyCollection : SchemaItemCollection<ForeignKey>
    {
        public ForeignKeyCollection()
        {
        }

        public ForeignKeyCollection(int capacity)
            : base(capacity)
        {
        }

        public ForeignKeyCollection(IEnumerable<ForeignKey> items)
            : base(items)
        {
        }
    }
}
