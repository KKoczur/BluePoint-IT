using System;
using System.Windows.Forms;
using ListViewGroupCollapse;

namespace StatlookLogViewer
{
    public class NewPage: TabPage
    {
        private readonly ListViewColumnSorter _lvwColumnSorter = new ListViewColumnSorter();
        private readonly bool[] _statlookColumnNeedToShow=new bool[10];
        private readonly bool[] _usmColumnNeedToShow=new bool[6];

        private readonly Configuration _config;


        public NewPage()
        {

        }

        /// <summary>
        /// Create new page
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="name">Page name</param>
        /// <param name="fullName">Page fullname</param>
        /// <param name="columnNames">Column names</param>
        /// <param name="lastWriteTime">File last write time</param>
        /// <param name="logType">Type of log</param>
        public NewPage(int index, string name, string fullName, string[] columnNames, DateTime lastWriteTime, LogType logType)
        {
            LogType = logType;

            this.Location = new System.Drawing.Point(4, 22);
            this.Name = name;
            this.Padding = new Padding(3);
            this.Size = new System.Drawing.Size(994, 610);
            this.TabIndex = index;
            this.Text = "      " + name;
            this.UseVisualStyleBackColor = true;
            this.ToolTipText = fullName;
            this.Tag = fullName;
            this.Controls.Add(ListViewExtended);

            _config = Configuration.GetConfiguration();

            int j = 0;
            foreach (Descriptor descriptor in _config.GetStatlookDescriptors())
            {
                _statlookColumnNeedToShow[j] = descriptor.Show;
                j++;
            }

            int k = 0;
            foreach (Descriptor descriptor in _config.GetUsmDescriptors())
            {
                _usmColumnNeedToShow[k] = descriptor.Show;
                k++;
            }
            

            ListViewExtended.ListViewItemSorter = _lvwColumnSorter;

            for (int i = 0; i < columnNames.Length; i++)
            {
                ListViewExtended.Columns.Add(columnNames[i], 0);
            }

            ListViewExtended.Dock = DockStyle.Fill;
            ListViewExtended.GridLines = true;
            ListViewExtended.Location = new System.Drawing.Point(3, 3);
            ListViewExtended.Name = name;
            ListViewExtended.Size = new System.Drawing.Size(988, 604);
            ListViewExtended.TabIndex = index;
            ListViewExtended.UseCompatibleStateImageBehavior = false;
            ListViewExtended.View = View.Details;
            ListViewExtended.GridLines = true;
            ListViewExtended.FullRowSelect = true;
            ListViewExtended.ListViewItemSorter = null;
            ListViewExtended.SetGroupState(ListViewGroupState.Collapsible);
            ListViewExtended.ColumnClick += listViewFiles_ColumnClick;

            switch (LogType)
            {
                case LogType.Statlook:
                    {
                        CheckColumnsVisibility(ListViewExtended, _statlookColumnNeedToShow);
                        break;
                    }
                case LogType.Usm:
                    {
                        CheckColumnsVisibility(ListViewExtended, _usmColumnNeedToShow);
                        break;
                    }

                default:
                    break;
            }

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
