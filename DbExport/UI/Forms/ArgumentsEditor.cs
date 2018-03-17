using System;
using System.Windows.Forms;

namespace DbExport.UI.Forms
{
    public partial class ArgumentsEditor : Form
    {
        public ArgumentsEditor()
        {
            InitializeComponent();
        }

        public string Arguments
        {
            get { return rtbArguments.Text; }
            set { rtbArguments.Text = value; }
        }

        private void tsbServer_Click(object sender, EventArgs e)
        {
            rtbArguments.SelectedText = "${" + ((ToolStripButton) sender).Tag + "}";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
