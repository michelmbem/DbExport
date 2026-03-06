namespace DbExport.Schema;

/// <summary>
/// Represents the types of columns that can be used in a database schema.
/// This enumeration provides a comprehensive list of supported data types
/// including numeric, textual, date/time, binary, and user-defined types.
/// </summary>
public enum ColumnType
{
    /// <summary>
    /// Represents an unknown or unspecified column data type.
    /// This value is used as a placeholder when the actual data type cannot be determined or is not provided.
    /// </summary>
    Unknown,

    /// <summary>
    /// Represents a Boolean data type, typically used to store true or false values.
    /// </summary>
    Boolean,

    /// <summary>
    /// Represents a tiny integer data type typically used to store very small integers.
    /// This data type commonly has a storage size of 1 byte and can store values
    /// within a limited range, often from 0 to 255 for unsigned or -128 to 127 for signed.
    /// </summary>
    TinyInt,

    /// <summary>
    /// Represents an 8-bit unsigned integer column type.
    /// This type is used to store non-negative integer values ranging from 0 to 255.
    /// </summary>
    UnsignedTinyInt,

    /// <summary>
    /// Represents a 16-bit signed integer column type in the database schema.
    /// Used for storing smaller numeric values within the range of -32,768 to 32,767.
    /// </summary>
    SmallInt,

    /// <summary>
    /// Represents an unsigned small integer column type in a database schema.
    /// This type is used for storing non-negative integer values with a smaller range compared to standard integers.
    /// </summary>
    UnsignedSmallInt,

    /// <summary>
    /// Represents a column data type that stores integer values in a database schema.
    /// This type is used for whole numbers without fractional or decimal components.
    /// </summary>
    Integer,

    /// <summary>
    /// Represents an unsigned 32-bit integer column data type.
    /// This type is used when the column stores non-negative whole numbers
    /// that fall within the range of an unsigned 32-bit integer.
    /// </summary>
    UnsignedInt,

    /// <summary>
    /// Represents a column data type for large integer values in a database schema.
    /// Typically used to store 64-bit signed integer data, allowing for a wide range of numeric values.
    /// </summary>
    BigInt,

    /// <summary>
    /// Represents a 64-bit unsigned integer column data type.
    /// This type is used for storing large non-negative whole numbers
    /// that require a greater range than 32-bit integers can provide.
    /// </summary>
    UnsignedBigInt,

    /// <summary>
    /// Represents a single-precision floating-point column data type.
    /// Typically used to store approximate numeric values with a 32-bit floating-point format,
    /// suitable for scenarios where reduced precision is acceptable for saving storage space.
    /// </summary>
    SinglePrecision,

    /// <summary>
    /// Represents a double-precision floating-point numeric column type.
    /// This value is typically used for storing high-precision numerical data requiring
    /// 64 bits of storage, adhering to the IEEE 754 standard.
    /// </summary>
    DoublePrecision,

    /// <summary>
    /// Represents a column data type for monetary or currency values.
    /// This type is commonly used to store precise financial data.
    /// </summary>
    Currency,

    /// <summary>
    /// Represents a column data type designed for high-precision fixed-point numeric values.
    /// This type is typically used for financial and monetary calculations where accuracy is critical.
    /// </summary>
    Decimal,

    /// <summary>
    /// Represents a date column data type.
    /// This value is used for columns that store calendar dates without time components.
    /// </summary>
    Date,

    /// <summary>
    /// Represents a column data type that stores time values without a date component.
    /// Typically used to define fields containing time-of-day information.
    /// </summary>
    Time,

    /// <summary>
    /// Represents a column data type used to store both date and time information.
    /// This value is commonly used for data that combines calendar dates with specific timestamps.
    /// </summary>
    DateTime,

    /// <summary>
    /// Represents a time-based interval or duration, typically used to store a span of time
    /// in a database column. This value is suited for scenarios requiring precise measurements
    /// of time elapsed between events or date/time calculations.
    /// </summary>
    Interval,

    /// <summary>
    /// Represents a fixed-length character column data type.
    /// Used for storing text of a specific length, typically for performance
    /// optimization and ensuring consistent data size for string values.
    /// </summary>
    Char,

    /// <summary>
    /// Represents a fixed-length Unicode character column in a database.
    /// Used to store text data with a predefined length and support for multilingual characters.
    /// </summary>
    NChar,

    /// <summary>
    /// Represents a variable-length character column data type.
    /// This type is typically used to store text data of variable length within a defined maximum size.
    /// </summary>
    VarChar,

    /// <summary>
    /// Represents a variable-length, Unicode character data type.
    /// This type is used for storing textual data that may include multilingual characters,
    /// supporting a wide range of character sets using the Unicode standard.
    /// </summary>
    NVarChar,

    /// <summary>
    /// Represents a column data type used for storing large amounts of textual data.
    /// This type is suitable for cases where text values of varying and potentially significant length need to be handled.
    /// </summary>
    Text,

    /// <summary>
    /// Represents a column data type used to store large Unicode text data.
    /// This type is typically employed for columns designed to handle text
    /// strings that exceed standard size limitations.
    /// </summary>
    NText,

    /// <summary>
    /// Represents a column data type used to store binary or logical bit values.
    /// This type is commonly used for fields that store boolean-like states or flag information.
    /// </summary>
    Bit,

    /// <summary>
    /// Represents a binary large object (BLOB) column data type,
    /// typically used for storing variable-length binary data such as images, files, or multimedia.
    /// </summary>
    Blob,

    /// <summary>
    /// Represents a column that stores file data or file-related information.
    /// This type is typically used when the column contains references to file paths,
    /// binary file data, or metadata about files.
    /// </summary>
    File,

    /// <summary>
    /// Represents a column data type designed to store XML data.
    /// This value is used for columns that require structured or semi-structured data
    /// stored in XML format, often used for interoperability or hierarchical data representation.
    /// </summary>
    Xml,

    /// <summary>
    /// Represents a column data type used to store JSON-encoded data.
    /// This type is suitable for columns containing structured or semi-structured data in JSON format.
    /// </summary>
    Json,

    /// <summary>
    /// Represents a spatial data type used to store geometric or geographical information,
    /// such as points, lines, and polygons. This type is typically used for mapping and spatial analysis.
    /// </summary>
    Geometry,

    /// <summary>
    /// Represents a column data type used for storing globally unique identifiers (GUIDs).
    /// Typically used to store unique keys or identifiers across distributed systems.
    /// </summary>
    Guid,

    /// <summary>
    /// Represents a column data type used for tracking and managing versioning of rows in a database table.
    /// Typically utilized for concurrency control to detect changes to data rows.
    /// </summary>
    RowVersion,

    /// <summary>
    /// Represents a user-defined column data type.
    /// This value is used when the column type is specified explicitly by the user,
    /// often for custom or database-specific data types that are not covered by standard types.
    /// </summary>
    UserDefined
}