using System.Diagnostics;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;

namespace DbExport.Gui.ViewModels;

public partial class AboutDialogViewModel : ViewModelBase
{
    [RelayCommand]
    private static void OpenGitHubRepo(string repoUrl)
    {
        Process.Start(new ProcessStartInfo(repoUrl) { UseShellExecute = true });
    }

    [RelayCommand]
    private static void Close(Window window)
    {
        window.Close();
    }
}