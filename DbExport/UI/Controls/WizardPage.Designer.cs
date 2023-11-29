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
            this.pîcBanner = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(150, 0);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(4);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(350, 350);
            this.pnlContent.TabIndex = 0;
            // 
            // pîcBanner
            // 
            this.pîcBanner.Dock = System.Windows.Forms.DockStyle.Left;
            this.pîcBanner.Location = new System.Drawing.Point(0, 0);
            this.pîcBanner.Margin = new System.Windows.Forms.Padding(4);
            this.pîcBanner.Name = "pîcBanner";
            this.pîcBanner.Size = new System.Drawing.Size(150, 350);
            this.pîcBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pîcBanner.TabIndex = 2;
            this.pîcBanner.TabStop = false;
            // 
            // WizardPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pîcBanner);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "WizardPage";
            this.Size = new System.Drawing.Size(500, 350);
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Panel pnlContent;
        protected System.Windows.Forms.PictureBox pîcBanner;

    }
}