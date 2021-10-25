using Ionic.Zip;
using StatlookkLogViewer.Tools;
using StatlookLogViewer.Controller;
using StatlookLogViewer.Model.Pattern;
using StatlookLogViewer.Parser;
using StatlookLogViewer.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;

namespace StatlookLogViewer
{
    public partial class MainForm : Form
    {
        #region Members

        private readonly ListViewColumnSorter _lvwColumnSorter = new();
        public string _logDirectory;
        public string _userLogDirectory;
        private readonly string[] _fileExtensions;
        private readonly Dictionary<string, ILogParser> _logParserMap;
        private readonly Dictionary<string, List<Tuple<string, bool>>> _columnToShowParserMap = new();
        private readonly Dictionary<string, ToolStripMenuItem> _toolStripMenuItemFilterColumnMap = new();

        private Configuration _config;

        #endregion Members

        #region Constructors

        public MainForm()
        {
            InitializeComponent();

            _config = Configuration.GetConfiguration();

            if (string.Compare(_config.CurrentLanguage, "en-us") != 0)
                ChangeLanguage(_config.CurrentLanguage);

            _logParserMap = LogLineCollection.GetLogParserMap();

            foreach (KeyValuePair<string, ILogParser> parser in _logParserMap)
            {
                List<Tuple<string, bool>> columnToShow = new();

                foreach (LogPattern pattern in parser.Value.GetLogPatterns())
                {
                    columnToShow.Add(Tuple.Create(pattern.KeyName, pattern.Show));
                }

                _columnToShowParserMap.Add(parser.Key, columnToShow);


                // Utworzenie menu widocznosci kolumn
                CreateToolStripMenuItem(parser.Value);
            }

            _logDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _config.StatlookLogDirectory;
            _userLogDirectory = _config.UserDirectory;
            _fileExtensions = _config.LogFileExtensions.Split(new char[] { ';' });


            listViewFiles.ListViewItemSorter = _lvwColumnSorter;

            IniTabPageInfo();
        }

        private void CreateToolStripMenuItem(ILogParser? logParser)
        {
            LogPattern[] logPatterns = logParser?.GetLogPatterns().ToArray();

            ToolStripMenuItem rootMenu = new()
            {
                Name = logParser.UniqueLogKey,
                Text = logParser.UniqueLogKey
            };

            rootMenu.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ToolStripMenuItemUplook_DropDownItemClicked);

            _toolStripMenuItemFilterColumnMap.Add(logParser.UniqueLogKey, rootMenu);
            viewToolStripMenuItem.DropDownItems.Add(rootMenu);



            List<ToolStripMenuItem> toolStripMenuItemCollection = new();

            foreach (LogPattern logPattern in logPatterns)
            {
                ToolStripMenuItem toolStripMenuItem = new()
                {
                    Checked = logPattern.Show,
                    Name = logPattern.KeyName,
                    Size = new Size(152, 22),
                    Text = logPattern.TextPattern
                };

                toolStripMenuItemCollection.Add(toolStripMenuItem);
            }

            rootMenu.DropDownItems.AddRange(toolStripMenuItemCollection.ToArray());
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
                        var fileInfoCollection = new List<FileInfo>();

                        foreach (string ext in _fileExtensions)
                        {
                            if (di.FullName != "C:\\")
                            {
                                fileInfoCollection.AddRange(di.GetFiles(ext, SearchOption.AllDirectories));
                            }
                            else
                            {
                                fileInfoCollection.AddRange(di.GetFiles(ext, SearchOption.TopDirectoryOnly));
                            }
                        }

                        int i = 0;
                        foreach (FileInfo fileInfo in fileInfoCollection)
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
                        toolStripStatusReady.Text = Properties.Resources.Ready;
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

        private static string GetFilePathFromListViewItem(ListViewItem listViewItem)
        {
            string fileName = listViewItem.SubItems[1].Text;
            string path = listViewItem.SubItems[4].Text;
            return path + "\\" + fileName;
        }

        private FileInfo GetFileInfoForSelectedTab()
        {
            string fileFullName = _tabControlMain.SelectedTab.ToolTipText;
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
                Title = Properties.Resources.SelectLogSourceTitle
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            //Otwarcie pojedynczego pliku o rozszerzeniu .zip
            if ((openFileDialog.FileNames.Length == 1) && (openFileDialog.SafeFileName.Substring(openFileDialog.SafeFileName.Length - 4, 4) == Configuration.ZIP_FILE_EXTENSION))
            {
                using OpenZip openZip = new(openFileDialog.FileName);

                using (ZipFile zipFile = ZipFile.Read(openFileDialog.FileName))
                {
                    int i = 1;
                    foreach (ZipEntry e in zipFile)
                    {
                        ListViewItem listViewItem = new()
                        {
                            Text = i.ToString()
                        };
                        listViewItem.SubItems.Add(e.FileName);
                        listViewItem.SubItems.Add(e.LastModified.ToString());
                        listViewItem.SubItems.Add(IOTools.FormatFileSize(e.UncompressedSize));
                        listViewItem.SubItems.Add(openFileDialog.InitialDirectory);
                        openZip.AddItem(listViewItem);
                        i++;
                    }
                }

                openZip.ShowDialog(this);

            }
            else
            {
                foreach (string filePath in openFileDialog.FileNames)
                {
                    // Nie przetwarzaj plików o rozszerzeniu .zip
                    FileInfo fileInfo = new(filePath);

                    if (fileInfo.Extension == Configuration.ZIP_FILE_EXTENSION)
                    {
                        using var openZip = new OpenZip();

                        openZip.ShowDialog(this);

                        if (openZip.ShowDialog(this) != DialogResult.OK)
                            continue;

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
                        AnalizeLog(filePath, fileInfo.LastWriteTime);
                    }
                }
            }
        }

        private void ShowListViewColumns(LogTapPage logTapPage)
        {
            ILogParser logParser = logTapPage.LogParser;

            if (_toolStripMenuItemFilterColumnMap.TryGetValue(logParser.UniqueLogKey, out ToolStripMenuItem rootMenu))
                logTapPage.SetListViewColumnsColumnsVisibility(rootMenu.DropDownItems);
        }

        private void CollapseAllGroups() => ChangeGroupState(ListViewGroupState.Collapsed);

        private void ExpandAllGroups() => ChangeGroupState(ListViewGroupState.Normal);

        private void ChangeGroupState(ListViewGroupState listViewGroupState)
        {
            TabPage selectedTabPage = GetSelectedTabPage();

            if (selectedTabPage == tabPageInfo)
                return;

            TabPage TabP = (TabPage)Controls.Find(selectedTabPage.Name, true)[0];

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

        private void ClosePage()
        {
            int selectedTapPageIndex = _tabControlMain.SelectedIndex;

            _tabControlMain.TabPages.Remove(_tabControlMain.SelectedTab);

            if (_tabControlMain.TabPages.Count > 1)
            {
                if (selectedTapPageIndex == _tabControlMain.TabPages.Count)
                {
                    _tabControlMain.SelectedIndex = selectedTapPageIndex - 1;
                }
                else
                {
                    _tabControlMain.SelectedIndex = selectedTapPageIndex;
                }
            }
        }

        /// <summary>
        /// Change language
        /// </summary>
        /// <param name="lang">Language code</param>
        private void ChangeLanguage(string lang)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);
            ApplyResourceToControl(
            this,
            new ComponentResourceManager(typeof(MainForm)),
            new CultureInfo(lang));

            _config.CurrentLanguage = lang;
        }

        private void ApplyResourceToControl(
           Control control,
           ComponentResourceManager cmp,
           CultureInfo cultureInfo)
            {
                foreach (Control child in control.Controls)
                {
                    //Store current position and size of the control
                    var childSize = child.Size;
                    var childLoc = child.Location;
                    //Apply CultureInfo to child control
                    ApplyResourceToControl(child, cmp, cultureInfo);
                    //Restore position and size
                    child.Location = childLoc;
                    child.Size = childSize;
                }
                //Do the same with the parent control
                var parentSize = control.Size;
                var parentLoc = control.Location;
                cmp.ApplyResources(control, control.Name, cultureInfo);
                control.Location = parentLoc;
                control.Size = parentSize;
        }

        #endregion Methods

        #region Event Handlers

        private void OpenFileToolStripMenuItem_Click(object sender, EventArgs e) 
            => OpenLogFile();

        private void ToolStripButton2_Click(object sender, EventArgs e) 
            => OpenLogFile();

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(Properties.Resources.CloseApplicationQuestion, Properties.Resources.Close, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
            else
                Configuration.SaveConfig(_config);
        }

        private void ExitToolStripMenuItem1_Click(object sender, EventArgs e) 
            => Close();

        private void ToolStripButton3_Click(object sender, EventArgs e) 
            => Close();

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) 
            => Close();

        private void ListViewFiles_ColumnClick(object sender, ColumnClickEventArgs e)
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
                listViewFiles.EndUpdate();
            }
        }

        private void ListViewFiles_DoubleClick(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in this.listViewFiles.SelectedItems)
            {
                string filePath = GetFilePathFromListViewItem(listViewItem);

                FileInfo fileInfo = new(filePath);

                //Nie przetwarzaj plików o rozszerzeniu .zip
                if (fileInfo.Extension == Configuration.ZIP_FILE_EXTENSION)
                {
                    using var openZip = new OpenZip();

                    using (ZipFile zipFile = ZipFile.Read(fileInfo.FullName))
                    {
                        int i = 1;

                        foreach (ZipEntry zipEntry in zipFile)
                        {
                            ListViewItem innerlistViewItem = new()
                            {
                                Text = i.ToString()
                            };

                            innerlistViewItem.SubItems.Add(zipEntry.FileName);
                            innerlistViewItem.SubItems.Add(zipEntry.LastModified.ToString());
                            innerlistViewItem.SubItems.Add(IOTools.FormatFileSize(zipEntry.UncompressedSize));
                            innerlistViewItem.SubItems.Add(fileInfo.DirectoryName);
                            openZip.AddItem(innerlistViewItem);
                            i++;
                        }
                    }

                    openZip.ShowDialog(this);
                }
                else
                {
                    _ = DateTime.TryParse(listViewItem.SubItems[2].Text, out DateTime lastWriteTime);

                    AnalizeLog(filePath, lastWriteTime);
                }
            }
        }

        private void TabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage selectedTabPage = GetSelectedTabPage();

            if (selectedTabPage != tabPageInfo)
            {
                //Wyzerowanie paska wyszukiwania
                toolStripTextBox.Text = string.Empty;

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

                LogTapPage tabPage = Controls.Find(selectedTabPage.Name, true)[0] as LogTapPage;

                foreach (Control control in tabPage.Controls)
                {
                    if (control.GetType() == typeof(ListViewExtended))
                    {

                        foreach (var item in _toolStripMenuItemFilterColumnMap)
                        {
                            if (item.Key == tabPage.LogParser.UniqueLogKey)
                            {
                                item.Value.Visible = item.Value.Enabled = true;
                            }
                            else
                            {
                                item.Value.Visible = item.Value.Enabled = false;
                            }
                        }

                        ShowListViewColumns(tabPage);
                    }
                }

                FileInfo fileInfo = new(selectedTabPage.Tag.ToString());

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
                    MessageBox.Show($"File {fileInfo.FullName} does not exist.", Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                toolStripStatusReady.Text = Properties.Resources.Ready;
                toolStripSeparator_1.Visible = false;
                toolStripLabelCreationTime.Text = string.Empty;
                toolStripSeparator_2.Visible = false;
                toolStripLableSize.Text = string.Empty;
                //Wyłączenie przycisku Close page
                closeToolStripMenuItem1.Enabled = false;
                //Deaktywacja przycisków menu File
                closeAllToolStripMenuItem.Enabled = false;
                closeAllWithoutActiveToolStripMenuItem.Enabled = false;

                foreach (var item in _toolStripMenuItemFilterColumnMap)
                    item.Value.Visible = false;

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

        private TabPage GetSelectedTabPage() => _tabControlMain.SelectedTab;


        private void CollapseAllGroupsToolStripMenuItem_Click(object sender, EventArgs e) 
            => CollapseAllGroups();

        private void ExpandAllGroupsToolStripMenuItem_Click(object sender, EventArgs e) 
            => ExpandAllGroups();

        private void ToolStripButtonCollapsedAll_Click(object sender, EventArgs e) => CollapseAllGroups();

        private void ToolStripButtonNormalAll_Click(object sender, EventArgs e) => ExpandAllGroups();

        private void CloseToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            if (_tabControlMain.SelectedTab == tabPageInfo)
                return;

            ClosePage();
        }

        private void ToolStripButtonClose_Click(object sender, EventArgs e) => ClosePage();


        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _tabControlMain.TabCount; i++)
            {
                if (i != 0)
                {
                    _tabControlMain.TabPages.Remove(_tabControlMain.TabPages[1]);
                }
            }
        }

        private void CloseAllWithoutActiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int selectedIndex = _tabControlMain.SelectedIndex;

            for (int i = _tabControlMain.TabCount - 1; i > 0; i--)
            {
                if ((i != 0) && (i != selectedIndex))
                {
                    _tabControlMain.TabPages.Remove(_tabControlMain.TabPages[i]);
                }
            }
        }

        private void TabControlMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            int tabIndex;

            for (tabIndex = 0; tabIndex <= _tabControlMain.TabCount - 1; tabIndex++)
            {
                if (_tabControlMain.GetTabRect(tabIndex).Contains(e.Location))
                {
                    _tabControlMain.SelectedIndex = tabIndex;

                    if (_tabControlMain.SelectedIndex != 0)
                    {
                        contextMenuStripPage.Show(_tabControlMain, e.Location);
                    }
                }
            }

        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (_tabControlMain.SelectedTab == tabPageInfo)
                return;

            int selectedIndex = _tabControlMain.SelectedIndex;
            _tabControlMain.TabPages.Remove(_tabControlMain.SelectedTab);
            if (_tabControlMain.TabPages.Count > 1)
            {
                if (selectedIndex == _tabControlMain.TabPages.Count)
                {
                    _tabControlMain.SelectedIndex = selectedIndex - 1;
                }
                else
                {
                    _tabControlMain.SelectedIndex = selectedIndex;
                }
            }

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int tabCount = _tabControlMain.TabCount - 1;
            int selectedIndex = _tabControlMain.SelectedIndex;
            for (int i = tabCount; i > 0; i--)
            {
                if ((i != 0) && (i != selectedIndex))
                {
                    _tabControlMain.TabPages.Remove(_tabControlMain.TabPages[i]);
                }
            }
        }

        private void ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            FileInfo fileInfo = GetFileInfoForSelectedTab();
            Clipboard.SetText(fileInfo.Name);
        }

        private void ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            FileInfo fileInfo = GetFileInfoForSelectedTab();
            Clipboard.SetText(fileInfo.FullName);
        }

        private void ToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            FileInfo fileInfo = GetFileInfoForSelectedTab();
            Clipboard.SetText(fileInfo.DirectoryName);
        }

        private void ToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            FileInfo fileInfo = GetFileInfoForSelectedTab();

            if (MessageBox.Show($"Really delete file: {fileInfo.FullName} ?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (!fileInfo.Exists)
                    return;

                try
                {
                    RemoveSelectedTab();
                    fileInfo.Delete();
                    listViewFiles.Items.Clear();
                    IniTabPageInfo();
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"{Properties.Resources.Error} : {exception.Message}", Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void RemoveSelectedTab()
        {
            _tabControlMain.TabPages.Remove(_tabControlMain.SelectedTab);
        }

        private void ToolStripMenuItemOpenFile_Click(object sender, EventArgs e)
        {
            //Otwarcie pojedynczego pliku o rozszerzeniu .zip z poziomu listy plików w zakładce Info
            if ((listViewFiles.SelectedItems.Count == 1) && (listViewFiles.SelectedItems[0].SubItems[1].Text.Substring(listViewFiles.SelectedItems[0].SubItems[1].Text.Length - 4, 4) == ".zip"))
            {
                string fileFullPath = GetFilePathFromListViewItem(listViewFiles.SelectedItems[0]);

                using OpenZip openZip = new();

                using (ZipFile zipFile = ZipFile.Read(fileFullPath))
                {
                    int i = 1;

                    foreach (ZipEntry e1 in zipFile)
                    {
                        ListViewItem listViewItem = new()
                        {
                            Text = i.ToString()
                        };

                        listViewItem.SubItems.Add(e1.FileName);
                        listViewItem.SubItems.Add(e1.LastModified.ToString());
                        listViewItem.SubItems.Add(IOTools.FormatFileSize(e1.UncompressedSize));
                        listViewItem.SubItems.Add(fileFullPath);
                        openZip.AddItem(listViewItem);
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
                    string fileFullPath = GetFilePathFromListViewItem(listViewItem);

                    FileInfo fileInfo = new(fileFullPath);

                    //Nie przetwarzaj plików o rozszerzeniu .zip
                    if (fileInfo.Extension == Configuration.ZIP_FILE_EXTENSION)
                    {
                        using var openZip = new OpenZip();
                        openZip.ShowDialog(this);
                    }
                    else
                    {
                        AnalizeLog(fileInfo.FullName, fileInfo.LastWriteTime);
                    }
                }
            }
        }

        private void ToolStripMenuItemDeleteFile_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listViewFiles.SelectedItems)
            {
                string fileFullPath = GetFilePathFromListViewItem(listViewItem);

                FileInfo fileInfo = new(fileFullPath);

                if (MessageBox.Show("Really delete file: " + fileFullPath + " ?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    continue;

                if (!fileInfo.Exists)
                    continue;

                try
                {
                    fileInfo.Delete();
                    listViewFiles.Items.Clear();
                    IniTabPageInfo();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{Properties.Resources.Error} : {ex.Message}", Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ToolStripMenuItem3_Click_1(object sender, EventArgs e)
        {
            string filePath = _tabControlMain.SelectedTab.ToolTipText;
            FileInfo fileInfo = new(filePath);
            Clipboard.SetText(fileInfo.Name);
            System.Diagnostics.Process.Start("explorer.exe", fileInfo.DirectoryName);
        }

        private void ToolStripMenuItemCopyFileName_Click(object sender, EventArgs e)
        {
            FileInfo[] fileCollection = GetListViewSelectedFiles();

            string filesNameText = default;

            foreach (FileInfo fileInfo in fileCollection)
                filesNameText += fileInfo.Name + Environment.NewLine;

            Clipboard.SetText(filesNameText, TextDataFormat.UnicodeText);
        }

        private void ToolStripMenuItemCopyFilePath_Click(object sender, EventArgs e)
        {
            FileInfo[] fileCollection = GetListViewSelectedFiles();

            string filesFullNameText = default;

            foreach (FileInfo fileInfo in fileCollection)
                filesFullNameText += fileInfo.FullName + Environment.NewLine;

            Clipboard.SetText(filesFullNameText, TextDataFormat.UnicodeText);
        }

        private void ToolStripMenuItemCopyCatalogPath_Click(object sender, EventArgs e)
        {
            FileInfo[] fileCollection = GetListViewSelectedFiles();

            string fileDirectoryNamesText = default;

            foreach (FileInfo fileInfo in fileCollection)
                fileDirectoryNamesText += fileInfo.DirectoryName + Environment.NewLine;

            Clipboard.SetText(fileDirectoryNamesText, TextDataFormat.UnicodeText);
        }

        FileInfo[] GetListViewSelectedFiles()
        {
            List<FileInfo> files = new();

            foreach (ListViewItem listViewItem in listViewFiles.SelectedItems)
            {
                string filePath = GetFilePathFromListViewItem(listViewItem);

                files.Add(new FileInfo(filePath));
            }

            return files.ToArray();
        }

        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e) 
            => IniTabPageInfo();


        private void ListViewFiles_MouseClick(object sender, MouseEventArgs e)
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

        private void OpenContainFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listViewFiles.SelectedItems)
            {
                string filePath = GetFilePathFromListViewItem(listViewItem);

                FileInfo fileInfo = new(filePath);

                System.Diagnostics.Process.Start("explorer.exe", fileInfo.DirectoryName);
            }
        }

        private void AnalizeLog(string filePath, DateTime lastWriteTime)
        {
            string fileName = Path.GetFileName(filePath);

            if (_tabControlMain.Controls.Find(fileName, false).Length == 0)
            {
                LogLineCollection logLineCollection = new();

                LogTapPage newPage = logLineCollection.AnalyzeLogFile(filePath);


                ToolStripMenuItem rootMenu = null;

                foreach (var item in _toolStripMenuItemFilterColumnMap)
                {
                    if (newPage.LogParser.UniqueLogKey == item.Key)
                    {
                        item.Value.Visible = item.Value.Enabled = true;
                        rootMenu = item.Value;
                    }
                    else
                    {
                        item.Value.Visible = item.Value.Enabled = false;
                    }
                }

                _columnToShowParserMap.TryGetValue(newPage.LogParser.UniqueLogKey, out List<Tuple<string, bool>> columnToShowCollection);

                foreach (ToolStripMenuItem toolStripMenuItem in rootMenu.DropDownItems)
                {
                    foreach (Tuple<string, bool> columnToShow in columnToShowCollection)
                    {
                        if (string.Compare(toolStripMenuItem.Name, columnToShow.Item1) == 0)
                        {
                            if (columnToShow.Item2)
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
                }



                _tabControlMain.Controls.Add(newPage);
                _tabControlMain.SelectTab(newPage);
                ShowListViewColumns(newPage);

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
                _tabControlMain.SelectTab(fileName);
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
                    }
                }
            }

            TabPage selectedTabPage = GetSelectedTabPage();

            if (selectedTabPage != tabPageInfo)
            {
                LogTapPage tabPage = Controls.Find(selectedTabPage.Name, true)[0] as LogTapPage;

                foreach (Control control in tabPage.Controls)
                {
                    if (control.GetType() == typeof(ListViewExtended))
                    {
                        ShowListViewColumns(tabPage);
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
                    }
                }
            }

            TabPage selectedTabPage = GetSelectedTabPage();

            if (selectedTabPage != tabPageInfo)
            {
                LogTapPage tabPage = Controls.Find(selectedTabPage.Name, true)[0] as LogTapPage;

                foreach (Control control in tabPage.Controls)
                {
                    if (control.GetType() == typeof(ListViewExtended))
                    {
                        ShowListViewColumns(tabPage);
                    }
                }
            }

        }

        private void SetDashboardData(string directoryLogPath, Label label, Label labelSize, Label labelCount)
        {
            try
            {
                DirectoryInfo s = new(directoryLogPath);

                if (!s.Exists)
                    return;

                DirectoryInfo[] directoryCollection = s.GetDirectories();

                label.Text = s.FullName;

                ArrayList myfileinfos = new();

                foreach (string ext in _fileExtensions)
                    myfileinfos.AddRange(s.GetFiles(ext, SearchOption.AllDirectories));

                FileInfo[] fileCollection = (FileInfo[])myfileinfos.ToArray(typeof(FileInfo));

                float totalSize = GetFilesTotalSize(fileCollection);

                labelSize.Text = IOTools.FormatFileSize(totalSize);

                labelCount.Text = (fileCollection.Length).ToString() + " files, " + directoryCollection.Length.ToString() + " directories";
            }
            catch
            {

            }
        }

        private static float GetFilesTotalSize(FileInfo[] filesCollection)
        {
            if (filesCollection?.Length == 0)
                return 0;

            float totalSize = 0;

            foreach (FileInfo file in filesCollection)
                totalSize += file.Length;

            return totalSize;
        }

        private void CheckBoxLogs_CheckedChanged(object sender, EventArgs e) 
            => WypelnijListe();


        private void CheckBoxUser_CheckedChanged(object sender, EventArgs e) 
            => WypelnijListe();

        private static ListViewItem[] CloneItems(ListViewItemCollection items)
        {
            List<ListViewItem> nodes = new();

            foreach (ListViewItem listViewItem in items)
                nodes.Add(listViewItem.Clone() as ListViewItem);

            return nodes.ToArray();
        }

        private void ToolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            timerFind.Enabled = false;
            timerFind.Enabled = true;
        }

        private void DelayedSearch() => timerFind.Start();


        private void TimerFind_Tick(object sender, EventArgs e)
        {
            //changed = false;
            timerFind.Enabled = false;
            Search();
        }

        #endregion Event Handlers

        #region Private Methods

        private void Search()
        {
            TabPage selectedTabPage = GetSelectedTabPage();

            if (selectedTabPage == tabPageInfo)
                SearchInDashboardView();
            else
                SearchInLogFileView(selectedTabPage);

        }

        private void SearchInLogFileView(TabPage selectedTabPage)
        {
            TabPage TabP = (TabPage)Controls.Find(selectedTabPage.Name, true)[0];

            foreach (Control control in TabP.Controls)
            {
                if (control.GetType() != typeof(ListViewExtended))
                    continue;

                ListViewExtended listViewExtended = (ListViewExtended)control;

                if (toolStripTextBox.Text?.Length == 0)
                {
                    listViewExtended.SuspendLayout();
                    listViewExtended.BeginUpdate();

                    try
                    {
                        foreach (ListViewItem listViewItem in listViewExtended.Items)
                        {
                            listViewExtended.SetOneGroupState(listViewItem.Group, state: ListViewGroupState.Collapsible | ListViewGroupState.Collapsed);
                            listViewItem.BackColor = Color.White;
                        }
                    }
                    finally
                    {
                        listViewExtended.EndUpdate();
                        listViewExtended.ResumeLayout();
                    }
                }
                else
                {
                    foreach (ListViewItem listViewItem in listViewExtended.Items)
                    {
                        listViewItem.BackColor = Color.White;
                    }

                    int colCount = listViewExtended.Columns.Count;

                    foreach (ListViewItem listViewItem in listViewExtended.Items)
                    {
                        for (int colAll = 0; colAll < colCount; colAll++)
                        {
                            if (listViewExtended.Columns[colAll].Width == 0)
                                continue;

                            string listViewSubItemText = listViewItem.SubItems[colAll].Text;

                            if (string.IsNullOrWhiteSpace(listViewSubItemText))
                                continue;

                            if (listViewSubItemText.IndexOf(toolStripTextBox.Text, StringComparison.OrdinalIgnoreCase) > -1)
                            {
                                listViewExtended.SuspendLayout();
                                listViewExtended.BeginUpdate();
                                try
                                {
                                    //ListV.SetGroupFooter(ListV.Items[lst12].Group, "Test");
                                    listViewExtended.SetOneGroupState(listViewItem.Group, ListViewGroupState.Collapsible);
                                    listViewItem.BackColor = Color.Aqua;
                                }
                                finally
                                {
                                    listViewExtended.EndUpdate();
                                    listViewExtended.ResumeLayout();
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

        private void SearchInDashboardView()
        {
            WypelnijListe();

            ListView listView = new();

            listView.Items.Clear();

            int colCount = listViewFiles.Columns.Count;

            for (int lst12 = 1; lst12 < listViewFiles.Items.Count; lst12++)
            {
                for (int colAll = 0; colAll < colCount; colAll++)
                {
                    if (listViewFiles.Items[lst12].SubItems[colAll].Text.IndexOf(toolStripTextBox.Text, StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        listView.Items.Add((ListViewItem)listViewFiles.Items[lst12].Clone());
                        break;
                    }

                }

            }

            listViewFiles.Items.Clear();
            listViewFiles.Items.AddRange(CloneItems(listView.Items));
        }

        private void AnalizeToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                using var options = new Options();

                if (options.ShowDialog(this) != DialogResult.OK)
                    return;

                _config = Configuration.GetConfiguration();

                IniTabPageInfo();
            }
            catch (Exception exception)
            {
            }
        }

        private void EnglishToolStripMenuItem_Click(object sender, EventArgs e) 
            => ChangeLanguage("en-us");

        private void PolishToolStripMenuItem_Click(object sender, EventArgs e) 
            => ChangeLanguage("pl-pl");

        #endregion Private Methods

    }

}
