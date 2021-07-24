using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UplookLogViewer
{
    public partial class OCatalogs : UserControl
    {
        public OCatalogs()
        {
            InitializeComponent();
        }

        private void buttonUserCatalog_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select a folder";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    textBoxUserCatalog.Text = dlg.SelectedPath;
                }
            }
        }
    }
}
