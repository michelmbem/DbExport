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
            ((System.ComponentModel.ISupportInitialize) (this.pîcBanner)).BeginInit();
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
            // 
            // grpProviderOptions
            // 
            this.grpProviderOptions.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpProviderOptions.Controls.Add(this.mySqlOptionPane);
            this.grpProviderOptions.Location = new System.Drawing.Point(8, 165);
            this.grpProviderOptions.Name = "grpProviderOptions";
            this.grpProviderOptions.Size = new System.Drawing.Size(260, 125);
            this.grpProviderOptions.TabIndex = 2;
            this.grpProviderOptions.TabStop = false;
            this.grpProviderOptions.Text = "Provider specific options";
            // 
            // mySqlOptionPane
            // 
            this.mySqlOptionPane.Location = new System.Drawing.Point(8, 16);
            this.mySqlOptionPane.Name = "mySqlOptionPane";
            this.mySqlOptionPane.Size = new System.Drawing.Size(240, 90);
            this.mySqlOptionPane.TabIndex = 0;
            // 
            // prgExportFlags
            // 
            this.prgExportFlags.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.prgExportFlags.Controls.Add(this.chkExportPKs);
            this.prgExportFlags.Controls.Add(this.chkExportIdentities);
            this.prgExportFlags.Controls.Add(this.chkExportDefaults);
            this.prgExportFlags.Controls.Add(this.chkExportFKs);
            this.prgExportFlags.Controls.Add(this.chkExportIndexes);
            this.prgExportFlags.Location = new System.Drawing.Point(8, 64);
            this.prgExportFlags.Name = "prgExportFlags";
            this.prgExportFlags.Size = new System.Drawing.Size(260, 95);
            this.prgExportFlags.TabIndex = 1;
            this.prgExportFlags.TabStop = false;
            this.prgExportFlags.Text = "Generate DDL for";
            // 
            // chkExportPKs
            // 
            this.chkExportPKs.AutoSize = true;
            this.chkExportPKs.Checked = true;
            this.chkExportPKs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExportPKs.Location = new System.Drawing.Point(7, 19);
            this.chkExportPKs.Name = "chkExportPKs";
            this.chkExportPKs.Size = new System.Drawing.Size(86, 17);
            this.chkExportPKs.TabIndex = 0;
            this.chkExportPKs.Text = "Primary Keys";
            this.chkExportPKs.UseVisualStyleBackColor = true;
            // 
            // chkExportIdentities
            // 
            this.chkExportIdentities.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkExportIdentities.AutoSize = true;
            this.chkExportIdentities.Location = new System.Drawing.Point(7, 65);
            this.chkExportIdentities.Name = "chkExportIdentities";
            this.chkExportIdentities.Size = new System.Drawing.Size(97, 17);
            this.chkExportIdentities.TabIndex = 4;
            this.chkExportIdentities.Text = "Serial Numbers";
            this.chkExportIdentities.UseVisualStyleBackColor = true;
            // 
            // chkExportDefaults
            // 
            this.chkExportDefaults.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkExportDefaults.AutoSize = true;
            this.chkExportDefaults.Location = new System.Drawing.Point(132, 42);
            this.chkExportDefaults.Name = "chkExportDefaults";
            this.chkExportDefaults.Size = new System.Drawing.Size(95, 17);
            this.chkExportDefaults.TabIndex = 3;
            this.chkExportDefaults.Text = "Default Values";
            this.chkExportDefaults.UseVisualStyleBackColor = true;
            // 
            // chkExportFKs
            // 
            this.chkExportFKs.AutoSize = true;
            this.chkExportFKs.Checked = true;
            this.chkExportFKs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExportFKs.Location = new System.Drawing.Point(132, 19);
            this.chkExportFKs.Name = "chkExportFKs";
            this.chkExportFKs.Size = new System.Drawing.Size(87, 17);
            this.chkExportFKs.TabIndex = 1;
            this.chkExportFKs.Text = "Foreign Keys";
            this.chkExportFKs.UseVisualStyleBackColor = true;
            // 
            // chkExportIndexes
            // 
            this.chkExportIndexes.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkExportIndexes.AutoSize = true;
            this.chkExportIndexes.Checked = true;
            this.chkExportIndexes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExportIndexes.Location = new System.Drawing.Point(7, 42);
            this.chkExportIndexes.Name = "chkExportIndexes";
            this.chkExportIndexes.Size = new System.Drawing.Size(63, 17);
            this.chkExportIndexes.TabIndex = 2;
            this.chkExportIndexes.Text = "Indexes";
            this.chkExportIndexes.UseVisualStyleBackColor = true;
            // 
            // grpMainOptions
            // 
            this.grpMainOptions.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMainOptions.Controls.Add(this.radExportBoth);
            this.grpMainOptions.Controls.Add(this.radExportData);
            this.grpMainOptions.Controls.Add(this.radExportSchema);
            this.grpMainOptions.Location = new System.Drawing.Point(8, 8);
            this.grpMainOptions.Name = "grpMainOptions";
            this.grpMainOptions.Size = new System.Drawing.Size(260, 50);
            this.grpMainOptions.TabIndex = 0;
            this.grpMainOptions.TabStop = false;
            this.grpMainOptions.Text = "Export what?";
            // 
            // radExportBoth
            // 
            this.radExportBoth.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radExportBoth.AutoSize = true;
            this.radExportBoth.Checked = true;
            this.radExportBoth.Location = new System.Drawing.Point(201, 19);
            this.radExportBoth.Name = "radExportBoth";
            this.radExportBoth.Size = new System.Drawing.Size(47, 17);
            this.radExportBoth.TabIndex = 2;
            this.radExportBoth.TabStop = true;
            this.radExportBoth.Text = "Both";
            this.radExportBoth.UseVisualStyleBackColor = true;
            // 
            // radExportData
            // 
            this.radExportData.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.radExportData.AutoSize = true;
            this.radExportData.Location = new System.Drawing.Point(114, 19);
            this.radExportData.Name = "radExportData";
            this.radExportData.Size = new System.Drawing.Size(48, 17);
            this.radExportData.TabIndex = 1;
            this.radExportData.Text = "Data";
            this.radExportData.UseVisualStyleBackColor = true;
            // 
            // radExportSchema
            // 
            this.radExportSchema.AutoSize = true;
            this.radExportSchema.Location = new System.Drawing.Point(7, 19);
            this.radExportSchema.Name = "radExportSchema";
            this.radExportSchema.Size = new System.Drawing.Size(64, 17);
            this.radExportSchema.TabIndex = 0;
            this.radExportSchema.Text = "Schema";
            this.radExportSchema.UseVisualStyleBackColor = true;
            // 
            // WizardPage4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "WizardPage4";
            this.pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.pîcBanner)).EndInit();
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