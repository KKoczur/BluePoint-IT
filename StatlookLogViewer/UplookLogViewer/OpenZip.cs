using System;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using Ionic.Zip;

namespace StatlookLogViewer
{
    public partial class OpenZip : Form
    {
        public ArrayList m_nowaKarta;
        private readonly string _zip;
        public static string ZipTmpDirectory = "\\A plus C Systems\\uplook3\\TMP\\";
        public static string ZipDirectory;

        public OpenZip()
        {
            ZipDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + ZipTmpDirectory;
            InitializeComponent();
            m_nowaKarta = new ArrayList();
        }

        public OpenZip(string zip)
            : this()
        {
            _zip = zip;
        }

        private void buttonZipCancel_Click(object sender, EventArgs e) => Close();


        private void buttonZipChoose_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listViewFiles.SelectedItems)
            {
                string fileName = listViewItem.SubItems[1].Text;
                string filePath = listViewItem.SubItems[4].Text;
                string fullName = filePath + "\\" + fileName;

                FileInfo fileInfo = new FileInfo(fullName);

                //Nie przetwarzaj plików o rozszerzeniu .zip
                if (fileInfo.Extension == ".zip")
                {
                    using var openZip = new OpenZip();
                    openZip.ShowDialog(this);
                }
                else
                {
                    //analizeUplookLog(atrybutyPlik.FullName, atrybutyPlik.Name, atrybutyPlik.LastWriteTime.ToString());
                }
            }

        }

        private void checkBoxSelectAll_Click(object sender, EventArgs e)
        {
            if (checkBoxSelectAll.Checked)
            {
                listViewFiles.BeginUpdate();
                foreach (ListViewItem item in listViewFiles.Items)
                {
                    item.Selected = true;
                }
                listViewFiles.EndUpdate();
                listViewFiles.Focus();
            }
            else
            {
                foreach (ListViewItem item in listViewFiles.Items)
                {
                    item.Selected = false;
                }
                listViewFiles.Focus();
            }
        }

        public void AddItem(ListViewItem listViewItem)
        {
            listViewFiles.Items.Add(listViewItem);

            // Loop through and size each column header to fit the column header text.
            foreach (ColumnHeader ch in this.listViewFiles.Columns)
            {
                ch.Width = -2;
            }
        }

        private void listViewFiles_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                checkBoxSelectAll.Checked = false;
            }
        }

        private void listViewFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        { 
            foreach (ListViewItem item in listViewFiles.SelectedItems)
            {
                string fileName = item.SubItems[1].Text;
                string fileFullPath = item.SubItems[4].Text + "\\" + item.SubItems[1].Text;

                using (ZipFile zip = ZipFile.Read(_zip))
                {
                    foreach (ZipEntry e1 in zip)
                    {
                        // e1.Extract(ZipDirectory, true);  // overwrite == true  
                    }
                }

                FileInfo fileInfo = new FileInfo(fileFullPath);

                var logLineCollection = new LogLineCollection();

                DateTime.TryParse(item.SubItems[2].Text, out DateTime lastWriteTime);

                m_nowaKarta.Add(logLineCollection.AnalyzeLogFile(fileFullPath));
            }

            DialogResult = DialogResult.OK;

            this.Close();
        }

    }
}
