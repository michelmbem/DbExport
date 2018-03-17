namespace DbExport.Schema
{
    public class Column : SchemaItem
    {
        public Column(Table table, string name, ColumnType type, string nativeType,
            short size, byte precision, byte scale, ColumnAttribute attributes,
            object defaultValue, string description)
            : base(table, name)
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

        public ColumnType ColumnType { get; private set; }

        public string NativeType { get; private set; }

        public short Size { get; private set; }

        public byte Precision { get; private set; }

        public byte Scale { get; private set; }

        public long IdentitySeed { get; private set; }

        public long IdentityIncrement { get; private set; }

        public ColumnAttribute Attributes { get; private set; }

        public object DefaultValue { get; private set; }

        public string Description { get; private set; }

        public Table Table
        {
            get { return (Table) Parent; }
        }

        public bool IsRequired
        {
            get { return (Attributes & ColumnAttribute.Required) != ColumnAttribute.None; }
        }

        public bool IsComputed
        {
            get { return (Attributes & ColumnAttribute.Computed) != ColumnAttribute.None; }
        }

        public bool IsIdentity
        {
            get { return (Attributes & ColumnAttribute.Identity) != ColumnAttribute.None; }
        }

        public bool IsGenerated
        {
            get { return IsComputed || IsIdentity || ColumnType == ColumnType.RowVersion; }
        }

        public bool IsPKColumn
        {
            get { return (Attributes & ColumnAttribute.PKColumn) != ColumnAttribute.None; }
        }

        public bool IsFKColumn
        {
            get { return (Attributes & ColumnAttribute.FKColumn) != ColumnAttribute.None; }
        }

        public bool IsKeyColumn
        {
            get { return IsPKColumn || IsFKColumn; }
        }

        public bool IsIndexColumn
        {
            get { return (Attributes & ColumnAttribute.IXColumn) != ColumnAttribute.None; }
        }

        public bool IsNumeric
        {
            get { return (Attributes & ColumnAttribute.Numeric) != ColumnAttribute.None; }
        }

        public bool IsAlphabetic
        {
            get { return (Attributes & ColumnAttribute.Alphabetic) != ColumnAttribute.None; }
        }

        public bool IsFixedLength
        {
            get { return (Attributes & ColumnAttribute.FixedLength) != ColumnAttribute.None; }
        }

        public bool IsUnsigned
        {
            get { return (Attributes & ColumnAttribute.Unsigned) != ColumnAttribute.None; }
        }

        public bool IsUnicode
        {
            get { return (Attributes & ColumnAttribute.Unicode) != ColumnAttribute.None; }
        }

        public bool IsIntegral
        {
            get { return IsNumeric && IsFixedLength; }
        }

        public bool IsNatural
        {
            get { return IsIntegral && IsUnsigned; }
        }

        public bool IsTemporal
        {
            get { return (Attributes & ColumnAttribute.Temporal) != ColumnAttribute.None; }
        }

        public bool IsBinary
        {
            get { return (Attributes & ColumnAttribute.Binary) != ColumnAttribute.None; }
        }

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
                case ColumnType.TinyInt:
                case ColumnType.SmallInt:
                case ColumnType.Integer:
                case ColumnType.BigInt:
                case ColumnType.Currency:
                case ColumnType.Decimal:
                    attributes |= ColumnAttribute.Numeric | ColumnAttribute.FixedLength;
                    break;
                case ColumnType.UnsignedTinyInt:
                case ColumnType.UnsignedSmallInt:
                case ColumnType.UnsignedInt:
                case ColumnType.UnsignedBigInt:
                    attributes |= ColumnAttribute.Numeric | ColumnAttribute.FixedLength | ColumnAttribute.Unsigned;
                    break;
                case ColumnType.SinglePrecision:
                case ColumnType.DoublePrecision:
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
                case ColumnType.Date:
                case ColumnType.Time:
                case ColumnType.DateTime:
                case ColumnType.Interval:
                    attributes |= ColumnAttribute.Temporal;
                    break;
                case ColumnType.Bit:
                case ColumnType.Blob:
                case ColumnType.File:
                    attributes |= ColumnAttribute.Binary;
                    break;
            }

            return attributes;
        }
    }
}
