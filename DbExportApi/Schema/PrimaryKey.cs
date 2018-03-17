using System.Collections.Generic;

namespace DbExport.Schema
{
    public class PrimaryKey : Key
    {
        public PrimaryKey(Table table, string name, IEnumerable<string> columnNames)
            : base(table, name, columnNames)
        {
            foreach (Column column in Columns)
                column.SetAttribute(ColumnAttribute.PKColumn);
        }

        public bool IsComputed
        {
            get { return Columns.Count == 1 && Columns[0].IsComputed; }
        }

        public bool IsIdentity
        {
            get { return Columns.Count == 1 && Columns[0].IsIdentity; }
        }

        public override void AcceptVisitor(IVisitor visitor)
        {
            visitor.VisitPrimaryKey(this);
        }
    }
}
