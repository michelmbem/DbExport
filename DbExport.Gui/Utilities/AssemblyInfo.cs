using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

[assembly: AssemblyTitle("DbExport")]
[assembly: AssemblyDescription("A relational database migration wizard")]
[assembly: AssemblyCompany("Addy")]
[assembly: AssemblyCopyright("Copyright © Addy 2014-2026")]
[assembly: AssemblyVersion("3.0.0")]
[assembly: AssemblyFileVersion("3.0.0.0")]
[assembly: AssemblyMetadata("GitHubRepoUrl", "https://github.com/michelmbem/DbExport")]

namespace DbExport.Gui.Utilities;

public static class AssemblyInfo
{
    public static string Title => GetAssemblyAttribute<AssemblyTitleAttribute>()?.Title ??
                                  Path.GetFileNameWithoutExtension(ExecutingAssembly.Location);

    public static string? Description => GetAssemblyAttribute<AssemblyDescriptionAttribute>()?.Description;

    public static string? Company => GetAssemblyAttribute<AssemblyCompanyAttribute>()?.Company;

    public static string? Copyright => GetAssemblyAttribute<AssemblyCopyrightAttribute>()?.Copyright;
    
    public static string? Version => ExecutingAssembly.GetName().Version?.ToString();

    public static string? GitHubRepoUrl => GetAssemblyMetadata("GitHubRepoUrl");

    private static Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();

    private static T? GetAssemblyAttribute<T>() where T : Attribute =>
        ExecutingAssembly.GetCustomAttribute<T>();

    private static IEnumerable<T> GetAssemblyAttributes<T>() where T : Attribute =>
        ExecutingAssembly.GetCustomAttributes<T>();

    private static string? GetAssemblyMetadata(string key) =>
        GetAssemblyAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == key)
            ?.Value;
}