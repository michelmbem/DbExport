namespace DbExport.Schema;

public class Column : SchemaItem, ICheckable
{
    public Column(Table table, string name, ColumnType type, string nativeType,
                  short size, byte precision, byte scale, ColumnAttribute attributes,
                  object defaultValue, string description) :
        base(table, name)
    {
        ColumnType = type;
        NativeType = nativeType;
        Size = size;
        Precision = precision;
        Scale = scale;
        Attributes = attributes | GetAttributesFromType(type);
        DefaultValue = defaultValue;
        Description = description;
    }

    public bool IsChecked { get; set; }

    public ColumnType ColumnType { get; }

    public string NativeType { get; }

    public short Size { get; }

    public byte Precision { get; }

    public byte Scale { get; }

    public long IdentitySeed { get; private set; }

    public long IdentityIncrement { get; private set; }

    public ColumnAttribute Attributes { get; private set; }

    public object DefaultValue { get; }

    public string Description { get; }

    public Table Table => (Table) Parent;

    public bool IsRequired => Attributes.HasFlag(ColumnAttribute.Required);

    public bool IsComputed => Attributes.HasFlag(ColumnAttribute.Computed);

    public bool IsIdentity => Attributes.HasFlag(ColumnAttribute.Identity);

    public bool IsGenerated => IsComputed || IsIdentity || ColumnType == ColumnType.RowVersion;

    public bool IsPKColumn => Attributes.HasFlag(ColumnAttribute.PKColumn);

    public bool IsFKColumn => Attributes.HasFlag(ColumnAttribute.FKColumn);

    public bool IsKeyColumn => IsPKColumn || IsFKColumn;

    public bool IsIndexColumn => Attributes.HasFlag(ColumnAttribute.IXColumn);

    public bool IsNumeric => Attributes.HasFlag(ColumnAttribute.Numeric);

    public bool IsAlphabetic => Attributes.HasFlag(ColumnAttribute.Alphabetic);

    public bool IsFixedLength => Attributes.HasFlag(ColumnAttribute.FixedLength);

    public bool IsUnsigned => Attributes.HasFlag(ColumnAttribute.Unsigned);

    public bool IsUnicode => Attributes.HasFlag(ColumnAttribute.Unicode);

    public bool IsIntegral => IsNumeric && IsFixedLength;

    public bool IsNatural => IsIntegral && IsUnsigned;

    public bool IsTemporal => Attributes.HasFlag(ColumnAttribute.Temporal);

    public bool IsBinary => Attributes.HasFlag(ColumnAttribute.Binary);

    public void SetAttribute(ColumnAttribute attribute)
    {
        Attributes |= attribute;
    }

    public void MakeIdentity(long seed, long increment)
    {
        SetAttribute(ColumnAttribute.Identity);
        IdentitySeed = seed;
        IdentityIncrement = increment;
    }

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitColumn(this);
    }

    private static ColumnAttribute GetAttributesFromType(ColumnType type)
    {
        var attributes = ColumnAttribute.None;

        switch (type)
        {
            case ColumnType.TinyInt or ColumnType.SmallInt or ColumnType.Integer or
                ColumnType.BigInt or ColumnType.Currency or ColumnType.Decimal:
                attributes |= ColumnAttribute.Numeric | ColumnAttribute.FixedLength;
                break;
            case ColumnType.UnsignedTinyInt or ColumnType.UnsignedSmallInt or
                ColumnType.UnsignedInt or ColumnType.UnsignedBigInt:
                attributes |= ColumnAttribute.Numeric | ColumnAttribute.FixedLength | ColumnAttribute.Unsigned;
                break;
            case ColumnType.SinglePrecision or ColumnType.DoublePrecision:
                attributes |= ColumnAttribute.Numeric;
                break;
            case ColumnType.Char:
                attributes |= ColumnAttribute.Alphabetic | ColumnAttribute.FixedLength;
                break;
            case ColumnType.NChar:
                attributes |= ColumnAttribute.Alphabetic | ColumnAttribute.FixedLength | ColumnAttribute.Unicode;
                break;
            case ColumnType.VarChar:
                attributes |= ColumnAttribute.Alphabetic;
                break;
            case ColumnType.NVarChar:
                attributes |= ColumnAttribute.Alphabetic | ColumnAttribute.Unicode;
                break;
            case ColumnType.Text:
                attributes |= ColumnAttribute.Alphabetic | ColumnAttribute.Binary;
                break;
            case ColumnType.NText:
                attributes |= ColumnAttribute.Alphabetic | ColumnAttribute.Binary | ColumnAttribute.Unicode;
                break;
            case ColumnType.Date or ColumnType.Time or ColumnType.DateTime or ColumnType.Interval:
                attributes |= ColumnAttribute.Temporal;
                break;
            case ColumnType.Bit or ColumnType.Blob or ColumnType.File:
                attributes |= ColumnAttribute.Binary;
                break;
        }

        return attributes;
    }
}