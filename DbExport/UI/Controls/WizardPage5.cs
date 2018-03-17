using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DbExport.Providers;

namespace DbExport.UI.Controls
{
    public partial class WizardPage5 : WizardPage 
    {
        public WizardPage5()
        {
            InitializeComponent();
        }

        public string ProviderName { get; private set; }

        public string ConnectionString { get; private set; }

        public string[] SelectedTableNames
        {
            get
            {
                var names = new List<string>();

                foreach (TreeNode node in tvItems.Nodes[0].Nodes)
                {
                    if (node.Checked)
                        names.Add(node.Text);
                }

                return names.ToArray();
            }
        }

        public void Init(string providerName, string connectionString)
        {
            if (string.IsNullOrEmpty(providerName) || string.IsNullOrEmpty(connectionString) ||
                providerName == ProviderName || connectionString == ConnectionString) return;

            var provider = SchemaProvider.GetProvider(Utility.GetRealProviderName(providerName), connectionString);
            var tableNames = provider.GetTableNames();

            tvItems.BeginUpdate();
            tvItems.Nodes.Clear();

            var tableRootNode = new TreeNode("Tables", 0, 0);
            tvItems.Nodes.Add(tableRootNode);
            tableRootNode.Checked = true;

            foreach (string tableName in tableNames)
            {
                var tableNode = new TreeNode(tableName, 1, 1);
                tableRootNode.Nodes.Add(tableNode);
                tableNode.Checked = true;
            }

            var viewRootNode = new TreeNode("Views", 0, 0);
            tvItems.Nodes.Add(viewRootNode);
            viewRootNode.Checked = true;

            var spRootNode = new TreeNode("Stored procedures", 0, 0);
            tvItems.Nodes.Add(spRootNode);
            spRootNode.Checked = true;

            tvItems.ExpandAll();
            tvItems.EndUpdate();
        }

        private void CascadeCheckUpward(TreeNode node)
        {
            var isChecked = false;

            foreach (TreeNode childNode in node.Nodes)
            {
                if (childNode.Checked)
                {
                    isChecked = true;
                    break;
                }
            }

            if (!isChecked) return;

            node.Checked = true;

            if (node.Parent != null)
            {
                CascadeCheckUpward(node.Parent);
            }
        }

        private void CascadeCheckDownward(TreeNode node)
        {
            node.Checked = false;

            foreach (TreeNode childNode in node.Nodes)
            {
                CascadeCheckDownward(childNode);
            }
        }

        private void CheckTreeNode(TreeNode node, bool isChecked)
        {
            node.Checked = isChecked;
            foreach (TreeNode childNode in node.Nodes)
            {
                CheckTreeNode(childNode, isChecked);
            }
        }

        private void tvItems_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown) return;

            if (!e.Node.Checked)
            {
                foreach (TreeNode childNode in e.Node.Nodes)
                {
                    CascadeCheckDownward(childNode);
                }
            }
            else if (e.Node.Parent != null)
            {
                CascadeCheckUpward(e.Node.Parent);
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in tvItems.Nodes)
            {
                CheckTreeNode(node, true);
            }
        }

        private void btnUnselectAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in tvItems.Nodes)
            {
                CheckTreeNode(node, false);
            }
        }
    }
}
