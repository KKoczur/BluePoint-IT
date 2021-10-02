using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Ionic.Zip;
using System.Collections.Generic;
using StatlookLogViewer.Views;
using StatlookLogViewer.Model;
using StatlookLogViewer.Controller;
using StatlookLogViewer.Model.Pattern;
using System.Linq;
using StatlookLogViewer.Parser;
using StatlookkLogViewer.Tools;

namespace StatlookLogViewer
{

    public partial class MainForm : Form
    {
        #region Members

        private readonly ListViewColumnSorter _lvwColumnSorter;
        public string _logDirectory;
        public string _userLogDirectory;
        private readonly string[] _fileExtensions;
        private readonly List<bool> show_uplook = new();
        private readonly List<bool> show_usm = new();
        private Configuration _config;

        #endregion Members

        #region Constructors

        public MainForm()
        {
            InitializeComponent();

            _config = Configuration.GetConfiguration();

            LogPattern[] udes = _config.GetStatlookLogPatterns().ToArray();

            foreach (LogPattern d in udes)
            {
                show_uplook.Add(d.Show);
            }

            LogPattern[] usmdes = _config.GetUsmLogPatterns().ToArray();

            foreach (LogPattern d in usmdes)
            {
                show_usm.Add(d.Show);
            }

            _logDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _config.StatlookLogDirectory;
            _userLogDirectory = _config.UserDirectory;
            _fileExtensions = _config.LogFileExtensions.Split(new char[] { ';' });

            // Utworzenie menu widocznosci kolumn
            CreateToolStripMenuItem(ToolStripMenuItemUplook, udes);
            CreateToolStripMenuItem(ToolStripMenuItemUSM, usmdes);


            _lvwColumnSorter = new ListViewColumnSorter();
            this.listViewFiles.ListViewItemSorter = _lvwColumnSorter;

            IniTabPageInfo();
        }

        private static void CreateToolStripMenuItem(ToolStripMenuItem rootMenu, LogPattern[] logPatterns)
        {
            foreach (LogPattern logPattern in logPatterns)
            {
                ToolStripMenuItem toolStripMenuItem = new()
                {
                    Checked = logPattern.Show,
                    Name = logPattern.KeyName,
                    Size = new Size(152, 22),
                    Text = logPattern.TextPattern
                };
                rootMenu.DropDownItems.Add(toolStripMenuItem);
            }
        }

        #endregion Constructors

        #region Methods

        //Wypełnia listę informacjami o plikach logów
        private void IniTabPageInfo()
        {
            //Podaje informacje o katalogu "Logs"
            SetDashboardData(_logDirectory, labelLogsPathValue, labelFilesSizeValue, labelFilesCountValue);

            //Podaje informacje statystyczne o katalogu usera
            SetDashboardData(_userLogDirectory, labelUserPathValue, labelFilesSizeValueUser, labelFilesCountValueUser);

            WypelnijListe();
        }

        private void WypelnijListe()
        {
            listViewFiles.Items.Clear();

            var directoryInfo = new DirectoryInfo[]{
            new DirectoryInfo(_logDirectory),
            new DirectoryInfo(_userLogDirectory)
            };

            bool[] showCatalog = new bool[]{
            checkBoxLogs.Checked,
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
                            listViewItem.SubItems.Add(IOTools.FormatFileSize(fileInfo.Length));
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

            if (openFileDialog.ShowDialog() != DialogResult.OK)           
                return;

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
                        plikInfo.SubItems.Add(IOTools.FormatFileSize(e.UncompressedSize));
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

        private void ShowListViewColumns(ListView listView, ILogParser logParser)
        {
            ToolStripItemCollection toolStripItemCollection = null;

            if (logParser is StatlookLogParser)
                toolStripItemCollection = ToolStripMenuItemUplook.DropDownItems;
            else if (logParser is UsmLogParser)
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

            if (TabC.SelectedTab != tabPageInfo)
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

                FileInfo fileInfo = new(fileFullPath);

                //Nie przetwarzaj plików o rozszerzeniu .zip
                if (fileInfo.Extension == Configuration.ZIP_FILE_EXTENSION)
                {
                    OpenZip openZip = new();

                    using (ZipFile zip = ZipFile.Read(fileInfo.FullName))
                    {
                        int i = 1;
                        foreach (ZipEntry e2 in zip)
                        {
                            ListViewItem plikInfo = new()
                            {
                                Text = i.ToString()
                            };
                            plikInfo.SubItems.Add(e2.FileName);
                            plikInfo.SubItems.Add(e2.LastModified.ToString());
                            plikInfo.SubItems.Add(IOTools.FormatFileSize(e2.UncompressedSize));
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
                        if (tabPage.LogParser is StatlookLogParser)
                        {
                            ToolStripMenuItemUplook.Visible = true;
                            ToolStripMenuItemUSM.Visible = false;
                        }
                        else if (tabPage.LogParser is UsmLogParser)
                        {
                            ToolStripMenuItemUplook.Visible = false;
                            ToolStripMenuItemUSM.Visible = true;
                        }

                        ListViewExtended listViewExtended = (ListViewExtended)control;

                        ShowListViewColumns(listViewExtended, tabPage.LogParser);
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
                    toolStripLableSize.Text = IOTools.FormatFileSize(fileInfo.Length);
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

                OpenZip openZip = new();

                using (ZipFile zipFile = ZipFile.Read(fileFullPath))
                {
                    int i = 1;

                    foreach (ZipEntry e1 in zipFile)
                    {
                        ListViewItem plikInfo = new()
                        {
                            Text = i.ToString()
                        };

                        plikInfo.SubItems.Add(e1.FileName);
                        plikInfo.SubItems.Add(e1.LastModified.ToString());                     
                        plikInfo.SubItems.Add(IOTools.FormatFileSize(e1.UncompressedSize));
                        plikInfo.SubItems.Add(fileFullPath);
                        openZip.DodajItem(plikInfo);
                        i++;
                    }
                    //listViewFiles.Items.Add(plikInfo);
                }

                openZip.ShowDialog(this);
            }
            else
            {
                foreach (ListViewItem listViewItem in listViewFiles.SelectedItems)
                {
                    string fileFullPath = GetFileFullPathFromListViewItem(listViewItem);

                    FileInfo fileInfo = new(fileFullPath);

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

                FileInfo fileInfo = new(fileFullPath);

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

                FileInfo fileInfo = new(fileFullPath);
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

                FileInfo fileInfo = new(fileFullPath);

                System.Diagnostics.Process.Start("explorer.exe", fileInfo.DirectoryName);
            }
        }

        private void analizeUplookLog(string filePath, DateTime lastWriteTime)
        {
            string fileName = Path.GetFileName(filePath);

            TabControl tabControl = (TabControl)Controls.Find("tabControlMain", true)[0];

            if (tabControl.Controls.Find(fileName, false).Length == 0)
            {
                LogLineCollection logLineCollection = new();

                LogTapPage newPage = logLineCollection.AnalyzeLogFile(filePath);

                if (newPage.LogParser is StatlookLogParser)
                {
                    ToolStripMenuItemUplook.Visible = true;
                    ToolStripMenuItemUplook.Enabled = true;
                    ToolStripMenuItemUSM.Visible = false;

                    int j = 0;
                    foreach (ToolStripMenuItem toolStripMenuItem in ToolStripMenuItemUplook.DropDownItems)
                    {
                        foreach (LogPattern logPattern in newPage.LogParser.GetLogPatterns())
                        {
                            if (string.Compare(toolStripMenuItem.Name, logPattern.KeyName) == 0)
                            {
                                if (show_uplook[j])
                                {
                                    toolStripMenuItem.CheckState = CheckState.Checked;
                                }
                                else
                                {
                                    toolStripMenuItem.CheckState = CheckState.Unchecked;
                                }
                                break;
                            }
                        }
                        j++;
                    }

                }
                else if (newPage.LogParser is UsmLogParser )
                {
                    ToolStripMenuItemUSM.Enabled = true;
                    ToolStripMenuItemUSM.Visible = true;
                    ToolStripMenuItemUplook.Visible = false;
                    int j = 0;
                    foreach (ToolStripMenuItem toolStripMenuItem in ToolStripMenuItemUSM.DropDownItems)
                    {
                        foreach (LogPattern logPattern in newPage.LogParser.GetLogPatterns())
                        {
                            if (string.Compare(toolStripMenuItem.Name, logPattern.KeyName) == 0)
                            {
                                if (show_usm[j])
                                {
                                    toolStripMenuItem.CheckState = CheckState.Checked; 
                                }
                                else
                                {
                                    toolStripMenuItem.CheckState = CheckState.Unchecked; 
                                }
                                break;
                            }
                        }
                        j++;
                    }
                }

                tabControlMain.Controls.Add(newPage);
                tabControlMain.SelectTab(newPage);
                ShowListViewColumns(newPage.ListViewExtended, newPage.LogParser);

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
                tabControl.SelectTab(fileName);
            }

        }

        private void ToolStripMenuItemUplook_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var logParser = new StatlookLogParser();    

            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)e.ClickedItem;

            if (toolStripMenuItem.CheckState == CheckState.Checked)
            {
                toolStripMenuItem.CheckState = CheckState.Unchecked;

                foreach (LogPattern logPattern in logParser.GetLogPatterns())
                {
                    if (toolStripMenuItem.Name == logPattern.KeyName)
                    {
                        logPattern.Show = false;
                        _config.SetStatlookHeaderVisibility(logPattern.KeyName, false);
                    }
                }
            }
            else
            {
                toolStripMenuItem.CheckState = CheckState.Checked;
                foreach (LogPattern logPattern in logParser.GetLogPatterns())
                {
                    if (toolStripMenuItem.Name == logPattern.KeyName)
                    {
                        logPattern.Show = true;
                        _config.SetStatlookHeaderVisibility(logPattern.KeyName, true);
                    }
                }
            }

            TabControl tabControl = (TabControl)Controls.Find("tabControlMain", true)[0];

            if (tabControl.SelectedTab != tabPageInfo)
            {
                TabPage TabP = (TabPage)Controls.Find(tabControl.SelectedTab.Name, true)[0];

                foreach (Control control in TabP.Controls)
                {
                    if (control.GetType() == typeof(ListViewExtended))
                    {
                        ListViewExtended listViewExtended = (ListViewExtended)control;
                        ShowListViewColumns(listViewExtended, logParser);
                    }
                }
            }
        }

        private void ToolStripMenuItemUSM_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)e.ClickedItem;

            var logParser = new UsmLogParser();

            if (toolStripMenuItem.CheckState == CheckState.Checked)
            {
                toolStripMenuItem.CheckState = CheckState.Unchecked;

                foreach (LogPattern usmd in logParser.GetLogPatterns())
                {
                    if (toolStripMenuItem.Name == usmd.KeyName)
                    {
                        usmd.Show = false;
                        _config.SetUsmHeaderVisibility(usmd.KeyName, false);
                    }
                }
            }
            else
            {
                toolStripMenuItem.CheckState = CheckState.Checked;
                foreach (LogPattern usmd in logParser.GetLogPatterns())
                {
                    if (toolStripMenuItem.Name == usmd.KeyName)
                    {
                        usmd.Show = true;
                        _config.SetUsmHeaderVisibility(usmd.KeyName, true);
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
                        ShowListViewColumns(ListV, logParser);
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

                    labelSize.Text = IOTools.FormatFileSize(rozmiar);

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

            if (TabC.SelectedTab== tabPageInfo)
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
                        if (listViewFiles.Items[lst12].SubItems[colAll].Text.IndexOf(toolStripTextBox.Text, StringComparison.OrdinalIgnoreCase) > -1)
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
                    if (control.GetType() != typeof(ListViewExtended))
                        continue;

                    ListViewExtended ListV = (ListViewExtended)control;

                    if (toolStripTextBox.Text?.Length == 0)
                    {
                        ListV.SuspendLayout();
                        ListV.BeginUpdate();

                        try
                        {
                            foreach (ListViewItem listViewItem in ListV.Items)
                            {
                                ListV.SetOneGroupState(listViewItem.Group, state: ListViewGroupState.Collapsible | ListViewGroupState.Collapsed);
                                listViewItem.BackColor = Color.White;
                            }
                        }
                        finally
                        {
                            ListV.EndUpdate();
                            ListV.ResumeLayout();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ListV.Items.Count; i++)
                        {
                            ListV.Items[i].BackColor = Color.White;
                        }

                        int colCount = ListV.Columns.Count;

                        foreach (ListViewItem listViewItem in ListV.Items)
                        {
                            for (int colAll = 0; colAll < colCount; colAll++)
                            {
                                if (ListV.Columns[colAll].Width == 0)
                                    continue;

                                string listViewSubItemText = listViewItem.SubItems[colAll].Text;

                                if (string.IsNullOrWhiteSpace(listViewSubItemText))
                                    continue;

                                if (listViewSubItemText.IndexOf(toolStripTextBox.Text, StringComparison.OrdinalIgnoreCase) > -1)
                                {
                                    ListV.SuspendLayout();
                                    ListV.BeginUpdate();
                                    try
                                    {
                                        //ListV.SetGroupFooter(ListV.Items[lst12].Group, "Test");
                                        ListV.SetOneGroupState(listViewItem.Group, ListViewGroupState.Collapsible);
                                        listViewItem.BackColor = Color.Aqua;
                                    }
                                    finally
                                    {
                                        ListV.EndUpdate();
                                        ListV.ResumeLayout();
                                    }
                                }
                                else
                                {
                                    //ListV.SetOneGroupState(ListV.Items[lst12].Group, ListViewGroupState.Collapsed);
                                }
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
