namespace DbExport.Schema;

public interface IDataItem
{
    ColumnType ColumnType { get; }

    string NativeType { get; }

    short Size { get; }

    byte Precision { get; }

    byte Scale { get; }
    
    bool IsRequired { get; }
    
    object DefaultValue { get; }

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