using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Serialization;
using log4net;

namespace DbExport.UI.Forms
{
    public partial class SqlEditor : Form
    {
        private const int VK_CAPITAL = 0x14;
        private const int VK_INSERT = 0x2D;
        private const int VK_NUMLOCK = 0x90;
        private const int MAX_SEARCH_STRINGS = 20;

        private static readonly ILog log = LogManager.GetLogger(typeof(SqlEditor));

        private ScintillaNet.PageSettings pageSettings = new ScintillaNet.PageSettings();
        private List<CustomCommand> customCommands;
        private string fileName;
        private bool saved = true;
        
        public SqlEditor()
        {
            InitializeComponent();
        }

        public SqlEditor(string fileName)
            : this()
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
            sciEditor.UndoRedo.EmptyUndoBuffer();
            UpdateUndoRedo();
            UpdateCutCopyCaretInfo();
        }

        private void UpdateUndoRedo()
        {
            tsbUndo.Enabled = undoToolStripMenuItem.Enabled = sciEditor.UndoRedo.CanUndo;
            tsbRedo.Enabled = redoToolStripMenuItem.Enabled = sciEditor.UndoRedo.CanRedo;
        }

        private void UpdateCutCopyCaretInfo()
        {
            clearToolStripMenuItem.Enabled = cutToolStripMenuItem.Enabled
                                             = copyToolStripMenuItem.Enabled
                                               = tsbCut.Enabled
                                                 = tsbCopy.Enabled
                                                   = sciEditor.Selection.Length > 0;

            statusItemCaretInfo.Text = string.Format("Ln: {0}, Col: {1}, Sel: {2}",
                                                     sciEditor.Caret.LineNumber + 1,
                                                     sciEditor.GetColumn(sciEditor.Caret.Position) + 1,
                                                     sciEditor.Selection.Length);
        }

        private void RegisterSearchString(string searchString)
        {
            tscFind.Items.Remove(searchString);
            tscFind.Items.Insert(0, searchString);
            while (tscFind.Items.Count > MAX_SEARCH_STRINGS)
                tscFind.Items.RemoveAt(MAX_SEARCH_STRINGS);
            sciEditor.FindReplace.LastFindString = searchString;
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
            var path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DbExport"), "commands.xml");

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
                              Arguments = "-h ${server} -u ${uid} –p ${pwd} < \"${filePath}\""
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
            var svr = settings["server"];
            var uid = settings.ContainsKey("uid") ? settings["uid"] : string.Empty;
            var pwd = settings.ContainsKey("pwd") ? settings["pwd"] : string.Empty;
            var dbName = settings.ContainsKey("database") ? settings["database"] : string.Empty;

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
            tsbPaste.Enabled = pasteToolStripMenuItem.Enabled = sciEditor.Clipboard.CanPaste;

            var insLock = (GetKeyState(VK_INSERT) & 0xFFF) != 0;
            statusItemInsLock.Text = insLock ? "OVR" : "INS";

            var capsLock = (GetKeyState(VK_CAPITAL) & 0xFFF) != 0;
            statusItemCapsLock.Text = capsLock ? "CAPS" : string.Empty;

            var numLock = (GetKeyState(VK_NUMLOCK) & 0xFFF) != 0;
            statusItemNumLock.Text = numLock ? "NUM" : string.Empty;

            var hw = 0;

            foreach (var line in sciEditor.Lines.VisibleLines)
                hw = Math.Max(hw, 8 * line.Length);

            if (hw > sciEditor.Scrolling.HorizontalWidth)
                sciEditor.Scrolling.HorizontalWidth = hw;
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
            using (var psd = new PageSetupDialog())
            {
                psd.PageSettings = pageSettings;
                if (psd.ShowDialog() == DialogResult.Cancel) return;
                pageSettings = new ScintillaNet.PageSettings
                                   {
                                       PrinterSettings = psd.PageSettings.PrinterSettings,
                                       PrinterResolution = psd.PageSettings.PrinterResolution,
                                       PaperSource = psd.PageSettings.PaperSource,
                                       PaperSize = psd.PageSettings.PaperSize,
                                       Margins = psd.PageSettings.Margins,
                                       Landscape = psd.PageSettings.Landscape,
                                       Color = psd.PageSettings.Color
                                   };
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(fileName))
                sciEditor.Printing.PrintDocument.DocumentName = Path.GetFileName(fileName);

            sciEditor.Printing.PageSettings = pageSettings;
            sciEditor.Printing.Print(sender == printToolStripMenuItem);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.UndoRedo.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.UndoRedo.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.Clipboard.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.Clipboard.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.Clipboard.Paste();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.Selection.Clear();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.Selection.SelectAll();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.FindReplace.ShowFind();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sciEditor.FindReplace.ShowReplace();
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
                case "System.Data.SqlClient":
                    weblink = "http://msdn.microsoft.com/en-us/library/ms189826%28v=sql.90%29.aspx";
                    break;
                case "System.Data.OracleClient":
                    weblink = "http://download.oracle.com/docs/cd/B14117_01/server.101/b10759/toc.htm";
                    break;
                case "MySql.Data.MySqlClient":
                    weblink = "http://dev.mysql.com/doc/refman/5.0/en/index.html";
                    break;
                case "Npgsql":
                    weblink = "http://www.postgresql.org/docs/8.2/static/sql.html";
                    break;
            }

            if (!string.IsNullOrEmpty(weblink))
                Process.Start(weblink);
        }

        private void tsbFind_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tscFind.Text)) return;
            var range = sciEditor.FindReplace.FindNext(tscFind.Text, true);
            if (range != null) range.Select();
            RegisterSearchString(tscFind.Text);
        }

        private void tscFind_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                tsbFind_Click(null, null);
        }

        private void sciEditor_TextLengthChanged(object sender, ScintillaNet.TextModifiedEventArgs e)
        {
            UpdateUndoRedo();
        }

        private void sciEditor_SelectionChanged(object sender, EventArgs e)
        {
            UpdateCutCopyCaretInfo();
        }
    }
}