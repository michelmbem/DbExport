namespace DbExport.UI.Controls
{
    partial class WizardPage
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
            this.pnlContent = new System.Windows.Forms.Panel();
            this.p�cBanner = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize) (this.p�cBanner)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(120, 0);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(280, 300);
            this.pnlContent.TabIndex = 0;
            // 
            // p�cBanner
            // 
            this.p�cBanner.Dock = System.Windows.Forms.DockStyle.Left;
            this.p�cBanner.Location = new System.Drawing.Point(0, 0);
            this.p�cBanner.Name = "p�cBanner";
            this.p�cBanner.Size = new System.Drawing.Size(120, 300);
            this.p�cBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.p�cBanner.TabIndex = 2;
            this.p�cBanner.TabStop = false;
            // 
            // WizardPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.p�cBanner);
            this.Name = "WizardPage";
            this.Size = new System.Drawing.Size(400, 300);
            ((System.ComponentModel.ISupportInitialize) (this.p�cBanner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Panel pnlContent;
        protected System.Windows.Forms.PictureBox p�cBanner;

    }
}