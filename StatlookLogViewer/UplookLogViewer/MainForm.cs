﻿using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using ListViewGroupCollapse;
using Ionic.Zip;


namespace StatlookLogViewer
{

	public partial class MainForm : Form
    {
        #region Members

        private readonly ListViewColumnSorter _lvwColumnSorter;
	    public string OSVersion;
	    public string LogDirectory;
        public string USMDirectory;
        public string _userDirectory;
        private readonly string[] _fileExtensions;
        private readonly Headers uplookDeskryptor = new Headers();
        private readonly bool[] show_uplook=new bool[10];
        private readonly bool[] show_usm=new bool[6];
        private Configuration _config;

        #endregion Members

        public MainForm()
		{
            InitializeComponent();

            _config = Configuration.GetConfiguration();

            Descriptor[] udes = _config.GetStatlookDescriptors();
            int j = 0;
            foreach (Descriptor d in udes)
            {
                show_uplook[j]= d.Show;
                j++;
            }
            Descriptor[] usmdes = _config.GetUsmDescriptors();
            int k = 0;
            foreach (Descriptor d in usmdes)
            {
                show_usm[k] = d.Show;
                k++;
            }

			OSVersion = Environment.OSVersion.Version.Major.ToString();
			LogDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _config.StatlookLogDirectory;
            USMDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _config.StatlookUsmLogDirectory;
            _userDirectory = _config.UserDirectory;
            _fileExtensions= _config.LogFileExtensions.Split(new char[] { ';' });

            
            // 
            // Wypełnia menu widocznosci kolumn
            //            
            //
            for (int i = 0; i < udes.Length; i++)
            {
                ToolStripMenuItem viewMenuItem = new ToolStripMenuItem
                {
                    Checked = true
                };

                if (udes[i].Show)
            {
                viewMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                viewMenuItem.CheckState = CheckState.Unchecked;
            }
            viewMenuItem.Name = udes[i].KeyName; 
            viewMenuItem.Size = new Size(152, 22);
            viewMenuItem.Text =udes[i].HeaderText;
            ToolStripMenuItemUplook.DropDownItems.Add(viewMenuItem);
            }

            for (int i = 0; i < usmdes.Length; i++)
            {
                ToolStripMenuItem viewMenuItem = new ToolStripMenuItem
                {
                    Checked = true
                };

                if (usmdes[i].Show)
                {
                    viewMenuItem.CheckState = CheckState.Checked;
                }
                else 
                {
                    viewMenuItem.CheckState = CheckState.Unchecked;
                }
                viewMenuItem.Name = usmdes[i].KeyName;
                viewMenuItem.Size = new Size(152, 22);
                viewMenuItem.Text = usmdes[i].HeaderText;
                ToolStripMenuItemUSM.DropDownItems.Add(viewMenuItem);
            }

            
		    _lvwColumnSorter = new ListViewColumnSorter();
            this.listViewFiles.ListViewItemSorter = _lvwColumnSorter;
            IniTabPageInfo(); 
		}
		
        //Wypełnia listę informacjami o plikach logów
		private void IniTabPageInfo()
		{            
            //Podaje informacje o katalogu "Logs"
            SetDashboardData(LogDirectory, labelLogsPathValue, labelFilesSizeValue, labelFilesCountValue);
            
            //Podaje informacje statystyczne o katalogu "uplook system monitor"
            SetDashboardData(USMDirectory, labelLogsPathValueUSM, labelFilesSizeValueUSM, labelFilesCountValueUSM);
            
            //Podaje informacje statystyczne o katalogu usera
            SetDashboardData(_userDirectory, labelUserPathValue, labelFilesSizeValueUser, labelFilesCountValueUser);

            WypelnijListe();	
		}

        private void WypelnijListe()
        {
            listViewFiles.Items.Clear();
            DirectoryInfo[] catalogs = new DirectoryInfo[]{
            new DirectoryInfo(LogDirectory),
            new DirectoryInfo(USMDirectory),
            new DirectoryInfo(_userDirectory)
            };
            bool[] ShowCatalog =new bool[]{
            checkBoxLogs.Checked,
            checkBoxUSM.Checked,
            checkBoxUser.Checked
            };
            try
            {
                int j=0;
                foreach (DirectoryInfo di in catalogs)
                {
                    if (di.Exists&& ShowCatalog[j])
                    {
                        ArrayList myFileInfo = new ArrayList();
                        foreach (string ext in _fileExtensions)
                        {
                            if (di.FullName != "C:\\")
                            {
                                myFileInfo.AddRange(di.GetFiles(ext, SearchOption.AllDirectories));
                            }
                            else
                            {
                                myFileInfo.AddRange(di.GetFiles(ext, SearchOption.TopDirectoryOnly));
                            }
                        }
                        FileInfo[] pliki = (FileInfo[])myFileInfo.ToArray(typeof(FileInfo));
                        for (int i = 0; i < pliki.Length; i++)
                        {
                            ListViewItem plikInfo = new ListViewItem
                            {
                                Text = i.ToString()
                            };

                            plikInfo.SubItems.Add(pliki[i].Name);
                            plikInfo.SubItems.Add(pliki[i].LastWriteTime.ToString());
                            plikInfo.SubItems.Add(FileSize(pliki[i].Length,false));
                            plikInfo.SubItems.Add(pliki[i].DirectoryName);
                            listViewFiles.Items.Add(plikInfo);
                            toolStripButtonIcon.Image = Properties.Resources.ok_16;
                            toolStripStatusReady.Text = "Ready";
                        }
                    }
                    j++;
                }

                // Loop through and size each column header to fit the column header text.
                    foreach (ColumnHeader ch in this.listViewFiles.Columns)
                    {
                        if(ch.Index==0)
                        {
                            ch.Width = 0;
                        }
                        else
                        {
                            ch.Width = -2;
                        }
                    }
            }
            catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			} 
        }

        private void WypelnijListe(DirectoryInfo[] Catalogs)
        {
            listViewFiles.Items.Clear();
            try
            {
                foreach (DirectoryInfo di in Catalogs)
                {
                    if (di.Exists)
                    {
                        ArrayList myFileInfo = new ArrayList();
                        foreach (string ext in _fileExtensions)
                        {
                            if (di.FullName != "C:\\")
                            {
                                myFileInfo.AddRange(di.GetFiles(ext, SearchOption.AllDirectories));
                            }
                            else
                            {
                                myFileInfo.AddRange(di.GetFiles(ext, SearchOption.TopDirectoryOnly));
                            }
                        }
                        FileInfo[] pliki = (FileInfo[])myFileInfo.ToArray(typeof(FileInfo));
                        for (int i = 1; i <= pliki.Length; i++)
                        {
                            ListViewItem plikInfo = new ListViewItem
                            {
                                Text = i.ToString()
                            };
                            plikInfo.SubItems.Add(pliki[i - 1].Name);
                            plikInfo.SubItems.Add(pliki[i - 1].CreationTime.ToString());
                            plikInfo.SubItems.Add(FileSize(pliki[i-1].Length,true));
                            plikInfo.SubItems.Add(pliki[i - 1].DirectoryName);
                            listViewFiles.Items.Add(plikInfo);
                            toolStripButtonIcon.Image = Properties.Resources.ok_16;
                            toolStripStatusReady.Text = "Ready";
                        }
                    }
                }

                // Loop through and size each column header to fit the column header text.
                    foreach (ColumnHeader ch in this.listViewFiles.Columns)
                    {
                        if(ch.Index==0)
                        {
                            ch.Width = 0;
                        }
                        else
                        {
                        ch.Width = -2;
                        }
                    }
            }
            catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			} 
        }
		
		// Otwiera wskazany plik logu
		private void OpenLogFile()
		{
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Log files (*.log)|*.log|Text files (*.txt)|*.txt| Zip files (*.zip)|*.zip| All files (*.*)|*.*",
                Title = "Please select log source file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
 
                //Otwarcie pojedynczego pliku o rozszerzeniu .zip
                if((openFileDialog.FileNames.Length==1) && (openFileDialog.SafeFileName.Substring(openFileDialog.SafeFileName.Length - 4,4)==".zip"))
                {
                   OpenZip otworzZip = new OpenZip(openFileDialog.FileName);
                   #region ReadZip 
                    using (ZipFile zip = ZipFile.Read(openFileDialog.FileName))
                    {
                        int i=1;
                        foreach (ZipEntry e in zip)
                        {
                            ListViewItem plikInfo = new ListViewItem
                            {
                                Text = i.ToString()
                            };
                            plikInfo.SubItems.Add(e.FileName);
                            plikInfo.SubItems.Add(e.LastModified.ToString());
                            if ((e.UncompressedSize / 1024) < 1)
                            {
                                plikInfo.SubItems.Add(e.UncompressedSize.ToString() +" B");
                            }
                            else if (((e.UncompressedSize / 1024) >= 1) && ((e.UncompressedSize / 1024) < 1024))
                            {
                                plikInfo.SubItems.Add((e.UncompressedSize/1024).ToString() + " KB");
                            }
                            else if ((e.UncompressedSize / (1024 * 1024)) >= 1)
                            {
                                plikInfo.SubItems.Add((e.UncompressedSize / (1024 * 1024)).ToString() + " MB");
                            }
                            plikInfo.SubItems.Add(openFileDialog.InitialDirectory);
                            otworzZip.DodajItem(plikInfo);
                            i++;
                        }
                    }
                  #endregion ReadZip  

                   otworzZip.ShowDialog(this);

                }
                else
                {
                    for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                    {
                        //Nie przetwarzaj plików o rozszerzeniu .zip
                        FileInfo atrybutyPlik = new FileInfo(openFileDialog.FileNames[i]);
                        if (atrybutyPlik.Extension == ".zip")
                        {
                            Form otworzZip = new OpenZip();
                            otworzZip.ShowDialog(this);
                            if (otworzZip.ShowDialog(this) == DialogResult.OK)
                            {
                                //NewPage[] pliki = (NewPage[])otworzZip.nowaKarta.ToArray(typeof(NewPage));
                                //tabControlMain.Controls.Add(otworzZip.);
                                //tabControlMain.SelectTab(nowaKarta.nowaZakladka);
                                //Aktywownie menu grupowania 
                                grupyToolStripMenuItem.Enabled = true;
                                zwinToolStripMenuItem.Enabled = true;
                                rozwinWszystkieToolStripMenuItem.Enabled = true;
                                //Aktywowanie przycisków grupowania
                                toolStripButtonCollapsedAll.Enabled = true;
                                toolStripButtonNormalAll.Enabled = true;
                                //Aktywowanie przycisków menu File
                                closeAllWithoutActiveToolStripMenuItem.Enabled = true;
                                closeAllToolStripMenuItem.Enabled = true;
                                ZwinGrupy();
                                //RozwinGrupy();

                            }

                        }
                        else
                        {
                            analizeUplookLog(openFileDialog.FileNames[i], openFileDialog.SafeFileNames[i], atrybutyPlik.LastWriteTime.ToString());

                        }
                    }
                }
			}
		}

        private string FileSize(float size,bool useByte)
        {
            float tmp_Size = size;
            string addByte = useByte ? " (bajtów: " + tmp_Size.ToString() + ")" : string.Empty;
            if ((tmp_Size / 1024) < 1)
            {
                return tmp_Size.ToString() + " B";
            }
            else if (((tmp_Size / 1024) >= 1) && ((tmp_Size / 1024) < 1024))
            {
                return Math.Round(tmp_Size / 1024, 1).ToString() + " KB" + addByte;
            }
            else if ((tmp_Size / (1024 * 1024)) >= 1)
            {
                return Math.Round(tmp_Size / (1024 * 1024), 1).ToString() + " MB" + addByte;
            }
            else
            {
                return null;
            }
        }

        private void ShowColumns(ListView ListViewTmp,string rodzajRaportu)
        {
            if (rodzajRaportu == "uplook")
            {
                // Loop through and size each column header to fit the column header text.
                foreach (ColumnHeader ch in ListViewTmp.Columns)
                {
                    foreach (ToolStripMenuItem t in ToolStripMenuItemUplook.DropDownItems)
                    {
                        if (ch.Text == t.Text)
                        {
                            if (t.CheckState== CheckState.Checked)
                                ch.Width = -2;
                            else
                                ch.Width = 0;
                        }
                    }
                }
            }
            else if (rodzajRaportu == "usm")
            {
                // Loop through and size each column header to fit the column header text.
                foreach (ColumnHeader ch in ListViewTmp.Columns)
                {
                    foreach (ToolStripMenuItem t in ToolStripMenuItemUSM.DropDownItems)
                    {
                        if (ch.Text == t.Text)
                        {
                            if (t.CheckState == CheckState.Checked)
                                ch.Width = -2;
                            else
                                ch.Width = 0;
                        }
                    }
                }
            }
        }
		
		private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
		    OpenLogFile();	
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			
		}

        /// <summary>
        /// Wyświetla pytanie i zamyka główne okno programu 
        /// </summary>

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
            if (MessageBox.Show("Czy na pewno chcesz zamknąć aplikację?", "Zamknij", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
            else
                Configuration.SaveConfig(_config);
		}

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e) => this.Close();

        private void toolStripButton2_Click(object sender, EventArgs e) => OpenLogFile();

        private void toolStripButton3_Click(object sender, EventArgs e) => this.Close();

        //Sortowanie kolumny w liście plików logu
        private void listViewFiles_ColumnClick(object sender, ColumnClickEventArgs e)
		{
            //listViewFiles.ListViewItemSorter = new IntegerComparer(0);
            this.listViewFiles.BeginUpdate();

            try
            {
                if (e.Column == _lvwColumnSorter.SortColumn)
                {
                    // Reverse the current sort direction for this column.
                    if (_lvwColumnSorter.Order == SortOrder.Ascending)
                    {
                        _lvwColumnSorter.Order = SortOrder.Descending;
                    }
                    else
                    {
                        _lvwColumnSorter.Order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // Set the column number that is to be sorted; default to ascending.
                    if (e.Column != 0)
                    {
                        _lvwColumnSorter.SortColumn = e.Column;
                        _lvwColumnSorter.Order = SortOrder.Ascending;
                    }
                }

                //lvwColumnSorter.Order = SortOrder.None;
                // Call the sort method to manually sort the column based on the ListViewItemComparer implementation.
                listViewFiles.Sort();
            }
            finally
            {
                this.listViewFiles.EndUpdate();
            }
  
        
    }

		private void listViewFiles_DoubleClick(object sender, EventArgs e)
		{
			foreach (ListViewItem item in this.listViewFiles.SelectedItems)
			{
                string FileName = item.SubItems[1].Text;
                string FullName= item.SubItems[4].Text+"\\"+item.SubItems[1].Text;
                FileInfo atrybutyPlik = new FileInfo(FullName);

                //Nie przetwarzaj plików o rozszerzeniu .zip
                if (atrybutyPlik.Extension == ".zip")
                {
                    OpenZip otworzZip = new OpenZip();
                    #region ReadZip
                    using (ZipFile zip = ZipFile.Read(atrybutyPlik.FullName))
                    {
                        int i = 1;
                        foreach (ZipEntry e2 in zip)
                        {
                            ListViewItem plikInfo = new ListViewItem();
                            plikInfo.Text = i.ToString();
                            plikInfo.SubItems.Add(e2.FileName);
                            plikInfo.SubItems.Add(e2.LastModified.ToString());
                            if ((e2.UncompressedSize / 1024) < 1)
                            {
                                plikInfo.SubItems.Add(e2.UncompressedSize.ToString() + " B");
                            }
                            else if (((e2.UncompressedSize / 1024) >= 1) && ((e2.UncompressedSize / 1024) < 1024))
                            {
                                plikInfo.SubItems.Add((e2.UncompressedSize / 1024).ToString() + " KB");
                            }
                            else if ((e2.UncompressedSize / (1024 * 1024)) >= 1)
                            {
                                plikInfo.SubItems.Add((e2.UncompressedSize / (1024 * 1024)).ToString() + " MB");
                            }
                            plikInfo.SubItems.Add(atrybutyPlik.DirectoryName);
                            otworzZip.DodajItem(plikInfo);
                            i++;
                        }
                    }
                    #endregion ReadZip
                    otworzZip.ShowDialog(this);
                }
                else
                {      

                    analizeUplookLog(FullName, item.SubItems[1].Text, item.SubItems[2].Text);
                    
                }
			}		
		}

        private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tabControl = (TabControl)Controls.Find("tabControlMain", true)[0];

            if (tabControl.SelectedTab.Name != "tabPageInfo")
            {
                //Wyzerowanie paska wyszukiwania
                toolStripTextBox.Text = string.Empty;
                //Wyłącznie menu widocznosci kolumn(atrapa)
                toolStripMenuItemGeneral.Visible = false;
                //Włączenie przycisku Close page
                closeToolStripMenuItem1.Enabled = true;
                //Aktywowanie przycisków menu File
                closeAllToolStripMenuItem.Enabled = true;
                closeAllWithoutActiveToolStripMenuItem.Enabled = true;
                //Aktywownie menu grupowania 
                grupyToolStripMenuItem.Enabled = true;
                zwinToolStripMenuItem.Enabled = true;
                rozwinWszystkieToolStripMenuItem.Enabled = true;
                //Aktywowanie przycisków grupowania
                toolStripButtonCollapsedAll.Enabled = true;
                toolStripButtonNormalAll.Enabled = true;
                //Aktywowanie przycisku zamykania zakładki
                toolStripButtonClose.Enabled = true;

                //TabPage TabP = (TabPage)Controls.Find(TabC.SelectedTab.Name, true)[0];
                TabPage tabPage = (TabPage)Controls.Find(tabControl.SelectedTab.Name, true)[0];
                foreach (Control control in tabPage.Controls)
                {
                    if (control.GetType() == typeof(ListViewExtended))
                    {
                        ListViewExtended ListV = (ListViewExtended)control;
                        if (tabPage.Tag.ToString() == "uplook")
                        {
                            ToolStripMenuItemUplook.Visible = true;
                            ToolStripMenuItemUSM.Visible = false;
                        }
                        else if (tabPage.Tag.ToString() == "usm")
                        {
                            ToolStripMenuItemUSM.Visible = true;
                            ToolStripMenuItemUplook.Visible = false;
                        }

                        ShowColumns(ListV, tabPage.Tag.ToString());
                    }
                }
                FileInfo OpisPliku = new FileInfo(tabControl.SelectedTab.Tag.ToString());
                toolStripButtonIcon.Image = Properties.Resources.ok_16;
                toolStripStatusReady.Text = OpisPliku.FullName;
                toolStripSeparator_1.Visible = true;
                toolStripLabelCreationTime.Text = OpisPliku.LastWriteTime.ToString();
                toolStripSeparator_2.Visible = true;

                toolStripLableSize.Text = FileSize(OpisPliku.Length, true);
            }
            else
            {
                toolStripStatusReady.Text = "Ready";
                toolStripSeparator_1.Visible = false;
                toolStripLabelCreationTime.Text = string.Empty;
                toolStripSeparator_2.Visible = false;
                toolStripLableSize.Text = string.Empty;
                //Wyłączenie przycisku Close page
                closeToolStripMenuItem1.Enabled = false;
                //Deaktywacja przycisków menu File
                closeAllToolStripMenuItem.Enabled = false;
                closeAllWithoutActiveToolStripMenuItem.Enabled = false;
                //Wyłączenie menu widocznosci kolumn
                ToolStripMenuItemUplook.Visible = false;
                ToolStripMenuItemUSM.Visible = false;
                //Aktywowanie menu widocznosci kolumn(atrapa)
                toolStripMenuItemGeneral.Visible = true;
                //Wyłączenie menu grupowania 
                grupyToolStripMenuItem.Enabled = false;
                zwinToolStripMenuItem.Enabled = false;
                rozwinWszystkieToolStripMenuItem.Enabled = false;
                //wyłączenie przycisków grupowania
                toolStripButtonCollapsedAll.Enabled = false;
                toolStripButtonNormalAll.Enabled = false;
                //Wyłączenie przycisku zamykania zakładki
                toolStripButtonClose.Enabled = false;
            }

        }

        private void zwinToolStripMenuItem_Click(object sender, EventArgs e) => ZwinGrupy();

        private void rozwinWszystkieToolStripMenuItem_Click(object sender, EventArgs e) => RozwinGrupy();

        private void toolStripButtonCollapsedAll_Click(object sender, EventArgs e) => ZwinGrupy();

        private void toolStripButtonNormalAll_Click(object sender, EventArgs e) => RozwinGrupy();

        private void ZwinGrupy()
        {
            TabControl tabControl = (TabControl)Controls.Find("tabControlMain", true)[0];

            if (tabControl.SelectedTab.Name != "tabPageInfo")
            {
                TabPage tabPage = (TabPage)Controls.Find(tabControl.SelectedTab.Name, true)[0];

                foreach (Control control in tabPage.Controls)
                {
                    if (control.GetType() == typeof(ListViewExtended))
                    {
                        ListViewExtended ListV = (ListViewExtended)control;
                        ListV.SuspendLayout();
                        ListV.BeginUpdate();
                        ListV.SetGroupState(ListViewGroupState.Collapsible | ListViewGroupState.Collapsed);
                        ListV.EndUpdate();
                        ListV.ResumeLayout();
                    }
                }
            }

        }

        private void RozwinGrupy()
        {
            TabControl TabC = (TabControl)Controls.Find("tabControlMain", true)[0];
            if (TabC.SelectedTab.Name != "tabPageInfo")
            {
                TabPage TabP = (TabPage)Controls.Find(TabC.SelectedTab.Name, true)[0];

                foreach (Control control in TabP.Controls)
                {
                    string kk = control.ToString();
                    if (control.GetType() == typeof(ListViewExtended))
                    {
                        ListViewExtended ListV = (ListViewExtended)control;
                        ListV.SuspendLayout();
                        ListV.BeginUpdate();
                        ListV.SetGroupState(ListViewGroupState.Collapsible | ListViewGroupState.Normal);
                        ListV.EndUpdate();
                        ListV.ResumeLayout();
                    }
                }
            }

        }
    
        private void closeToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab.Name != "tabPageInfo")
            {
                int NumerZakładki = tabControlMain.SelectedIndex;
                tabControlMain.TabPages.Remove(tabControlMain.SelectedTab);
                if (tabControlMain.TabPages.Count > 1)
                {
                    if (NumerZakładki == tabControlMain.TabPages.Count)
                    {
                        tabControlMain.SelectedIndex = NumerZakładki - 1;
                    }
                    else
                    {
                        tabControlMain.SelectedIndex = NumerZakładki;
                    }
                }
            }

        }

        private void toolStripButtonClose_Click(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab.Name != "tabPageInfo")
            {
                int NumerZakładki = tabControlMain.SelectedIndex;
                tabControlMain.TabPages.Remove(tabControlMain.SelectedTab);
                if (tabControlMain.TabPages.Count > 1)
                {
                    if (NumerZakładki == tabControlMain.TabPages.Count)
                    {
                        tabControlMain.SelectedIndex = NumerZakładki - 1;
                    }
                    else
                    {
                        tabControlMain.SelectedIndex = NumerZakładki;
                    }
                }
            }
            else 
            {
                toolStripMenuItemGeneral.Visible = true;
            }
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int liczbaZakladek = tabControlMain.TabCount;
            for (int i = 0; i < liczbaZakladek; i++)
            {
                if (i == 0)
                {
                }
                else
                {
                    tabControlMain.TabPages.Remove(tabControlMain.TabPages[1]);
                }
            }
        }

        private void closeAllWithoutActiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int liczbaZakladek = tabControlMain.TabCount-1;
            int aktywnaZakladka = tabControlMain.SelectedIndex;
            for (int i = liczbaZakladek; i > 0; i--)
            {
                if ((i == 0 )|| (i==aktywnaZakladka))
                {
                }
                else
                {
                    tabControlMain.TabPages.Remove(tabControlMain.TabPages[i]);
                }
            }

        }

        private void tabControlMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int tabIndex;
                for (tabIndex = 0; tabIndex <= tabControlMain.TabCount - 1; tabIndex++)
                {
                    if (tabControlMain.GetTabRect(tabIndex).Contains(e.Location))
                    {
                        tabControlMain.SelectedIndex = tabIndex;
                        if (tabControlMain.SelectedIndex != 0)
                        {
                            this.contextMenuStripPage.Show(this.tabControlMain, e.Location);
                        }
                    }
                }
                
            }

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab.Name != "tabPageInfo")
            {
                toolStripMenuItemGeneral.Visible = false;
                int NumerZakładki = tabControlMain.SelectedIndex;
                tabControlMain.TabPages.Remove(tabControlMain.SelectedTab);
                if (tabControlMain.TabPages.Count > 1)
                {
                    if (NumerZakładki == tabControlMain.TabPages.Count)
                    {
                        tabControlMain.SelectedIndex = NumerZakładki - 1;
                    }
                    else
                    {
                        tabControlMain.SelectedIndex = NumerZakładki;
                    }
                }
            }
            else
            {
                toolStripMenuItemGeneral.Visible = true;
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int liczbaZakladek = tabControlMain.TabCount - 1;
            int aktywnaZakladka = tabControlMain.SelectedIndex;
            for (int i = liczbaZakladek; i > 0; i--)
            {
                if ((i == 0) || (i == aktywnaZakladka))
                {
                }
                else
                {
                    tabControlMain.TabPages.Remove(tabControlMain.TabPages[i]);
                }
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            string FullName=tabControlMain.SelectedTab.ToolTipText.ToString();
            FileInfo Plik = new FileInfo(FullName);
            Clipboard.SetText(Plik.Name);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            string FullName = tabControlMain.SelectedTab.ToolTipText.ToString();
            FileInfo Plik = new FileInfo(FullName);
            Clipboard.SetText(Plik.FullName);
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            string FullName = tabControlMain.SelectedTab.ToolTipText.ToString();
            FileInfo Plik = new FileInfo(FullName);
            Clipboard.SetText(Plik.DirectoryName);         
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            string FullName = tabControlMain.SelectedTab.ToolTipText.ToString();
            FileInfo Plik = new FileInfo(FullName);

            if (MessageBox.Show("Really delete file: " + FullName + " ?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (Plik.Exists)
                {
                    try
                    {
                        tabControlMain.TabPages.Remove(tabControlMain.SelectedTab);
                        Plik.Delete();
                        listViewFiles.Items.Clear();
                        IniTabPageInfo();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error : " + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void toolStripMenuItemOpenFile_Click(object sender, EventArgs e)
        {
            //Otwarcie pojedynczego pliku o rozszerzeniu .zip z poziomu listy plików w zakładce Info
            if ((listViewFiles.SelectedItems.Count == 1) && (listViewFiles.SelectedItems[0].SubItems[1].Text.Substring((listViewFiles.SelectedItems[0].SubItems[1].Text.Length - 4), 4) == ".zip"))
            {

                string FileName = listViewFiles.SelectedItems[0].SubItems[1].Text;
                string FilePath = listViewFiles.SelectedItems[0].SubItems[4].Text;
                string FullName = FilePath + "\\" + FileName;

                OpenZip otworzZip = new OpenZip();
                #region ReadZip
                using (ZipFile zip = ZipFile.Read(FullName))
                {
                    int i = 1;
                    foreach (ZipEntry e1 in zip)
                    {
                        ListViewItem plikInfo = new ListViewItem();
                        plikInfo.Text = i.ToString();
                        plikInfo.SubItems.Add(e1.FileName);
                        plikInfo.SubItems.Add(e1.LastModified.ToString());
                        if ((e1.UncompressedSize / 1024) < 1)
                        {
                            plikInfo.SubItems.Add(e1.UncompressedSize.ToString() + " B");
                        }
                        else if (((e1.UncompressedSize / 1024) >= 1) && ((e1.UncompressedSize / 1024) < 1024))
                        {
                            plikInfo.SubItems.Add((e1.UncompressedSize / 1024).ToString() + " KB");
                        }
                        else if ((e1.UncompressedSize / (1024 * 1024)) >= 1)
                        {
                            plikInfo.SubItems.Add((e1.UncompressedSize / (1024 * 1024)).ToString() + " MB");
                        }
                        plikInfo.SubItems.Add(FullName);
                        otworzZip.DodajItem(plikInfo);
                        i++;
                    }
                    //listViewFiles.Items.Add(plikInfo);
                }
                #endregion ReadZip
                otworzZip.ShowDialog(this);
            }
            else
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
                        analizeUplookLog(atrybutyPlik.FullName, atrybutyPlik.Name, atrybutyPlik.LastWriteTime.ToString());
                    }
                }
            }
        }

        private void toolStripMenuItemDeleteFile_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem ZaznaczoneWiersze in listViewFiles.SelectedItems)
            {
                string FileName = ZaznaczoneWiersze.SubItems[1].Text;
                string FilePath = ZaznaczoneWiersze.SubItems[4].Text;
                string FullName = FilePath +"\\"+ FileName;
            
                FileInfo Plik = new FileInfo(FullName);

                if (MessageBox.Show("Really delete file: " + FullName + " ?", "Confirm delete", MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (Plik.Exists)
                    {
                        try
                        {
                            Plik.Delete();
                            listViewFiles.Items.Clear();
                            IniTabPageInfo();
                        }
                        catch (Exception ex)
                        {
                        MessageBox.Show("Error : " + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void toolStripMenuItemCopyFileName_Click(object sender, EventArgs e)
        {
            string listaNazwPlikow = null;
            foreach (ListViewItem ZaznaczoneWiersze in listViewFiles.SelectedItems)
            {
                string FileName = ZaznaczoneWiersze.SubItems[1].Text;
                string FilePath = ZaznaczoneWiersze.SubItems[4].Text;
                string FullName = FilePath + "\\" + FileName;
                FileInfo Plik = new FileInfo(FullName);
                listaNazwPlikow += Plik.Name;
                listaNazwPlikow += Environment.NewLine;
            }
            Clipboard.SetText(listaNazwPlikow, TextDataFormat.UnicodeText);
        }

        private void toolStripMenuItem3_Click_1(object sender, EventArgs e)
        {
            string fileFullName = tabControlMain.SelectedTab.ToolTipText;
            FileInfo fileInfo = new FileInfo(fileFullName);
            Clipboard.SetText(fileInfo.Name);
            System.Diagnostics.Process.Start("explorer.exe", fileInfo.DirectoryName); 
        }

        private void toolStripMenuItemCopyFilePath_Click(object sender, EventArgs e)
        {
            string listaNazwPlikow = null;
            foreach (ListViewItem ZaznaczoneWiersze in listViewFiles.SelectedItems)
            {
                string FileName = ZaznaczoneWiersze.SubItems[1].Text;
                string FilePath = ZaznaczoneWiersze.SubItems[4].Text;
                string FullName = FilePath + "\\" + FileName;
                FileInfo Plik = new FileInfo(FullName);
                listaNazwPlikow += Plik.FullName;
                listaNazwPlikow += Environment.NewLine; ;
            }
            Clipboard.SetText(listaNazwPlikow, TextDataFormat.UnicodeText);
        }

        private void toolStripMenuItemCopyCatalogPath_Click(object sender, EventArgs e)
        {
            string listaNazwPlikow = null;
            foreach (ListViewItem ZaznaczoneWiersze in listViewFiles.SelectedItems)
            {
                string FileName = ZaznaczoneWiersze.SubItems[1].Text;
                string FilePath = ZaznaczoneWiersze.SubItems[4].Text;
                string FullName = FilePath + "\\" + FileName;
                FileInfo Plik = new FileInfo(FullName);
                listaNazwPlikow += Plik.DirectoryName;
                listaNazwPlikow += Environment.NewLine;
            }
            Clipboard.SetText(listaNazwPlikow, TextDataFormat.UnicodeText);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listViewFiles.Items.Clear();
            IniTabPageInfo();
        }

        private void listViewFiles_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && listViewFiles.SelectedItems.Count > 1)
            {
                openContainFolderToolStripMenuItem.Enabled = false;
                contextMenuStripList.Show(MousePosition);
            }
            else if (e.Button == MouseButtons.Right && listViewFiles.SelectedItems.Count == 1)
            {
                openContainFolderToolStripMenuItem.Enabled = true;
                contextMenuStripList.Show(MousePosition);
            }
        }

        private void openContainFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listViewFiles.SelectedItems)
            {
                string FullName = item.SubItems[4].Text + "\\" + item.SubItems[1].Text;
                FileInfo Plik = new FileInfo(FullName);
                System.Diagnostics.Process.Start("explorer.exe", Plik.DirectoryName); 

            }
            
        }

        private void analizeUplookLog(string SafeFileName, string FileName,string dataUtworzenia)
        {
            TabControl TabC = (TabControl)Controls.Find("tabControlMain", true)[0];
            if (TabC.Controls.Find(FileName, false).Length == 0)
            {
                PlikLogu plik = new PlikLogu();
                NewPage nowaKarta = plik.analizeUplookLog(SafeFileName, FileName, dataUtworzenia, uplookDeskryptor);

                if (nowaKarta.TypeOfReport.Equals("uplook"))
                {
                    ToolStripMenuItemUplook.Enabled = true;
                    ToolStripMenuItemUplook.Visible = true;
                    ToolStripMenuItemUSM.Visible = false;
                    int j = 0;
                    foreach (ToolStripMenuItem t in ToolStripMenuItemUplook.DropDownItems)
                    {
                        for (int i = 0; i < uplookDeskryptor.uplook_Headers.Length; i++)
                        {
                            if (t.Name.Equals("uplook" + uplookDeskryptor.uplook_Headers[i]))
                            {
                                if (show_uplook[j])
                                {
                                    t.CheckState = CheckState.Checked; 
                                }
                                else
                                {
                                    t.CheckState = CheckState.Unchecked; 
                                }
                                break;
                            }
                        }
                        j++;
                    }
                    
                }
                if (nowaKarta.TypeOfReport.Equals("usm"))
                {
                    ToolStripMenuItemUplook.Visible = false;
                    ToolStripMenuItemUSM.Enabled = true;
                    ToolStripMenuItemUSM.Visible = true;
                    int j = 0;
                    foreach (ToolStripMenuItem t in ToolStripMenuItemUSM.DropDownItems)
                    {
                        for (int i = 0; i < uplookDeskryptor.usm_Headers.Length; i++)
                        {
                            if (t.Name.Equals("usm" + uplookDeskryptor.usm_Headers[i]))
                            {
                                if (show_usm[j])
                                {
                                    t.CheckState = CheckState.Checked; ;
                                }
                                else
                                {
                                    t.CheckState = CheckState.Unchecked; ;
                                }
                                break;
                            }
                        }
                        j++;
                    }
                }
                else 
                { 
                }


                tabControlMain.Controls.Add(nowaKarta.NewTabPage);
                tabControlMain.SelectTab(nowaKarta.NewTabPage);
                ShowColumns(nowaKarta.ListViewExtended, nowaKarta.TypeOfReport);
                //Aktywownie menu grupowania 
                grupyToolStripMenuItem.Enabled = true;
                zwinToolStripMenuItem.Enabled = true;
                rozwinWszystkieToolStripMenuItem.Enabled = true;
                //Aktywowanie przycisków grupowania
                toolStripButtonCollapsedAll.Enabled = true;
                toolStripButtonNormalAll.Enabled = true;
                //Aktywowanie przycisków menu File
                closeAllWithoutActiveToolStripMenuItem.Enabled = true;
                closeAllToolStripMenuItem.Enabled = true;
                //Aktywowanie przycisków widoku
                //ToolStripMenuItemUplook.Enabled = true;
                ZwinGrupy();
                //RozwinGrupy();
            }
            else
            {
                TabC.SelectTab(FileName);
            }

        }

        private void ToolStripMenuItemUplook_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)e.ClickedItem;
            Descriptor[] udes = _config.GetStatlookDescriptors();
            if (t.CheckState == CheckState.Checked)
            {
                t.CheckState = CheckState.Unchecked;
                foreach (Descriptor ud in udes)
                {
                    if (t.Name == ud.KeyName)
                    {
                        ud.Show = false;
                        _config.SetHeaderVisibility(ud.KeyName, false);
                    }
                }
            }
            else
            {
                t.CheckState = CheckState.Checked;
                foreach (Descriptor ud in udes)
                {
                    if (t.Name == ud.KeyName)
                    {
                        ud.Show = true;
                        _config.SetHeaderVisibility(ud.KeyName, true);
                    }
                }
            }

            TabControl TabC = (TabControl)Controls.Find("tabControlMain", true)[0];
            if (TabC.SelectedTab.Name != "tabPageInfo")
            {
                TabPage TabP = (TabPage)Controls.Find(TabC.SelectedTab.Name, true)[0];

                foreach (Control control in TabP.Controls)
                {
                    if (control.GetType() == typeof(ListViewExtended))
                    {
                        ListViewExtended ListV = (ListViewExtended)control;
                        ShowColumns(ListV, "uplook");
                    }
                }
            }
        }

        private void ToolStripMenuItemUSM_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)e.ClickedItem;
            Descriptor[] usmdes = _config.GetUsmDescriptors();
            if (t.CheckState == CheckState.Checked)
            {
                t.CheckState = CheckState.Unchecked;
                foreach (Descriptor usmd in usmdes)
                {
                    if (t.Name == usmd.KeyName)
                    {
                        usmd.Show = false;
                        _config.SetHeaderVisibility(usmd.KeyName, false);
                    }
                }
            }
            else
            {
                t.CheckState = CheckState.Checked;
                foreach (Descriptor usmd in usmdes)
                {
                    if (t.Name == usmd.KeyName)
                    {
                        usmd.Show = true;
                        _config.SetHeaderVisibility(usmd.KeyName, true);
                    }
                }
            }

            TabControl TabC = (TabControl)Controls.Find("tabControlMain", true)[0];
            if (TabC.SelectedTab.Name != "tabPageInfo")
            {
                TabPage TabP = (TabPage)Controls.Find(TabC.SelectedTab.Name, true)[0];

                foreach (Control control in TabP.Controls)
                {
                    string kk = control.ToString();
                    if (control.GetType() == typeof(ListViewExtended))
                    {
                        ListViewExtended ListV = (ListViewExtended)control;
                        ShowColumns(ListV, "usm");
                    }
                }
            }
 
        }

        private void SetDashboardData(string directoryLogPath, Label label ,Label labelSize, Label labelCount)
        {
            try
            {
                DirectoryInfo s = new DirectoryInfo(directoryLogPath);

                if(s.Exists)
                {
                    DirectoryInfo[] directories = s.GetDirectories();

                    label.Text = s.FullName;

                    ArrayList myfileinfos = new ArrayList();

                    foreach (string ext in _fileExtensions)
                    {
                        myfileinfos.AddRange(s.GetFiles(ext, SearchOption.AllDirectories));
                    }

                    FileInfo[] files = (FileInfo[])myfileinfos.ToArray(typeof(FileInfo));

                    float rozmiar = 0;

                    for (int i = 0; i < files.Length; i++)
                    {
                        rozmiar += files[i].Length;
                    }

                    labelSize.Text = FileSize(rozmiar, true);

                    labelCount.Text = (files.Length).ToString() + " plików, " + directories.Length.ToString() + " folderów";
                }
            }
            catch
            {

            }
        }

        private void checkBoxLogs_CheckedChanged(object sender, EventArgs e)
        {
            WypelnijListe();
        }

        private void checkBoxUSM_CheckedChanged(object sender, EventArgs e)
        {
            WypelnijListe();
        }

        private void checkBoxUser_CheckedChanged(object sender, EventArgs e)
        {
            WypelnijListe();
        }

        private static ListViewItem[] CloneItems(ListView.ListViewItemCollection items)
        {
            ListViewItem[] nodes = new ListViewItem[items.Count];

            for (int i = 0; i < items.Count; i++)
            {
                nodes[i] = items[i].Clone() as ListViewItem;
            }
            return nodes;
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {         
                timerFind.Enabled = false;
                timerFind.Enabled = true;
        }

        private void DelayedSearch()
        {
            timerFind.Start();
        }

        private void timerFind_Tick(object sender, EventArgs e)
        {
            //changed = false;
            timerFind.Enabled = false;
            Search();
        }
        private void Search()
        {
            TabControl TabC = (TabControl)Controls.Find("tabControlMain", true)[0];
            if (TabC.SelectedTab.Name == "tabPageInfo")
            {
                WypelnijListe();
                ListView LVTmp = new ListView();
                LVTmp.Items.Clear();
                //bool find = false;

                int colCount = listViewFiles.Columns.Count;

                for (int lst12 = 1; lst12 < listViewFiles.Items.Count; lst12++)
                {
                    for (int colAll = 0; colAll < colCount; colAll++)
                    {
                        if (listViewFiles.Items[lst12].SubItems[colAll].Text.IndexOf(toolStripTextBox.Text) > -1 |
                            listViewFiles.Items[lst12].SubItems[colAll].Text.IndexOf(toolStripTextBox.Text, StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            LVTmp.Items.Add((ListViewItem)listViewFiles.Items[lst12].Clone());
                            break;
                        }

                    }

                }
                listViewFiles.Items.Clear();
                listViewFiles.Items.AddRange(CloneItems(LVTmp.Items));
            }
            else
            {
                TabPage TabP = (TabPage)Controls.Find(TabC.SelectedTab.Name, true)[0];
                foreach (Control control in TabP.Controls)
                {
                    if (control.GetType() == typeof(ListViewExtended))
                    {
                        ListViewExtended ListV = (ListViewExtended)control;
                        if (toolStripTextBox.Text != String.Empty)
                        {
                            for (int i = 0; i < ListV.Items.Count; i++)
                            {
                                ListV.Items[i].BackColor = Color.White;
                            }
                            int colCount = ListV.Columns.Count;
                            for (int lst12 = 0; lst12 < ListV.Items.Count; lst12++)
                            {
                                for (int colAll = 0; colAll < colCount; colAll++)
                                {
                                    if (ListV.Columns[colAll].Width != 0)
                                    {
                                        if (ListV.Items[lst12].SubItems[colAll].Text.IndexOf(toolStripTextBox.Text) > -1 |
                                        ListV.Items[lst12].SubItems[colAll].Text.IndexOf(toolStripTextBox.Text, StringComparison.OrdinalIgnoreCase) > -1)
                                        {
                                            ListV.SuspendLayout();
                                            ListV.BeginUpdate();
                                            //ListV.SetGroupFooter(ListV.Items[lst12].Group, "Test");
                                            ListV.SetOneGroupState(ListV.Items[lst12].Group, ListViewGroupState.Collapsible);
                                            ListV.Items[lst12].BackColor = Color.Aqua;
                                            ListV.EndUpdate();
                                            ListV.ResumeLayout();
                                        }
                                        else
                                        {
                                            //ListV.SetOneGroupState(ListV.Items[lst12].Group, ListViewGroupState.Collapsed);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ListV.Items.Count; i++)
                            {
                                ListV.SuspendLayout();
                                ListV.BeginUpdate();
                                ListV.SetOneGroupState(ListV.Items[i].Group, ListViewGroupState.Collapsible | ListViewGroupState.Collapsed);
                                ListV.Items[i].BackColor = Color.White;
                                ListV.EndUpdate();
                                ListV.ResumeLayout();
                            }

                        }
                    }
                }

            }

        }

        private void analizeToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                using var options = new Options();

                if (options.ShowDialog(this) == DialogResult.OK)
                {
                    _config = Configuration.GetConfiguration();

                    IniTabPageInfo();
                }
            }
            catch (Exception exception)
            { 
            }
        }

    }

}
