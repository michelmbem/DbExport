using System.Collections.Generic;
using System.Linq;

namespace DbExport.Schema;

public class Table(Database db, string name, string owner) : ColumnSet(db, name)
{
    public string Owner { get; } = owner;

    public PrimaryKey PrimaryKey { get; private set; }

    public IndexCollection Indexes { get; } = [];

    public ForeignKeyCollection ForeignKeys { get; } = [];

    public Database Database => (Database) Parent;

    public bool HasPrimaryKey => PrimaryKey != null && PrimaryKey.Columns.Count > 0;

    public bool HasIndex => Indexes.Any(index => index.Columns.Count > 0);

    public bool HasForeignKey => ForeignKeys.Any(fk => fk.Columns.Count > 0);

    public ColumnCollection NonPKColumns => [..Columns.Where(column => !column.IsPKColumn)];

    public ColumnCollection NonFKColumns => [..Columns.Where(column => !column.IsFKColumn)];

    public ColumnCollection NonKeyColumns => [..Columns.Where(column => !column.IsKeyColumn)];

    public TableCollection ReferencedTables => [..ForeignKeys.Select(fk => fk.RelatedTable)];

    public TableCollection ReferencingTables =>
        [..Database.Tables.Where(table => table.GetReferencingKey(this) != null)];

    public string FullName => string.IsNullOrEmpty(Owner) ? Name : $"{Owner}.{Name}";

    public void GeneratePrimaryKey(string name, IEnumerable<string> columnNames)
    {
        PrimaryKey = new PrimaryKey(this, name, columnNames);
    }

    public ForeignKey GetReferencingKey(Table table) =>
        ForeignKeys.FirstOrDefault(fk => fk.RelatedTable.Equals(table));

    public bool IsAssociationTable() =>
        ReferencedTables.Count > 1 && Columns.All(column => column.IsFKColumn || column.IsGenerated);

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitTable(this);
    }
}