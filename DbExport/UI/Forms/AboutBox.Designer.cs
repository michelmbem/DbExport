﻿namespace DbExport.UI.Forms
{
    partial class AboutBox
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
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lnkWebSite = new System.Windows.Forms.LinkLabel();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.picBanner = new System.Windows.Forms.PictureBox();
            this.pnlContent.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBanner)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.lnkWebSite);
            this.pnlContent.Controls.Add(this.lblCopyright);
            this.pnlContent.Controls.Add(this.lblDescription);
            this.pnlContent.Controls.Add(this.lblVersion);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 105);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(404, 132);
            this.pnlContent.TabIndex = 1;
            this.pnlContent.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlContent_Paint);
            // 
            // lnkWebSite
            // 
            this.lnkWebSite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkWebSite.AutoSize = true;
            this.lnkWebSite.Location = new System.Drawing.Point(292, 111);
            this.lnkWebSite.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lnkWebSite.Name = "lnkWebSite";
            this.lnkWebSite.Size = new System.Drawing.Size(105, 17);
            this.lnkWebSite.TabIndex = 3;
            this.lnkWebSite.TabStop = true;
            this.lnkWebSite.Text = "Visit Addy\'s Blog";
            this.lnkWebSite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWebSite_LinkClicked);
            // 
            // lblCopyright
            // 
            this.lblCopyright.AutoSize = true;
            this.lblCopyright.Location = new System.Drawing.Point(9, 65);
            this.lblCopyright.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(73, 17);
            this.lblCopyright.TabIndex = 2;
            this.lblCopyright.Text = "[Copyright]";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(9, 38);
            this.lblDescription.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(82, 17);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "[Description]";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(9, 10);
            this.lblVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(70, 17);
            this.lblVersion.TabIndex = 0;
            this.lblVersion.Text = "Version {0}";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 237);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(404, 44);
            this.panel1.TabIndex = 2;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(163, 8);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 30);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // picBanner
            // 
            this.picBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.picBanner.Image = global::DbExport.Properties.Resources.About;
            this.picBanner.Location = new System.Drawing.Point(0, 0);
            this.picBanner.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picBanner.Name = "picBanner";
            this.picBanner.Size = new System.Drawing.Size(404, 105);
            this.picBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBanner.TabIndex = 0;
            this.picBanner.TabStop = false;
            // 
            // AboutBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 281);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.picBanner);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About {0}";
            this.Load += new System.EventHandler(this.AboutBox_Load);
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBanner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picBanner;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel lnkWebSite;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Button btnClose;
    }
}