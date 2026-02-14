namespace DbExport.Gui.ViewModels;

public class WizardPage3ViewModel : WizardPage2ViewModel
{
    public WizardPage3ViewModel()
    {
        fileConnection.IsDestination = serverConnection.IsDestination = true;
    }
}