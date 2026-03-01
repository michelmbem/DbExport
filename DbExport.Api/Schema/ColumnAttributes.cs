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
    None        = 0,
    Required    = 1,
    Computed    = 2,
    Identity    = 4,
    PKColumn    = 8,
    FKColumn    = 16,
    IXColumn    = 32,
    Numeric     = 64,
    Alphabetic  = 128,
    FixedLength = 256,
    Unsigned    = 512,
    Unicode     = 1024,
    Temporal    = 2048,
    Binary      = 4096
}