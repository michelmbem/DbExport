using Avalonia.Data.Converters;

namespace DbExport.Gui.Converters;

public static class NullableConverters
{
    public static readonly IValueConverter Int32 = new FuncValueConverter<int?, string?>(
        i => i?.ToString() ?? string.Empty,
        s => int.TryParse(s, out var i) ? i : null);
}