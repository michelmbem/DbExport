using System.Collections.Generic;

namespace DbExport.Schema;

public class PrimaryKey : Key
{
    public PrimaryKey(Table table, string name, IEnumerable<string> columnNames) :
        base(table, name, columnNames)
    {
        foreach (var column in Columns)
            column.SetAttribute(ColumnAttribute.PKColumn);
    }

    public bool IsComputed => Columns.Count == 1 && Columns[0].IsComputed;

    public bool IsIdentity => Columns.Count == 1 && Columns[0].IsIdentity;

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitPrimaryKey(this);
    }
}