using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DbExport.Gui.ViewModels;

public partial class ProgressViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? message;
    
    [ObservableProperty]
    private bool isIndeterminate = true;
    
    [ObservableProperty]
    private double minimum;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsComplete))]
    private double maximum = 100;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsComplete))]
    private double value;
    
    public bool IsComplete => Value >= Maximum;
    
    [RelayCommand]
    private void Reset()
    {
        Value = Minimum;
    }
}