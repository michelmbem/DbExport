using Avalonia.Data.Converters;

namespace DbExport.Gui.Converters;

public static class ValueConverters
{
    public static readonly IValueConverter Int32 = new FuncValueConverter<int?, string?>(
        i => i?.ToString(),
        s => int.TryParse(s, out var i) ? i : null);
}