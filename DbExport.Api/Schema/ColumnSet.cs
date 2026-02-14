namespace DbExport.Schema;

public abstract class ColumnSet(SchemaItem parent, string name) :
    SchemaItem(parent, name), ICheckable
{
    public ColumnCollection Columns { get; } = [];

    public bool IsChecked { get; set; }
}