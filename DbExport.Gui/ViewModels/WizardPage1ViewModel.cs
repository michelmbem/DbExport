namespace DbExport.Gui.ViewModels;

public class WizardPage1ViewModel : WizardPageViewModel
{
    public WizardPage1ViewModel()
    {
        Header.Title = "Welcome";
        Header.Description =
            """
            # Welcome to DbExport.

            DbExport is a relational database migration wizard that lets you migrate tables
            structure and/or contents from one database to another.
            DbExport supports six of the most popular relational database management systems.
            These are MS Access, MS SQL Server, Oracle, MySQL, PostgreSQL, and SQLite.

            Click "Next" to start your own migration process.
            """;
    }

    public override bool CanMoveBackward => false;
}
