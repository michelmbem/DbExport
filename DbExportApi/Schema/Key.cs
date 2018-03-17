using System.Collections.Generic;

namespace DbExport.Schema
{
    public class Key : ColumnSet
    {
        public Key(Table table, string name, IEnumerable<string> columnNames)
            : base(table, name)
        {
            foreach (string columnName in columnNames)
                Columns.Add(table.Columns[columnName]);
        }

        public Table Table
        {
            get { return (Table) Parent; }
        }

        public bool MatchesSignature(Key other)
        {
            if (Columns.Count != other.Columns.Count) return false;

            for (int i = 0; i < Columns.Count; i++)
                if (Columns[i].Name != other.Columns[i].Name)
                    return false;

            return true;
        }
    }
}
