using System.Collections.Generic;

namespace DbExport.Schema
{
    public class Table : ColumnSet, ICheckable
    {
        private readonly IndexCollection indexes = new IndexCollection();
        private readonly ForeignKeyCollection foreignKeys = new ForeignKeyCollection();

        public Table(Database db, string name, string owner)
            : base(db, name)
        {
            Owner = owner;
        }

        public string Owner { get; private set; }

        public PrimaryKey PrimaryKey { get; private set; }

        public bool Checked { get; set; }

        public IndexCollection Indexes
        {
            get { return indexes; }
        }

        public ForeignKeyCollection ForeignKeys
        {
            get { return foreignKeys; }
        }

        public Database Database
        {
            get { return (Database) Parent; }
        }

        public bool HasPrimaryKey
        {
            get { return PrimaryKey != null && PrimaryKey.Columns.Count > 0; }
        }

        public bool HasIndex
        {
            get
            {
                if (Indexes.Count <= 0) return false;

                foreach (Index index in Indexes)
                    if (index.Columns.Count > 0) return true;

                return false;
            }
        }

        public bool HasForeignKey
        {
            get
            {
                if (ForeignKeys.Count <= 0) return false;

                foreach (ForeignKey fk in ForeignKeys)
                    if (fk.Columns.Count > 0) return true;

                return false;
            }
        }

        public ColumnCollection NonPKColumns
        {
            get
            {
                var columns = new ColumnCollection();

                foreach (Column column in Columns)
                {
                    if (column.IsPKColumn) continue;
                    columns.Add(column);
                }

                return columns;
            }
        }

        public ColumnCollection NonFKColumns
        {
            get
            {
                var columns = new ColumnCollection();

                foreach (Column column in Columns)
                {
                    if (column.IsFKColumn) continue;
                    columns.Add(column);
                }

                return columns;
            }
        }

        public ColumnCollection NonKeyColumns
        {
            get
            {
                var columns = new ColumnCollection();

                foreach (Column column in Columns)
                {
                    if (column.IsKeyColumn) continue;
                    columns.Add(column);
                }

                return columns;
            }
        }

        public TableCollection ReferencedTables
        {
            get
            {
                var tables = new TableCollection();

                foreach (ForeignKey fk in ForeignKeys)
                    tables.Add(fk.RelatedTable);

                return tables;
            }
        }

        public TableCollection ReferencingTables
        {
            get
            {
                var tables = new TableCollection();

                foreach (Table table in Database.Tables)
                {
                    if (table.GetReferencingKey(this) == null) continue;
                    tables.Add(table);
                }

                return tables;
            }
        }

        public string FullName
        {
            get { return string.IsNullOrEmpty(Owner) ? Name : Owner + "." + Name; }
        }

        public void GeneratePrimaryKey(string name, IEnumerable<string> columnNames)
        {
            PrimaryKey = new PrimaryKey(this, name, columnNames);
        }

        public ForeignKey GetReferencingKey(Table table)
        {
            foreach (ForeignKey fk in ForeignKeys)
                if (fk.RelatedTable.Equals(table))
                    return fk;

            return null;
        }

        public bool IsAssociationTable()
        {
            if (ReferencedTables.Count <= 1) return false;

            foreach (Column column in Columns)
                if (!(column.IsFKColumn || column.IsGenerated))
                    return false;
            
            return true;
        }

        public override void AcceptVisitor(IVisitor visitor)
        {
            visitor.VisitTable(this);
        }
    }
}
