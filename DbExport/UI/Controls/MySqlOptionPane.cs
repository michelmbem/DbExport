using System.ComponentModel;
using System.Windows.Forms;
using DbExport.Providers.MySqlClient;

namespace DbExport.UI.Controls
{
    public partial class MySqlOptionPane : UserControl
    {
        public MySqlOptionPane()
        {
            InitializeComponent();

            cboStorageEngine.DataSource = MySqlOptions.StorageEngines;
            cboCharSet.DataSource = MySqlOptions.CharacterSets;
        }

        [Browsable(false)]
        public MySqlOptions Options
        {
            get
            {
                return new MySqlOptions
                           {
                               StorageEngine = cboStorageEngine.Text,
                               CharacterSet = cboCharSet.Text,
                               SortOrder = cboSortOrder.Text
                           };
            }
            set
            {
                if (value == null)
                {
                    cboStorageEngine.Text = string.Empty;
                    cboCharSet.Text = string.Empty;
                    cboSortOrder.Text = string.Empty;
                }
                else
                {
                    cboStorageEngine.Text = value.StorageEngine;
                    cboCharSet.Text = value.CharacterSet;
                    cboSortOrder.Text = value.SortOrder;
                }
            }
        }

        private void cboCharSet_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            cboSortOrder.DataSource = MySqlOptions.GetSortOrders(cboCharSet.Text);
        }
    }
}
