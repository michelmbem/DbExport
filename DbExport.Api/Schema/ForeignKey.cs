using System.Collections.Generic;

namespace DbExport.Schema;

public class ForeignKey : Key
{
    public ForeignKey(Table table, string name, IEnumerable<string> columnNames,
                      string relatedName, string relatedOwner, string[] relatedColumns,
                      ForeignKeyRule updateRule, ForeignKeyRule deleteRule) :
        base(table, name, columnNames)
    {
        RelatedTableName = relatedName;
        RelatedTableOwner = relatedOwner;
        RelatedColumnNames = relatedColumns;
        UpdateRule = updateRule;
        DeleteRule = deleteRule;

        foreach (var column in Columns)
            column.SetAttribute(ColumnAttributes.FKColumn);
    }

    public string RelatedTableName { get; }

    public string RelatedTableOwner { get; }

    public string RelatedTableFullName => string.IsNullOrEmpty(RelatedTableOwner)
        ? RelatedTableName
        : $"{RelatedTableOwner}.{RelatedTableName}";

    public string[] RelatedColumnNames { get; }

    public ForeignKeyRule UpdateRule { get; }

    public ForeignKeyRule DeleteRule { get; }

    public Table RelatedTable =>
        Table.Database.Tables.TryGetValue(RelatedTableFullName, out var related) ? related : null;

    public Column GetRelatedColumn(int i) => RelatedTable?.Columns[RelatedColumnNames[i]];

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitForeignKey(this);
    }
}