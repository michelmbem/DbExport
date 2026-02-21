using System.Collections.Generic;
using System.Collections.Immutable;

namespace DbExport.Schema;

public class DataType(
    Database database,
    string name,
    string owner,
    ColumnType type,
    string nativeType,
    short size,
    byte precision,
    byte scale,
    bool nullable,
    bool enumerated,
    object defaultValue,
    IEnumerable<object> possibleValues)
    : SchemaItem(database, name), ICheckable
{
    public string Owner { get; } = owner;
    
    public ColumnType ColumnType { get; } = type;

    public string NativeType { get; } = nativeType;

    public short Size { get; } = size;

    public byte Precision { get; } = precision;

    public byte Scale { get; } = scale;
    
    public bool IsNullable { get; } = nullable;
    
    public bool IsEnumerated { get; } = enumerated;

    public object DefaultValue { get; } = defaultValue;
    
    public ImmutableHashSet<object> PossibleValues { get; } = ImmutableHashSet.CreateRange(possibleValues);

    public Database Database => (Database)Parent;

    public bool IsChecked { get; set; }
    
    public override string FullName => string.IsNullOrEmpty(Owner) ? Name : $"{Owner}.{Name}";

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitDataType(this);
    }
}