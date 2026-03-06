using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DbExport.Gui.ViewModels;

public partial class FileConnectionViewModel : ConnectionViewModel
{
    [ObservableProperty]
    private string filePath = string.Empty;
    
    [ObservableProperty]
    private string? username;
    
    [ObservableProperty]
    private string? password;

    public override string ConnectionString =>
        DataProvider?.ConnectionStringFactory.Build(FilePath, null, null, false, Username, Password) ?? string.Empty;

    [RelayCommand]
    private async Task ChooseFile(Window window)
    {
        var fileTypes = ToFileTypeList(DataProvider?.DatabaseListQuery);
        var file = IsDestination
            ? await GetSaveFileName(window, "Save database as...", fileTypes)
            : (await GetOpenFileNames(window, "Open database file", fileTypes)).FirstOrDefault();
        
        if (file is null) return;
        
        FilePath = file;
    }

    private static IReadOnlyList<FilePickerFileType> ToFileTypeList(string? pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern)) return [FilePickerFileTypes.All];
        
        var parts = Utility.Split(pattern, '|');
        var limit = parts.Length - 2;
        List<FilePickerFileType> fileTypes = [];
        
        for (var i = 0; i <= limit; i += 2)
        {
            var name = parts[i];
            var extensions = Utility.Split(parts[i + 1], ';');
            
            fileTypes.Add(new FilePickerFileType(name) { Patterns = extensions });
        }
        
        return [..fileTypes, FilePickerFileTypes.All];
    }

    private static async Task<IEnumerable<string>> GetOpenFileNames(
        Window window, string title, IReadOnlyList<FilePickerFileType> fileTypes, bool allowMultiple = false)
    {
        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = allowMultiple,
            FileTypeFilter = fileTypes
        };

        var files = await window.StorageProvider.OpenFilePickerAsync(options);

        return files.Select(f => f.Path.LocalPath);
    }

    private static async Task<string?> GetSaveFileName(
        Window window, string title, IReadOnlyList<FilePickerFileType> fileTypes)
    {
        var options = new FilePickerSaveOptions
        {
            Title = title,
            DefaultExtension = fileTypes[0].Patterns?[0],
            FileTypeChoices = fileTypes
        };

        var file = await window.StorageProvider.SaveFilePickerAsync(options);

        return file?.Path.LocalPath;
    }
}