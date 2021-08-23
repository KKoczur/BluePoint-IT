using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using ListViewGroupCollapse;
using Ionic.Zip;
using System.Collections.Generic;

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
        private readonly LogHeader _logHeader = new LogHeader();
        private readonly List<bool> show_uplook = new List<bool>();
        private readonly List<bool> show_usm = new List<bool>();
        private Configuration _config;

        #endregion Members

        #region Constructors

        public MainForm()
        {
            InitializeComponent();

            _config = Configuration.GetConfiguration();

            Descriptor[] udes = _config.GetStatlookDescriptors();

            foreach (Descriptor d in udes)
            {
                show_uplook.Add(d.Show);
            }

            Descriptor[] usmdes = _config.GetUsmDescriptors();

            foreach (Descriptor d in usmdes)
            {
                show_usm.Add(d.Show);
            }

            OSVersion = Environment.OSVersion.Version.Major.ToString();
            LogDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _config.StatlookLogDirectory;
            USMDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _config.StatlookUsmLogDirectory;
            _userDirectory = _config.UserDirectory;
            _fileExtensions = _config.LogFileExtensions.Split(new char[] { ';' });


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
                viewMenuItem.Text = udes[i].RowCaption;
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
                viewMenuItem.Text = usmdes[i].RowCaption;
                ToolStripMenuItemUSM.DropDownItems.Add(viewMenuItem);
            }


            _lvwColumnSorter = new ListViewColumnSorter();
            this.listViewFiles.ListViewItemSorter = _lvwColumnSorter;
            IniTabPageInfo();
        }

        #endregion Constructors

        #region Methods

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

            var directoryInfo = new DirectoryInfo[]{
            new DirectoryInfo(LogDirectory),
            new DirectoryInfo(USMDirectory),
            new DirectoryInfo(_userDirectory)
            };

            bool[] showCatalog = new bool[]{
            checkBoxLogs.Checked,
            checkBoxUSM.Checked,
            checkBoxUser.Checked
            };

            try
            {
                int j = 0;
                foreach (DirectoryInfo di in directoryInfo)
                {
                    if (di.Exists && showCatalog[j])
                    {
                        var fileCollection = new List<FileInfo>();

                        foreach (string ext in _fileExtensions)
                        {
                            if (di.FullName != "C:\\")
                            {
                                fileCollection.AddRange(di.GetFiles(ext, SearchOption.AllDirectories));
                            }
                            else
                            {
                                fileCollection.AddRange(di.GetFiles(ext, SearchOption.TopDirectoryOnly));
                            }
                        }

                        int i = 0;
                        foreach (FileInfo fileInfo in fileCollection)
                        {
                            var listViewItem = new ListViewItem
                            {
                                Text = i.ToString()
                            };

                            listViewItem.SubItems.Add(fileInfo.Name);
                            listViewItem.SubItems.Add(fileInfo.LastWriteTime.ToString());
                            listViewItem.SubItems.Add(FileSize(fileInfo.Length, false));
                            listViewItem.SubItems.Add(fileInfo.DirectoryName);
                            listViewFiles.Items.Add(listViewItem);

                            i++;
                        }
                        toolStripButtonIcon.Image = Properties.Resources.ok_16;
                        toolStripStatusReady.Text = "Ready";
                    }
                    j++;
                }

                // Loop through and size each column header to fit the column header text.
                foreach (ColumnHeader ch in this.listViewFiles.Columns)
                {
                    if (ch.Index == 0)
                    {
                        ch.Width = 0;
                    }
                    else
                    {
                        ch.Width = -2;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
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
                            plikInfo.SubItems.Add(FileSize(pliki[i - 1].Length, true));
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
                    if (ch.Index == 0)
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

        private string GetFileFullPathFromListViewItem(ListViewItem listViewItem)
        {
            string fileName = listViewItem.SubItems[1].Text;
            string path = listViewItem.SubItems[4].Text;
            return path + "\\" + fileName;
        }

        private FileInfo GetFileInfoForSelectedTab()
        {
            string fileFullName = tabControlMain.SelectedTab.ToolTipText;
            return new FileInfo(fileFullName);
        }

        /// <summary>
        /// Open log file
        /// </summary>
        private void OpenLogFile()
        {
            using var openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = Configuration.LOG_FILE_EXTENSIONS,
                Title = "Please select log source file"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                //Otwarcie pojedynczego pliku o rozszerzeniu .zip
                if ((openFileDialog.FileNames.Length == 1) && (openFileDialog.SafeFileName.Substring(openFileDialog.SafeFileName.Length - 4, 4) == Configuration.ZIP_FILE_EXTENSION))
                {
                    OpenZip openZip = new OpenZip(openFileDialog.FileName);

                    using (ZipFile zip = ZipFile.Read(openFileDialog.FileName))
                    {
                        int i = 1;
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
                                plikInfo.SubItems.Add(e.UncompressedSize.ToString() + " B");
                            }
                            else if (((e.UncompressedSize / 1024) >= 1) && ((e.UncompressedSize / 1024) < 1024))
                            {
                                plikInfo.SubItems.Add((e.UncompressedSize / 1024).ToString() + " KB");
                            }
                            else if ((e.UncompressedSize / (1024 * 1024)) >= 1)
                            {
                                plikInfo.SubItems.Add((e.UncompressedSize / (1024 * 1024)).ToString() + " MB");
                            }
                            plikInfo.SubItems.Add(openFileDialog.InitialDirectory);
                            openZip.DodajItem(plikInfo);
                            i++;
                        }
                    }

                    openZip.ShowDialog(this);

                }
                else
                {
                    for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                    {
                        // Nie przetwarzaj plików o rozszerzeniu .zip
                        FileInfo fileInfo = new FileInfo(openFileDialog.FileNames[i]);

                        if (fileInfo.Extension == Configuration.ZIP_FILE_EXTENSION)
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
                                CollapseAllGroups();
                                //RozwinGrupy();

                            }

                        }
                        else
                        {
                            analizeUplookLog(openFileDialog.FileNames[i], fileInfo.LastWriteTime);
                        }
                    }
                }
            }
        }

        private string FileSize(float size, bool useByte)
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

        private void ShowListViewColumns(ListView listView, LogType logType)
        {
            ToolStripItemCollection toolStripItemCollection = null;

            if (logType == LogType.Statlook)
                toolStripItemCollection = ToolStripMenuItemUplook.DropDownItems;
            else if (logType == LogType.Usm)
                toolStripItemCollection = ToolStripMenuItemUSM.DropDownItems;

            if (toolStripItemCollection == null)
                return;

            // Loop through and size each column header to fit the column header text.
            foreach (ColumnHeader columnHeader in listView.Columns)
            {
                foreach (ToolStripMenuItem toolStripMenuItem in toolStripItemCollection)
                {
                    if (string.Compare(columnHeader.Text, toolStripMenuItem.Text, true) == 0)
                    {
                        if (toolStripMenuItem.CheckState == CheckState.Checked)
                            columnHeader.Width = -2;
                        else
                            columnHeader.Width = 0;
                    }
                }
            }
        }

        private void CollapseAllGroups() => ChangeGroupState(ListViewGroupState.Collapsed);

        private void ExpandAllGroups() => ChangeGroupState(ListViewGroupState.Normal);

        private void ChangeGroupState(ListViewGroupState listViewGroupState)
        {
            TabControl TabC = (TabControl)Controls.Find("tabControlMain", true)[0];
            if (TabC.SelectedTab.Name != "tabPageInfo")
            {
                TabPage TabP = (TabPage)Controls.Find(TabC.SelectedTab.Name, true)[0];

                foreach (Control control in TabP.Controls)
                {
                    if (control.GetType() == typeof(ListViewExtended))
                    {
                        ListViewExtended listViewExtended = (ListViewExtended)control;

                        listViewExtended.SuspendLayout();
                        listViewExtended.BeginUpdate();

                        try
                        {
                            listViewExtended.SetGroupState(ListViewGroupState.Collapsible | listViewGroupState);
                        }
                        finally
                        {
                            listViewExtended.EndUpdate();
                            listViewExtended.ResumeLayout();
                        }
                    }
                }
            }
        }

        private void ClosePage()
        {
            int selectedTapPageIndex = tabControlMain.SelectedIndex;

            tabControlMain.TabPages.Remove(tabControlMain.SelectedTab);

            if (tabControlMain.TabPages.Count > 1)
            {
                if (selectedTapPageIndex == tabControlMain.TabPages.Count)
                {
                    tabControlMain.SelectedIndex = selectedTapPageIndex - 1;
                }
                else
                {
                    tabControlMain.SelectedIndex = selectedTapPageIndex;
                }
            }
        }

        #endregion Methods

        #region Event Handlers

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e) => OpenLogFile();

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
            foreach (ListViewItem listViewItem in this.listViewFiles.SelectedItems)
            {
                string fileFullPath = GetFileFullPathFromListViewItem(listViewItem);

                FileInfo fileInfo = new FileInfo(fileFullPath);

                //Nie przetwarzaj plików o rozszerzeniu .zip
                if (fileInfo.Extension == Configuration.ZIP_FILE_EXTENSION)
                {
                    OpenZip openZip = new OpenZip();

                    using (ZipFile zip = ZipFile.Read(fileInfo.FullName))
                    {
                        int i = 1;
                        foreach (ZipEntry e2 in zip)
                        {
                            ListViewItem plikInfo = new ListViewItem
                            {
                                Text = i.ToString()
                            };
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
                            plikInfo.SubItems.Add(fileInfo.DirectoryName);
                            openZip.DodajItem(plikInfo);
                            i++;
                        }
                    }

                    openZip.ShowDialog(this);
                }
                else
                {
                    DateTime.TryParse(listViewItem.SubItems[2].Text, out DateTime lastWriteTime);

                    analizeUplookLog(fileFullPath, lastWriteTime);
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
                LogTapPage tabPage = Controls.Find(tabControl.SelectedTab.Name, true)[0] as LogTapPage;

                foreach (Control control in tabPage.Controls)
                {
                    if (control.GetType() == typeof(ListViewExtended))
                    {
                        if (tabPage.LogType == LogType.Statlook)
                        {
                            ToolStripMenuItemUplook.Visible = true;
                            ToolStripMenuItemUSM.Visible = false;
                        }
                        else if (tabPage.LogType == LogType.Usm)
                        {
                            ToolStripMenuItemUplook.Visible = false;
                            ToolStripMenuItemUSM.Visible = true;
                        }

                        ListViewExtended listViewExtended = (ListViewExtended)control;

                        ShowListViewColumns(listViewExtended, tabPage.LogType);
                    }
                }

                FileInfo fileInfo = new FileInfo(tabControl.SelectedTab.Tag.ToString());

                if (fileInfo.Exists)
                {
                    toolStripButtonIcon.Image = Properties.Resources.ok_16;
                    toolStripStatusReady.Text = fileInfo.FullName;
                    toolStripSeparator_1.Visible = true;
                    toolStripLabelCreationTime.Text = fileInfo.LastWriteTime.ToString();
                    toolStripSeparator_2.Visible = true;
                    toolStripLableSize.Text = FileSize(fileInfo.Length, true);
                }
                else
                {
                    MessageBox.Show($"File {fileInfo.FullName} does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        private void zwinToolStripMenuItem_Click(object sender, EventArgs e) => CollapseAllGroups();

        private void rozwinWszystkieToolStripMenuItem_Click(object sender, EventArgs e) => ExpandAllGroups();

        private void toolStripButtonCollapsedAll_Click(object sender, EventArgs e) => CollapseAllGroups();

        private void toolStripButtonNormalAll_Click(object sender, EventArgs e) => ExpandAllGroups();

        private void closeToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab.Name == "tabPageInfo")
                return;

            ClosePage();
        }

        private void toolStripButtonClose_Click(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab.Name == "tabPageInfo")
                toolStripMenuItemGeneral.Visible = true;
            else
                ClosePage();
        }


        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tabControlMain.TabCount; i++)
            {
                if (i != 0)
                {
                    tabControlMain.TabPages.Remove(tabControlMain.TabPages[1]);
                }
            }
        }

        private void closeAllWithoutActiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selectedIndex = tabControlMain.SelectedIndex;
            for (int i = tabControlMain.TabCount - 1; i > 0; i--)
            {
                if ((i != 0) && (i != selectedIndex))
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
                int selectedIndex = tabControlMain.SelectedIndex;
                tabControlMain.TabPages.Remove(tabControlMain.SelectedTab);
                if (tabControlMain.TabPages.Count > 1)
                {
                    if (selectedIndex == tabControlMain.TabPages.Count)
                    {
                        tabControlMain.SelectedIndex = selectedIndex - 1;
                    }
                    else
                    {
                        tabControlMain.SelectedIndex = selectedIndex;
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
            int tabCount = tabControlMain.TabCount - 1;
            int selectedIndex = tabControlMain.SelectedIndex;
            for (int i = tabCount; i > 0; i--)
            {
                if ((i != 0) && (i != selectedIndex))
                {
                    tabControlMain.TabPages.Remove(tabControlMain.TabPages[i]);
                }
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            FileInfo fileInfo = GetFileInfoForSelectedTab();
            Clipboard.SetText(fileInfo.Name);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            FileInfo fileInfo = GetFileInfoForSelectedTab();
            Clipboard.SetText(fileInfo.FullName);
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            FileInfo fileInfo = GetFileInfoForSelectedTab();
            Clipboard.SetText(fileInfo.DirectoryName);
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            FileInfo fileInfo = GetFileInfoForSelectedTab();

            if (MessageBox.Show($"Really delete file: {fileInfo.FullName} ?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (fileInfo.Exists)
                {
                    try
                    {
                        tabControlMain.TabPages.Remove(tabControlMain.SelectedTab);
                        fileInfo.Delete();
                        listViewFiles.Items.Clear();
                        IniTabPageInfo();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void toolStripMenuItemOpenFile_Click(object sender, EventArgs e)
        {
            //Otwarcie pojedynczego pliku o rozszerzeniu .zip z poziomu listy plików w zakładce Info
            if ((listViewFiles.SelectedItems.Count == 1) && (listViewFiles.SelectedItems[0].SubItems[1].Text.Substring(listViewFiles.SelectedItems[0].SubItems[1].Text.Length - 4, 4) == ".zip"))
            {
                string fileFullPath = GetFileFullPathFromListViewItem(listViewFiles.SelectedItems[0]);

                OpenZip otworzZip = new OpenZip();

                using (ZipFile zipFile = ZipFile.Read(fileFullPath))
                {
                    int i = 1;

                    foreach (ZipEntry e1 in zipFile)
                    {
                        ListViewItem plikInfo = new ListViewItem
                        {
                            Text = i.ToString()
                        };

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
                        plikInfo.SubItems.Add(fileFullPath);
                        otworzZip.DodajItem(plikInfo);
                        i++;
                    }
                    //listViewFiles.Items.Add(plikInfo);
                }

                otworzZip.ShowDialog(this);
            }
            else
            {
                foreach (ListViewItem listViewItem in listViewFiles.SelectedItems)
                {
                    string fileFullPath = GetFileFullPathFromListViewItem(listViewItem);

                    FileInfo fileInfo = new FileInfo(fileFullPath);

                    //Nie przetwarzaj plików o rozszerzeniu .zip
                    if (fileInfo.Extension == Configuration.ZIP_FILE_EXTENSION)
                    {
                        using var openZip = new OpenZip();
                        openZip.ShowDialog(this);
                    }
                    else
                    {
                        analizeUplookLog(fileInfo.FullName, fileInfo.LastWriteTime);
                    }
                }
            }
        }

        private void toolStripMenuItemDeleteFile_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listViewFiles.SelectedItems)
            {
                string fileFullPath = GetFileFullPathFromListViewItem(listViewItem);

                FileInfo fileInfo = new FileInfo(fileFullPath);

                if (MessageBox.Show("Really delete file: " + fileFullPath + " ?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    if (fileInfo.Exists)
                    {
                        try
                        {
                            fileInfo.Delete();
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
            string fileNamesText = string.Empty;

            foreach (ListViewItem listViewItem in listViewFiles.SelectedItems)
            {
                string fileFullPath = GetFileFullPathFromListViewItem(listViewItem);

                FileInfo fileInfo = new FileInfo(fileFullPath);
                fileNamesText += fileInfo.Name + Environment.NewLine;
            }

            Clipboard.SetText(fileNamesText, TextDataFormat.UnicodeText);
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
            string fileFullNamesText = string.Empty;

            foreach (ListViewItem listViewItem in listViewFiles.SelectedItems)
            {
                string fileFullPath = GetFileFullPathFromListViewItem(listViewItem);

                FileInfo fileInfo = new FileInfo(fileFullPath);
                fileFullNamesText += fileInfo.FullName + Environment.NewLine;
            }

            Clipboard.SetText(fileFullNamesText, TextDataFormat.UnicodeText);
        }

        private void toolStripMenuItemCopyCatalogPath_Click(object sender, EventArgs e)
        {
            string fileDirectoryNamesText = string.Empty;

            foreach (ListViewItem listViewItem in listViewFiles.SelectedItems)
            {
                string fileFullPath = GetFileFullPathFromListViewItem(listViewItem);

                FileInfo fileInfo = new FileInfo(fileFullPath);
                fileDirectoryNamesText += fileInfo.DirectoryName + Environment.NewLine;
            }

            Clipboard.SetText(fileDirectoryNamesText, TextDataFormat.UnicodeText);
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
            foreach (ListViewItem listViewItem in this.listViewFiles.SelectedItems)
            {
                string fileFullPath = GetFileFullPathFromListViewItem(listViewItem);

                FileInfo fileInfo = new FileInfo(fileFullPath);

                System.Diagnostics.Process.Start("explorer.exe", fileInfo.DirectoryName);
            }
        }

        private void analizeUplookLog(string filePath, DateTime lastWriteTime)
        {
            string fileName = Path.GetFileName(filePath);

            TabControl TabC = (TabControl)Controls.Find("tabControlMain", true)[0];
            if (TabC.Controls.Find(fileName, false).Length == 0)
            {
                LogLineCollection logLineCollection = new LogLineCollection();

                LogTapPage newPage = logLineCollection.LogAnalyze(filePath);

                if (newPage.LogType == LogType.Statlook)
                {
                    ToolStripMenuItemUplook.Enabled = true;
                    ToolStripMenuItemUplook.Visible = true;
                    ToolStripMenuItemUSM.Visible = false;
                    int j = 0;
                    foreach (ToolStripMenuItem t in ToolStripMenuItemUplook.DropDownItems)
                    {
                        for (int i = 0; i < _logHeader.GetStatlookTextHeaders().Length; i++)
                        {
                            if (t.Name.Equals("uplook" + _logHeader.GetStatlookTextHeaders()[i]))
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
                if (newPage.LogType == LogType.Usm)
                {
                    ToolStripMenuItemUplook.Visible = false;
                    ToolStripMenuItemUSM.Enabled = true;
                    ToolStripMenuItemUSM.Visible = true;
                    int j = 0;
                    foreach (ToolStripMenuItem t in ToolStripMenuItemUSM.DropDownItems)
                    {
                        for (int i = 0; i < _logHeader.GetUsmTextHeaders().Length; i++)
                        {
                            if (t.Name.Equals("usm" + _logHeader.GetUsmTextHeaders()[i]))
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

                tabControlMain.Controls.Add(newPage);
                tabControlMain.SelectTab(newPage);
                ShowListViewColumns(newPage.ListViewExtended, newPage.LogType);

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
                CollapseAllGroups();

            }
            else
            {
                TabC.SelectTab(fileName);
            }

        }

        private void ToolStripMenuItemUplook_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Descriptor[] udes = _config.GetStatlookDescriptors();

            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)e.ClickedItem;

            if (toolStripMenuItem.CheckState == CheckState.Checked)
            {
                toolStripMenuItem.CheckState = CheckState.Unchecked;

                foreach (Descriptor ud in udes)
                {
                    if (toolStripMenuItem.Name == ud.KeyName)
                    {
                        ud.Show = false;
                        _config.SetHeaderVisibility(ud.KeyName, false);
                    }
                }
            }
            else
            {
                toolStripMenuItem.CheckState = CheckState.Checked;
                foreach (Descriptor ud in udes)
                {
                    if (toolStripMenuItem.Name == ud.KeyName)
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
                        ShowListViewColumns(ListV, LogType.Statlook);
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
                        ShowListViewColumns(ListV, LogType.Usm);
                    }
                }
            }

        }

        private void SetDashboardData(string directoryLogPath, Label label, Label labelSize, Label labelCount)
        {
            try
            {
                DirectoryInfo s = new DirectoryInfo(directoryLogPath);

                if (s.Exists)
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
                                ListV.SetOneGroupState(ListV.Items[i].Group, state: ListViewGroupState.Collapsible
                                    | ListViewGroupState.Collapsed);
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

        #endregion Event Handlers

    }

}
