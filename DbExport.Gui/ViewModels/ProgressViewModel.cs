using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DbExport.Gui.ViewModels;

public partial class ProgressViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? message;
    
    [ObservableProperty]
    private bool isIndeterminate;
    
    [ObservableProperty]
    private double minimum = 0;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsComplete))]
    private double maximum = 100;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsComplete))]
    private double value = 0;
    
    public bool IsComplete => Value >= Maximum;
    
    [RelayCommand]
    private void Reset()
    {
        Value = Minimum;
    }
}