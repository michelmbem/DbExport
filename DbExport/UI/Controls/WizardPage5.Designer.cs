﻿namespace DbExport.UI.Controls
{
    partial class WizardPage5
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardPage5));
            this.lblTitle = new System.Windows.Forms.Label();
            this.bottomPane = new System.Windows.Forms.Panel();
            this.btnUnselectAll = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.tvItems = new System.Windows.Forms.TreeView();
            this.itemsIcons = new System.Windows.Forms.ImageList(this.components);
            this.pnlContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).BeginInit();
            this.bottomPane.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.tvItems);
            this.pnlContent.Controls.Add(this.bottomPane);
            this.pnlContent.Controls.Add(this.lblTitle);
            // 
            // pîcBanner
            // 
            this.pîcBanner.Image = global::DbExport.Properties.Resources.Stage4;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(280, 23);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Select the items that you would like to export";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bottomPane
            // 
            this.bottomPane.Controls.Add(this.btnUnselectAll);
            this.bottomPane.Controls.Add(this.btnSelectAll);
            this.bottomPane.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPane.Location = new System.Drawing.Point(0, 270);
            this.bottomPane.Name = "bottomPane";
            this.bottomPane.Size = new System.Drawing.Size(280, 30);
            this.bottomPane.TabIndex = 2;
            // 
            // btnUnselectAll
            // 
            this.btnUnselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUnselectAll.Location = new System.Drawing.Point(202, 4);
            this.btnUnselectAll.Name = "btnUnselectAll";
            this.btnUnselectAll.Size = new System.Drawing.Size(75, 23);
            this.btnUnselectAll.TabIndex = 1;
            this.btnUnselectAll.Text = "None";
            this.btnUnselectAll.UseVisualStyleBackColor = true;
            this.btnUnselectAll.Click += new System.EventHandler(this.btnUnselectAll_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectAll.Location = new System.Drawing.Point(126, 4);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAll.TabIndex = 0;
            this.btnSelectAll.Text = "All";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // tvItems
            // 
            this.tvItems.CheckBoxes = true;
            this.tvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvItems.ImageIndex = 0;
            this.tvItems.ImageList = this.itemsIcons;
            this.tvItems.Location = new System.Drawing.Point(0, 23);
            this.tvItems.Name = "tvItems";
            this.tvItems.SelectedImageIndex = 0;
            this.tvItems.Size = new System.Drawing.Size(280, 247);
            this.tvItems.TabIndex = 1;
            this.tvItems.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvItems_AfterCheck);
            // 
            // itemsIcons
            // 
            this.itemsIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("itemsIcons.ImageStream")));
            this.itemsIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.itemsIcons.Images.SetKeyName(0, "folder");
            this.itemsIcons.Images.SetKeyName(1, "table");
            this.itemsIcons.Images.SetKeyName(2, "view");
            this.itemsIcons.Images.SetKeyName(3, "function");
            // 
            // WizardPage5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "WizardPage5";
            this.pnlContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pîcBanner)).EndInit();
            this.bottomPane.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel bottomPane;
        private System.Windows.Forms.TreeView tvItems;
        private System.Windows.Forms.Button btnUnselectAll;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.ImageList itemsIcons;
    }
}
