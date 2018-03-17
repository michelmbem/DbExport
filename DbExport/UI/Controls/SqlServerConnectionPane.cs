using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using log4net;

namespace DbExport.UI.Controls
{
    public partial class SqlServerConnectionPane : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SqlServerConnectionPane));
        private string providerName;
        
        public SqlServerConnectionPane()
        {
            InitializeComponent();
        }

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
            get { return GetConnectionString(true); }
        }

        public void Reset()
        {
            txtHost.Clear();
            txtPort.Clear();
            txtUserID.Clear();
            txtPassword.Clear();
            cboDatabase.Items.Clear();
            cboDatabase.Text = string.Empty;
            txtHost.Focus();
        }

        private void OnProviderNameChanged()
        {
            var notOracle = !providerName.Contains("Oracle");

            lblDatabase.Visible = notOracle;
            cboDatabase.Visible = notOracle;
            
            if (providerName.Contains("MySql") || providerName.Equals("Npgsql"))
            {
                chkTrustedCnx.Checked = false;
                chkTrustedCnx.Enabled = false;

                txtPort.Enabled = true;
            }
            else
            {
                chkTrustedCnx.Enabled = true;

                txtPort.Clear();
                txtPort.Enabled = false;
            }
        }

        private string GetConnectionString(bool includeDatabase)
        {
            if (txtHost.Text.Trim().Length <= 0 ||
                (!chkTrustedCnx.Checked && txtUserID.Text.Trim().Length <= 0))
                return string.Empty;

            var sb = new StringBuilder();

            sb.Append("Server=").Append(txtHost.Text);

            if (txtPort.Text.Trim().Length > 0)
                sb.Append(";Port=").Append(txtPort.Text);

            if (chkTrustedCnx.Checked)
                sb.Append(";Integrated Security=YES");
            else
            {
                sb.Append(";UID=").Append(txtUserID.Text);
                sb.Append(";PWD=").Append(txtPassword.Text);
            }

            if (includeDatabase && cboDatabase.Visible)
                sb.Append(";Database=").Append(cboDatabase.Text);
            else if (providerName == "Npgsql")
                sb.Append(";Database=postgres");

            return sb.ToString();
        }

        private void txtPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar));
        }

        private void cboDatabase_DropDown(object sender, EventArgs e)
        {
            cboDatabase.Items.Clear();

            string sql;

            switch (providerName)
            {
                case "System.Data.SqlClient":
                    sql = "EXEC sp_databases";
                    break;
                case "MySql.Data.MySqlClient":
                    sql = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA ORDER BY SCHEMA_NAME";
                    break;
                case "Npgsql":
                    sql = "SELECT datname FROM pg_catalog.pg_database ORDER BY datname";
                    break;
                default:
                    return;
            }

            var conStr = GetConnectionString(false);
            if (conStr.Length <= 0) return;

            try
            {
                using (var helper = new SqlHelper(providerName, conStr))
                {
                    var list = helper.Query<List<object>>(sql, SqlHelper.FetchList);

                    foreach (object item in list)
                        cboDatabase.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex);
            }
        }
    }
}