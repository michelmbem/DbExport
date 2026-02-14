using System;
using System.IO;
using Avalonia.Platform;
using RazorEngine;
using RazorEngine.Templating;

namespace DbExport.Gui.Utilities;

public static class RazorTemplateLoader
{
    public static string LoadTemplate<TModel>(string templatePath, TModel model)
    {
        var templateUri = new Uri($"avares://dbexport/Assets/{templatePath}");
        using var templateStream = AssetLoader.Open(templateUri);
        using var templateReader = new StreamReader(templateStream);
        return Engine.Razor.RunCompile(templateReader.ReadToEnd(), templatePath, typeof(TModel), model);
    }
}