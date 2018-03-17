using System.Collections.Generic;

namespace DbExport.Schema
{
    public class Index : Key
    {
        public Index(Table table, string name, IEnumerable<string> columnNames,
            bool unique, bool primaryKey)
            : base(table, name, columnNames)
        {
            Unique = unique;
            PrimaryKey = primaryKey;

            foreach (Column column in Columns)
                column.SetAttribute(ColumnAttribute.IXColumn);
        }

        public bool Unique { get; private set; }

        public bool PrimaryKey { get; private set; }

        public bool MatchesPrimaryKey
        {
            get { return Table.HasPrimaryKey && MatchesSignature(Table.PrimaryKey); }
        }

        public bool MatchesForeignKey
        {
            get
            {
                if (!Table.HasForeignKey) return false;

                foreach (ForeignKey fk in Table.ForeignKeys)
                    if (MatchesSignature(fk)) return true;

                return false;
            }
        }

        public bool MatchesKey
        {
            get { return MatchesPrimaryKey || MatchesForeignKey; }
        }

        public override void AcceptVisitor(IVisitor visitor)
        {
            visitor.VisitIndex(this);
        }
    }
}
