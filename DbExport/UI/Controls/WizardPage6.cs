using System.ComponentModel;

namespace DbExport.UI.Controls
{
    public partial class WizardPage6 : WizardPage
    {
        public WizardPage6()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        public string Summary
        {
            get { return rtbSummary.Text; }
            set { rtbSummary.Text = value; }
        }
    }
}