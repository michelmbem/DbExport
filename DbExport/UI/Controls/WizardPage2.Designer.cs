namespace DbExport.UI.Controls
{
    partial class WizardPage2
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
            this.pnlCredentials = new System.Windows.Forms.Panel();
            this.pnlConnectionPane = new System.Windows.Forms.Panel();
            this.dataFileConnectionPane = new DataFileConnectionPane();
            this.sqlServerConnectionPane = new SqlServerConnectionPane();
            this.btnTest = new System.Windows.Forms.Button();
            this.pnlProvider = new System.Windows.Forms.Panel();
            this.cboProvider = new System.Windows.Forms.ComboBox();
            this.lblProvider = new System.Windows.Forms.Label();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.pîcBanner)).BeginInit();
            this.pnlCredentials.SuspendLayout();
            this.pnlConnectionPane.SuspendLayout();
            this.pnlProvider.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.pnlCredentials);
            this.pnlContent.Controls.Add(this.pnlProvider);
            // 
            // pîcBanner
            // 
            this.pîcBanner.Image = global::DbExport.Properties.Resources.Stage2;
            // 
            // pnlCredentials
            // 
            this.pnlCredentials.Controls.Add(this.pnlConnectionPane);
            this.pnlCredentials.Controls.Add(this.btnTest);
            this.pnlCredentials.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCredentials.Location = new System.Drawing.Point(0, 50);
            this.pnlCredentials.Name = "pnlCredentials";
            this.pnlCredentials.Size = new System.Drawing.Size(280, 250);
            this.pnlCredentials.TabIndex = 1;
            // 
            // pnlConnectionPane
            // 
            this.pnlConnectionPane.Controls.Add(this.dataFileConnectionPane);
            this.pnlConnectionPane.Controls.Add(this.sqlServerConnectionPane);
            this.pnlConnectionPane.Location = new System.Drawing.Point(10, 6);
            this.pnlConnectionPane.Name = "pnlConnectionPane";
            this.pnlConnectionPane.Size = new System.Drawing.Size(260, 160);
            this.pnlConnectionPane.TabIndex = 9;
            // 
            // dataFileConnectionPane
            // 
            this.dataFileConnectionPane.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                                                      | System.Windows.Forms.AnchorStyles.Right)));
            this.dataFileConnectionPane.Location = new System.Drawing.Point(0, 0);
            this.dataFileConnectionPane.Name = "dataFileConnectionPane";
            this.dataFileConnectionPane.Size = new System.Drawing.Size(260, 160);
            this.dataFileConnectionPane.TabIndex = 0;
            // 
            // sqlServerConnectionPane
            // 
            this.sqlServerConnectionPane.Location = new System.Drawing.Point(0, 0);
            this.sqlServerConnectionPane.Name = "sqlServerConnectionPane";
            this.sqlServerConnectionPane.ProviderName = null;
            this.sqlServerConnectionPane.Size = new System.Drawing.Size(260, 160);
            this.sqlServerConnectionPane.TabIndex = 1;
            // 
            // btnTest
            // 
            this.btnTest.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnTest.Location = new System.Drawing.Point(90, 172);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(100, 23);
            this.btnTest.TabIndex = 8;
            this.btnTest.Text = "Test Connection";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // pnlProvider
            // 
            this.pnlProvider.Controls.Add(this.cboProvider);
            this.pnlProvider.Controls.Add(this.lblProvider);
            this.pnlProvider.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProvider.Location = new System.Drawing.Point(0, 0);
            this.pnlProvider.Name = "pnlProvider";
            this.pnlProvider.Size = new System.Drawing.Size(280, 50);
            this.pnlProvider.TabIndex = 0;
            // 
            // cboProvider
            // 
            this.cboProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProvider.FormattingEnabled = true;
            this.cboProvider.Location = new System.Drawing.Point(62, 16);
            this.cboProvider.Name = "cboProvider";
            this.cboProvider.Size = new System.Drawing.Size(180, 21);
            this.cboProvider.TabIndex = 1;
            this.cboProvider.SelectedIndexChanged += new System.EventHandler(this.cboProvider_SelectedIndexChanged);
            // 
            // lblProvider
            // 
            this.lblProvider.AutoSize = true;
            this.lblProvider.Location = new System.Drawing.Point(7, 19);
            this.lblProvider.Name = "lblProvider";
            this.lblProvider.Size = new System.Drawing.Size(49, 13);
            this.lblProvider.TabIndex = 0;
            this.lblProvider.Text = "Provider:";
            // 
            // WizardPage2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "WizardPage2";
            this.pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.pîcBanner)).EndInit();
            this.pnlCredentials.ResumeLayout(false);
            this.pnlConnectionPane.ResumeLayout(false);
            this.pnlProvider.ResumeLayout(false);
            this.pnlProvider.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlCredentials;
        private System.Windows.Forms.Panel pnlProvider;
        private System.Windows.Forms.ComboBox cboProvider;
        private System.Windows.Forms.Label lblProvider;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Panel pnlConnectionPane;
        private SqlServerConnectionPane sqlServerConnectionPane;
        private DataFileConnectionPane dataFileConnectionPane;
    }
}