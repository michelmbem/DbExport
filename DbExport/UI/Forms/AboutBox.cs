using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace DbExport.UI.Forms
{
    public partial class AboutBox : Form
    {
        private const string WEB_SITE = "http://michelmbem.wordpress.com";

        public AboutBox()
        {
            InitializeComponent();
        }

        private void AboutBox_Load(object sender, EventArgs e)
        {
            Text = string.Format(Text, AssemblyTitle);
            lblVersion.Text = String.Format(lblVersion.Text, AssemblyVersion);
            lblDescription.Text = AssemblyDescription;
            lblCopyright.Text = AssemblyCopyright;
        }

        private void pnlContent_Paint(object sender, PaintEventArgs e)
        {
            var control = (Control) sender;

            e.Graphics.DrawLine(Pens.DarkGray, 0, 0, control.Width, 0);
            e.Graphics.DrawLine(Pens.White, 0, 1, control.Width, 1);
        }

        private void lnkWebSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(WEB_SITE);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region Assembly Attribute Accessors

        internal static string AssemblyTitle
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

                if (attributes.Length > 0)
                {
                    var titleAttribute = (AssemblyTitleAttribute) attributes[0];
                    if (!string.IsNullOrEmpty(titleAttribute.Title))
                        return titleAttribute.Title;
                }

                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        internal static string AssemblyVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        internal static string AssemblyDescription
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyDescriptionAttribute) attributes[0]).Description;
            }
        }

        internal static string AssemblyCopyright
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute) attributes[0]).Copyright;
            }
        }

        #endregion
    }
}