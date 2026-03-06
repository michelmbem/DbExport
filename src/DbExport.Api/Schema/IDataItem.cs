namespace DbExport.Schema;

/// <summary>
/// Represents a data item that encapsulates schema-related properties
/// and metadata for a database column or type definition.
/// </summary>
public interface IDataItem
{
    /// <summary>
    /// Gets the name of the data item.
    /// </summary>
    ColumnType ColumnType { get; }

    /// <summary>
    /// Gets the native type of the data item.
    /// </summary>
    string NativeType { get; }

    /// <summary>
    /// Gets the size (or character length) of the data item.
    /// </summary>
    short Size { get; }

    /// <summary>
    /// Gets the decimal precision of the data item.
    /// </summary>
    byte Precision { get; }

    /// <summary>
    /// Gets the decimal scale of the data item.
    /// </summary>
    byte Scale { get; }
    
    /// <summary>
    /// Gets a value indicating whether the data item is nullable.
    /// </summary>
    bool IsRequired { get; }
    
    /// <summary>
    /// Gets the default value of the data item.
    /// </summary>
    object DefaultValue { get; }

    /// <summary>
    /// Generates the full type name representation for the provided data item.
    /// </summary>
    /// <param name="item">The data item for which the type name needs to be generated.</param>
    /// <param name="native">Determines whether to use the native database type or map to a generic type representation. Defaults to true.</param>
    /// <returns>The full type name, including size, precision, and scale if applicable.</returns>
    static string GetFullTypeName(IDataItem item, bool native = true)
    {
        string typeName;

        if (native)
            typeName = item.NativeType;
        else
        {
            typeName = item.ColumnType.ToString().ToLower();
            if (typeName.StartsWith("unsigned")) return $"{typeName[8..]} unsigned";
        }

        return item.ColumnType switch
        {
            ColumnType.Char or ColumnType.NChar =>  $"{typeName}({item.Size})",
            ColumnType.VarChar => item.Size > 0 ? $"{typeName}({item.Size})" : "text",
            ColumnType.NVarChar => item.Size > 0 ? $"{typeName}({item.Size})" : "ntext",
            ColumnType.Bit => item.Size > 0 ? $"{typeName}({item.Size})" : "blob",
            ColumnType.Decimal when item.Precision == 0 => $"{typeName}",
            ColumnType.Decimal when item.Scale == 0 => $"{typeName}({item.Precision})",
            ColumnType.Decimal => $"{typeName}({item.Precision}, {item.Scale})",
            _ => typeName
        };
    }
}