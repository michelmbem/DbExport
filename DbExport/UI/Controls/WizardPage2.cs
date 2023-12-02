using System;
using System.ComponentModel;
using System.Data.Common;
using System.Windows.Forms;

namespace DbExport.UI.Controls
{
    public partial class WizardPage2 : WizardPage
    {
        public WizardPage2()
        {
            InitializeComponent();

            cboProvider.DataSource = DataProvider.All;
            cboProvider.ValueMember = "Name";
            cboProvider.DisplayMember = "Description";
            cboProvider_SelectedIndexChanged(null, null);
        }

        [Browsable(false)]
        public string ProviderName
        {
            get { return cboProvider.SelectedValue.ToString(); }
        }

        [Browsable(false)]
        public string ConnectionString
        {
            get
            {
                switch (cboProvider.SelectedIndex)
                {
                    case 0: // Access
                    case 1: // LocalDB
                    case 6:  // SQLite
                        return dataFileConnectionPane.ConnectionString;
                    default:
                        return sqlServerConnectionPane.ConnectionString;
                }
            }
        }

        private void cboProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboProvider.SelectedIndex)
            {
                case 0: // Access
                case 1: // LocalDB
                case 6:  // SQLite
                    sqlServerConnectionPane.Visible = false;
                    dataFileConnectionPane.Reset();
                    dataFileConnectionPane.Width = pnlCredentials.Width;
                    dataFileConnectionPane.Visible = true;
                    dataFileConnectionPane.ProviderName = ProviderName;
                    break;
                default:
                    dataFileConnectionPane.Visible = false;
                    sqlServerConnectionPane.Reset();
                    sqlServerConnectionPane.Width = pnlCredentials.Width;
                    sqlServerConnectionPane.Visible = true;
                    sqlServerConnectionPane.ProviderName = ProviderName;
                    break;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                using (DbConnection connection = Utility.GetConnection(ProviderName, ConnectionString))
                {
                    connection.Open();
                    connection.Close();
                }

                MessageBox.Show("Success!", "Test success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Test failure", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}