namespace DbExport.UI.Controls
{
    partial class MySqlOptionPane
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboStorageEngine = new System.Windows.Forms.ComboBox();
            this.cboCharSet = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboSortOrder = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Storage Engine:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Character Set:";
            // 
            // cboStorageEngine
            // 
            this.cboStorageEngine.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboStorageEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStorageEngine.FormattingEnabled = true;
            this.cboStorageEngine.Location = new System.Drawing.Point(92, 8);
            this.cboStorageEngine.Name = "cboStorageEngine";
            this.cboStorageEngine.Size = new System.Drawing.Size(145, 21);
            this.cboStorageEngine.TabIndex = 1;
            // 
            // cboCharSet
            // 
            this.cboCharSet.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCharSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCharSet.FormattingEnabled = true;
            this.cboCharSet.Location = new System.Drawing.Point(92, 35);
            this.cboCharSet.Name = "cboCharSet";
            this.cboCharSet.Size = new System.Drawing.Size(145, 21);
            this.cboCharSet.TabIndex = 3;
            this.cboCharSet.SelectedIndexChanged += new System.EventHandler(this.cboCharSet_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Sort Order:";
            // 
            // cboSortOrder
            // 
            this.cboSortOrder.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSortOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSortOrder.FormattingEnabled = true;
            this.cboSortOrder.Location = new System.Drawing.Point(92, 62);
            this.cboSortOrder.Name = "cboSortOrder";
            this.cboSortOrder.Size = new System.Drawing.Size(145, 21);
            this.cboSortOrder.TabIndex = 5;
            // 
            // MySqlOptionPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboSortOrder);
            this.Controls.Add(this.cboCharSet);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboStorageEngine);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MySqlOptionPane";
            this.Size = new System.Drawing.Size(240, 90);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboStorageEngine;
        private System.Windows.Forms.ComboBox cboCharSet;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboSortOrder;
    }
}
