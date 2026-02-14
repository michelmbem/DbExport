using System.Collections.Generic;
using System.Linq;

namespace DbExport.Schema;

public class Key : ColumnSet
{
    protected Key(Table table, string name, IEnumerable<string> columnNames) : base(table, name)
    {
        foreach (var columnName in columnNames)
            Columns.Add(table.Columns[columnName]);
    }

    public Table Table => (Table)Parent;

    public bool MatchesSignature(Key other)
    {
        if (Columns.Count != other.Columns.Count) return false;
        return !Columns.Where((t, i) => t.Name != other.Columns[i].Name)
                       .Any();
    }
}