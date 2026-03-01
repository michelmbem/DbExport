using System.Collections.Generic;
using System.Collections.Immutable;

namespace DbExport.Schema;

/// <summary>
/// Represents a user-defined database data type with associated metadata such as size, precision, scale, and other characteristics.
/// </summary>
/// <remarks>
/// This class encapsulates schema metadata for a database column type. It holds information such as its owner,
/// size-related properties (size, precision, and scale), nullability, enumerated status, default value, and
/// possible values. The class extends <c>SchemaItem</c> and implements <c>IDataItem</c> and <c>ICheckable</c>.
/// </remarks>
/// <param name="database">The parent <c>Database</c> object associated with the data type.</param>
/// <param name="name">The name of the data type.</param>
/// <param name="owner">The owning schema or namespace of the data type.</param>
/// <param name="type">The column type, specified as an enum of <c>ColumnType</c>.</param>
/// <param name="nativeType">The native database-specific type definition.</param>
/// <param name="size">The size of the data type, typically applicable for text or binary data.</param>
/// <param name="precision">The precision value, applicable for numeric types.</param>
/// <param name="scale">The scale value, applicable for numeric types with fractional parts.</param>
/// <param name="nullable">Indicates whether the data type is nullable.</param>
/// <param name="enumerated">Indicates whether the data type is an enumerated (enum) type.</param>
/// <param name="defaultValue">The default value of the data type, if any.</param>
/// <param name="possibleValues">A collection of possible values for the data type, if it is enumerated.</param>
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
    : SchemaItem(database, name), IDataItem, ICheckable
{
    /// <summary>
    /// The owner of the data type.
    /// </summary>
    public string Owner { get; } = owner;

    /// <summary>
    /// The column type of the data type.
    /// </summary>
    public ColumnType ColumnType { get; } = type;

    /// <summary>
    /// The native database-specific type definition.
    /// </summary>
    public string NativeType { get; } = nativeType;

    /// <summary>
    /// The size (or character length) of the data type.
    /// </summary>
    public short Size { get; } = size;

    /// <summary>
    /// The decimal precision of the data type.
    /// </summary>
    public byte Precision { get; } = precision;

    /// <summary>
    /// The decimal scale of the data type.
    /// </summary>
    public byte Scale { get; } = scale;

    /// <summary>
    /// Indicates whether the data type is nullable.
    /// </summary>
    public bool IsNullable { get; } = nullable;
    
    /// <summary>
    /// Indicates whether the data type is an enumerated (enum) type.
    /// </summary>
    public bool IsEnumerated { get; } = enumerated;

    /// <summary>
    /// The default value of the data type, if any.
    /// </summary>
    public object DefaultValue { get; } = defaultValue;
    
    /// <summary>
    /// A collection of possible values for the data type, if it is enumerated.
    /// </summary>
    public ImmutableHashSet<object> PossibleValues { get; } = ImmutableHashSet.CreateRange(possibleValues);

    /// <summary>
    /// The database that owns the data type.
    /// </summary>
    public Database Database => (Database)Parent;
    
    /// <summary>
    /// Gets a value indicating whether the data type is required.
    /// </summary>
    public bool IsRequired => !IsNullable;

    public bool IsChecked { get; set; }
    
    public override string FullName => string.IsNullOrEmpty(Owner) ? Name : $"{Owner}.{Name}";

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitDataType(this);
    }
}