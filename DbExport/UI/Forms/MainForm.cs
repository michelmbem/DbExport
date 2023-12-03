using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DbExport.Providers;
using DbExport.Schema;
using DbExport.UI.Controls;
using log4net;

namespace DbExport.UI.Forms
{
    public partial class MainForm : Form
    {
        private const int WORKER_THREAD_DELAY = 100;

        private static readonly ILog log = LogManager.GetLogger(typeof(MainForm));
        
        private readonly WizardPage[] pages;
        private int activePage = -1; // To force the initial execution of OnActivePageChanged
        private string sourceProviderName;
        private string sourceConnectionString;
        private string targetProviderName;
        private string targetConnectionString;
        private ExportOptions options;
        private string[] selectedTableNames;
        private bool freezed;

        public MainForm()
        {
            InitializeComponent();

            pages = new WizardPage[] {wizardPage1, wizardPage2, wizardPage3, wizardPage4, wizardPage5, wizardPage6, wizardPage7};
            ActivePage = 0;
        }

        public int ActivePage
        {
            get { return activePage; }
            set
            {
                if (activePage == value) return;
                if (!OnActivePageChanging(value)) return;
                activePage = value;
                OnActivePageChanged();
            }
        }

        private bool OnActivePageChanging(int nextValue)
        {
            if (nextValue < activePage) return true;

            var result = true;
            var message = string.Empty;

            switch (activePage)
            {
                case 1:
                    sourceProviderName = wizardPage2.ProviderName;
                    sourceConnectionString = wizardPage2.ConnectionString;
                    if (sourceConnectionString.Length <= 0)
                    {
                        message = "The connection settings are invalid or insufficient";
                        result = false;
                    }
                    break;
                case 2:
                    wizardPage4.ProviderName = targetProviderName = wizardPage3.ProviderName;
                    targetConnectionString = wizardPage3.ConnectionString;
                    switch (targetProviderName)
                    {
                        case "System.Data.OleDb":
                        case "LocalDB":
                            if (targetConnectionString.Length <= 0)
                            {
                                message = "For Access and SQL Server data files, you must provide the full path to the target database";
                                result = false;
                            }
                            break;
                    }
                    break;
                case 3:
                    options = wizardPage4.ExportOptions;
                    wizardPage5.Init(sourceProviderName, sourceConnectionString);
                    break;
                case 4:
                    selectedTableNames = wizardPage5.SelectedTableNames;
                    if (selectedTableNames.Length <= 0)
                    {
                        message = "At least, one table should be selected";
                        result = false;
                    }
                    wizardPage6.Summary = GetSummary();
                    break;
                case 5:
                    switch (targetProviderName)
                    {
                        case "System.Data.OleDb":
                        case "LocalDB":
                            wizardPage7.ScripGenerationEnabled = false;
                            break;
                        default:
                            wizardPage7.ScripGenerationEnabled = true;
                            break;
                    }
                    wizardPage7.StatusMessage = string.Empty;
                    break;
            }

            if (!result)
                MessageBox.Show(message, "Invalid settings", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return result;
        }

        private void OnActivePageChanged()
        {
            for (int i = 0; i < pages.Length; ++i)
                pages[i].Visible = i == activePage;

            btnPrevious.Enabled = activePage > 0;
            btnNext.Visible = activePage < pages.Length - 1;
            btnDone.Visible = activePage == pages.Length - 1;
        }

        private string GetSummary()
        {
            var sb = new StringBuilder();
            var spaces = "".PadLeft(4);

            sb.AppendLine("Source Provider:").Append(spaces).AppendLine(sourceProviderName).AppendLine();
            sb.AppendLine("Source Connection String:").Append(spaces).AppendLine(sourceConnectionString).AppendLine();
            sb.AppendLine("Target Provider: ").Append(spaces).AppendLine(targetProviderName).AppendLine();
            sb.AppendLine("Target Connection String:").Append(spaces).AppendLine(targetConnectionString).AppendLine();
            
            if (options.ExportSchema)
            {
                if (options.ExportData)
                    sb.Append("Export schema and data");
                else
                    sb.Append("Export schema only");

                if (options.Flags == ExportFlags.ExportNothing)
                    sb.AppendLine();
                else
                {
                    var comma = false;
                    sb.Append(" (Including ");

                    if ((options.Flags & ExportFlags.ExportPrimaryKeys) != ExportFlags.ExportNothing)
                    {
                        sb.Append("Primary Keys");
                        comma = true;
                    }

                    if ((options.Flags & ExportFlags.ExportIndexes) != ExportFlags.ExportNothing)
                    {
                        if (comma) sb.Append(", ");
                        sb.Append("Indexes");
                        comma = true;
                    }

                    if ((options.Flags & ExportFlags.ExportForeignKeys) != ExportFlags.ExportNothing)
                    {
                        if (comma) sb.Append(", ");
                        sb.Append("Foreign Keys");
                        comma = true;
                    }

                    if ((options.Flags & ExportFlags.ExportDefaults) != ExportFlags.ExportNothing)
                    {
                        if (comma) sb.Append(", ");
                        sb.Append("Default Values");
                        comma = true;
                    }

                    if ((options.Flags & ExportFlags.ExportIdentities) != ExportFlags.ExportNothing)
                    {
                        if (comma) sb.Append(", ");
                        sb.Append("Serial Numbers");
                    }

                    sb.AppendLine(")");
                }
            }
            else
                sb.AppendLine("Export data only");

            sb.AppendLine();
            sb.AppendLine("Export the following objects:");
            sb.AppendLine("---------------------------------------------");
            
            foreach (string tableName in selectedTableNames)
            {
                sb.Append(spaces).Append(tableName).Append(" : ").AppendLine("table");
            }

            return sb.ToString();
        }

        private void Freeze()
        {
            pnlNavBar.Enabled = false;
            Cursor = Cursors.WaitCursor;
            wizardPage7.StartAnimation();
            freezed = true;
        }

        private void Unfreeze()
        {
            pnlNavBar.Enabled = true;
            Cursor = Cursors.Default;
            wizardPage7.StopAnimation();
            freezed = false;
        }

        private void ReportStatus(string statusMessage)
        {
            wizardPage7.StatusMessage = statusMessage;
        }

        private Database GetDatabase()
        {
            ReportStatus("Retrieving the source database's schema...");
            
            Database database = null;
            Exception exception = null;

            var worker = new Thread(() => {
                try
                {
                    database = SchemaProvider.GetDatabase(Utility.GetRealProviderName(sourceProviderName), sourceConnectionString);
                    foreach (string tableName in selectedTableNames)
                    {
                        database.Tables[tableName].Checked = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    exception = ex;
                }
            });
            worker.Start();

            while (worker.IsAlive)
            {
                Application.DoEvents();
                Thread.Sleep(WORKER_THREAD_DELAY);
            }

            if (exception != null) throw exception;

            return database;
        }

        private void GenerateSql(TextWriter output, Database database)
        {
            ReportStatus("Generating the migration script...");

            Exception exception = null;
            var codegen = CodeGenerator.Get(Utility.GetRealProviderName(targetProviderName), output);
            codegen.ExportOptions = options;

            if (codegen is Providers.Npgsql.NpgsqlCodeGenerator && !string.IsNullOrEmpty(targetConnectionString))
            {
                var settings = Utility.ParseConnectionString(targetConnectionString);
                ((Providers.Npgsql.NpgsqlCodeGenerator) codegen).DbOwner = settings["uid"];
            }

            var worker = new Thread(() => {
                try
                {
                    database.AcceptVisitor(codegen);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    exception = ex;
                }
            });
            worker.Start();

            while (worker.IsAlive)
            {
                Application.DoEvents();
                Thread.Sleep(WORKER_THREAD_DELAY);
            }

            if (exception != null) throw exception;
        }

        private void GenerateMdb(Database database)
        {
            ReportStatus("Generating the target database...");

            Exception exception = null;
            
            var builder = new Providers.Access.AccessSchemaBuilder(targetConnectionString) { ExportOptions = options };
            var worker = new Thread(() => {
                try
                {
                    database.AcceptVisitor(builder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    exception = ex;
                }
            });
            worker.Start();

            while (worker.IsAlive)
            {
                Application.DoEvents();
                Thread.Sleep(WORKER_THREAD_DELAY);
            }

            if (exception != null) throw exception;
        }

        private void GenerateMdf(Database database)
        {
            var settings = Utility.ParseConnectionString(targetConnectionString);

            using (var helper1 = new SqlHelper("System.Data.SqlClient", @"Data Source=" + settings["data source"]))
            {
                try { helper1.Execute($"DROP Database {database.Name}"); } catch { }
                helper1.Execute($"CREATE Database {database.Name} ON (Name= N'{database.Name}', FileName='{settings["attachdbfilename"]}')");
            }

            var output = new StringWriter();
            GenerateSql(output, database);

            var script = output.ToString();
            var offset = script.IndexOf("CREATE TABLE ");
            if (offset < 0) offset = script.IndexOf("INSERT INTO ");
            if (offset >= 0) script = script.Substring(offset);

            using (var helper2 = new SqlHelper("System.Data.SqlClient", targetConnectionString))
            {
                helper2.ExecuteScript(script);
            }
        }

        private void ShowSqlEditor()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var sqlEditor = new SqlEditor(wizardPage7.FileName)
                {
                    ProviderName = targetProviderName,
                    ConnectionString = targetConnectionString
                };

                sqlEditor.FormClosed += (sender, e) => Close();
                sqlEditor.Show();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("gedit", wizardPage7.FileName);
                Close();
            }
            else
            {
                Close();
            }
        }

        private bool ShouldSaveToFile()
        {
            return wizardPage7.GenerateScript;
        }

        private void SaveToFile(Database database)
        {
            Utility.Encoding = wizardPage7.Encoding;

            var output = new StreamWriter(wizardPage7.FileName, false, Utility.Encoding);
            GenerateSql(output, database);
            output.Flush();
            output.Close();
        }

        private bool RunScript(Database database)
        {
            if (string.IsNullOrEmpty(targetConnectionString))
            {
                MessageBox.Show("You must provide valid connection settings to the target database before running the script",
                                "Could not run the script", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            switch (targetProviderName)
            {
                case "System.Data.OleDb":
                    GenerateMdb(database);
                    break;
                case "LocalDB":
                    GenerateMdf(database);
                    break;
                default:
                    using (var helper = new SqlHelper(targetProviderName, targetConnectionString))
                    {
                        var output = new StringWriter();
                        GenerateSql(output, database);
                        helper.ExecuteScript(output.ToString());
                    }
                    break;
            }

            MessageBox.Show("Database successfully exported!", "End of task",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = freezed;
        }

        private void pnlNavBar_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.White, 0, 0, ((Control) sender).Width, 0);
            e.Graphics.DrawLine(Pens.DarkGray, 0, 1, ((Control) sender).Width, 1);
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            --ActivePage;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            ++ActivePage;
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            string onComplete = null;

            Freeze();

            try
            {
                var database = GetDatabase();

                if (ShouldSaveToFile())
                {
                    SaveToFile(database);
                    ShowSqlEditor();
                    onComplete = "hide";
                }
                else if (RunScript(database))
                    onComplete = "close";
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Unfreeze();
                ReportStatus(string.Empty);

                switch (onComplete)
                {
                    case "hide":
                        Hide();
                        break;
                    case "close":
                        Close();
                        break;
                }
            }
        }
    }
}