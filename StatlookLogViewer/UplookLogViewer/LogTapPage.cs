using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ListViewGroupCollapse;

namespace StatlookLogViewer
{
    public class LogTapPage : TabPage
    {
        private readonly ListViewColumnSorter _lvwColumnSorter = new ListViewColumnSorter();
        private readonly List<bool> _statlookColumnNeedToShow = new List<bool>();
        private readonly List<bool> _usmColumnNeedToShow = new List<bool>();

        private readonly Configuration _config;

        public LogTapPage()
        {

        }

        /// <summary>
        /// Create new page
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="fileNameWithPath">Page name</param>
        /// <param name="logType">Type of log</param>
        public LogTapPage(int index, string fileNameWithPath, LogType logType)
        {
            string fileName = Path.GetFileName(fileNameWithPath);

            LogType = logType;

            _config = Configuration.GetConfiguration();
            List<string> columnNames = new List<string>();

            switch (LogType)
            {
                case LogType.Statlook:
                    {
                        foreach (Descriptor descriptor in _config.GetStatlookDescriptors())
                        {
                            _statlookColumnNeedToShow.Add(descriptor.Show);
                            columnNames.Add(descriptor.RowCaption);
                        }


                        for (int i = 0; i < columnNames.Count; i++)
                        {
                            ListViewExtended.Columns.Add(columnNames[i], 0);
                        }

                        CheckColumnsVisibility(ListViewExtended, _statlookColumnNeedToShow.ToArray());
                        break;
                    }
                case LogType.Usm:
                    {

                        foreach (Descriptor descriptor in _config.GetUsmDescriptors())
                        {
                            _usmColumnNeedToShow.Add(descriptor.Show);
                            columnNames.Add(descriptor.RowCaption);
                        }

                        for (int i = 0; i < columnNames.Count; i++)
                        {
                            ListViewExtended.Columns.Add(columnNames[i], 0);
                        }

                        CheckColumnsVisibility(ListViewExtended, _usmColumnNeedToShow.ToArray());
                        break;
                    }
            }



            ListViewExtended.ListViewItemSorter = _lvwColumnSorter;

            ListViewExtended.Dock = DockStyle.Fill;
            ListViewExtended.GridLines = true;
            ListViewExtended.Location = new System.Drawing.Point(3, 3);
            ListViewExtended.Name = fileNameWithPath;
            ListViewExtended.Size = new System.Drawing.Size(988, 604);
            ListViewExtended.TabIndex = index;
            ListViewExtended.UseCompatibleStateImageBehavior = false;
            ListViewExtended.View = View.Details;
            ListViewExtended.GridLines = true;
            ListViewExtended.FullRowSelect = true;
            ListViewExtended.ListViewItemSorter = null;
            ListViewExtended.SetGroupState(ListViewGroupState.Collapsible);
            ListViewExtended.ColumnClick += listViewFiles_ColumnClick;



            Location = new System.Drawing.Point(4, 22);
            Name = fileName;
            Padding = new Padding(3);
            Size = new System.Drawing.Size(994, 610);
            TabIndex = index;
            Text = "      " + fileName;
            UseVisualStyleBackColor = true;
            ToolTipText = fileNameWithPath;
            Tag = fileNameWithPath;
            Controls.Add(ListViewExtended);

        }

        #region Properties

        public ListViewExtended ListViewExtended { get; } = new ListViewExtended();

        public LogType LogType { get; set; }

        #endregion Properties

        private void listViewFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewExtended.BeginUpdate();

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

                ListViewExtended.Sort();
            }
            finally
            {
                ListViewExtended.EndUpdate();
            }
        }

        private void CheckColumnsVisibility(ListView listView, bool[] show)
        {
            // Loop through and size each column header to fit the column header text.

            int j = 0;
            foreach (ColumnHeader columnHeader in listView.Columns)
            {
                if (show[j])
                    columnHeader.Width = -2;
                else
                    columnHeader.Width = 0;
                j++;
            }

        }
    }
}
