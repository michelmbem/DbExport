using System;
using System.IO;
using Avalonia.Platform;
using RazorEngine;
using RazorEngine.Templating;

namespace DbExport.Gui.Utilities;

public static class RazorTemplateLoader
{
    public static string LoadTemplate<TModel>(string templateName, TModel model)
    {
        var templateUri = new Uri($"avares://dbexport/Assets/Templates/{templateName}.razor");
        using var templateStream = AssetLoader.Open(templateUri);
        using var templateReader = new StreamReader(templateStream);
        return Engine.Razor.RunCompile(templateReader.ReadToEnd(), templateName, typeof(TModel), model);
    }
}