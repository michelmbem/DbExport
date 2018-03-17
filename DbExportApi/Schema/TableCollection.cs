using System.Collections.Generic;

namespace DbExport.Schema
{
    public class TableCollection : SchemaItemCollection<Table>
    {
        public TableCollection()
        {
        }

        public TableCollection(int capacity)
            : base(capacity)
        {
        }

        public TableCollection(IEnumerable<Table> items)
            : base(items)
        {
        }
    }
}
