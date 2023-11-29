namespace DbExport.UI.Controls
{
    partial class WizardPage6
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.rtbSummary = new System.Windows.Forms.RichTextBox();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.pnlSummary = new System.Windows.Forms.Panel();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).BeginInit();
            this.pnlTitle.SuspendLayout();
            this.pnlSummary.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.pnlSummary);
            this.pnlContent.Controls.Add(this.pnlTitle);
            this.pnlContent.Padding = new System.Windows.Forms.Padding(5);
            // 
            // pîcBanner
            // 
            this.pîcBanner.Image = global::DbExport.Properties.Resources.Stage5;
            this.pîcBanner.Margin = new System.Windows.Forms.Padding(5);
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Location = new System.Drawing.Point(6, 7);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(328, 71);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Here is a summary of the settings you have provided. If those settings do not mat" +
    "ch your requirements, you can go back to any previous stage to make the necessar" +
    "y changes.";
            // 
            // rtbSummary
            // 
            this.rtbSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbSummary.Location = new System.Drawing.Point(6, 7);
            this.rtbSummary.Margin = new System.Windows.Forms.Padding(4);
            this.rtbSummary.Name = "rtbSummary";
            this.rtbSummary.ReadOnly = true;
            this.rtbSummary.Size = new System.Drawing.Size(328, 241);
            this.rtbSummary.TabIndex = 0;
            this.rtbSummary.Text = "";
            this.rtbSummary.WordWrap = false;
            // 
            // pnlTitle
            // 
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(5, 5);
            this.pnlTitle.Margin = new System.Windows.Forms.Padding(4);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.pnlTitle.Size = new System.Drawing.Size(340, 85);
            this.pnlTitle.TabIndex = 0;
            // 
            // pnlSummary
            // 
            this.pnlSummary.Controls.Add(this.rtbSummary);
            this.pnlSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSummary.Location = new System.Drawing.Point(5, 90);
            this.pnlSummary.Margin = new System.Windows.Forms.Padding(4);
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.pnlSummary.Size = new System.Drawing.Size(340, 255);
            this.pnlSummary.TabIndex = 1;
            // 
            // WizardPage6
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "WizardPage6";
            this.pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).EndInit();
            this.pnlTitle.ResumeLayout(false);
            this.pnlSummary.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbSummary;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlSummary;
        private System.Windows.Forms.Panel pnlTitle;
    }
}