using CommunityToolkit.Mvvm.ComponentModel;

namespace DbExport.Gui.ViewModels;

public partial class WizardPageViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanMoveForward), nameof(CanMoveBackward))]
    private bool isBusy;
    
    public PageHeaderViewModel Header { get; } = new();
    
    public ProgressViewModel Progress { get; } = new();
    
    public virtual bool CanMoveForward => !IsBusy;
    
    public virtual bool CanMoveBackward => !IsBusy;
}