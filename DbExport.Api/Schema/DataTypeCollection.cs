using System.Collections.Generic;

namespace DbExport.Schema;

public class DataTypeCollection : SchemaItemCollection<DataType>
{
    public DataTypeCollection() { }

    public DataTypeCollection(int capacity) : base(capacity) { }

    public DataTypeCollection(IEnumerable<DataType> items) : base(items) { }
}