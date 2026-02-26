using System.Collections.Generic;
using System.Linq;

namespace DbExport.Schema;

public abstract class Key : ColumnSet
{
    protected Key(Table table, string name, IEnumerable<string> columnNames) : base(table, name)
    {
        foreach (var columnName in columnNames)
            Columns.Add(table.Columns[columnName]);
    }

    public Table Table => (Table)Parent;

    public bool MatchesSignature(Key other) =>
        Columns.Count == other.Columns.Count && !Columns.Where((c, i) => c.Name != other.Columns[i].Name).Any();
}