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

        private void ButtonUserCatalog_Click(object sender, EventArgs e)
        {
            using var folderBrowserDialog = new FolderBrowserDialog();

            folderBrowserDialog.Description = "Select a folder";

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                textBoxUserCatalog.Text = folderBrowserDialog.SelectedPath;
        }
    }
}
