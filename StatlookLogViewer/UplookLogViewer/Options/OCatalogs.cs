using System;
using System.Windows.Forms;

namespace StatlookLogViewer
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
