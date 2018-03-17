using DbExport.UI.Controls;

namespace DbExport.UI.Forms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pnlActivePage = new System.Windows.Forms.Panel();
            this.wizardPage7 = new DbExport.UI.Controls.WizardPage7();
            this.wizardPage6 = new DbExport.UI.Controls.WizardPage6();
            this.wizardPage5 = new DbExport.UI.Controls.WizardPage5();
            this.wizardPage4 = new DbExport.UI.Controls.WizardPage4();
            this.wizardPage3 = new DbExport.UI.Controls.WizardPage3();
            this.wizardPage2 = new DbExport.UI.Controls.WizardPage2();
            this.wizardPage1 = new DbExport.UI.Controls.WizardPage1();
            this.pnlNavBar = new System.Windows.Forms.Panel();
            this.btnDone = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.pnlActivePage.SuspendLayout();
            this.pnlNavBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlActivePage
            // 
            this.pnlActivePage.Controls.Add(this.wizardPage7);
            this.pnlActivePage.Controls.Add(this.wizardPage6);
            this.pnlActivePage.Controls.Add(this.wizardPage5);
            this.pnlActivePage.Controls.Add(this.wizardPage4);
            this.pnlActivePage.Controls.Add(this.wizardPage3);
            this.pnlActivePage.Controls.Add(this.wizardPage2);
            this.pnlActivePage.Controls.Add(this.wizardPage1);
            this.pnlActivePage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlActivePage.Location = new System.Drawing.Point(0, 0);
            this.pnlActivePage.Name = "pnlActivePage";
            this.pnlActivePage.Size = new System.Drawing.Size(400, 300);
            this.pnlActivePage.TabIndex = 0;
            // 
            // wizardPage7
            // 
            this.wizardPage7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage7.Location = new System.Drawing.Point(0, 0);
            this.wizardPage7.Name = "wizardPage7";
            this.wizardPage7.ScripGenerationEnabled = true;
            this.wizardPage7.Size = new System.Drawing.Size(400, 300);
            this.wizardPage7.StatusMessage = "lblStatus";
            this.wizardPage7.TabIndex = 6;
            // 
            // wizardPage6
            // 
            this.wizardPage6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage6.Location = new System.Drawing.Point(0, 0);
            this.wizardPage6.Name = "wizardPage6";
            this.wizardPage6.Size = new System.Drawing.Size(400, 300);
            this.wizardPage6.Summary = "";
            this.wizardPage6.TabIndex = 5;
            // 
            // wizardPage5
            // 
            this.wizardPage5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage5.Location = new System.Drawing.Point(0, 0);
            this.wizardPage5.Name = "wizardPage5";
            this.wizardPage5.Size = new System.Drawing.Size(400, 300);
            this.wizardPage5.TabIndex = 4;
            // 
            // wizardPage4
            // 
            this.wizardPage4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage4.Location = new System.Drawing.Point(0, 0);
            this.wizardPage4.Name = "wizardPage4";
            this.wizardPage4.ProviderName = null;
            this.wizardPage4.Size = new System.Drawing.Size(400, 300);
            this.wizardPage4.TabIndex = 3;
            // 
            // wizardPage3
            // 
            this.wizardPage3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage3.Location = new System.Drawing.Point(0, 0);
            this.wizardPage3.Name = "wizardPage3";
            this.wizardPage3.Size = new System.Drawing.Size(400, 300);
            this.wizardPage3.TabIndex = 2;
            // 
            // wizardPage2
            // 
            this.wizardPage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage2.Location = new System.Drawing.Point(0, 0);
            this.wizardPage2.Name = "wizardPage2";
            this.wizardPage2.Size = new System.Drawing.Size(400, 300);
            this.wizardPage2.TabIndex = 1;
            // 
            // wizardPage1
            // 
            this.wizardPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPage1.Location = new System.Drawing.Point(0, 0);
            this.wizardPage1.Name = "wizardPage1";
            this.wizardPage1.Size = new System.Drawing.Size(400, 300);
            this.wizardPage1.TabIndex = 0;
            // 
            // pnlNavBar
            // 
            this.pnlNavBar.Controls.Add(this.btnDone);
            this.pnlNavBar.Controls.Add(this.btnNext);
            this.pnlNavBar.Controls.Add(this.btnPrevious);
            this.pnlNavBar.Controls.Add(this.btnQuit);
            this.pnlNavBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlNavBar.Location = new System.Drawing.Point(0, 300);
            this.pnlNavBar.Name = "pnlNavBar";
            this.pnlNavBar.Size = new System.Drawing.Size(400, 40);
            this.pnlNavBar.TabIndex = 1;
            this.pnlNavBar.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlNavBar_Paint);
            // 
            // btnDone
            // 
            this.btnDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDone.Location = new System.Drawing.Point(313, 10);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 23);
            this.btnDone.TabIndex = 3;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Visible = false;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(313, 10);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 2;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrevious.Location = new System.Drawing.Point(232, 10);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(75, 23);
            this.btnPrevious.TabIndex = 1;
            this.btnPrevious.Text = "Previous";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuit.Location = new System.Drawing.Point(151, 10);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(75, 23);
            this.btnQuit.TabIndex = 0;
            this.btnQuit.Text = "Quit";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 340);
            this.Controls.Add(this.pnlActivePage);
            this.Controls.Add(this.pnlNavBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Addy\'s Database Migration Wizard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.pnlActivePage.ResumeLayout(false);
            this.pnlNavBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlActivePage;
        private System.Windows.Forms.Panel pnlNavBar;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnDone;
        private WizardPage7 wizardPage7;
        private WizardPage6 wizardPage6;
        private WizardPage4 wizardPage4;
        private WizardPage3 wizardPage3;
        private WizardPage2 wizardPage2;
        private WizardPage1 wizardPage1;
        private WizardPage5 wizardPage5;

    }
}