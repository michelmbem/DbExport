namespace DbExport.UI.Controls
{
    partial class WizardPage1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardPage1));
            this.lblWelcomeMessage = new System.Windows.Forms.Label();
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.lblWelcomeMessage);
            // 
            // pîcBanner
            // 
            this.pîcBanner.Image = global::DbExport.Properties.Resources.Stage1;
            this.pîcBanner.Margin = new System.Windows.Forms.Padding(5);
            // 
            // lblWelcomeMessage
            // 
            this.lblWelcomeMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWelcomeMessage.Location = new System.Drawing.Point(12, 90);
            this.lblWelcomeMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWelcomeMessage.Name = "lblWelcomeMessage";
            this.lblWelcomeMessage.Size = new System.Drawing.Size(325, 160);
            this.lblWelcomeMessage.TabIndex = 0;
            this.lblWelcomeMessage.Text = resources.GetString("lblWelcomeMessage.Text");
            // 
            // WizardPage1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "WizardPage1";
            this.pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblWelcomeMessage;

    }
}