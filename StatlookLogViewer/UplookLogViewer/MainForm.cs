using Ionic.Zip;
using StatlookkLogViewer.Tools;
using StatlookLogViewer.Controller;
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
        private readonly string _logDirectoryPath;
        private readonly string _userLogDirectoryPath;
        private readonly string[] _fileExtensions;
        private readonly Dictionary<string, List<Tuple<string, bool>>> _columnToShowParserMap = new();
        private readonly Dictionary<string, ToolStripMenuItem> _toolStripMenuItemFilterColumnMap = new();

        private Configuration _config;

        #endregion Members

        #region Constructors

        public MainForm()
        {
            InitializeComponent();

            _config = Configuration.GetConfiguration();

            if (string.CompareOrdinal(_config.CurrentLanguage, "en-us") != 0)
                ChangeLanguage(_config.CurrentLanguage);

            var logParserMap = LogParserTools.GetLogParserMap() ?? throw new ArgumentNullException("LogParserTools.GetLogParserMap()");

            foreach (var (key, logParser) in logParserMap)
            {
                var columnToShow = logParser.GetLogPatterns().Select(pattern => Tuple.Create(pattern.KeyName, pattern.Show)).ToList();

                _columnToShowParserMap.Add(key, columnToShow);

                CreateToolStripMenuItem(logParser);
            }

            _logDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _config.StatlookLogDirectory;
            _userLogDirectoryPath = _config.UserDirectory;
            _fileExtensions = _config.LogFileExtensions.Split(new[] { ';' });


            listViewFiles.ListViewItemSorter = _lvwColumnSorter;
        }

        private void CreateToolStripMenuItem(ILogParser? logParser)
        {
            var logPatterns = logParser?.GetLogPatterns().ToArray();

            if (logParser == null) return;

            ToolStripMenuItem rootMenu = new()
            {
                Name = logParser.UniqueLogKey,
                Text = logParser.UniqueLogKey
            };

            rootMenu.DropDownItemClicked += this.ToolStripMenuItemUplook_DropDownItemClicked;

            _toolStripMenuItemFilterColumnMap.Add(logParser.UniqueLogKey, rootMenu);
            viewToolStripMenuItem.DropDownItems.Add(rootMenu);

            rootMenu.DropDownItems.AddRange(logPatterns.Select(logPattern => new ToolStripMenuItem() { Checked = logPattern.Show, Name = logPattern.KeyName, Size = new Size(152, 22), Text = logPattern.TextPattern }).ToArray());
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Set init tab page info
        /// </summary>
        private void IniTabPageInfo()
        {
            SetDashboardData(_logDirectoryPath, labelLogsPathValue, labelFilesSizeValue, labelFilesCountValue);

            SetDashboardData(_userLogDirectoryPath, labelUserPathValue, labelFilesSizeValueUser, labelFilesCountValueUser);

            SetFilesInfoGridViewData();
        }

        private void SetFilesInfoGridViewData()
        {
            listViewFiles.Items.Clear();

            List<(DirectoryInfo directoryInfo, bool include)> directoryInfoCollection = new()
            {
                (new DirectoryInfo(_logDirectoryPath), checkBoxLogs.Checked),
                (new DirectoryInfo(_userLogDirectoryPath), checkBoxUser.Checked)
            };

            try
            {

                foreach (var (directoryInfo, include) in directoryInfoCollection)
                {
                    if (!directoryInfo.Exists || !include) continue;

                    var fileInfoCollection = GetFileInfoCollectionForSelectedDirectory(directoryInfo);

                    SetFileInfoCollection(fileInfoCollection);
                }

                // Loop through and size each column header to fit the column header text.
                foreach (ColumnHeader columnHeader in listViewFiles.Columns)
                    columnHeader.Width = (columnHeader.Index == 0) ? 0 : -2;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }

            toolStripButtonIcon.Image = Properties.Resources.ok_16;
            toolStripStatusReady.Text = Properties.Resources.Ready;
        }

        private void SetFileInfoCollection(List<FileInfo> fileInfoCollection)
        {
            listViewFiles.BeginUpdate();

            ListViewItemCollection listViewItemCollection = new(listViewFiles);

            var i = 1;

            foreach (var fileInfo in fileInfoCollection)
            {
                var listViewItem = new ListViewItem
                {
                    Text = i.ToString()
                };

                listViewItem.SubItems.Add(fileInfo.Name);
                listViewItem.SubItems.Add(fileInfo.LastWriteTime.ToString(CultureInfo.CurrentCulture));
                listViewItem.SubItems.Add(IOTools.FormatFileSize(fileInfo.Length));
                listViewItem.SubItems.Add(fileInfo.DirectoryName);
                listViewItemCollection.Add(listViewItem);

                i++;
            }


            listViewFiles.EndUpdate();
        }

        private List<FileInfo> GetFileInfoCollectionForSelectedDirectory(DirectoryInfo directoryInfo)
        {
            var fileInfoCollection = new List<FileInfo>();

            foreach (var ext in _fileExtensions)
            {
                fileInfoCollection.AddRange(directoryInfo.FullName != "C:\\"
                    ? directoryInfo.GetFiles(ext, SearchOption.AllDirectories)
                    : directoryInfo.GetFiles(ext, SearchOption.TopDirectoryOnly));
            }

            return fileInfoCollection;
        }

        private static string GetFilePathFromListViewItem(ListViewItem listViewItem)
        {
            var fileName = listViewItem.SubItems[1].Text;
            var path = listViewItem.SubItems[4].Text;
            return path + "\\" + fileName;
        }

        private FileInfo GetFileInfoForSelectedTab()
        {
            var fileFullName = GetSelectedTabPage().ToolTipText;
            return new FileInfo(fileFullName);
        }

        /// <summary>
        /// Open log file
        /// </summary>
        private void OpenLogFile()
        {
            using var fileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = Configuration.LOG_FILE_EXTENSIONS,
                Title = Properties.Resources.SelectLogSourceTitle
            };

            if (fileDialog.ShowDialog() != DialogResult.OK)
                return;

            // Otwarcie pojedynczego pliku o rozszerzeniu .zip
            if ((fileDialog.FileNames.Length == 1) && (fileDialog.SafeFileName?.Substring(fileDialog.SafeFileName.Length - 4, 4) == Configuration.ZIP_FILE_EXTENSION))
            {
                using OpenZip openZip = new(fileDialog.FileName);

                using (var zipFile = ZipFile.Read(fileDialog.FileName))
                {
                    var i = 1;
                    foreach (var e in zipFile)
                    {
                        ListViewItem listViewItem = new()
                        {
                            Text = i.ToString()
                        };
                        listViewItem.SubItems.Add(e.FileName);
                        listViewItem.SubItems.Add(e.LastModified.ToString(CultureInfo.CurrentCulture));
                        listViewItem.SubItems.Add(IOTools.FormatFileSize(e.UncompressedSize));
                        listViewItem.SubItems.Add(fileDialog.InitialDirectory);
                        openZip.AddItem(listViewItem);
                        i++;
                    }
                }

                openZip.ShowDialog(this);

            }
            else
            {
                foreach (var filePath in fileDialog.FileNames)
                {
                    // Nie przetwarzaj plików o rozszerzeniu .zip
                    FileInfo fileInfo = new(filePath);

                    if (fileInfo.Extension == Configuration.ZIP_FILE_EXTENSION)
                    {
                        using var openZip = new OpenZip();

                        openZip.ShowDialog(this);

                        if (openZip.ShowDialog(this) != DialogResult.OK)
                            continue;

                        grupyToolStripMenuItem.Enabled = true;
                        zwinToolStripMenuItem.Enabled = true;
                        rozwinWszystkieToolStripMenuItem.Enabled = true;

                        toolStripButtonCollapsedAll.Enabled = true;
                        toolStripButtonNormalAll.Enabled = true;

                        closeAllWithoutActiveToolStripMenuItem.Enabled = true;
                        closeAllToolStripMenuItem.Enabled = true;
                        CollapseAllGroups();

                    }
                    else
                    {
                        AnalizeLog(filePath);
                    }
                }
            }
        }

        private void ShowListViewColumns(LogTapPage logTapPage)
        {
            var logParser = logTapPage.LogParser;

            if (_toolStripMenuItemFilterColumnMap.TryGetValue(logParser.UniqueLogKey, out var rootMenu))
                logTapPage.SetListViewColumnsColumnsVisibility(rootMenu.DropDownItems);
        }

        private void CollapseAllGroups()
            => ChangeGroupState(Views.ListViewGroupState.Collapsed);

        private void ExpandAllGroups()
            => ChangeGroupState(Views.ListViewGroupState.Normal);

        private void ChangeGroupState(Views.ListViewGroupState listViewGroupState)
        {
            var selectedTabPage = GetSelectedTabPage();

            if (selectedTabPage == tabPageInfo)
                return;

            var tabP = (TabPage)Controls.Find(selectedTabPage.Name, true)[0];

            foreach (Control control in tabP.Controls)
            {
                if (control.GetType() != typeof(ListViewExtended)) continue;

                var listViewExtended = (ListViewExtended)control;

                listViewExtended.SuspendLayout();
                listViewExtended.BeginUpdate();

                try
                {
                    listViewExtended.SetGroupState(Views.ListViewGroupState.Collapsible | listViewGroupState);
                }
                finally
                {
                    listViewExtended.EndUpdate();
                    listViewExtended.ResumeLayout();
                }
            }
        }

        private void ClosePage()
        {
            if (GetSelectedTabPage() == tabPageInfo)
                return;

            var selectedTapPageIndex = _tabControlMain.SelectedIndex;

            _tabControlMain.TabPages.Remove(GetSelectedTabPage());

            if (_tabControlMain.TabPages.Count <= 1) return;

            if (selectedTapPageIndex == _tabControlMain.TabPages.Count)
            {
                _tabControlMain.SelectedIndex = selectedTapPageIndex - 1;
            }
            else
            {
                _tabControlMain.SelectedIndex = selectedTapPageIndex;
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
            => ShowClosingApplicationQuestion(e);

        private void ShowClosingApplicationQuestion(FormClosingEventArgs e)
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
                    _lvwColumnSorter.Order = _lvwColumnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
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
                var filePath = GetFilePathFromListViewItem(listViewItem);

                FileInfo fileInfo = new(filePath);

                // Nie przetwarzaj plików o rozszerzeniu .zip
                if (fileInfo.Extension == Configuration.ZIP_FILE_EXTENSION)
                {
                    using var openZip = new OpenZip();

                    using (var zipFile = ZipFile.Read(fileInfo.FullName))
                    {
                        var i = 1;

                        foreach (var zipEntry in zipFile)
                        {
                            ListViewItem innerlistViewItem = new()
                            {
                                Text = i.ToString()
                            };

                            innerlistViewItem.SubItems.Add(zipEntry.FileName);
                            innerlistViewItem.SubItems.Add(zipEntry.LastModified.ToString(CultureInfo.CurrentCulture));
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
                    AnalizeLog(filePath);
                }
            }
        }

        private void TabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedTabPage = GetSelectedTabPage();

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


                if (Controls.Find(selectedTabPage.Name, true)[0] is LogTapPage tabPage)
                {
                    foreach (Control control in tabPage.Controls)
                    {
                        if (control.GetType() != typeof(ListViewExtended)) continue;

                        foreach (var (key, toolStripMenuItem) in _toolStripMenuItemFilterColumnMap)
                        {
                            if (key == tabPage.LogParser.UniqueLogKey)
                            {
                                toolStripMenuItem.Visible = toolStripMenuItem.Enabled = true;
                            }
                            else
                            {
                                toolStripMenuItem.Visible = toolStripMenuItem.Enabled = false;
                            }
                        }

                        ShowListViewColumns(tabPage);
                    }
                }

                FileInfo fileInfo = new(selectedTabPage.Tag.ToString() ?? string.Empty);

                if (fileInfo.Exists)
                {
                    toolStripButtonIcon.Image = Properties.Resources.ok_16;
                    toolStripStatusReady.Text = fileInfo.FullName;
                    toolStripSeparator_1.Visible = true;
                    toolStripLabelCreationTime.Text = fileInfo.LastWriteTime.ToString(CultureInfo.CurrentCulture);
                    toolStripSeparator_2.Visible = true;
                    toolStripLableSize.Text = IOTools.FormatFileSize(fileInfo.Length);
                }
                else
                {
                    MessageBox.Show(String.Format(Properties.Resources.FileDoesNotExist, fileInfo.FullName), Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        /// <summary>
        /// Main form loaded
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
          => IniTabPageInfo();


        private void CollapseAllGroupsToolStripMenuItem_Click(object sender, EventArgs e)
            => CollapseAllGroups();

        private void ExpandAllGroupsToolStripMenuItem_Click(object sender, EventArgs e)
            => ExpandAllGroups();

        private void ToolStripButtonCollapsedAll_Click(object sender, EventArgs e)
            => CollapseAllGroups();

        private void ToolStripButtonNormalAll_Click(object sender, EventArgs e)
            => ExpandAllGroups();

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
            => ClosePage();


        private void ToolStripButtonClose_Click(object sender, EventArgs e)
            => ClosePage();


        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < _tabControlMain.TabCount; i++)
            {
                if (i != 0)
                {
                    _tabControlMain.TabPages.Remove(_tabControlMain.TabPages[1]);
                }
            }
        }

        private void CloseAllWithoutActiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedIndex = _tabControlMain.SelectedIndex;

            for (var i = _tabControlMain.TabCount - 1; i > 0; i--)
            {
                if (i != selectedIndex)
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
                if (!_tabControlMain.GetTabRect(tabIndex).Contains(e.Location)) continue;

                _tabControlMain.SelectedIndex = tabIndex;

                if (_tabControlMain.SelectedIndex != 0)
                {
                    contextMenuStripPage.Show(_tabControlMain, e.Location);
                }
            }

        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (GetSelectedTabPage() == tabPageInfo)
                return;

            var selectedIndex = _tabControlMain.SelectedIndex;
            _tabControlMain.TabPages.Remove(GetSelectedTabPage());

            if (_tabControlMain.TabPages.Count <= 1) return;

            if (selectedIndex == _tabControlMain.TabPages.Count)
            {
                _tabControlMain.SelectedIndex = selectedIndex - 1;
            }
            else
            {
                _tabControlMain.SelectedIndex = selectedIndex;
            }

        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var tabCount = _tabControlMain.TabCount - 1;

            var selectedIndex = _tabControlMain.SelectedIndex;

            for (var i = tabCount; i > 0; i--)
            {
                if (i != selectedIndex)
                {
                    _tabControlMain.TabPages.Remove(_tabControlMain.TabPages[i]);
                }
            }
        }

        private void ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            var fileInfo = GetFileInfoForSelectedTab();
            Clipboard.SetText(fileInfo.Name);
        }

        private void ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            var fileInfo = GetFileInfoForSelectedTab();
            Clipboard.SetText(fileInfo.FullName);
        }

        private void ToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            var fileInfo = GetFileInfoForSelectedTab();
            if (fileInfo.DirectoryName != null) Clipboard.SetText(fileInfo.DirectoryName);
        }

        private void ToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            var fileInfo = GetFileInfoForSelectedTab();

            if (MessageBox.Show(string.Format(Properties.Resources.ConfirmDeletePrompt, fileInfo.FullName),
                    Properties.Resources.ConfirmDeleteCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) !=
                DialogResult.Yes) return;

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
                MessageBox.Show($@"{Properties.Resources.Error} : {exception.Message}", Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void RemoveSelectedTab()
        {
            _tabControlMain.TabPages.Remove(GetSelectedTabPage());
        }

        private void ToolStripMenuItemOpenFile_Click(object sender, EventArgs e)
        {
            //Otwarcie pojedynczego pliku o rozszerzeniu .zip z poziomu listy plików w zakładce Info
            if ((listViewFiles.SelectedItems.Count == 1) && (listViewFiles.SelectedItems[0].SubItems[1].Text.Substring(listViewFiles.SelectedItems[0].SubItems[1].Text.Length - 4, 4) == ".zip"))
            {
                var fileFullPath = GetFilePathFromListViewItem(listViewFiles.SelectedItems[0]);

                using OpenZip openZip = new();

                using (var zipFile = ZipFile.Read(fileFullPath))
                {
                    var i = 1;

                    foreach (var e1 in zipFile)
                    {
                        ListViewItem listViewItem = new()
                        {
                            Text = i.ToString()
                        };

                        listViewItem.SubItems.Add(e1.FileName);
                        listViewItem.SubItems.Add(e1.LastModified.ToString(CultureInfo.CurrentCulture));
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
                    var fileFullPath = GetFilePathFromListViewItem(listViewItem);

                    FileInfo fileInfo = new(fileFullPath);

                    //Nie przetwarzaj plików o rozszerzeniu .zip
                    if (fileInfo.Extension == Configuration.ZIP_FILE_EXTENSION)
                    {
                        using var openZip = new OpenZip();
                        openZip.ShowDialog(this);
                    }
                    else
                    {
                        AnalizeLog(fileInfo.FullName);
                    }
                }
            }
        }

        private void ToolStripMenuItemDeleteFile_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listViewFiles.SelectedItems)
            {
                var fileFullPath = GetFilePathFromListViewItem(listViewItem);

                FileInfo fileInfo = new(fileFullPath);

                if (MessageBox.Show(string.Format(Properties.Resources.ConfirmDeletePrompt, fileFullPath), Properties.Resources.ConfirmDeleteCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
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
                    MessageBox.Show($@"{Properties.Resources.Error} : {ex.Message}", Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ToolStripMenuItem3_Click_1(object sender, EventArgs e)
        {
            var filePath = GetSelectedTabPage().ToolTipText;
            FileInfo fileInfo = new(filePath);
            Clipboard.SetText(fileInfo.Name);

            if (fileInfo.DirectoryName != null)
                System.Diagnostics.Process.Start("explorer.exe", fileInfo.DirectoryName);
        }

        private void ToolStripMenuItemCopyFileName_Click(object sender, EventArgs e)
        {
            var fileCollection = GetListViewSelectedFiles();

            string filesNameText = fileCollection.Aggregate<FileInfo, string>(default, (current, fileInfo) => current + (fileInfo.Name + Environment.NewLine));

            Clipboard.SetText(filesNameText, TextDataFormat.UnicodeText);
        }

        private void ToolStripMenuItemCopyFilePath_Click(object sender, EventArgs e)
        {
            var fileCollection = GetListViewSelectedFiles();

            string filesFullNameText = fileCollection.Aggregate<FileInfo, string>(default, (current, fileInfo) => current + (fileInfo.FullName + Environment.NewLine));

            Clipboard.SetText(filesFullNameText, TextDataFormat.UnicodeText);
        }

        private void ToolStripMenuItemCopyCatalogPath_Click(object sender, EventArgs e)
        {
            var fileCollection = GetListViewSelectedFiles();

            var fileDirectoryNamesText = fileCollection.Aggregate<FileInfo, string>(default, (current, fileInfo) => current + (fileInfo.DirectoryName + Environment.NewLine));

            if (fileDirectoryNamesText != null) Clipboard.SetText(fileDirectoryNamesText, TextDataFormat.UnicodeText);
        }

        private FileInfo[] GetListViewSelectedFiles()
        {
            return (from ListViewItem listViewItem in listViewFiles.SelectedItems select GetFilePathFromListViewItem(listViewItem) into filePath select new FileInfo(filePath)).ToArray();
        }

        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
            => IniTabPageInfo();


        private void ListViewFiles_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right when listViewFiles.SelectedItems.Count > 1:
                    openContainFolderToolStripMenuItem.Enabled = false;
                    contextMenuStripList.Show(MousePosition);
                    break;
                case MouseButtons.Right when listViewFiles.SelectedItems.Count == 1:
                    openContainFolderToolStripMenuItem.Enabled = true;
                    contextMenuStripList.Show(MousePosition);
                    break;
            }
        }

        private void OpenContainFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listViewFiles.SelectedItems)
            {
                var filePath = GetFilePathFromListViewItem(listViewItem);

                FileInfo fileInfo = new(filePath);

                if (fileInfo.DirectoryName != null)
                    System.Diagnostics.Process.Start("explorer.exe", fileInfo.DirectoryName);
            }
        }

        private void AnalizeLog(string filePath)
        {
            var fileName = Path.GetFileName(filePath);

            if (_tabControlMain.Controls.Find(fileName, false).Length == 0)
            {
                var newPage = LogAnalyzer.GetLogTapePage(filePath);

                ToolStripMenuItem rootMenu = null;

                foreach (var (key, toolStripMenuItem) in _toolStripMenuItemFilterColumnMap)
                {
                    if (newPage.LogParser.UniqueLogKey == key)
                    {
                        toolStripMenuItem.Visible = toolStripMenuItem.Enabled = true;
                        rootMenu = toolStripMenuItem;
                    }
                    else
                    {
                        toolStripMenuItem.Visible = toolStripMenuItem.Enabled = false;
                    }
                }

                _columnToShowParserMap.TryGetValue(newPage.LogParser.UniqueLogKey, out var columnToShowCollection);

                if (rootMenu != null)
                {
                    foreach (ToolStripMenuItem toolStripMenuItem in rootMenu.DropDownItems)
                    {
                        if (columnToShowCollection == null) continue;

                        foreach (var columnToShow in columnToShowCollection.Where(columnToShow =>
                                     string.CompareOrdinal(toolStripMenuItem.Name, columnToShow.Item1) == 0))
                        {
                            toolStripMenuItem.CheckState =
                                columnToShow.Item2 ? CheckState.Checked : CheckState.Unchecked;

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

            var toolStripMenuItem = (ToolStripMenuItem)e.ClickedItem;

            if (toolStripMenuItem.CheckState == CheckState.Checked)
            {
                toolStripMenuItem.CheckState = CheckState.Unchecked;

                foreach (var logPattern in logParser.GetLogPatterns())
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
                foreach (var logPattern in logParser.GetLogPatterns())
                {
                    if (toolStripMenuItem.Name == logPattern.KeyName)
                    {
                        logPattern.Show = true;
                    }
                }
            }

            var selectedTabPage = GetSelectedTabPage();

            if (selectedTabPage == tabPageInfo) return;

            var tabPage = Controls.Find(selectedTabPage.Name, true)[0] as LogTapPage;

            if (tabPage?.Controls == null) return;

            foreach (Control control in tabPage.Controls)
            {
                if (control.GetType() == typeof(ListViewExtended))
                {
                    ShowListViewColumns(tabPage);
                }
            }
        }

        private void SetDashboardData(string directoryLogPath, Control label, Control labelSize, Control labelCount)
        {
            try
            {
                DirectoryInfo s = new(directoryLogPath);

                if (!s.Exists)
                    return;

                var directoryCollection = s.GetDirectories();

                label.Text = s.FullName;

                ArrayList myfileinfos = new();

                foreach (var ext in _fileExtensions)
                    myfileinfos.AddRange(s.GetFiles(ext, SearchOption.AllDirectories));

                var fileCollection = (FileInfo[])myfileinfos.ToArray(typeof(FileInfo));

                var totalSize = GetFilesTotalSize(fileCollection);

                labelSize.Text = IOTools.FormatFileSize(totalSize);

                labelCount.Text = (fileCollection.Length) + " files, " + directoryCollection.Length.ToString() + " directories";
            }
            catch
            {
                // ignored
            }
        }

        private static float GetFilesTotalSize(IReadOnlyCollection<FileInfo> filesCollection)
        {
            if (filesCollection?.Count == 0)
                return 0;

            return filesCollection?.Aggregate<FileInfo, float>(0, (current, file) => current + file.Length) ?? 0;
        }

        private void CheckBoxLogs_CheckedChanged(object sender, EventArgs e)
            => SetFilesInfoGridViewData();


        private void CheckBoxUser_CheckedChanged(object sender, EventArgs e)
            => SetFilesInfoGridViewData();

        private static ListViewItem[] CloneItems(ListViewItemCollection items)
        {
            return (from ListViewItem listViewItem in items select listViewItem.Clone() as ListViewItem).ToArray();
        }

        private void ToolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            timerFind.Enabled = false;
            timerFind.Enabled = true;
        }

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
            var selectedTabPage = GetSelectedTabPage();

            if (selectedTabPage == tabPageInfo)
                SearchInDashboardView();
            else
                SearchInLogFileView(selectedTabPage);

        }

        private void SearchInLogFileView(Control selectedTabPage)
        {
            var tabP = (TabPage)Controls.Find(selectedTabPage.Name, true)[0];

            foreach (Control control in tabP.Controls)
            {
                if (control.GetType() != typeof(ListViewExtended))
                    continue;

                var listViewExtended = (ListViewExtended)control;

                if (toolStripTextBox.Text?.Length == 0)
                {
                    listViewExtended.SuspendLayout();
                    listViewExtended.BeginUpdate();

                    try
                    {
                        foreach (ListViewItem listViewItem in listViewExtended.Items)
                        {
                            listViewExtended.SetOneGroupState(listViewItem.Group, state: Views.ListViewGroupState.Collapsible | Views.ListViewGroupState.Collapsed);
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

                    var colCount = listViewExtended.Columns.Count;

                    foreach (ListViewItem listViewItem in listViewExtended.Items)
                    {
                        for (var colAll = 0; colAll < colCount; colAll++)
                        {
                            if (listViewExtended.Columns[colAll].Width == 0)
                                continue;

                            var listViewSubItemText = listViewItem.SubItems[colAll].Text;

                            if (string.IsNullOrWhiteSpace(listViewSubItemText))
                                continue;

                            if (toolStripTextBox.Text != null && listViewSubItemText.IndexOf(toolStripTextBox.Text, StringComparison.OrdinalIgnoreCase) > -1)
                            {
                                listViewExtended.SuspendLayout();
                                listViewExtended.BeginUpdate();
                                try
                                {
                                    //ListV.SetGroupFooter(ListV.Items[lst12].Group, "Test");
                                    listViewExtended.SetOneGroupState(listViewItem.Group, Views.ListViewGroupState.Collapsible);
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
            SetFilesInfoGridViewData();

            ListView listView = new();

            listView.Items.Clear();

            var colCount = listViewFiles.Columns.Count;

            for (var lst12 = 1; lst12 < listViewFiles.Items.Count; lst12++)
            {
                for (var colAll = 0; colAll < colCount; colAll++)
                {
                    if (listViewFiles.Items[lst12].SubItems[colAll].Text
                            .IndexOf(toolStripTextBox.Text, StringComparison.OrdinalIgnoreCase) <= -1) continue;

                    listView.Items.Add((ListViewItem)listViewFiles.Items[lst12].Clone());
                    break;

                }

            }

            listViewFiles.Items.Clear();
            listViewFiles.Items.AddRange(CloneItems(listView.Items));
        }

        private void AnalizeToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                using var options = new Options.Options();

                if (options.ShowDialog(this) != DialogResult.OK)
                    return;

                _config = Configuration.GetConfiguration();

                IniTabPageInfo();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void EnglishToolStripMenuItem_Click(object sender, EventArgs e)
            => ChangeLanguage("en-us");

        private void PolishToolStripMenuItem_Click(object sender, EventArgs e)
            => ChangeLanguage("pl-pl");

        #endregion Private Methods

    }

}
