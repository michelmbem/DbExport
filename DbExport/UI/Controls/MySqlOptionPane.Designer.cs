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
            this.label1.Location = new System.Drawing.Point(4, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Storage Engine:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 50);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Character Set:";
            // 
            // cboStorageEngine
            // 
            this.cboStorageEngine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboStorageEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStorageEngine.FormattingEnabled = true;
            this.cboStorageEngine.Location = new System.Drawing.Point(107, 10);
            this.cboStorageEngine.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboStorageEngine.Name = "cboStorageEngine";
            this.cboStorageEngine.Size = new System.Drawing.Size(178, 25);
            this.cboStorageEngine.TabIndex = 1;
            // 
            // cboCharSet
            // 
            this.cboCharSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCharSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCharSet.FormattingEnabled = true;
            this.cboCharSet.Location = new System.Drawing.Point(107, 46);
            this.cboCharSet.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboCharSet.Name = "cboCharSet";
            this.cboCharSet.Size = new System.Drawing.Size(178, 25);
            this.cboCharSet.TabIndex = 3;
            this.cboCharSet.SelectedIndexChanged += new System.EventHandler(this.cboCharSet_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 85);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Sort Order:";
            // 
            // cboSortOrder
            // 
            this.cboSortOrder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboSortOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSortOrder.FormattingEnabled = true;
            this.cboSortOrder.Location = new System.Drawing.Point(107, 81);
            this.cboSortOrder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboSortOrder.Name = "cboSortOrder";
            this.cboSortOrder.Size = new System.Drawing.Size(178, 25);
            this.cboSortOrder.TabIndex = 5;
            // 
            // MySqlOptionPane
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.cboSortOrder);
            this.Controls.Add(this.cboCharSet);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboStorageEngine);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MySqlOptionPane";
            this.Size = new System.Drawing.Size(290, 120);
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
