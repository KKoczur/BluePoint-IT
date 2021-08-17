using System.Windows.Forms;
using System.IO;
using ListViewGroupCollapse;

namespace StatlookLogViewer
{
    public class NewPage:TabPage
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
        /// <param name="createdDate">Created date</param>
        /// <param name="logType">Type of log</param>
        public NewPage(int index, string name, string fullName, string[] columnNames, string createdDate, LogType logType)
        {
            _config = Configuration.GetConfiguration();

            int j = 0;
            foreach (Descriptor d in _config.GetStatlookDescriptors())
            {
                _statlookColumnNeedToShow[j] = d.Show;
                j++;
            }

            int k = 0;
            foreach (Descriptor d in _config.GetUsmDescriptors())
            {
                _usmColumnNeedToShow[k] = d.Show;
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

            switch (logType)
            {
                case LogType.Statlook:
                    {
                        CheckColumnsVisibility(ListViewExtended, _statlookColumnNeedToShow);
                        NewTabPage.Tag = "uplook";
                        break;
                    }
                case LogType.Usm:
                    {
                        CheckColumnsVisibility(ListViewExtended, _usmColumnNeedToShow);
                        NewTabPage.Tag = "usm";
                        break;
                    }

                default:
                    break;
            }

            // 
            // TabPages
            // 
            NewTabPage.Location = new System.Drawing.Point(4, 22);
            NewTabPage.Name = name;
            NewTabPage.Padding = new Padding(3);
            NewTabPage.Size = new System.Drawing.Size(994, 610);
            NewTabPage.TabIndex = index;
            NewTabPage.Text = "      " + name;
            NewTabPage.UseVisualStyleBackColor = true;
            NewTabPage.ToolTipText = fullName;
            NewTabPage.Tag = fullName;
            NewTabPage.Controls.Add(ListViewExtended);
        }

        #region Properties

        public TabPage NewTabPage { get; } = new TabPage();

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
            foreach (ColumnHeader ch in listView.Columns)
            {
                if (show[j])
                    ch.Width = -2;
                else
                    ch.Width = 0;
                j++;
            }

        }
    }
}
