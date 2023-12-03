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
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.dataFileConnectionPane = new DbExport.UI.Controls.DataFileConnectionPane();
            this.connectionPane = new DbExport.UI.Controls.SqlServerConnectionPane();
            this.pnlTop.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.connectionPaneContainer.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.lblProviderName);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.pnlTop.Size = new System.Drawing.Size(434, 39);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlTop_Paint);
            // 
            // lblProviderName
            // 
            this.lblProviderName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProviderName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProviderName.Location = new System.Drawing.Point(6, 7);
            this.lblProviderName.Name = "lblProviderName";
            this.lblProviderName.Size = new System.Drawing.Size(422, 25);
            this.lblProviderName.TabIndex = 0;
            this.lblProviderName.Text = "[Provider Name]";
            this.lblProviderName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.Controls.Add(this.connectionPaneContainer);
            this.pnlMiddle.Controls.Add(this.btnTestConnection);
            this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMiddle.Location = new System.Drawing.Point(105, 39);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Size = new System.Drawing.Size(329, 270);
            this.pnlMiddle.TabIndex = 1;
            // 
            // connectionPaneContainer
            // 
            this.connectionPaneContainer.Controls.Add(this.dataFileConnectionPane);
            this.connectionPaneContainer.Controls.Add(this.connectionPane);
            this.connectionPaneContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.connectionPaneContainer.Location = new System.Drawing.Point(0, 0);
            this.connectionPaneContainer.Name = "connectionPaneContainer";
            this.connectionPaneContainer.Size = new System.Drawing.Size(329, 209);
            this.connectionPaneContainer.TabIndex = 3;
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnTestConnection.Location = new System.Drawing.Point(106, 220);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(117, 30);
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
            this.pnlBottom.Location = new System.Drawing.Point(0, 309);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(434, 52);
            this.pnlBottom.TabIndex = 2;
            this.pnlBottom.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlBottom_Paint);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(336, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(243, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 30);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pnlLeft
            // 
            this.pnlLeft.BackColor = System.Drawing.Color.White;
            this.pnlLeft.BackgroundImage = global::DbExport.Properties.Resources.data_connection;
            this.pnlLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 39);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pnlLeft.Size = new System.Drawing.Size(105, 270);
            this.pnlLeft.TabIndex = 3;
            // 
            // dataFileConnectionPane
            // 
            this.dataFileConnectionPane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataFileConnectionPane.BrowseMode = DbExport.UI.Controls.FileDialogMode.Save;
            this.dataFileConnectionPane.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataFileConnectionPane.Location = new System.Drawing.Point(0, 0);
            this.dataFileConnectionPane.Margin = new System.Windows.Forms.Padding(4);
            this.dataFileConnectionPane.Name = "dataFileConnectionPane";
            this.dataFileConnectionPane.ProviderName = null;
            this.dataFileConnectionPane.Size = new System.Drawing.Size(329, 209);
            this.dataFileConnectionPane.TabIndex = 0;
            // 
            // connectionPane
            // 
            this.connectionPane.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionPane.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectionPane.Location = new System.Drawing.Point(0, 0);
            this.connectionPane.Margin = new System.Windows.Forms.Padding(4);
            this.connectionPane.Name = "connectionPane";
            this.connectionPane.ProviderName = null;
            this.connectionPane.Size = new System.Drawing.Size(329, 209);
            this.connectionPane.TabIndex = 0;
            // 
            // ConnectionDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(434, 361);
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlBottom);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Panel pnlBottom;
        private SqlServerConnectionPane connectionPane;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Label lblProviderName;
        private DataFileConnectionPane dataFileConnectionPane;
        private System.Windows.Forms.Panel connectionPaneContainer;

    }
}