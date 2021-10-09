using StatlookLogViewer.Controller;
using StatlookLogViewer.Parser;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace StatlookLogViewer.Views
{
    public class LogTapPage : TabPage
    {
        private readonly ListViewColumnSorter _lvwColumnSorter = new();

        #region Constructors

        /// <summary>
        /// Create new page
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="filePath">Page name</param>
        /// <param name="logParser">Log parser</param>
        public LogTapPage(int index, string filePath, ILogParser logParser)
        {
            LogParser = logParser;

            _listViewExtended = CreateListViewExtended(filePath, index);

            AddListViewColumns(LogParser);

            Location = new System.Drawing.Point(4, 22);

            string fileName = Path.GetFileName(filePath);
            Name = fileName;
            Padding = new Padding(3);
            Size = new System.Drawing.Size(994, 610);
            TabIndex = index;
            Text = "      " + fileName;
            UseVisualStyleBackColor = true;
            ToolTipText = filePath;
            Tag = filePath;

            Controls.Add(_listViewExtended);
        }

        #endregion Constructors

        #region Members

        private readonly ListViewExtended _listViewExtended ;

        #endregion Members

        public void SetListViewGroups( List<ListViewGroup> group)
        {
            _listViewExtended.BeginUpdate();
            _listViewExtended.SuspendLayout();
            try
            {
                _listViewExtended.Groups.AddRange(group.ToArray());
            }
            finally
            {
                _listViewExtended.EndUpdate();
                _listViewExtended.ResumeLayout();
            }
        }

        public void SetListViewItems(ListViewItem[] listViewItemCollection)
        {
            _listViewExtended.BeginUpdate();
            _listViewExtended.SuspendLayout();
            try
            {
                _listViewExtended.Items.AddRange(listViewItemCollection);
            }
            finally
            {
                _listViewExtended.EndUpdate();
                _listViewExtended.ResumeLayout();
            }
        }

        private ListViewExtended CreateListViewExtended(string filePath ,int tabIndex)
        {
            ListViewExtended listViewExtended = new();
            listViewExtended.ListViewItemSorter = _lvwColumnSorter;
            listViewExtended.Dock = DockStyle.Fill;
            listViewExtended.GridLines = true;
            listViewExtended.Location = new System.Drawing.Point(3, 3);
            listViewExtended.Name = filePath;
            listViewExtended.Size = new System.Drawing.Size(988, 604);
            listViewExtended.TabIndex = tabIndex;
            listViewExtended.UseCompatibleStateImageBehavior = false;
            listViewExtended.View = View.Details;
            listViewExtended.GridLines = true;
            listViewExtended.FullRowSelect = true;
            listViewExtended.ListViewItemSorter = null;
            listViewExtended.SetGroupState(ListViewGroupState.Collapsible);
            listViewExtended.ColumnClick += ListViewFiles_ColumnClick;

            return listViewExtended;
        }

        private void AddListViewColumns(ILogParser logParser)
        {
            _listViewExtended.BeginUpdate();
            _listViewExtended.SuspendLayout();

            try
            {
                List<ColumnHeader> columnHeaders = new ();

                foreach (var pattern in logParser.GetLogPatterns())
                {
                    columnHeaders.Add(new ColumnHeader() { Text = pattern.TextPattern, Width = -2 });
                }

                _listViewExtended.Columns.AddRange(columnHeaders.ToArray());
            }
            finally
            {
                _listViewExtended.EndUpdate();
                _listViewExtended.ResumeLayout();
            }
        }

        public void SetListViewColumnsColumnsVisibility(ToolStripItemCollection toolStripItemCollection)
        {
            // Loop through and size each column header to fit the column header text.
            foreach (ColumnHeader columnHeader in _listViewExtended.Columns)
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

        #region Properties

        public ILogParser LogParser { get; set; }

        #endregion Properties

        #region Event Handlers

        private void ListViewFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            _listViewExtended.BeginUpdate();

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

                _listViewExtended.Sort();
            }
            finally
            {
                _listViewExtended.EndUpdate();
            }
        }

        #endregion Event Handlers
    }
}
