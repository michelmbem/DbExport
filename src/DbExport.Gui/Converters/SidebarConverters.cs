using System;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace DbExport.Gui.Converters;

public static class SidebarConverters
{
    public static readonly IValueConverter Foreground = new FuncValueConverter<bool, IBrush>(
        selected => selected ? Brushes.Yellow : Brushes.White,
        _ => throw new NotSupportedException());

    public static readonly IValueConverter IconPath = new FuncValueConverter<bool, string>(
        selected => $"/Assets/Images/{(selected ? "" : "un")}selected-item.svg",
        _ => throw new NotSupportedException());
}
