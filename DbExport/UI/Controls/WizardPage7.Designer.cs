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
            ((System.ComponentModel.ISupportInitialize) (this.pîcBanner)).BeginInit();
            this.pnlOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.picAnimation)).BeginInit();
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
            this.pnlOptions.Location = new System.Drawing.Point(0, 90);
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(280, 210);
            this.pnlOptions.TabIndex = 0;
            // 
            // cboEncoding
            // 
            this.cboEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEncoding.Enabled = false;
            this.cboEncoding.Location = new System.Drawing.Point(91, 54);
            this.cboEncoding.Name = "cboEncoding";
            this.cboEncoding.Size = new System.Drawing.Size(176, 21);
            this.cboEncoding.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Encoding:";
            // 
            // picAnimation
            // 
            this.picAnimation.Image = ((System.Drawing.Image) (resources.GetObject("picAnimation.Image")));
            this.picAnimation.Location = new System.Drawing.Point(9, 180);
            this.picAnimation.Name = "picAnimation";
            this.picAnimation.Size = new System.Drawing.Size(258, 20);
            this.picAnimation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picAnimation.TabIndex = 6;
            this.picAnimation.TabStop = false;
            this.picAnimation.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(6, 164);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 13);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "lblStatus";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "File\'s path:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Enabled = false;
            this.btnBrowse.Location = new System.Drawing.Point(192, 134);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileName.Enabled = false;
            this.txtFileName.Location = new System.Drawing.Point(91, 83);
            this.txtFileName.Multiline = true;
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(176, 45);
            this.txtFileName.TabIndex = 5;
            // 
            // radGenerateScript
            // 
            this.radGenerateScript.AutoSize = true;
            this.radGenerateScript.Location = new System.Drawing.Point(8, 31);
            this.radGenerateScript.Name = "radGenerateScript";
            this.radGenerateScript.Size = new System.Drawing.Size(191, 17);
            this.radGenerateScript.TabIndex = 1;
            this.radGenerateScript.Text = "Save the script to a file for later use";
            this.radGenerateScript.UseVisualStyleBackColor = true;
            this.radGenerateScript.CheckedChanged += new System.EventHandler(this.radGenerateScript_CheckedChanged);
            // 
            // radDirectMigration
            // 
            this.radDirectMigration.AutoSize = true;
            this.radDirectMigration.Checked = true;
            this.radDirectMigration.Location = new System.Drawing.Point(8, 8);
            this.radDirectMigration.Name = "radDirectMigration";
            this.radDirectMigration.Size = new System.Drawing.Size(169, 17);
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
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Padding = new System.Windows.Forms.Padding(5);
            this.pnlTitle.Size = new System.Drawing.Size(280, 90);
            this.pnlTitle.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Location = new System.Drawing.Point(5, 5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(270, 80);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = resources.GetString("lblTitle.Text");
            // 
            // WizardPage6
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "WizardPage6";
            this.pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.pîcBanner)).EndInit();
            this.pnlOptions.ResumeLayout(false);
            this.pnlOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.picAnimation)).EndInit();
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