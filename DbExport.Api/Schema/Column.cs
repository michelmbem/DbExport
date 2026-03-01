namespace DbExport.Schema;

/// <summary>
/// Represents a column in a database table with metadata and attributes that
/// provide detailed information about its configuration and behavior within the schema.
/// </summary>
/// <remarks>
/// The <c>Column</c> class captures information such as the type, size, precision, scale,
/// attributes, and constraints of a database column. It includes properties that
/// reflect specialized characteristics such as being a primary key, foreign key,
/// computed column, or identity column. Additionally, it supports methods to manage
/// its attributes or configure it as an identity column.
/// </remarks>
/// <param name="table">The table to which this column belongs.</param>
/// <param name="name">The name of the column within the table.</param>
/// <param name="type">The logical type of the column, defined by <c>ColumnType</c>.</param>
/// <param name="nativeType">The database-specific type of the column as a string.</param>
/// <param name="size">The maximum size of the column's data in bytes or characters.</param>
/// <param name="precision">The precision of the column, primarily used for numeric types.</param>
/// <param name="scale">The scale (number of decimal places) of numeric types.</param>
/// <param name="attributes">The attributes that describe the column's behavior and constraints.</param>
/// <param name="defaultValue">The default value defined for the column if no value is provided during insertion.</param>
/// <param name="description">The description or documentation of the column, often used for metadata purposes.</param>
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
    /// <summary>
    /// Gets the type of the column, which is defined by the <c>ColumnType</c> enumeration.
    /// </summary>
    public ColumnType ColumnType { get; } = type;

    /// <summary>
    /// Gets the native type of the column as a string.
    /// </summary>
    public string NativeType { get; } = nativeType;

    /// <summary>
    /// Gets the size (or character length) of the column's data.'
    /// </summary>
    public short Size { get; } = size;

    /// <summary>
    /// Gets the precision of the column, primarily used for numeric types.
    /// </summary>
    public byte Precision { get; } = precision;

    /// <summary>
    /// Gets the scale (number of decimal places) of numeric types.
    /// </summary>
    public byte Scale { get; } = scale;

    /// <summary>
    /// Gets the attributes that describe the column's behavior and constraints.'
    /// </summary>
    public ColumnAttributes Attributes { get; private set; } = attributes | GetAttributesFromType(type);

    /// <summary>
    /// Gets the default value defined for the column if no value is provided during insertion.
    /// </summary>
    public object DefaultValue { get; } = defaultValue;

    /// <summary>
    /// Gets the description or documentation of the column, often used for metadata purposes.
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    /// Gets or sets the seed value for the identity column.
    /// </summary>
    public long IdentitySeed { get; private set; }

    /// <summary>
    /// Gets or sets the increment value for the identity column.
    /// </summary>
    public long IdentityIncrement { get; private set; }

    /// <summary>
    /// Gets the table to which this column belongs.
    /// </summary>
    public Table Table => (Table)Parent;
    
    /// <summary>
    /// Gets the definition of the column's data type if it's user-defined and available in imported schema.
    /// </summary>
    public DataType DataType =>
        Table.Database.DataTypes.TryGetValue(NativeType, out var dataType) ? dataType : null;

    /// <summary>
    /// Gets a value indicating whether the column is nullable.
    /// </summary>
    public bool IsRequired => Attributes.HasFlag(ColumnAttributes.Required);

    /// <summary>
    /// Gets a value indicating whether the column is a computed column.
    /// </summary>
    public bool IsComputed => Attributes.HasFlag(ColumnAttributes.Computed);

    /// <summary>
    /// Gets a value indicating whether the column is an identity column.
    /// </summary>
    public bool IsIdentity => Attributes.HasFlag(ColumnAttributes.Identity);

    /// <summary>
    /// Gets a value indicating whether the column is a row version column.
    /// </summary>
    public bool IsGenerated => IsComputed || IsIdentity || ColumnType == ColumnType.RowVersion;

    /// <summary>
    /// Gets a value indicating whether the column is a primary key column.
    /// </summary>
    public bool IsPKColumn => Attributes.HasFlag(ColumnAttributes.PKColumn);

    /// <summary>
    /// Gets a value indicating whether the column is a foreign key column.
    /// </summary>
    public bool IsFKColumn => Attributes.HasFlag(ColumnAttributes.FKColumn);

    /// <summary>
    /// Gets a value indicating whether the column is a key column.
    /// </summary>
    public bool IsKeyColumn => IsPKColumn || IsFKColumn;

    /// <summary>
    /// Gets a value indicating whether the column is an index column.
    /// </summary>
    public bool IsIndexColumn => Attributes.HasFlag(ColumnAttributes.IXColumn);

    /// <summary>
    /// Gets a value indicating whether the column is of a numeric type.
    /// </summary>
    public bool IsNumeric => Attributes.HasFlag(ColumnAttributes.Numeric);

    /// <summary>
    /// Gets a value indicating whether the column is of an alphabetic type.
    /// </summary>
    public bool IsAlphabetic => Attributes.HasFlag(ColumnAttributes.Alphabetic);

    /// <summary>
    /// Gets a value indicating whether the column is fixed-length.
    /// </summary>
    public bool IsFixedLength => Attributes.HasFlag(ColumnAttributes.FixedLength);

    /// <summary>
    /// Gets a value indicating whether the column is of an unsigned integer type.
    /// </summary>
    public bool IsUnsigned => Attributes.HasFlag(ColumnAttributes.Unsigned);

    /// <summary>
    /// Gets a value indicating whether the column is of a Unicode type.
    /// </summary>
    public bool IsUnicode => Attributes.HasFlag(ColumnAttributes.Unicode);

    /// <summary>
    /// Gets a value indicating whether the column is of an integral type.
    /// </summary>
    public bool IsIntegral => IsNumeric && IsFixedLength;

    /// <summary>
    /// Gets a value indicating whether the column is of a natural integer type, i.e., an unsigned integer
    /// </summary>
    public bool IsNatural => IsIntegral && IsUnsigned;

    /// <summary>
    /// Gets a value indicating whether the column is of a temporal type.
    /// </summary>
    public bool IsTemporal => Attributes.HasFlag(ColumnAttributes.Temporal);

    /// <summary>
    /// Gets a value indicating whether the column is of a binary type.
    /// </summary>
    public bool IsBinary => Attributes.HasFlag(ColumnAttributes.Binary);

    public bool IsChecked { get; set; }

    /// <summary>
    /// Sets the attribute of the column.
    /// </summary>
    /// <param name="attribute">The attribute to set.</param>
    public void SetAttribute(ColumnAttributes attribute)
    {
        Attributes |= attribute;
    }

    /// <summary>
    /// Configures the column as an identity column with a specified seed and increment.
    /// </summary>
    /// <param name="seed">The initial value of the identity column.</param>
    /// <param name="increment">The step value used to increment the identity column.</param>
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

    /// <summary>
    /// Retrieves the attributes associated with the specified column type.
    /// </summary>
    /// <param name="type">The type of the column for which attributes are being retrieved.</param>
    /// <returns>The attributes derived from the specified column type.</returns>
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