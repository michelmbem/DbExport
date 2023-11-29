namespace DbExport.UI.Controls
{
    partial class WizardPage7
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardPage7));
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.cboEncoding = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.picAnimation = new System.Windows.Forms.PictureBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.radGenerateScript = new System.Windows.Forms.RadioButton();
            this.radDirectMigration = new System.Windows.Forms.RadioButton();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).BeginInit();
            this.pnlOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAnimation)).BeginInit();
            this.pnlTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.pnlOptions);
            this.pnlContent.Controls.Add(this.pnlTitle);
            // 
            // pîcBanner
            // 
            this.pîcBanner.Image = global::DbExport.Properties.Resources.Stage6;
            this.pîcBanner.Margin = new System.Windows.Forms.Padding(5);
            // 
            // pnlOptions
            // 
            this.pnlOptions.Controls.Add(this.cboEncoding);
            this.pnlOptions.Controls.Add(this.label1);
            this.pnlOptions.Controls.Add(this.picAnimation);
            this.pnlOptions.Controls.Add(this.lblStatus);
            this.pnlOptions.Controls.Add(this.label2);
            this.pnlOptions.Controls.Add(this.btnBrowse);
            this.pnlOptions.Controls.Add(this.txtFileName);
            this.pnlOptions.Controls.Add(this.radGenerateScript);
            this.pnlOptions.Controls.Add(this.radDirectMigration);
            this.pnlOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOptions.Location = new System.Drawing.Point(0, 80);
            this.pnlOptions.Margin = new System.Windows.Forms.Padding(4);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(350, 270);
            this.pnlOptions.TabIndex = 0;
            // 
            // cboEncoding
            // 
            this.cboEncoding.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEncoding.Enabled = false;
            this.cboEncoding.Location = new System.Drawing.Point(106, 71);
            this.cboEncoding.Margin = new System.Windows.Forms.Padding(4);
            this.cboEncoding.Name = "cboEncoding";
            this.cboEncoding.Size = new System.Drawing.Size(230, 25);
            this.cboEncoding.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 75);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Encoding:";
            // 
            // picAnimation
            // 
            this.picAnimation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picAnimation.Image = ((System.Drawing.Image)(resources.GetObject("picAnimation.Image")));
            this.picAnimation.Location = new System.Drawing.Point(10, 235);
            this.picAnimation.Margin = new System.Windows.Forms.Padding(4);
            this.picAnimation.Name = "picAnimation";
            this.picAnimation.Size = new System.Drawing.Size(324, 16);
            this.picAnimation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAnimation.TabIndex = 6;
            this.picAnimation.TabStop = false;
            this.picAnimation.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(9, 214);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(57, 17);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "lblStatus";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 112);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "File\'s path:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Enabled = false;
            this.btnBrowse.Location = new System.Drawing.Point(248, 175);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(88, 30);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileName.Enabled = false;
            this.txtFileName.Location = new System.Drawing.Point(106, 109);
            this.txtFileName.Margin = new System.Windows.Forms.Padding(4);
            this.txtFileName.Multiline = true;
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(230, 58);
            this.txtFileName.TabIndex = 5;
            // 
            // radGenerateScript
            // 
            this.radGenerateScript.AutoSize = true;
            this.radGenerateScript.Location = new System.Drawing.Point(9, 41);
            this.radGenerateScript.Margin = new System.Windows.Forms.Padding(4);
            this.radGenerateScript.Name = "radGenerateScript";
            this.radGenerateScript.Size = new System.Drawing.Size(234, 21);
            this.radGenerateScript.TabIndex = 1;
            this.radGenerateScript.Text = "Save the script to a file for later use";
            this.radGenerateScript.UseVisualStyleBackColor = true;
            this.radGenerateScript.CheckedChanged += new System.EventHandler(this.radGenerateScript_CheckedChanged);
            // 
            // radDirectMigration
            // 
            this.radDirectMigration.AutoSize = true;
            this.radDirectMigration.Checked = true;
            this.radDirectMigration.Location = new System.Drawing.Point(9, 10);
            this.radDirectMigration.Margin = new System.Windows.Forms.Padding(4);
            this.radDirectMigration.Name = "radDirectMigration";
            this.radDirectMigration.Size = new System.Drawing.Size(210, 21);
            this.radDirectMigration.TabIndex = 0;
            this.radDirectMigration.TabStop = true;
            this.radDirectMigration.Text = "Directly run the migration script";
            this.radDirectMigration.UseVisualStyleBackColor = true;
            // 
            // pnlTitle
            // 
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(0, 0);
            this.pnlTitle.Margin = new System.Windows.Forms.Padding(4);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.pnlTitle.Size = new System.Drawing.Size(350, 80);
            this.pnlTitle.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(6, 7);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(338, 66);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = resources.GetString("lblTitle.Text");
            // 
            // WizardPage7
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "WizardPage7";
            this.pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).EndInit();
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAnimation)).EndInit();
            this.pnlTitle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.RadioButton radGenerateScript;
        private System.Windows.Forms.RadioButton radDirectMigration;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox picAnimation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboEncoding;
    }
}