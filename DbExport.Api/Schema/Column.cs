namespace DbExport.Schema;

public class Column(
    Table table,
    string name,
    ColumnType type,
    string nativeType,
    short size,
    byte precision,
    byte scale,
    ColumnAttributes attributes,
    object defaultValue,
    string description)
    : SchemaItem(table, name), IDataItem, ICheckable
{
    public ColumnType ColumnType { get; } = type;

    public string NativeType { get; } = nativeType;

    public short Size { get; } = size;

    public byte Precision { get; } = precision;

    public byte Scale { get; } = scale;

    public ColumnAttributes Attributes { get; private set; } = attributes | GetAttributesFromType(type);

    public object DefaultValue { get; } = defaultValue;

    public string Description { get; } = description;

    public long IdentitySeed { get; private set; }

    public long IdentityIncrement { get; private set; }

    public Table Table => (Table)Parent;
    
    public DataType DataType =>
        Table.Database.DataTypes.TryGetValue(NativeType, out var dataType) ? dataType : null;

    public bool IsRequired => Attributes.HasFlag(ColumnAttributes.Required);

    public bool IsComputed => Attributes.HasFlag(ColumnAttributes.Computed);

    public bool IsIdentity => Attributes.HasFlag(ColumnAttributes.Identity);

    public bool IsGenerated => IsComputed || IsIdentity || ColumnType == ColumnType.RowVersion;

    public bool IsPKColumn => Attributes.HasFlag(ColumnAttributes.PKColumn);

    public bool IsFKColumn => Attributes.HasFlag(ColumnAttributes.FKColumn);

    public bool IsKeyColumn => IsPKColumn || IsFKColumn;

    public bool IsIndexColumn => Attributes.HasFlag(ColumnAttributes.IXColumn);

    public bool IsNumeric => Attributes.HasFlag(ColumnAttributes.Numeric);

    public bool IsAlphabetic => Attributes.HasFlag(ColumnAttributes.Alphabetic);

    public bool IsFixedLength => Attributes.HasFlag(ColumnAttributes.FixedLength);

    public bool IsUnsigned => Attributes.HasFlag(ColumnAttributes.Unsigned);

    public bool IsUnicode => Attributes.HasFlag(ColumnAttributes.Unicode);

    public bool IsIntegral => IsNumeric && IsFixedLength;

    public bool IsNatural => IsIntegral && IsUnsigned;

    public bool IsTemporal => Attributes.HasFlag(ColumnAttributes.Temporal);

    public bool IsBinary => Attributes.HasFlag(ColumnAttributes.Binary);

    public bool IsChecked { get; set; }

    public void SetAttribute(ColumnAttributes attribute)
    {
        Attributes |= attribute;
    }

    public void MakeIdentity(long seed, long increment)
    {
        SetAttribute(ColumnAttributes.Identity);
        IdentitySeed = seed;
        IdentityIncrement = increment;
    }

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitColumn(this);
    }

    private static ColumnAttributes GetAttributesFromType(ColumnType type)
    {
        var attributes = ColumnAttributes.None;

        switch (type)
        {
            case ColumnType.TinyInt or ColumnType.SmallInt or ColumnType.Integer or
                ColumnType.BigInt or ColumnType.Currency or ColumnType.Decimal:
                attributes |= ColumnAttributes.Numeric | ColumnAttributes.FixedLength;
                break;
            case ColumnType.UnsignedTinyInt or ColumnType.UnsignedSmallInt or
                ColumnType.UnsignedInt or ColumnType.UnsignedBigInt:
                attributes |= ColumnAttributes.Numeric | ColumnAttributes.FixedLength | ColumnAttributes.Unsigned;
                break;
            case ColumnType.SinglePrecision or ColumnType.DoublePrecision:
                attributes |= ColumnAttributes.Numeric;
                break;
            case ColumnType.Char:
                attributes |= ColumnAttributes.Alphabetic | ColumnAttributes.FixedLength;
                break;
            case ColumnType.NChar:
                attributes |= ColumnAttributes.Alphabetic | ColumnAttributes.FixedLength | ColumnAttributes.Unicode;
                break;
            case ColumnType.VarChar:
                attributes |= ColumnAttributes.Alphabetic;
                break;
            case ColumnType.NVarChar:
                attributes |= ColumnAttributes.Alphabetic | ColumnAttributes.Unicode;
                break;
            case ColumnType.Text:
                attributes |= ColumnAttributes.Alphabetic | ColumnAttributes.Binary;
                break;
            case ColumnType.NText:
                attributes |= ColumnAttributes.Alphabetic | ColumnAttributes.Binary | ColumnAttributes.Unicode;
                break;
            case ColumnType.Date or ColumnType.Time or ColumnType.DateTime or ColumnType.Interval:
                attributes |= ColumnAttributes.Temporal;
                break;
            case ColumnType.Bit or ColumnType.Blob or ColumnType.File:
                attributes |= ColumnAttributes.Binary;
                break;
        }

        return attributes;
    }
}