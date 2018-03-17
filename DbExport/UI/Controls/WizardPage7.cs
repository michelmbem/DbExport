using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace DbExport.UI.Controls
{
    public partial class WizardPage7 : WizardPage
    {
        public WizardPage7()
        {
            InitializeComponent();

            cboEncoding.DataSource = Encoding.GetEncodings();
            cboEncoding.ValueMember = "CodePage";
            cboEncoding.DisplayMember = "Name";
            cboEncoding.SelectedValue = Utility.DEFAULT_CODE_PAGE;
        }

        [Browsable(false)]
        public bool ScripGenerationEnabled
        {
            get { return radGenerateScript.Enabled; }
            set
            {
                if (value)
                    radGenerateScript.Enabled = true;
                else
                {
                    radDirectMigration.Checked = true;
                    radGenerateScript.Enabled = false;
                }
            }
        }

        [Browsable(false)]
        public bool GenerateScript
        {
            get { return radGenerateScript.Checked; }
        }

        [Browsable(false)]
        public string FileName
        {
            get { return txtFileName.Text; }
        }

        [Browsable(false)]
        public Encoding Encoding
        {
            get { return ((EncodingInfo) cboEncoding.SelectedItem).GetEncoding(); }
        }

        [Browsable(false)]
        public string StatusMessage
        {
            get { return lblStatus.Text; }
            set { lblStatus.Text = value; }
        }

        public void StartAnimation()
        {
            picAnimation.Visible = true;
        }

        public void StopAnimation()
        {
            picAnimation.Visible = false;
        }

        private void radGenerateScript_CheckedChanged(object sender, EventArgs e)
        {
            cboEncoding.Enabled = txtFileName.Enabled = btnBrowse.Enabled = radGenerateScript.Checked;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var fd = new SaveFileDialog())
            {
                fd.Title = "Save the script as";
                fd.Filter = "SQL script file (*.sql)|*.sql|All files (*.*)|*.*";
                if (fd.ShowDialog(this) == DialogResult.Cancel) return;
                txtFileName.Text = fd.FileName;
            }
        }
    }
}