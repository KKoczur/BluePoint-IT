using Ionic.Zip;
using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace StatlookLogViewer
{
    public partial class OpenZip : Form
    {
        private readonly ArrayList m_nowaKarta;
        private readonly string _zip;
        private static readonly string ZipTmpDirectory = "\\A plus C Systems\\uplook3\\TMP\\";
        private static string _zipDirectory;

        public OpenZip()
        {
            _zipDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + ZipTmpDirectory;
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
                var fileName = listViewItem.SubItems[1].Text;
                var filePath = listViewItem.SubItems[4].Text;
                var fullName = filePath + "\\" + fileName;

                var fileInfo = new FileInfo(fullName);

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

        private void CheckBoxSelectAll_Click(object sender, EventArgs e)
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
            foreach (ColumnHeader ch in listViewFiles.Columns)
            {
                ch.Width = -2;
            }
        }

        private void ListViewFiles_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            checkBoxSelectAll.Checked = false;
        }

        private void ListViewFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            foreach (ListViewItem item in listViewFiles.SelectedItems)
            {
                var fileName = item.SubItems[1].Text;
                var fileFullPath = item.SubItems[4].Text + "\\" + item.SubItems[1].Text;

                using (var zip = ZipFile.Read(_zip))
                {
                    foreach (var e1 in zip)
                    {
                        // e1.Extract(ZipDirectory, true);  // overwrite == true  
                    }
                }

                _ = DateTime.TryParse(item.SubItems[2].Text, out _);

                m_nowaKarta.Add(LogAnalyzer.GetLogTapePage(fileFullPath));
            }

            DialogResult = DialogResult.OK;

            this.Close();
        }

    }
}
