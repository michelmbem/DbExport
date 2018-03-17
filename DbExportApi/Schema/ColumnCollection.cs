using System.Collections.Generic;

namespace DbExport.Schema
{
    public class ColumnCollection : SchemaItemCollection<Column>
    {
        public ColumnCollection()
        {
        }

        public ColumnCollection(int capacity)
            : base(capacity)
        {
        }

        public ColumnCollection(IEnumerable<Column> items)
            : base(items)
        {
        }
    }
}
