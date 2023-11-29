namespace DbExport.UI.Controls
{
    partial class WizardPage4
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
            DbExport.Providers.MySqlClient.MySqlOptions mySqlOptions1 = new DbExport.Providers.MySqlClient.MySqlOptions();
            this.grpProviderOptions = new System.Windows.Forms.GroupBox();
            this.mySqlOptionPane = new DbExport.UI.Controls.MySqlOptionPane();
            this.prgExportFlags = new System.Windows.Forms.GroupBox();
            this.chkExportPKs = new System.Windows.Forms.CheckBox();
            this.chkExportIdentities = new System.Windows.Forms.CheckBox();
            this.chkExportDefaults = new System.Windows.Forms.CheckBox();
            this.chkExportFKs = new System.Windows.Forms.CheckBox();
            this.chkExportIndexes = new System.Windows.Forms.CheckBox();
            this.grpMainOptions = new System.Windows.Forms.GroupBox();
            this.radExportBoth = new System.Windows.Forms.RadioButton();
            this.radExportData = new System.Windows.Forms.RadioButton();
            this.radExportSchema = new System.Windows.Forms.RadioButton();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).BeginInit();
            this.grpProviderOptions.SuspendLayout();
            this.prgExportFlags.SuspendLayout();
            this.grpMainOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.grpProviderOptions);
            this.pnlContent.Controls.Add(this.prgExportFlags);
            this.pnlContent.Controls.Add(this.grpMainOptions);
            // 
            // pîcBanner
            // 
            this.pîcBanner.Image = global::DbExport.Properties.Resources.Stage4;
            this.pîcBanner.Margin = new System.Windows.Forms.Padding(5);
            // 
            // grpProviderOptions
            // 
            this.grpProviderOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpProviderOptions.Controls.Add(this.mySqlOptionPane);
            this.grpProviderOptions.Location = new System.Drawing.Point(9, 192);
            this.grpProviderOptions.Margin = new System.Windows.Forms.Padding(4);
            this.grpProviderOptions.Name = "grpProviderOptions";
            this.grpProviderOptions.Padding = new System.Windows.Forms.Padding(4);
            this.grpProviderOptions.Size = new System.Drawing.Size(329, 151);
            this.grpProviderOptions.TabIndex = 2;
            this.grpProviderOptions.TabStop = false;
            this.grpProviderOptions.Text = "Provider specific options";
            // 
            // mySqlOptionPane
            // 
            this.mySqlOptionPane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mySqlOptionPane.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mySqlOptionPane.Location = new System.Drawing.Point(9, 21);
            this.mySqlOptionPane.Margin = new System.Windows.Forms.Padding(4);
            this.mySqlOptionPane.Name = "mySqlOptionPane";
            mySqlOptions1.CharacterSet = "";
            mySqlOptions1.SortOrder = "";
            mySqlOptions1.StorageEngine = "";
            this.mySqlOptionPane.Options = mySqlOptions1;
            this.mySqlOptionPane.Size = new System.Drawing.Size(312, 118);
            this.mySqlOptionPane.TabIndex = 0;
            // 
            // prgExportFlags
            // 
            this.prgExportFlags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgExportFlags.Controls.Add(this.chkExportPKs);
            this.prgExportFlags.Controls.Add(this.chkExportIdentities);
            this.prgExportFlags.Controls.Add(this.chkExportDefaults);
            this.prgExportFlags.Controls.Add(this.chkExportFKs);
            this.prgExportFlags.Controls.Add(this.chkExportIndexes);
            this.prgExportFlags.Location = new System.Drawing.Point(9, 70);
            this.prgExportFlags.Margin = new System.Windows.Forms.Padding(4);
            this.prgExportFlags.Name = "prgExportFlags";
            this.prgExportFlags.Padding = new System.Windows.Forms.Padding(4);
            this.prgExportFlags.Size = new System.Drawing.Size(329, 114);
            this.prgExportFlags.TabIndex = 1;
            this.prgExportFlags.TabStop = false;
            this.prgExportFlags.Text = "Generate DDL for";
            // 
            // chkExportPKs
            // 
            this.chkExportPKs.AutoSize = true;
            this.chkExportPKs.Checked = true;
            this.chkExportPKs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExportPKs.Location = new System.Drawing.Point(8, 25);
            this.chkExportPKs.Margin = new System.Windows.Forms.Padding(4);
            this.chkExportPKs.Name = "chkExportPKs";
            this.chkExportPKs.Size = new System.Drawing.Size(102, 21);
            this.chkExportPKs.TabIndex = 0;
            this.chkExportPKs.Text = "Primary Keys";
            this.chkExportPKs.UseVisualStyleBackColor = true;
            // 
            // chkExportIdentities
            // 
            this.chkExportIdentities.AutoSize = true;
            this.chkExportIdentities.Location = new System.Drawing.Point(199, 54);
            this.chkExportIdentities.Margin = new System.Windows.Forms.Padding(4);
            this.chkExportIdentities.Name = "chkExportIdentities";
            this.chkExportIdentities.Size = new System.Drawing.Size(117, 21);
            this.chkExportIdentities.TabIndex = 4;
            this.chkExportIdentities.Text = "Serial Numbers";
            this.chkExportIdentities.UseVisualStyleBackColor = true;
            // 
            // chkExportDefaults
            // 
            this.chkExportDefaults.AutoSize = true;
            this.chkExportDefaults.Location = new System.Drawing.Point(8, 83);
            this.chkExportDefaults.Margin = new System.Windows.Forms.Padding(4);
            this.chkExportDefaults.Name = "chkExportDefaults";
            this.chkExportDefaults.Size = new System.Drawing.Size(109, 21);
            this.chkExportDefaults.TabIndex = 3;
            this.chkExportDefaults.Text = "Default Values";
            this.chkExportDefaults.UseVisualStyleBackColor = true;
            // 
            // chkExportFKs
            // 
            this.chkExportFKs.AutoSize = true;
            this.chkExportFKs.Checked = true;
            this.chkExportFKs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExportFKs.Location = new System.Drawing.Point(199, 25);
            this.chkExportFKs.Margin = new System.Windows.Forms.Padding(4);
            this.chkExportFKs.Name = "chkExportFKs";
            this.chkExportFKs.Size = new System.Drawing.Size(102, 21);
            this.chkExportFKs.TabIndex = 1;
            this.chkExportFKs.Text = "Foreign Keys";
            this.chkExportFKs.UseVisualStyleBackColor = true;
            // 
            // chkExportIndexes
            // 
            this.chkExportIndexes.AutoSize = true;
            this.chkExportIndexes.Checked = true;
            this.chkExportIndexes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExportIndexes.Location = new System.Drawing.Point(8, 54);
            this.chkExportIndexes.Margin = new System.Windows.Forms.Padding(4);
            this.chkExportIndexes.Name = "chkExportIndexes";
            this.chkExportIndexes.Size = new System.Drawing.Size(71, 21);
            this.chkExportIndexes.TabIndex = 2;
            this.chkExportIndexes.Text = "Indexes";
            this.chkExportIndexes.UseVisualStyleBackColor = true;
            // 
            // grpMainOptions
            // 
            this.grpMainOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMainOptions.Controls.Add(this.radExportBoth);
            this.grpMainOptions.Controls.Add(this.radExportData);
            this.grpMainOptions.Controls.Add(this.radExportSchema);
            this.grpMainOptions.Location = new System.Drawing.Point(9, 7);
            this.grpMainOptions.Margin = new System.Windows.Forms.Padding(4);
            this.grpMainOptions.Name = "grpMainOptions";
            this.grpMainOptions.Padding = new System.Windows.Forms.Padding(4);
            this.grpMainOptions.Size = new System.Drawing.Size(329, 55);
            this.grpMainOptions.TabIndex = 0;
            this.grpMainOptions.TabStop = false;
            this.grpMainOptions.Text = "Export what?";
            // 
            // radExportBoth
            // 
            this.radExportBoth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radExportBoth.AutoSize = true;
            this.radExportBoth.Checked = true;
            this.radExportBoth.Location = new System.Drawing.Point(264, 22);
            this.radExportBoth.Margin = new System.Windows.Forms.Padding(4);
            this.radExportBoth.Name = "radExportBoth";
            this.radExportBoth.Size = new System.Drawing.Size(52, 21);
            this.radExportBoth.TabIndex = 2;
            this.radExportBoth.TabStop = true;
            this.radExportBoth.Text = "Both";
            this.radExportBoth.UseVisualStyleBackColor = true;
            // 
            // radExportData
            // 
            this.radExportData.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.radExportData.AutoSize = true;
            this.radExportData.Location = new System.Drawing.Point(146, 22);
            this.radExportData.Margin = new System.Windows.Forms.Padding(4);
            this.radExportData.Name = "radExportData";
            this.radExportData.Size = new System.Drawing.Size(53, 21);
            this.radExportData.TabIndex = 1;
            this.radExportData.Text = "Data";
            this.radExportData.UseVisualStyleBackColor = true;
            // 
            // radExportSchema
            // 
            this.radExportSchema.AutoSize = true;
            this.radExportSchema.Location = new System.Drawing.Point(8, 22);
            this.radExportSchema.Margin = new System.Windows.Forms.Padding(4);
            this.radExportSchema.Name = "radExportSchema";
            this.radExportSchema.Size = new System.Drawing.Size(71, 21);
            this.radExportSchema.TabIndex = 0;
            this.radExportSchema.Text = "Schema";
            this.radExportSchema.UseVisualStyleBackColor = true;
            // 
            // WizardPage4
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "WizardPage4";
            this.pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).EndInit();
            this.grpProviderOptions.ResumeLayout(false);
            this.prgExportFlags.ResumeLayout(false);
            this.prgExportFlags.PerformLayout();
            this.grpMainOptions.ResumeLayout(false);
            this.grpMainOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpProviderOptions;
        private System.Windows.Forms.GroupBox prgExportFlags;
        private System.Windows.Forms.CheckBox chkExportPKs;
        private System.Windows.Forms.CheckBox chkExportDefaults;
        private System.Windows.Forms.CheckBox chkExportFKs;
        private System.Windows.Forms.CheckBox chkExportIndexes;
        private System.Windows.Forms.GroupBox grpMainOptions;
        private System.Windows.Forms.RadioButton radExportBoth;
        private System.Windows.Forms.RadioButton radExportData;
        private System.Windows.Forms.RadioButton radExportSchema;
        private System.Windows.Forms.CheckBox chkExportIdentities;
        private MySqlOptionPane mySqlOptionPane;
    }
}