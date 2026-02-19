using System.Collections.Generic;

namespace DbExport.Schema;

public class ForeignKey : Key
{
    public ForeignKey(Table table, string name, IEnumerable<string> columnNames,
                      string relatedTableName, string[] relatedColumnNames,
                      ForeignKeyRule updateRule, ForeignKeyRule deleteRule) :
        base(table, name, columnNames)
    {
        RelatedTableName = relatedTableName;
        RelatedColumnNames = relatedColumnNames;
        UpdateRule = updateRule;
        DeleteRule = deleteRule;

        foreach (var column in Columns)
            column.SetAttribute(ColumnAttributes.FKColumn);
    }

    public string RelatedTableName { get; }

    public string[] RelatedColumnNames { get; }

    public ForeignKeyRule UpdateRule { get; }

    public ForeignKeyRule DeleteRule { get; }

    public Table RelatedTable =>
        Table.Database.Tables.TryGetValue(RelatedTableName, out var related) ? related : null;

    public Column GetRelatedColumn(int i) => RelatedTable?.Columns[RelatedColumnNames[i]];

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitForeignKey(this);
    }
}