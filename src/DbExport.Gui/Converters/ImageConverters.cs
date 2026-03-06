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

    public static readonly IValueConverter FromWindowIcon =
        new FuncValueConverter<WindowIcon?, IImage?>(WindowIcon2Image, Image2WindowIcon);

    #endregion

    #region Helper methods

    private static IImage? WindowIcon2Image(WindowIcon? icon)
    {
        if (icon == null) return null;

        using var stream = new MemoryStream();
        icon.Save(stream);
        stream.Seek(0, SeekOrigin.Begin);

        return new Bitmap(stream);
    }

    private static WindowIcon Image2WindowIcon(IImage? image) => throw new NotSupportedException();

    #endregion
}
