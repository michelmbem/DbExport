using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DbExport.Gui.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Serilog;

namespace DbExport.Gui.ViewModels;

public partial class ConnectionViewModel : ViewModelBase
{
    [ObservableProperty]
    private DataProvider? dataProvider;

    [ObservableProperty]
    private bool isDestination;

    public virtual string? ConnectionString { get; }

    [RelayCommand]
    private async Task TestConnection()
    {
        string message;
        Icon icon;
        
        try
        {
            await using var connection = Utility.GetConnection(DataProvider?.Name, ConnectionString);
            await connection.OpenAsync();
            message = "Test succeeded!";
            icon = Icon.Info;
        }
        catch (Exception e)
        {
            message = "Test failed!";
            icon = Icon.Error;
            
            Log.Warning(e, "Failed to test connection");
        }
        
        await MessageBoxManager
              .GetMessageBoxStandard("Connection Test",
                                     message,
                                     ButtonEnum.Ok,
                                     icon)
              .ShowAsync();
    }
}