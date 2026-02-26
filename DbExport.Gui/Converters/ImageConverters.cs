using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace DbExport.Gui.Converters;

public static class ImageConverters
{
    #region Converters

    public static readonly IValueConverter WindowIcon = new FuncValueConverter<WindowIcon?, IImage?>(WinIcon2Img, Img2WinIcon);

    #endregion

    #region Helper methods

    private static IImage? WinIcon2Img(WindowIcon? icon)
    {
        if (icon == null) return null;

        using var stream = new MemoryStream();
        icon.Save(stream);
        stream.Seek(0, SeekOrigin.Begin);

        return new Bitmap(stream);
    }

    private static WindowIcon? Img2WinIcon(IImage? image) => throw new NotSupportedException();

    #endregion
}
