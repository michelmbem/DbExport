using System.Collections.Generic;
using System.Linq;

namespace DbExport.Schema;

public class Index : Key
{
    public Index(Table table, string name, IEnumerable<string> columnNames, bool unique, bool primaryKey) :
        base(table, name, columnNames)
    {
        IsUnique = unique;
        IsPrimaryKey = primaryKey;

        foreach (var column in Columns)
            column.SetAttribute(ColumnAttributes.IXColumn);
    }

    public bool IsUnique { get; }

    public bool IsPrimaryKey { get; }

    public bool MatchesPrimaryKey => Table.HasPrimaryKey && MatchesSignature(Table.PrimaryKey);

    public bool MatchesForeignKey => Table.HasForeignKey && Table.ForeignKeys.Any(MatchesSignature);

    public bool MatchesKey => MatchesPrimaryKey || MatchesForeignKey;

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitIndex(this);
    }
}