namespace DbExport.UI.Controls
{
    partial class WizardPage3
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlProvider = new System.Windows.Forms.Panel();
            this.cboProvider = new System.Windows.Forms.ComboBox();
            this.lblProvider = new System.Windows.Forms.Label();
            this.pnlCredentials = new System.Windows.Forms.Panel();
            this.pnlConnectionPane = new System.Windows.Forms.Panel();
            this.dataFileConnectionPane = new DbExport.UI.Controls.DataFileConnectionPane();
            this.sqlServerConnectionPane = new DbExport.UI.Controls.SqlServerConnectionPane();
            this.btnTest = new System.Windows.Forms.Button();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).BeginInit();
            this.pnlProvider.SuspendLayout();
            this.pnlCredentials.SuspendLayout();
            this.pnlConnectionPane.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.pnlCredentials);
            this.pnlContent.Controls.Add(this.pnlProvider);
            // 
            // pîcBanner
            // 
            this.pîcBanner.Image = global::DbExport.Properties.Resources.Stage3;
            this.pîcBanner.Margin = new System.Windows.Forms.Padding(5);
            // 
            // pnlProvider
            // 
            this.pnlProvider.Controls.Add(this.cboProvider);
            this.pnlProvider.Controls.Add(this.lblProvider);
            this.pnlProvider.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProvider.Location = new System.Drawing.Point(0, 0);
            this.pnlProvider.Margin = new System.Windows.Forms.Padding(4);
            this.pnlProvider.Name = "pnlProvider";
            this.pnlProvider.Size = new System.Drawing.Size(350, 65);
            this.pnlProvider.TabIndex = 0;
            // 
            // cboProvider
            // 
            this.cboProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProvider.FormattingEnabled = true;
            this.cboProvider.Location = new System.Drawing.Point(82, 21);
            this.cboProvider.Margin = new System.Windows.Forms.Padding(4);
            this.cboProvider.Name = "cboProvider";
            this.cboProvider.Size = new System.Drawing.Size(254, 25);
            this.cboProvider.TabIndex = 1;
            this.cboProvider.SelectedIndexChanged += new System.EventHandler(this.cboProvider_SelectedIndexChanged);
            // 
            // lblProvider
            // 
            this.lblProvider.AutoSize = true;
            this.lblProvider.Location = new System.Drawing.Point(14, 24);
            this.lblProvider.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProvider.Name = "lblProvider";
            this.lblProvider.Size = new System.Drawing.Size(60, 17);
            this.lblProvider.TabIndex = 0;
            this.lblProvider.Text = "Provider:";
            // 
            // pnlCredentials
            // 
            this.pnlCredentials.Controls.Add(this.pnlConnectionPane);
            this.pnlCredentials.Controls.Add(this.btnTest);
            this.pnlCredentials.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCredentials.Location = new System.Drawing.Point(0, 65);
            this.pnlCredentials.Margin = new System.Windows.Forms.Padding(4);
            this.pnlCredentials.Name = "pnlCredentials";
            this.pnlCredentials.Size = new System.Drawing.Size(350, 285);
            this.pnlCredentials.TabIndex = 1;
            // 
            // pnlConnectionPane
            // 
            this.pnlConnectionPane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlConnectionPane.Controls.Add(this.dataFileConnectionPane);
            this.pnlConnectionPane.Controls.Add(this.sqlServerConnectionPane);
            this.pnlConnectionPane.Location = new System.Drawing.Point(0, 0);
            this.pnlConnectionPane.Margin = new System.Windows.Forms.Padding(4);
            this.pnlConnectionPane.Name = "pnlConnectionPane";
            this.pnlConnectionPane.Size = new System.Drawing.Size(350, 210);
            this.pnlConnectionPane.TabIndex = 9;
            // 
            // dataFileConnectionPane
            // 
            this.dataFileConnectionPane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataFileConnectionPane.BrowseMode = DbExport.UI.Controls.FileDialogMode.Save;
            this.dataFileConnectionPane.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataFileConnectionPane.Location = new System.Drawing.Point(0, 0);
            this.dataFileConnectionPane.Margin = new System.Windows.Forms.Padding(5);
            this.dataFileConnectionPane.Name = "dataFileConnectionPane";
            this.dataFileConnectionPane.ProviderName = null;
            this.dataFileConnectionPane.Size = new System.Drawing.Size(350, 210);
            this.dataFileConnectionPane.TabIndex = 0;
            // 
            // sqlServerConnectionPane
            // 
            this.sqlServerConnectionPane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlServerConnectionPane.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sqlServerConnectionPane.Location = new System.Drawing.Point(0, 0);
            this.sqlServerConnectionPane.Margin = new System.Windows.Forms.Padding(5);
            this.sqlServerConnectionPane.Name = "sqlServerConnectionPane";
            this.sqlServerConnectionPane.ProviderName = null;
            this.sqlServerConnectionPane.Size = new System.Drawing.Size(350, 210);
            this.sqlServerConnectionPane.TabIndex = 1;
            // 
            // btnTest
            // 
            this.btnTest.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnTest.Location = new System.Drawing.Point(117, 225);
            this.btnTest.Margin = new System.Windows.Forms.Padding(4);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(117, 30);
            this.btnTest.TabIndex = 0;
            this.btnTest.Text = "Test Connection";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // WizardPage3
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "WizardPage3";
            this.pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).EndInit();
            this.pnlProvider.ResumeLayout(false);
            this.pnlProvider.PerformLayout();
            this.pnlCredentials.ResumeLayout(false);
            this.pnlConnectionPane.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlProvider;
        private System.Windows.Forms.ComboBox cboProvider;
        private System.Windows.Forms.Label lblProvider;
        private System.Windows.Forms.Panel pnlCredentials;
        private System.Windows.Forms.Panel pnlConnectionPane;
        private DataFileConnectionPane dataFileConnectionPane;
        private SqlServerConnectionPane sqlServerConnectionPane;
        private System.Windows.Forms.Button btnTest;
    }
}