using System.Collections.Generic;

namespace DbExport.Schema
{
    public class ForeignKey : Key
    {
        public ForeignKey(Table table, string name, IEnumerable<string> columnNames,
            string relatedTableName, string[] relatedColumnNames,
            ForeignKeyRule updateRule, ForeignKeyRule deleteRule)
            : base(table, name, columnNames)
        {
            RelatedTableName = relatedTableName;
            RelatedColumnNames = relatedColumnNames;
            UpdateRule = updateRule;
            DeleteRule = deleteRule;

            foreach (Column column in Columns)
                column.SetAttribute(ColumnAttribute.FKColumn);
        }

        public string RelatedTableName { get; private set; }

        public string[] RelatedColumnNames { get; private set; }

        public ForeignKeyRule UpdateRule { get; private set; }

        public ForeignKeyRule DeleteRule { get; private set; }

        public Table RelatedTable
        {
            get { return Table.Database.Tables[RelatedTableName]; }
        }

        public Column GetRelatedColumn(int i)
        {
            return RelatedTable.Columns[RelatedColumnNames[i]];
        }

        public override void AcceptVisitor(IVisitor visitor)
        {
            visitor.VisitForeignKey(this);
        }
    }
}
