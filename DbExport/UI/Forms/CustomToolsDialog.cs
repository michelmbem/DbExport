using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using log4net;

namespace DbExport.UI.Forms
{
    public partial class CustomToolsDialog : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CustomToolsDialog));

        private List<CustomCommand> customCommands;
        private bool valid = true;
        
        public CustomToolsDialog()
        {
            InitializeComponent();
        }

        public List<CustomCommand> CustomCommands
        {
            get { return customCommands; }
            set { customCommandBindingSource.DataSource = customCommands = value; }
        }

        private void grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 3:
                    using (var fd = new OpenFileDialog())
                    {
                        fd.Title = "Browse";
                        fd.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
                        if (fd.ShowDialog(this) == DialogResult.Cancel) return;
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value = fd.FileName;
                    }
                    break;
                case 5:
                    using (var ad = new ArgumentsEditor())
                    {
                        ad.Arguments = Convert.ToString(grid.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value);
                        if (ad.ShowDialog(this) == DialogResult.Cancel) return;
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value = ad.Arguments;
                    }
                    break;
            }
        }

        private void grid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != 0) return;

            for (int i = 0; i < grid.Rows.Count; ++i)
                if (i != e.RowIndex &&
                    string.Compare(Convert.ToString(grid.Rows[i].Cells[0].Value).Trim(),
                                   Convert.ToString(e.FormattedValue).Trim()) == 0)
                {
                    e.Cancel = true;
                    lblMessages.Text = "Another command is registered with the name \"" + e.FormattedValue + "\"";
                    Console.Beep();
                    break;
                }
        }

        private void grid_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            lblMessages.Text = string.Empty;
        }

        private void grid_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            for (int i = 0; i < grid.ColumnCount; ++i)
            {
                if (i == 3 || i == 5) continue;

                if (Utility.IsEmpty(grid.Rows[e.RowIndex].Cells[i].Value))
                {
                    e.Cancel = true;
                    valid = false;
                    lblMessages.Text = "All properties have not been set for the selected command";
                    Console.Beep();
                    break;
                }
            }
        }

        private void grid_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            valid = true;
            lblMessages.Text = string.Empty;
        }

        private void grid_Validating(object sender, CancelEventArgs e)
        {
            if (valid) return;
            Console.Beep();
            e.Cancel = true;
        }

        private void grid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var response = MessageBox.Show("Do you really want to delete this tool?", "Confirm action",
                                           MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            e.Cancel = response == DialogResult.No;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                customCommandBindingSource.EndEdit();
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}