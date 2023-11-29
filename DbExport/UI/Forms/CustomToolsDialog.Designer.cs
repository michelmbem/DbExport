namespace DbExport.UI.Forms
{
    partial class CustomToolsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomToolsDialog));
            this.grid = new System.Windows.Forms.DataGridView();
            this.customCommandBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblMessages = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.commandDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ButtonColumn1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Arguments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ButtonColumn2 = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customCommandBindingSource)).BeginInit();
            this.pnlTop.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.AutoGenerateColumns = false;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.commandDataGridViewTextBoxColumn,
            this.ButtonColumn1,
            this.Arguments,
            this.ButtonColumn2});
            this.grid.DataSource = this.customCommandBindingSource;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(0, 39);
            this.grid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grid.Name = "grid";
            this.grid.RowHeadersWidth = 25;
            this.grid.Size = new System.Drawing.Size(784, 370);
            this.grid.TabIndex = 0;
            this.grid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellClick);
            this.grid.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellValidated);
            this.grid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.grid_CellValidating);
            this.grid.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_RowValidated);
            this.grid.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.grid_RowValidating);
            this.grid.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.grid_UserDeletingRow);
            this.grid.Validating += new System.ComponentModel.CancelEventHandler(this.grid_Validating);
            // 
            // customCommandBindingSource
            // 
            this.customCommandBindingSource.DataSource = typeof(DbExport.UI.CustomCommand);
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.Ivory;
            this.pnlTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTop.Controls.Add(this.lblTitle);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.pnlTop.Size = new System.Drawing.Size(784, 39);
            this.pnlTop.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(6, 7);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(770, 23);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Configure custom command that will be invoked from the \"Tools\" menu";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.lblMessages);
            this.pnlBottom.Controls.Add(this.btnCancel);
            this.pnlBottom.Controls.Add(this.btnOK);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 409);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(784, 52);
            this.pnlBottom.TabIndex = 2;
            // 
            // lblMessages
            // 
            this.lblMessages.ForeColor = System.Drawing.Color.Red;
            this.lblMessages.Location = new System.Drawing.Point(6, 7);
            this.lblMessages.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMessages.Name = "lblMessages";
            this.lblMessages.Size = new System.Drawing.Size(233, 39);
            this.lblMessages.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(395, 12);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnOK.Location = new System.Drawing.Point(301, 12);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ToolTipText = "The unique tool\'s name";
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.ToolTipText = "Will be displayed on the menu item";
            this.descriptionDataGridViewTextBoxColumn.Width = 200;
            // 
            // commandDataGridViewTextBoxColumn
            // 
            this.commandDataGridViewTextBoxColumn.DataPropertyName = "Command";
            this.commandDataGridViewTextBoxColumn.HeaderText = "Command";
            this.commandDataGridViewTextBoxColumn.Name = "commandDataGridViewTextBoxColumn";
            this.commandDataGridViewTextBoxColumn.ToolTipText = "The program to be launched";
            this.commandDataGridViewTextBoxColumn.Width = 150;
            // 
            // ButtonColumn1
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = "...";
            this.ButtonColumn1.DefaultCellStyle = dataGridViewCellStyle1;
            this.ButtonColumn1.HeaderText = "";
            this.ButtonColumn1.Name = "ButtonColumn1";
            this.ButtonColumn1.Width = 25;
            // 
            // Arguments
            // 
            this.Arguments.DataPropertyName = "Arguments";
            this.Arguments.HeaderText = "Arguments";
            this.Arguments.Name = "Arguments";
            this.Arguments.Width = 250;
            // 
            // ButtonColumn2
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = "...";
            this.ButtonColumn2.DefaultCellStyle = dataGridViewCellStyle2;
            this.ButtonColumn2.HeaderText = "";
            this.ButtonColumn2.Name = "ButtonColumn2";
            this.ButtonColumn2.Width = 25;
            // 
            // CustomToolsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomToolsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Tools";
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customCommandBindingSource)).EndInit();
            this.pnlTop.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.BindingSource customCommandBindingSource;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblMessages;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn commandDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewButtonColumn ButtonColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Arguments;
        private System.Windows.Forms.DataGridViewButtonColumn ButtonColumn2;
    }
}