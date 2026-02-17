using System.Linq;

namespace DbExport.Schema;

public abstract class ColumnSet(SchemaItem parent, string name) :
    SchemaItem(parent, name), ICheckable
{
    public ColumnCollection Columns { get; } = [];

    public bool IsChecked { get; set; }

    public bool AllColumnsAreChecked => Columns.All(column => column.IsChecked);

    public bool NoColumnIsChecked => Columns.All(column => !column.IsChecked);

    public bool AnyColumnIsChecked => Columns.Any(column => column.IsChecked);

    public bool AnyColumnIsUnchecked => Columns.Any(column => !column.IsChecked);
}