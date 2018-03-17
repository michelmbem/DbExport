namespace DbExport.UI.Forms
{
    partial class ArgumentsEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArgumentsEditor));
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.tsbServer = new System.Windows.Forms.ToolStripButton();
            this.tsbUid = new System.Windows.Forms.ToolStripButton();
            this.tsbPwd = new System.Windows.Forms.ToolStripButton();
            this.tsbDbName = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbFilePath = new System.Windows.Forms.ToolStripButton();
            this.tsbFileName = new System.Windows.Forms.ToolStripButton();
            this.tsbFileExt = new System.Windows.Forms.ToolStripButton();
            this.tsbDirName = new System.Windows.Forms.ToolStripButton();
            this.rtbArguments = new System.Windows.Forms.RichTextBox();
            this.pnlBottom.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnCancel);
            this.pnlBottom.Controls.Add(this.btnOK);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 144);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(244, 32);
            this.pnlBottom.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(125, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(60, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(59, 5);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(60, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbServer,
            this.tsbUid,
            this.tsbPwd,
            this.tsbDbName,
            this.toolStripSeparator1,
            this.tsbFilePath,
            this.tsbFileName,
            this.tsbFileExt,
            this.tsbDirName});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip.Size = new System.Drawing.Size(244, 25);
            this.toolStrip.TabIndex = 3;
            this.toolStrip.Text = "toolStrip1";
            // 
            // tsbServer
            // 
            this.tsbServer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbServer.Image = ((System.Drawing.Image) (resources.GetObject("tsbServer.Image")));
            this.tsbServer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbServer.Name = "tsbServer";
            this.tsbServer.Size = new System.Drawing.Size(23, 22);
            this.tsbServer.Tag = "server";
            this.tsbServer.Text = "Insert a placeholder for the server\'s location";
            this.tsbServer.Click += new System.EventHandler(this.tsbServer_Click);
            // 
            // tsbUid
            // 
            this.tsbUid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbUid.Image = ((System.Drawing.Image) (resources.GetObject("tsbUid.Image")));
            this.tsbUid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUid.Name = "tsbUid";
            this.tsbUid.Size = new System.Drawing.Size(23, 22);
            this.tsbUid.Tag = "uid";
            this.tsbUid.Text = "Insert a placeholder  for the user\'s name";
            this.tsbUid.Click += new System.EventHandler(this.tsbServer_Click);
            // 
            // tsbPwd
            // 
            this.tsbPwd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPwd.Image = ((System.Drawing.Image) (resources.GetObject("tsbPwd.Image")));
            this.tsbPwd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPwd.Name = "tsbPwd";
            this.tsbPwd.Size = new System.Drawing.Size(23, 22);
            this.tsbPwd.Tag = "pwd";
            this.tsbPwd.Text = "Insert a placeholder  for the user\'s password";
            this.tsbPwd.Click += new System.EventHandler(this.tsbServer_Click);
            // 
            // tsbDbName
            // 
            this.tsbDbName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDbName.Image = ((System.Drawing.Image) (resources.GetObject("tsbDbName.Image")));
            this.tsbDbName.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDbName.Name = "tsbDbName";
            this.tsbDbName.Size = new System.Drawing.Size(23, 22);
            this.tsbDbName.Tag = "dbName";
            this.tsbDbName.Text = "Insert a placeholder  for the database\'s name";
            this.tsbDbName.Click += new System.EventHandler(this.tsbServer_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbFilePath
            // 
            this.tsbFilePath.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFilePath.Image = ((System.Drawing.Image) (resources.GetObject("tsbFilePath.Image")));
            this.tsbFilePath.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFilePath.Name = "tsbFilePath";
            this.tsbFilePath.Size = new System.Drawing.Size(23, 22);
            this.tsbFilePath.Tag = "filePath";
            this.tsbFilePath.Text = "Insert a placeholder  for the full file\'s path";
            this.tsbFilePath.Click += new System.EventHandler(this.tsbServer_Click);
            // 
            // tsbFileName
            // 
            this.tsbFileName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFileName.Image = ((System.Drawing.Image) (resources.GetObject("tsbFileName.Image")));
            this.tsbFileName.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFileName.Name = "tsbFileName";
            this.tsbFileName.Size = new System.Drawing.Size(23, 22);
            this.tsbFileName.Tag = "fileName";
            this.tsbFileName.Text = "Insert a placeholder  for the canonical file\'s name";
            this.tsbFileName.Click += new System.EventHandler(this.tsbServer_Click);
            // 
            // tsbFileExt
            // 
            this.tsbFileExt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFileExt.Image = ((System.Drawing.Image) (resources.GetObject("tsbFileExt.Image")));
            this.tsbFileExt.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFileExt.Name = "tsbFileExt";
            this.tsbFileExt.Size = new System.Drawing.Size(23, 22);
            this.tsbFileExt.Tag = "fileExt";
            this.tsbFileExt.Text = "Insert a placeholder  for the file\'s extension";
            this.tsbFileExt.Click += new System.EventHandler(this.tsbServer_Click);
            // 
            // tsbDirName
            // 
            this.tsbDirName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDirName.Image = ((System.Drawing.Image) (resources.GetObject("tsbDirName.Image")));
            this.tsbDirName.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDirName.Name = "tsbDirName";
            this.tsbDirName.Size = new System.Drawing.Size(23, 22);
            this.tsbDirName.Tag = "dirName";
            this.tsbDirName.Text = "Insert a placeholder  for the directory\'s name";
            this.tsbDirName.Click += new System.EventHandler(this.tsbServer_Click);
            // 
            // rtbArguments
            // 
            this.rtbArguments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbArguments.Location = new System.Drawing.Point(0, 25);
            this.rtbArguments.Name = "rtbArguments";
            this.rtbArguments.Size = new System.Drawing.Size(244, 119);
            this.rtbArguments.TabIndex = 4;
            this.rtbArguments.Text = "";
            // 
            // ArgumentsEditor
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(244, 176);
            this.ControlBox = false;
            this.Controls.Add(this.rtbArguments);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.pnlBottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArgumentsEditor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Arguments Editor";
            this.pnlBottom.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.RichTextBox rtbArguments;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ToolStripButton tsbServer;
        private System.Windows.Forms.ToolStripButton tsbUid;
        private System.Windows.Forms.ToolStripButton tsbPwd;
        private System.Windows.Forms.ToolStripButton tsbDbName;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbFilePath;
        private System.Windows.Forms.ToolStripButton tsbFileName;
        private System.Windows.Forms.ToolStripButton tsbFileExt;
        private System.Windows.Forms.ToolStripButton tsbDirName;
    }
}