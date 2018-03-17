using System.ComponentModel;
using System.Windows.Forms;

namespace DbExport.UI.Controls
{
    public partial class WizardPage4 : WizardPage
    {
        private string providerName;

        public WizardPage4()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        public string ProviderName
        {
            get { return providerName; }
            set
            {
                if (value == providerName) return;
                providerName = value;
                OnProviderNameChanged();
            }
        }

        [Browsable(false)]
        public ExportOptions ExportOptions
        {
            get
            {
                var options = new ExportOptions
                                  {
                                      ExportSchema = (radExportSchema.Checked || radExportBoth.Checked),
                                      ExportData = (radExportData.Checked || radExportBoth.Checked)
                                  };

                var flags = ExportFlags.ExportNothing;
                if (chkExportPKs.Checked) flags |= ExportFlags.ExportPrimaryKeys;
                if (chkExportFKs.Checked) flags |= ExportFlags.ExportForeignKeys;
                if (chkExportIndexes.Checked) flags |= ExportFlags.ExportIndexes;
                if (chkExportDefaults.Checked) flags |= ExportFlags.ExportDefaults;
                if (chkExportIdentities.Checked) flags |= ExportFlags.ExportIdentities;
                options.Flags = flags;

                switch (providerName)
                {
                    case "MySql.Data.MySqlClient":
                        options.ProviderSpecific = mySqlOptionPane.Options;
                        break;
                }

                return options;
            }
        }

        private void OnProviderNameChanged()
        {
            foreach (Control control in grpProviderOptions.Controls)
                control.Visible = false;

            switch (providerName)
            {
                case "MySql.Data.MySqlClient":
                    mySqlOptionPane.Visible = true;
                    break;
            }
        }
    }
}