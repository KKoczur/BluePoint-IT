﻿using System;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using Ionic.Zip;

namespace StatlookLogViewer
{
    public partial class OpenZip : Form
    {
       public ArrayList m_nowaKarta;
       private string m_zip;
       public static string ZipTmpDirectory = "\\A plus C Systems\\uplook3\\TMP\\";
       public static string ZipDirectory;
       private LogHeader uplookDeskryptor = new LogHeader();

        public OpenZip()
        {
            ZipDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + ZipTmpDirectory;
            InitializeComponent();
            m_nowaKarta = new ArrayList();
        }

        public OpenZip(string zip)
        {
            ZipDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + ZipTmpDirectory;
            m_zip = zip;
            m_nowaKarta = new ArrayList();
            InitializeComponent();
        }

        private void buttonZipCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonZipChoose_Click(object sender, EventArgs e)
        {
                foreach (ListViewItem ZaznaczoneWiersze in listViewFiles.SelectedItems)
                {
                    string FileName = ZaznaczoneWiersze.SubItems[1].Text;
                    string FilePath = ZaznaczoneWiersze.SubItems[4].Text;
                    string FullName = FilePath + "\\" + FileName;
                    FileInfo atrybutyPlik = new FileInfo(FullName);

                    //Nie przetwarzaj plików o rozszerzeniu .zip
                    if (atrybutyPlik.Extension == ".zip")
                    {
                        Form otworzZip = new OpenZip();
                        otworzZip.ShowDialog(this);
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
        
        public void DodajItem(ListViewItem plikInfo)
        {
            listViewFiles.Items.Add(plikInfo);

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
            ListView.SelectedListViewItemCollection Li = this.listViewFiles.SelectedItems;
            int i = 0;
            foreach (ListViewItem item in Li)
            {
                
                string FileName = item.SubItems[1].Text;
                string FullName = item.SubItems[4].Text + "\\" + item.SubItems[1].Text;

                using (ZipFile zip = ZipFile.Read(m_zip))
                {
                    foreach (ZipEntry e1 in zip)
                    {
                       // e1.Extract(ZipDirectory, true);  // overwrite == true  
                    }  
                }

                FileInfo atrybutyPlik = new FileInfo(FullName);

                PlikLogu plik = new PlikLogu();
                m_nowaKarta.Add(plik.analizeUplookLog(FullName, item.SubItems[1].Text, item.SubItems[2].Text, uplookDeskryptor));
                i++;
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }

        #region Wlasciwosci

        public ArrayList nowaKarta
        {
            set
            {
                m_nowaKarta = value;
            }
            get
            {
                return m_nowaKarta;
            }
        }

        #endregion Wlasciwosci

    }
}
