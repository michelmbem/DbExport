using System.Diagnostics;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using DbExport.Gui.Utilities;

namespace DbExport.Gui.ViewModels;

public partial class AboutDialogViewModel : ViewModelBase
{
    public string Title => AssemblyInfo.Title;
    
    public string? Description => AssemblyInfo.Description;
    
    public string? Company => AssemblyInfo.Company;
    
    public string? Copyright => AssemblyInfo.Copyright;
    
    public string? Version => AssemblyInfo.Version;
    
    public string? GitHubRepoUrl => AssemblyInfo.GitHubRepoUrl;

    [RelayCommand]
    private void OpenGitHubRepo()
    {
        Process.Start(new ProcessStartInfo(GitHubRepoUrl) { UseShellExecute = true });
    }

    [RelayCommand]
    private static void Close(Window window)
    {
        window.Close();
    }
}