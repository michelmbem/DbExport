using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace DbExport.UI.Controls
{

    #region FileDialogMode Enum

    public enum FileDialogMode
    {
        Open,
        Save
    }

    #endregion

    public partial class DataFileConnectionPane : UserControl
    {
        private string providerName;
        
        public DataFileConnectionPane()
        {
            InitializeComponent();
        }

        [DefaultValue(typeof(FileDialogMode), "Open")]
        public FileDialogMode BrowseMode { get; set; }

        [Browsable(false)]
        public string ProviderName
        {
            get { return providerName; }
            set
            {
                if (providerName == value) return;
                providerName = value;
                OnProviderNameChanged();
            }
        }

        [Browsable(false)]
        public string ConnectionString
        {
            get
            {
                if (txtDataFile.Text.Trim().Length <= 0)
                    return string.Empty;

                var sb = new StringBuilder();

                switch (providerName)
                {
                    case "System.Data.OleDb":
                        sb.AppendFormat("Provider=Microsoft.Jet.OleDB.4.0;Data Source=\"{0}\"", txtDataFile.Text);
                        if (txtUserID.Text.Trim().Length > 0) sb.AppendFormat(";User ID={0}", txtUserID.Text);
                        if (txtPassword.Text.Trim().Length > 0) sb.AppendFormat(";Password={0}", txtPassword.Text);
                        break;
                    case "LocalDB":
                        sb.AppendFormat("Data Source=(LocalDB)\\v11.0;AttachDbFilename=\"{0}\";Integrated Security=True", txtDataFile.Text);
                        break;
                    case "System.Data.SQLite":
                        sb.AppendFormat("Data Source=\"{0}\"", txtDataFile.Text);
                        if (txtPassword.Text.Trim().Length > 0) sb.AppendFormat(";Password={0}", txtPassword.Text);
                        break;
                }

                return sb.ToString();
            }
        }

        public void Reset()
        {
            txtDataFile.Clear();
            txtUserID.Clear();
            txtPassword.Clear();
            txtDataFile.Focus();
        }

        private void OnProviderNameChanged()
        {
            switch (providerName)
            {
                case "System.Data.OleDb":
                    txtUserID.Enabled = txtPassword.Enabled = true;
                    break;
                case "LocalDB":
                    txtUserID.Clear();
                    txtPassword.Clear();
                    txtUserID.Enabled = txtPassword.Enabled = false;
                    break;
                case "System.Data.SQLite":
                    txtUserID.Clear();
                    txtUserID.Enabled = false;
                    txtPassword.Enabled = true;
                    break;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FileDialog fd;

            switch (BrowseMode)
            {
                case FileDialogMode.Open:
                    fd = new OpenFileDialog {Title = "Open a database file"};
                    break;
                case FileDialogMode.Save:
                    fd = new SaveFileDialog {Title = "Save the database as"};
                    break;
                default:
                    return;
            }

            var dbFileFilter = string.Empty;
            switch (providerName)
            {
                case "System.Data.OleDb":
                    dbFileFilter = "Access database (*.mdb)|*.mdb|All files (*.*)|*.*";
                    break;
                case "LocalDB":
                    dbFileFilter = "SQL Server data file (*.mdf)|*.mdf|All files (*.*)|*.*";
                    break;
                case "System.Data.SQLite":
                    dbFileFilter = "SQLite data file (*.db)|*.db|All files (*.*)|*.*";
                    break;
            }

            using (fd)
            {
                fd.Filter = dbFileFilter;
                if (fd.ShowDialog(this) == DialogResult.Cancel) return;
                txtDataFile.Text = fd.FileName;
            }
        }
    }
}