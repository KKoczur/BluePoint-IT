using StatlookLogViewer.Controller;
using StatlookLogViewer.Parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace StatlookLogViewer.Views;

public sealed class LogTapPage : TabPage
{
    #region Members

    private readonly ListViewExtended _listViewExtended;

    #endregion Members

    private readonly ListViewColumnSorter _lvwColumnSorter = new();

    #region Properties

    public ILogParser LogParser { get; set; }

    #endregion Properties

    public void SetListViewGroups(List<ListViewGroup> group)
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

    private ListViewExtended CreateListViewExtended(string filePath, int tabIndex)
    {
        ListViewExtended listViewExtended = new();
        listViewExtended.ListViewItemSorter = _lvwColumnSorter;
        listViewExtended.Dock = DockStyle.Fill;
        listViewExtended.GridLines = true;
        listViewExtended.Location = new Point(3, 3);
        listViewExtended.Name = filePath;
        listViewExtended.Size = new Size(988, 604);
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
            _listViewExtended.Columns.AddRange(logParser.GetLogPatterns()
                .Select(pattern => new ColumnHeader { Text = pattern.TextPattern, Width = -2 }).ToArray());
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
            foreach (ToolStripMenuItem toolStripMenuItem in toolStripItemCollection)
            {
                if (string.Compare(columnHeader.Text, toolStripMenuItem.Text, StringComparison.OrdinalIgnoreCase) !=
                    0) continue;

                if (toolStripMenuItem.CheckState == CheckState.Checked)
                    columnHeader.Width = -2;
                else
                    columnHeader.Width = 0;
            }
    }

    #region Event Handlers

    private void ListViewFiles_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        _listViewExtended.BeginUpdate();

        try
        {
            if (e.Column == _lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                _lvwColumnSorter.Order = _lvwColumnSorter.Order == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
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

    #region Constructors

    /// <summary>
    ///     Create new page
    /// </summary>
    /// <param name="index">Index</param>
    /// <param name="filePath">Page name</param>
    /// <param name="logParser">Log parser</param>
    public LogTapPage(int index, string filePath, ILogParser logParser)
    {
        LogParser = logParser;

        _listViewExtended = CreateListViewExtended(filePath, index);

        AddListViewColumns(LogParser);

        Location = new Point(4, 22);

        var fileName = Path.GetFileName(filePath);
        Name = fileName;
        Padding = new Padding(3);
        Size = new Size(994, 610);
        TabIndex = index;
        Text = $@"      {fileName}";
        UseVisualStyleBackColor = true;
        ToolTipText = filePath;
        Tag = filePath;

        Controls.Add(_listViewExtended);
    }

    /// <summary>
    ///     Create new page
    /// </summary>
    public LogTapPage(string filePath, ILogParser logParser, List<ListViewGroup> groups,
        List<ListViewItem> listViewItems)
        : this(0, filePath, logParser)
    {
        SetListViewGroups(groups);

        SetListViewItems(listViewItems.ToArray());
    }

    #endregion Constructors
}