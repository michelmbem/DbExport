using System;

namespace DbExport.Schema;

/// <summary>
/// Specifies a set of attributes that can be associated with a database column.
/// </summary>
/// <remarks>
/// This enumeration is used to define specific metadata about a column,
/// such as whether it is required, part of a key, or represents specific data types.
/// It supports bitwise combination of its values due to the <see cref="System.FlagsAttribute"/>.
/// </remarks>
[Flags]
public enum ColumnAttributes
{
    /// <summary>
    /// Indicates that no specific attributes are associated with the column.
    /// </summary>
    /// <remarks>
    /// This value represents the absence of any column-specific metadata or characteristics.
    /// It serves as the default state for a column when no additional attributes are applied.
    /// </remarks>
    None = 0,

    /// <summary>
    /// Indicates that the column is required and must have a value.
    /// </summary>
    /// <remarks>
    /// This attribute enforces that the column cannot accept null or missing values.
    /// It is typically used to ensure data integrity by preventing the omission of critical information for a particular column.
    /// </remarks>
    Required = 1,

    /// <summary>
    /// Indicates that the column's value is computed by the database.
    /// </summary>
    /// <remarks>
    /// This value signifies that the column does not store user-provided data directly,
    /// but instead its value is derived or calculated based on an expression or formula defined in the database schema.
    /// Computed columns are often used for encapsulating derived data or simplifying queries.
    /// </remarks>
    Computed = 2,

    /// <summary>
    /// Indicates that the column is an identity column.
    /// </summary>
    /// <remarks>
    /// This attribute is used to specify that the column's value is automatically generated
    /// by the database, typically for primary key purposes. The values are often unique
    /// and incremented sequentially.
    /// </remarks>
    Identity = 4,

    /// <summary>
    /// Indicates that the column is part of the primary key of a table.
    /// </summary>
    /// <remarks>
    /// This attribute signifies that the column is used to uniquely identify records within the table.
    /// It ensures that the values in this column are unique and not null.
    /// Columns marked with this attribute are often combined into a composite key when multiple columns constitute the primary key.
    /// </remarks>
    PKColumn = 8,

    /// <summary>
    /// Indicates that the column is a foreign key in the database schema.
    /// </summary>
    /// <remarks>
    /// This attribute identifies the column as referencing a primary key in another table.
    /// It is used to represent relationships between tables in a relational database.
    /// </remarks>
    FKColumn = 16,

    /// <summary>
    /// Indicates that the column is part of an index.
    /// </summary>
    /// <remarks>
    /// This attribute specifies that the column is involved in an indexing operation,
    /// which may improve performance for queries that filter or sort by the column's data.
    /// It is commonly used to identify columns that contribute to database optimization related to search and retrieval operations.
    /// </remarks>
    IXColumn = 32,

    /// <summary>
    /// Indicates that the column holds numeric data.
    /// </summary>
    /// <remarks>
    /// This attribute identifies columns that are designed to store numeric values,
    /// such as integers, decimals, or floating-point numbers. It can be used to
    /// apply specific handling or validation logic related to numeric data types.
    /// </remarks>
    Numeric = 64,

    /// <summary>
    /// Indicates that the column contains alphabetic or textual data.
    /// </summary>
    /// <remarks>
    /// This attribute is used to signify that the data within the column is expected to consist primarily of alphabetic characters.
    /// It provides additional semantic meaning about the type of data stored in the column, which may influence schema generation or validation processes.
    /// </remarks>
    Alphabetic = 128,

    /// <summary>
    /// Specifies that the column has a fixed length.
    /// </summary>
    /// <remarks>
    /// This attribute indicates that the column's data type is configured to hold values of a consistent, predefined length.
    /// It is commonly used to ensure uniformity in storage and validation for string or binary data types.
    /// </remarks>
    FixedLength = 256,

    /// <summary>
    /// Specifies that the column represents an unsigned numeric value.
    /// </summary>
    /// <remarks>
    /// This attribute is used to indicate that the column's numeric data is of an unsigned type,
    /// meaning it does not accept negative values and has a zero-based range.
    /// </remarks>
    Unsigned = 512,

    /// <summary>
    /// Specifies that the column supports or is encoded using Unicode characters.
    /// </summary>
    /// <remarks>
    /// This attribute indicates that the column is designed to store text represented in the Unicode standard,
    /// allowing for a comprehensive range of characters from various scripts and symbols worldwide.
    /// </remarks>
    Unicode = 1024,

    /// <summary>
    /// Indicates that the column is associated with temporal data or is used to track changes over time.
    /// </summary>
    /// <remarks>
    /// This attribute typically signifies that the column is used with features such as temporal tables
    /// or stores date and time values, enabling time-based operations or versioning within the database schema.
    /// </remarks>
    Temporal = 2048,

    /// <summary>
    /// Indicates that the column stores binary data.
    /// </summary>
    /// <remarks>
    /// This attribute specifies that the column is designed to hold binary data, such as images,
    /// files, or other non-textual data types, typically represented as a sequence of bytes.
    /// </remarks>
    Binary = 4096
}