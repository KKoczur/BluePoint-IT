using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using ListViewGroupCollapse;
using Yoramo.GuiLib;

namespace StatlookLogViewer
{
    public class NewPage:TabPage
    {
        private const string CONFIG_FILE_NAME = "config.xml";
        private readonly ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();
        private bool[] show_uplook=new bool[10];
        private bool[] show_usm=new bool[6];


        public NewPage()
        {

        }

        /// <summary>
        /// Create new page
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="name">Page name</param>
        /// <param name="Fullname">Page fullname</param>
        /// <param name="columnNames">Column names</param>
        /// <param name="createdDate">Creaded date</param>
        /// <param name="typeOfLog">Type of log</param>
        public NewPage(int index, string name, string Fullname, string[] columnNames, string createdDate, LogType typeOfLog)
        {
            Configuration config;

            if (!File.Exists(CONFIG_FILE_NAME))
            {
                // Create a new configuration object
                // and initialize some variables
                Configuration c = new Configuration();

                // Serialize the configuration object to a file
                Configuration.Serialize(CONFIG_FILE_NAME, c);

                // Read the configuration object from a file
                config = Configuration.Deserialize(CONFIG_FILE_NAME);
            }
            else
            {
                // Read the configuration object from a file
                config = Configuration.Deserialize(CONFIG_FILE_NAME);
            }

            Descriptor[] udes = config.GetStatlookHeaders();

            int j = 0;
            foreach (Descriptor d in udes)
            {
                show_uplook[j] = d.Show;
                j++;
            }

            Descriptor[] usmdes = config.GetUsmHeaders();
            int k = 0;
            foreach (Descriptor d in usmdes)
            {
                show_usm[k] = d.Show;
                k++;
            }
            

            ListViewExtended.ListViewItemSorter = lvwColumnSorter;

            for (int i = 0; i < columnNames.Length; i++)
            {
                ListViewExtended.Columns.Add(columnNames[i], 0);
            }

            ListViewExtended.Dock = System.Windows.Forms.DockStyle.Fill;
            ListViewExtended.GridLines = true;
            ListViewExtended.Location = new System.Drawing.Point(3, 3);
            ListViewExtended.Name = name;
            ListViewExtended.Size = new System.Drawing.Size(988, 604);
            ListViewExtended.TabIndex = index;
            ListViewExtended.UseCompatibleStateImageBehavior = false;
            ListViewExtended.View = System.Windows.Forms.View.Details;
            ListViewExtended.GridLines = true;
            ListViewExtended.FullRowSelect = true;
            ListViewExtended.ListViewItemSorter = null;
            ListViewExtended.SetGroupState(ListViewGroupState.Collapsible);
            ListViewExtended.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(listViewFiles_ColumnClick);

            switch (typeOfLog)
            {
                case LogType.Statlook:
                    {
                        sprawdzenieWidocznoscikolumn(ListViewExtended, show_uplook);
                        NewTabPage.Tag = "uplook";
                        break;
                    }
                case LogType.Usm:
                    {
                        sprawdzenieWidocznoscikolumn(ListViewExtended, show_usm);
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
            NewTabPage.Padding = new System.Windows.Forms.Padding(3);
            NewTabPage.Size = new System.Drawing.Size(994, 610);
            NewTabPage.TabIndex = index;
            NewTabPage.Text = "      " + name;
            NewTabPage.UseVisualStyleBackColor = true;
            NewTabPage.ToolTipText = Fullname;
            NewTabPage.Tag = Fullname;
            NewTabPage.Controls.Add(ListViewExtended);
        }

        #region Properties

        public TabPage NewTabPage { get; } = new TabPage();

        public ListViewExtended ListViewExtended { get; } = new ListViewExtended();

        public string TypeOfReport { get; set; }

        #endregion Properties

        private void listViewFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewExtended.BeginUpdate();
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                if (e.Column != 0)
                {
                    lvwColumnSorter.SortColumn = e.Column;
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            //lvwColumnSorter.Order = SortOrder.None;
            // Call the sort method to manually sort the column based on the ListViewItemComparer implementation.
            ListViewExtended.Sort();
            ListViewExtended.EndUpdate();
        }

        private void sprawdzenieWidocznoscikolumn(ListView listView, bool[] show)
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
