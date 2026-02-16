namespace DbExport.Gui.ViewModels;

public class WizardPage3ViewModel : WizardPage2ViewModel
{
    public WizardPage3ViewModel()
    {
        fileConnection.IsDestination = serverConnection.IsDestination = true;

        SelectedProvider = AllProviders[^1];

        Header.Title = "Target database";
        Header.Description = "Select the database you want to migrate to.";
    }
}