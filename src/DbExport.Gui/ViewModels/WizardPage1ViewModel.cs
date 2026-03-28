namespace DbExport.Gui.ViewModels;

public class WizardPage1ViewModel : WizardPageViewModel
{
    public WizardPage1ViewModel()
    {
        Header.Title = "Welcome";
        Header.Description = """
            # Welcome to DbExport.
            
            DbExport is a relational database migration assistant that allows you to transfer
            the structure and/or content of your tables from one database to another.
            It supports eight of the most popular relational database management systems:
            MS Access, MS SQL Server, Oracle, DB2, MySQL, PostgreSQL, Firebird, and SQLite.
            
            Click the "Next" button to start your own migration process.
            """;
    }

    public override bool CanMoveBackward => false;
}
