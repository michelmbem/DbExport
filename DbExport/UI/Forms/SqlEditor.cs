using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;
using log4net;
using ScintillaNET;
using ScintillaNET_FindReplaceDialog;
using ScintillaPrinting;

namespace DbExport.UI.Forms
{
    public partial class SqlEditor : Form
    {
        private const int VK_CAPITAL = 0x14;
        private const int VK_INSERT = 0x2D;
        private const int VK_NUMLOCK = 0x90;
        private const string DEFAULT_FONT_FACE = "Consolas";
        private const int DEFAULT_FONT_SIZE = 11;
        private const string COMMENT_FONT_FACE = "Comic Sans MS";
        private const int COMMENT_FONT_SIZE = DEFAULT_FONT_SIZE - 2;
        private const int COMMENT_COLOR = 0x00008000;
        private const int NUMBER_COLOR = 0x00FF8000;
        private const int OPERATOR_COLOR = 0x00000080;
        private const int CHARACTER_COLOR = 0x00808080;
        private const int MARGIN_BACKGROUND = 0x00EFEFEF;
        private const int MARGIN_FOREGROUND = 0x007F7F7F;
        private const int NUMBER_MARGIN = 1;
        private const int BOOKMARK_MARGIN = 2;
        private const int BOOKMARK_MARKER = 2;
        private const int FOLDING_MARGIN = 3;

        private static readonly ILog log = LogManager.GetLogger(typeof(SqlEditor));

        private Scintilla sciEditor;
        private FindReplace findReplace;
        private IncrementalSearcher incrementalSearcher;
        private Printing printing;
        private List<CustomCommand> customCommands;
        private string fileName;
        private bool saved = true;
        
        public SqlEditor()
        {
            InitializeComponent();
            InitializeStyling();
            InitFindReplace();
            InitPrinting();
        }

        #region Initialization of the Scintilla Control

        private void InitializeStyling()
        {
            sciEditor = new Scintilla
            {
                LexerName = "sql",
                ScrollWidth = 1,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                TabIndex = 1,
            };

            pnlContent.Controls.Add(sciEditor);
            sciEditor.BringToFront();

            sciEditor.Styles[Style.Sql.Default].Font = DEFAULT_FONT_FACE;
            sciEditor.Styles[Style.Sql.Default].Size = DEFAULT_FONT_SIZE;

            sciEditor.Styles[Style.Sql.Comment].Font = COMMENT_FONT_FACE;
            sciEditor.Styles[Style.Sql.Comment].Size = COMMENT_FONT_SIZE;
            sciEditor.Styles[Style.Sql.Comment].ForeColor = Color.FromArgb(COMMENT_COLOR);
            
            sciEditor.Styles[Style.Sql.CommentLine].Font = COMMENT_FONT_FACE;
            sciEditor.Styles[Style.Sql.CommentLine].Size = COMMENT_FONT_SIZE;
            sciEditor.Styles[Style.Sql.CommentLine].ForeColor = Color.FromArgb(COMMENT_COLOR);
            
            sciEditor.Styles[Style.Sql.CommentDoc].Font = COMMENT_FONT_FACE;
            sciEditor.Styles[Style.Sql.CommentDoc].Size = COMMENT_FONT_SIZE;
            sciEditor.Styles[Style.Sql.CommentDoc].ForeColor = Color.FromArgb(COMMENT_COLOR);
            
            sciEditor.Styles[Style.Sql.Number].ForeColor = Color.FromArgb(NUMBER_COLOR);

            sciEditor.Styles[Style.Sql.Word].ForeColor = Color.Blue;
            sciEditor.Styles[Style.Sql.Word].Bold = true;
            
            sciEditor.Styles[Style.Sql.Character].ForeColor = Color.FromArgb(CHARACTER_COLOR);

            sciEditor.Styles[Style.Sql.Operator].ForeColor = Color.FromArgb(OPERATOR_COLOR);
            sciEditor.Styles[Style.Sql.Operator].Bold = true;

            sciEditor.SetKeywords(0, @"
                    abs absolute access acos add add_months adddate admin after aggregate all allocate alter and any app_name are array
                    as asc ascii asin assertion at atan atn2 audit authid authorization auto_increment autonomous_transaction avg before
                    begin benchmark between bfile bfilename bigint bigserial bin binary binary_checksum binary_integer bit bit_count
                    bit_and bit_or blob body bool boolean both breadth bulk by bytea call cascade cascaded case cast catalog ceil ceiling
                    char char_base character charindex charset chartorowid check checkpoint checksum checksum_agg chr class clob close
                    cluster coalesce col_length col_name collate collation collect column comment commit completion compress concat
                    concat_ws connect connection constant constraint constraints constructorcreate contains containsable continue conv
                    convert corr corresponding cos cot count count_big covar_pop covar_samp create cross cube cume_dist current current_date
                    current_path current_role current_time current_timestamp current_user currval cursor cycle data datalength databasepropertyex
                    database date date_add date_format date_sub dateadd datediff datename datepart datetime day db_id db_name deallocate dec
                    declare decimal decode default deferrable deferred degrees delete dense_rank depth deref desc describe descriptor destroy
                    destructor deterministic diagnostics dictionary disconnect difference distinct do domain double drop dump dynamic each
                    else elsif empth encode encrypt end end-exec engine equals escape every except exception exclusive exec execute exists
                    exit exp export_set extends external extract false fetch first first_value file float floor file_id file_name filegroup_id
                    filegroup_name filegroupproperty fileproperty for forall foreign format formatmessage found freetexttable from from_days
                    fulltextcatalog fulltextservice function general get get_lock getdate getansinull getutcdate global go goto grant greatest
                    group grouping guid having heap hex hextoraw host host_id host_name hour ident_incr ident_seed ident_current identified
                    identity if ifnull ignore image immediate in increment index index_col indexproperty indicator initcap initial initialize
                    initially inner inout input insert instr instrb int integer interface intersect interval into is is_member is_srvrolemember
                    is_null is_numeric isdate isnull isolation iterate java join key lag language large last last_day last_value lateral lcase
                    lead leading least left len length lengthb less level like limit limited ln lpad local localtime localtimestamp locator
                    lock log log10 long longblob longtext loop lower ltrim make_ref map match max maxextents mediumblob mediumint mediumtext
                    mid min minus minute mlslabel mod mode modifies modify module money month months_between names national natural naturaln
                    nchar nclob new new_time newid next next_day nextval no noaudit nocompress nocopy none not nowait ntext null nullif number
                    number_base numeric nvarchar nvarchar2 nvl nvl2 object object_id object_name object_property ocirowid oct of off offline
                    old on online only opaque open operator operation option or ord order ordinalityorganization others out outer output owner
                    package pad parameter parameters partial partition path pctfree percent_rank pi pls_integer positive positiven postfix pow
                    power pragma precision prefix preorder prepare preserve primary prior private privileges procedure public radians raise
                    raiserror rand range rank ratio_to_export raw rawtohex read reads real record recursive ref references referencing reftohex
                    relative release release_lock rename repeat replace resource restrict result return returns reverse revoke right rollback
                    rollup round routine row row_number rowid rowidtochar rowlabel rownum rows rowtype rpad rtrim savepoint schema scroll scope
                    search second section seddev_samp select separate sequence serial session session_user set sets share shortblob shorttext
                    sign sin sinh size smalldatetime smallint smallmoney some soundex space specific specifictype sql sqlcode sqlerrm sqlexception
                    sqlstate sqlwarning sqrt start state statement static std stddev stdev_pop strcmp structure subdate substr substrb substring
                    substring_index subtype successful sum synonym sys_context sys_guid sysdate system_user table tan tanh temporary terminate
                    text than then time timestamp timezone_abbr timezone_minute timezone_hour timezone_region tinyint to to_char to_date to_days
                    to_number to_single_byte trailing transaction translate translation treat trigger trim true trunc truncate type ucase uid
                    under union unique uniqueidentifier unknown unnest unsigned update upper usage use user userenv using uuid validate value
                    values var_pop var_samp varbinary varchar varchar2 variable variance varying view vsize when whenever where with without
                    while with work write year zone
            ");

            sciEditor.Styles[Style.LineNumber].BackColor = Color.FromArgb(MARGIN_BACKGROUND);
            sciEditor.Styles[Style.LineNumber].ForeColor = Color.FromArgb(MARGIN_FOREGROUND);
            sciEditor.Styles[Style.IndentGuide].ForeColor = Color.FromArgb(MARGIN_FOREGROUND);
            sciEditor.Styles[Style.IndentGuide].BackColor = Color.FromArgb(MARGIN_BACKGROUND);

            Margin lineNumbers = sciEditor.Margins[NUMBER_MARGIN];
            lineNumbers.Type = MarginType.Number;
            lineNumbers.Mask = 0;
            lineNumbers.Sensitive = true;
            lineNumbers.Width = 35;

            sciEditor.MarginClick += sciEditor_MarginClick;

            Margin bookmarks = sciEditor.Margins[BOOKMARK_MARGIN];
            bookmarks.Type = MarginType.Symbol;
            bookmarks.Mask = (1 << BOOKMARK_MARKER);
            bookmarks.Sensitive = true;
            bookmarks.Width = 15;

            Marker marker = sciEditor.Markers[BOOKMARK_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(Color.Blue);
            marker.SetForeColor(Color.White);
            marker.SetAlpha(100);

            sciEditor.SetFoldMarginColor(true, Color.FromArgb(MARGIN_BACKGROUND));
            sciEditor.SetFoldMarginHighlightColor(true, Color.FromArgb(MARGIN_BACKGROUND));
            sciEditor.SetProperty("fold", "1");
            sciEditor.SetProperty("fold.compact", "1");

            Margin codeFolding = sciEditor.Margins[FOLDING_MARGIN];
            codeFolding.Type = MarginType.Symbol;
            codeFolding.Mask = Marker.MaskFolders;
            codeFolding.Sensitive = true;
            codeFolding.Width = 10;

            // styles for [+] and [-]
            for (int i = 25; i <= 31; i++)
            {
                sciEditor.Markers[i].SetForeColor(Color.FromArgb(MARGIN_BACKGROUND));
                sciEditor.Markers[i].SetBackColor(Color.FromArgb(MARGIN_FOREGROUND));
            }

            // Configure folding markers with respective symbols
            sciEditor.Markers[Marker.Folder].Symbol = MarkerSymbol.CirclePlus;
            sciEditor.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.CircleMinus;
            sciEditor.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.CirclePlusConnected;
            sciEditor.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            sciEditor.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.CircleMinusConnected;
            sciEditor.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            sciEditor.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            sciEditor.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }

        private void InitFindReplace()
        {
            findReplace = new FindReplace { Scintilla = sciEditor };
            findReplace.KeyPressed += findReplace_KeyPressed;

            incrementalSearcher = new IncrementalSearcher
            {
                Scintilla = sciEditor,
                FindReplace = findReplace,
                AutoPosition = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ToolItem = true,
                Visible = false,
                TabIndex = 0,
            };

            pnlContent.Controls.Add(incrementalSearcher);
            incrementalSearcher.BringToFront();
        }

        private void InitPrinting()
        {
            printing = new Printing(sciEditor);
        }

        #endregion

        public SqlEditor(string fileName) : this()
        {
            Open(fileName);
        }

        public string ProviderName { get; set; }

        public string ConnectionString { get; set; }

        public string FileName
        {
            get { return fileName; }
            set
            {
                if (value == fileName) return;
                fileName = value;
                OnFileNameChanged();
            }
        }

        [DllImport("user32")]
        private static extern short GetKeyState(int keyCode);

        private void OnFileNameChanged()
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Text = "DbExport's SQL Editor";
                statusItemMain.Text = string.Empty;
            }
            else
            {
                Text = "DbExport's SQL Editor - " + Path.GetFileName(fileName);
                statusItemMain.Text = fileName;
            }
        }

        private void Clear()
        {
            FileName = sciEditor.Text = string.Empty;
            saved = true;
            UpdateAll();
        }

        private void Open(string path)
        {
            using (var sr = new StreamReader(path, Utility.Encoding))
            {
                sciEditor.Text = sr.ReadToEnd();
                sr.Close();
            }

            FileName = path;
            saved = true;
            UpdateAll();
        }

        private void SaveTo(string path)
        {
            using (var sw = new StreamWriter(path, false, Utility.Encoding))
            {
                sw.Write(sciEditor.Text);
                sw.Flush();
                sw.Close();
            }

            FileName = path;
            saved = true;
            UpdateAll();
        }

        private void UpdateAll()
        {
            sciEditor.EmptyUndoBuffer();
            UpdateUndoRedo();
            UpdateCutCopyCaretInfo();
        }

        private void UpdateUndoRedo()
        {
            tsbUndo.Enabled = undoToolStripMenuItem.Enabled = sciEditor.CanUndo;
            tsbRedo.Enabled = redoToolStripMenuItem.Enabled = sciEditor.CanRedo;
        }

        private void UpdateCutCopyCaretInfo()
        {
            clearToolStripMenuItem.Enabled = cutToolStripMenuItem.Enabled
                                           = copyToolStripMenuItem.Enabled
                                           = tsbCut.Enabled
                                           = tsbCopy.Enabled
                                           = sciEditor.SelectedText.Length > 0;

            statusItemCaretInfo.Text = string.Format("Ln: {0}, Col: {1}, Sel: {2}",
                                                     sciEditor.CurrentLine + 1,
                                                     sciEditor.GetColumn(sciEditor.CurrentPosition) + 1,
                                                     sciEditor.SelectedText.Length);
        }

        private bool PromptToSave()
        {
            var answer = MessageBox.Show("Save the script before exiting?",
                                         Text,
                                         MessageBoxButtons.YesNoCancel,
                                         MessageBoxIcon.Question);

            switch (answer)
            {
                case DialogResult.Yes:
                    saveToolStripMenuItem_Click(null, null);
                    return true;
                case DialogResult.Cancel:
                    return false;
                default:
                    return true;
            }
        }

        private void LoadCustomCommands()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DbExport", "commands.xml");

            if (File.Exists(path))
            {
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                var deserializer = new XmlSerializer(typeof(List<CustomCommand>));
                customCommands = (List<CustomCommand>) deserializer.Deserialize(stream);
                stream.Close();
            }
            else
            {
                customCommands = new List<CustomCommand>();

                var row = new CustomCommand
                    {
                        Name = "sqlcmd",
                        Description = "Run SqlCmd",
                        Command = "SqlCmd",
                        Arguments = "-S ${server} -U ${uid} -P ${pwd} -i \"${filePath}\""
                    };
                customCommands.Add(row);

                row = new CustomCommand
                    {
                        Name = "sqlplus",
                        Description = "Run SQL*Plus",
                        Command = "sqlplus",
                        Arguments = "${uid}/${pwd}@${server} @\"${filePath}\""
                    };
                customCommands.Add(row);

                row = new CustomCommand
                    {
                        Name = "mysql",
                        Description = "MySQL Command Line",
                        Command = "mysql",
                        Arguments = "-h ${server} -u ${uid} �p ${pwd} < \"${filePath}\""
                    };
                customCommands.Add(row);

                row = new CustomCommand
                    {
                        Name = "psql",
                        Description = "PostgreSQL Command Line",
                        Command = "psql",
                        Arguments = "-h ${server} -U ${uid} -d ${dbName} -f \"${filePath}\""
                    };
                customCommands.Add(row);
            }

            CreateCustomToolMenuItems();
        }

        private void SaveCustomCommands()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DbExport");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = Path.Combine(path, "commands.xml");

            var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            var serializer = new XmlSerializer(typeof(List<CustomCommand>));
            serializer.Serialize(stream, customCommands);
            stream.Close();
        }

        private void CreateCustomToolMenuItems()
        {
            while (toolsToolStripMenuItem.DropDownItems.Count > 2)
            {
                var ddi = toolsToolStripMenuItem.DropDownItems[2];
                toolsToolStripMenuItem.DropDownItems.Remove(ddi);
                ddi.Dispose();
            }

            foreach (CustomCommand row in customCommands)
            {
                var menuItem = new ToolStripMenuItem(row.Description) { Tag = row };
                menuItem.Click += (s1, e1) =>
                    {
                        var cmdRow = (CustomCommand) ((ToolStripMenuItem) s1).Tag;
                        RunCommand(cmdRow.Command, cmdRow.Arguments);
                    };
                toolsToolStripMenuItem.DropDownItems.Add(menuItem);
            }
        }

        private void RunCommand(string command, string argsFormat)
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                MessageBox.Show("You must configure the connection to the target database before running this command",
                                "Connection required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var settings = Utility.ParseConnectionString(ConnectionString);

            var svr = string.Empty;
            if (settings.ContainsKey("data source"))
                svr = settings["data source"];
            else if (settings.ContainsKey("server"))
                svr = settings["server"];
            
            var uid = string.Empty;
            if (settings.ContainsKey("user id"))
                uid = settings["user id"];
            else if (settings.ContainsKey("uid"))
                uid = settings["uid"];

            var pwd = string.Empty;
            if (settings.ContainsKey("password"))
                pwd = settings["password"];
            else if (settings.ContainsKey("pwd"))
                pwd = settings["pwd"];

            var dbName = string.Empty;
            if (settings.ContainsKey("initial catalog"))
                dbName = settings["initial catalog"];
            else if (settings.ContainsKey("database"))
                dbName = settings["database"];

            var arguments = argsFormat.Replace("${server}", svr)
                .Replace("${uid}", uid)
                .Replace("${pwd}", pwd)
                .Replace("${dbName}", dbName)
                .Replace("${filePath}", fileName)
                .Replace("${fileName}", Path.GetFileNameWithoutExtension(fileName))
                .Replace("${fileExt}", Path.GetExtension(fileName))
                .Replace("${dirName}", Path.GetDirectoryName(fileName));

            try
            {
                Process.Start(command, arguments);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            tsbPaste.Enabled = pasteToolStripMenuItem.Enabled = sciEditor.CanPaste;

            var insLock = (GetKeyState(VK_INSERT) & 0xFFF) != 0;
            statusItemInsLock.Text = insLock ? "OVR" : "INS";

            var capsLock = (GetKeyState(VK_CAPITAL) & 0xFFF) != 0;
            statusItemCapsLock.Text = capsLock ? "CAPS" : string.Empty;

            var numLock = (GetKeyState(VK_NUMLOCK) & 0xFFF) != 0;
            statusItemNumLock.Text = numLock ? "NUM" : string.Empty;

            int hw = 0, first = sciEditor.FirstVisibleLine, last = first + sciEditor.LinesOnScreen;

            for(int i = first; i < last; ++i)
                hw = Math.Max(hw, DEFAULT_FONT_SIZE * sciEditor.Lines[i].Length);

            if (hw > sciEditor.ScrollWidth)
                sciEditor.ScrollWidth = hw;
        }

        private void SqlEditor_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
            LoadCustomCommands();
        }

        private void SqlEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!(saved || PromptToSave()))
            {
                e.Cancel = true;
                return;
            }

            Application.Idle -= Application_Idle;
            SaveCustomCommands();
        }

        private void saveasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string selectedFileName;

            using (var fd = new SaveFileDialog())
            {
                fd.Title = "Save the script as";
                fd.Filter = "SQL script (*.sql)|*.sql|All files (*.*)|*.*";
                fd.FileName = fileName;
                if (fd.ShowDialog() == DialogResult.Cancel) return;
                selectedFileName = fd.FileName;
            }

            if (selectedFileName == null) return;

            SaveTo(selectedFileName);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(fileName))
                saveasToolStripMenuItem_Click(null, null);
            else
                SaveTo(fileName);
        }

        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printing.ShowPageSetupDialog();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printing.Print(sender == printToolStripMenuItem);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.Paste();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.ClearSelections();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.SelectAll();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            findReplace.ShowFind();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            findReplace.ShowReplace();
        }

        private void checkBoxMenuItem_Click(object sender, EventArgs e)
        {
            var tsm = ((ToolStripMenuItem) sender);
            tsm.Checked = !tsm.Checked;
        }

        private void toolbarToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            toolStrip.Visible = ((ToolStripMenuItem) sender).Checked;
        }

        private void statusbarToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            statusStrip.Visible = ((ToolStripMenuItem) sender).Checked;
        }

        private void lineNumbersToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            sciEditor.Margins[0].Width = ((ToolStripMenuItem) sender).Checked ? 32 : 0;
        }

        private void configureConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var cd = new ConnectionDialog())
            {
                cd.ProviderName = ProviderName;
                if (cd.ShowDialog(this) == DialogResult.Cancel) return;
                ConnectionString = cd.ConnectionString;
            }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                MessageBox.Show("You must configure the connection to the target database before running the script",
                                "Could not run the script", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var helper = new SqlHelper(ProviderName, ConnectionString))
                    helper.ExecuteScript(sciEditor.Text);

                MessageBox.Show("Database successfully exported!", "End of task",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void configureToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ctd = new CustomToolsDialog())
            {
                ctd.CustomCommands = new List<CustomCommand>(customCommands);
                if (ctd.ShowDialog(this) == DialogResult.Cancel) return;
                customCommands.Clear();
                customCommands.AddRange(ctd.CustomCommands);
                CreateCustomToolMenuItems();
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void aboutDbExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var ab = new AboutBox())
            {
                ab.ShowDialog(this);
            }
        }

        private void sQLHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string weblink = null;

            switch (ProviderName)
            {
                case "System.Data.OleDb":
                    weblink = "https://learn.microsoft.com/en-us/dotnet/api/system.data.oledb?view=dotnet-plat-ext-8.0";
                    break;
                case "System.Data.SqlClient":
                    weblink = "https://learn.microsoft.com/en-us/sql/connect/ado-net/introduction-microsoft-data-sqlclient-namespace?view=sql-server-ver16";
                    break;
                case "Oracle.ManagedDataAccess.Client":
                    weblink = "https://www.oracle.com/database/technologies/appdev/dotnet.html";
                    break;
                case "MySql.Data.MySqlClient":
                    weblink = "https://dev.mysql.com/doc/connector-net/en/";
                    break;
                case "Npgsql":
                    weblink = "https://www.npgsql.org/";
                    break;
                case "System.Data.SQLite":
                    weblink = "https://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki";
                    break;
            }

            if (!string.IsNullOrEmpty(weblink))
                Process.Start(weblink);
        }

        private void tsbFind_Click(object sender, EventArgs e)
        {
            tsbFind.Checked = !tsbFind.Checked;
            incrementalSearcher.Visible = tsbFind.Checked;
        }

        private void tscFind_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                tsbFind_Click(null, null);
        }

        private void sciEditor_TextLengthChanged(object sender, EventArgs e)
        {
            UpdateUndoRedo();
        }

        private void sciEditor_SelectionChanged(object sender, EventArgs e)
        {
            UpdateCutCopyCaretInfo();
        }

        private void sciEditor_MarginClick(object sender, MarginClickEventArgs e)
        {
            if (e.Margin == BOOKMARK_MARGIN)
            {
                const uint mask = (1 << BOOKMARK_MARKER);
                var line = sciEditor.Lines[sciEditor.LineFromPosition(e.Position)];
                
                if ((line.MarkerGet() & mask) > 0)
                {
                    // Remove existing bookmark
                    line.MarkerDelete(BOOKMARK_MARKER);
                }
                else
                {
                    // Add bookmark
                    line.MarkerAdd(BOOKMARK_MARKER);
                }
            }
        }

        private void findReplace_KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                findReplace.ShowFind();
                e.SuppressKeyPress = true;
            }
            else if (e.Shift && e.KeyCode == Keys.F3)
            {
                findReplace.Window.FindPrevious();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.F3)
            {
                findReplace.Window.FindNext();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.H)
            {
                findReplace.ShowReplace();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.I)
            {
                findReplace.ShowIncrementalSearch();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.G)
            {
                var goTo = new GoTo((Scintilla)sender);
                goTo.ShowGoToDialog();
                e.SuppressKeyPress = true;
            }
        }
    }
}