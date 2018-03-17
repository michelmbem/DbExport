using System;
using System.Data.Common;
using System.Drawing;
using System.Windows.Forms;

namespace DbExport.UI.Forms
{
    public partial class ConnectionDialog : Form
    {
        private DataProvider provider;

        public ConnectionDialog()
        {
            InitializeComponent();
        }

        public string ProviderName
        {
            get { return provider == null ? null : provider.Name; }
            set
            {
                provider = DataProvider.Get(value); ;
                OnProviderNameChanged();
            }
        }

        public string ConnectionString
        {
            get
            {
                switch (ProviderName)
                {
                    case "LocalDB":
                    case "System.Data.SQLite":
                        return dataFileConnectionPane.ConnectionString;
                    default:
                        return connectionPane.ConnectionString;
                }
            }
        }

        private void OnProviderNameChanged()
        {
            if (provider == null)
            {
                lblProviderName.Text = string.Empty;
                connectionPane.Visible = dataFileConnectionPane.Visible = false;
            }
            else
            {
                lblProviderName.Text = provider.Description;

                switch (ProviderName)
                {
                    case "LocalDB":
                    case "System.Data.SQLite":
                        connectionPane.Visible = false;
                        dataFileConnectionPane.Visible = true;
                        dataFileConnectionPane.ProviderName = provider.Name;
                        break;
                    default:
                        connectionPane.Visible = true;
                        connectionPane.ProviderName = provider.Name;
                        dataFileConnectionPane.Visible = false;
                        break;
                }
            }
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
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

        private void pnlTop_Paint(object sender, PaintEventArgs e)
        {
            var control = (Control) sender;

            e.Graphics.DrawLine(Pens.White, 0, control.Bottom - 2, control.Width, control.Bottom - 2);
            e.Graphics.DrawLine(Pens.DarkGray, 0, control.Bottom - 1, control.Width, control.Bottom - 1);
        }

        private void pnlBottom_Paint(object sender, PaintEventArgs e)
        {
            var control = (Control) sender;

            e.Graphics.DrawLine(Pens.White, 0, 0, control.Width, 0);
            e.Graphics.DrawLine(Pens.DarkGray, 0, 1, control.Width, 1);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ConnectionString))
                MessageBox.Show("The connection settings are invalid or insufficient",
                                "Invalid settings",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            else
                DialogResult = DialogResult.OK;
        }
    }
}