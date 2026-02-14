namespace DbExport.Gui.ViewModels;

public class WizardPageViewModel : ViewModelBase
{
    public ProgressViewModel Progress { get; } = new();
    
    public virtual bool CanMoveForward => true;
    
    public virtual bool CanMoveBackward => true;
}