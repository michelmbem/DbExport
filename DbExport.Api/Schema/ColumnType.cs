namespace DbExport.Schema;

/// <summary>
/// Represents the types of columns that can be used in a database schema.
/// This enumeration provides a comprehensive list of supported data types
/// including numeric, textual, date/time, binary, and user-defined types.
/// </summary>
public enum ColumnType
{
    Unknown,
    Boolean,
    TinyInt,
    UnsignedTinyInt,
    SmallInt,
    UnsignedSmallInt,
    Integer,
    UnsignedInt,
    BigInt,
    UnsignedBigInt,
    SinglePrecision,
    DoublePrecision,
    Currency,
    Decimal,
    Date,
    Time,
    DateTime,
    Interval,
    Char,
    NChar,
    VarChar,
    NVarChar,
    Text,
    NText,
    Bit,
    Blob,
    File,
    Xml,
    Json,
    Geometry,
    Guid,
    RowVersion,
    UserDefined
}