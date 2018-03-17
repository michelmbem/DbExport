using DbExport.UI.Controls;

namespace DbExport.UI.Forms
{
    partial class ConnectionDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblProviderName = new System.Windows.Forms.Label();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.connectionPaneContainer = new System.Windows.Forms.Panel();
            this.dataFileConnectionPane = new DbExport.UI.Controls.DataFileConnectionPane();
            this.connectionPane = new DbExport.UI.Controls.SqlServerConnectionPane();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.picBanner = new System.Windows.Forms.PictureBox();
            this.pnlTop.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.connectionPaneContainer.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBanner)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.lblProviderName);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(5);
            this.pnlTop.Size = new System.Drawing.Size(354, 30);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlTop_Paint);
            // 
            // lblProviderName
            // 
            this.lblProviderName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProviderName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProviderName.Location = new System.Drawing.Point(5, 5);
            this.lblProviderName.Name = "lblProviderName";
            this.lblProviderName.Size = new System.Drawing.Size(344, 20);
            this.lblProviderName.TabIndex = 0;
            this.lblProviderName.Text = "[Provider Name]";
            this.lblProviderName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.Controls.Add(this.connectionPaneContainer);
            this.pnlMiddle.Controls.Add(this.btnTestConnection);
            this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMiddle.Location = new System.Drawing.Point(90, 30);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Size = new System.Drawing.Size(264, 212);
            this.pnlMiddle.TabIndex = 1;
            // 
            // connectionPaneContainer
            // 
            this.connectionPaneContainer.Controls.Add(this.dataFileConnectionPane);
            this.connectionPaneContainer.Controls.Add(this.connectionPane);
            this.connectionPaneContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionPaneContainer.Location = new System.Drawing.Point(0, 0);
            this.connectionPaneContainer.Name = "connectionPaneContainer";
            this.connectionPaneContainer.Size = new System.Drawing.Size(264, 160);
            this.connectionPaneContainer.TabIndex = 3;
            // 
            // sqliteConnectionPane
            // 
            this.dataFileConnectionPane.BrowseMode = DbExport.UI.Controls.FileDialogMode.Save;
            this.dataFileConnectionPane.Location = new System.Drawing.Point(0, 10);
            this.dataFileConnectionPane.Name = "sqliteConnectionPane";
            this.dataFileConnectionPane.Size = new System.Drawing.Size(264, 140);
            this.dataFileConnectionPane.TabIndex = 0;
            // 
            // connectionPane
            // 
            this.connectionPane.Location = new System.Drawing.Point(0, 0);
            this.connectionPane.Name = "connectionPane";
            this.connectionPane.ProviderName = null;
            this.connectionPane.Size = new System.Drawing.Size(264, 160);
            this.connectionPane.TabIndex = 0;
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Location = new System.Drawing.Point(82, 166);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(100, 23);
            this.btnTestConnection.TabIndex = 1;
            this.btnTestConnection.Text = "Test Connection";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnCancel);
            this.pnlBottom.Controls.Add(this.btnOK);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 242);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(354, 40);
            this.pnlBottom.TabIndex = 2;
            this.pnlBottom.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlBottom_Paint);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(267, 9);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(186, 9);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.picBanner);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 30);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Padding = new System.Windows.Forms.Padding(2);
            this.pnlLeft.Size = new System.Drawing.Size(90, 212);
            this.pnlLeft.TabIndex = 3;
            // 
            // picBanner
            // 
            this.picBanner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBanner.Image = global::DbExport.Properties.Resources.DbConnect;
            this.picBanner.Location = new System.Drawing.Point(2, 2);
            this.picBanner.Name = "picBanner";
            this.picBanner.Size = new System.Drawing.Size(86, 208);
            this.picBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBanner.TabIndex = 0;
            this.picBanner.TabStop = false;
            // 
            // ConnectionDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(354, 282);
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectionDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure a connection";
            this.pnlTop.ResumeLayout(false);
            this.pnlMiddle.ResumeLayout(false);
            this.connectionPaneContainer.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBanner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Panel pnlBottom;
        private SqlServerConnectionPane connectionPane;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.PictureBox picBanner;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label lblProviderName;
        private DataFileConnectionPane dataFileConnectionPane;
        private System.Windows.Forms.Panel connectionPaneContainer;

    }
}