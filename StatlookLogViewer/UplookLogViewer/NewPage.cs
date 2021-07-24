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

namespace UplookLogViewer
{
    public class NewPage:TabPage
    {
        private ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();
        private TabPage m_nowaZakladka = new TabPage();
        private ListViewExtended m_nowaLista = new ListViewExtended();
        private enum _rodzaj : int { uplook, usm };
        private string m_typRaportu;
        private bool[] show_uplook=new bool[10];
        private bool[] show_usm=new bool[6];


        public NewPage()
        {

        }
        //Tworzy nowa zakładkę
        public NewPage(int index, string name, string Fullname, string[] NazwaKolumny, string dataUtworzenia, int rodzajLogu)
        {

            Configuration config;
            if (!File.Exists("config.xml"))
            {
                // Create a new configuration object
                // and initialize some variables
                Configuration c = new Configuration();

                // Serialize the configuration object to a file
                Configuration.Serialize("config.xml", c);

                // Read the configuration object from a file
                config = Configuration.Deserialize("config.xml");
            }
            else
            {
                // Read the configuration object from a file
                config = Configuration.Deserialize("config.xml");
            }
            Deskryptor[] udes = config.UReadHeaders();
            int j = 0;
            foreach (Deskryptor d in udes)
            {
                show_uplook[j] = d.Show;
                j++;
            }
            Deskryptor[] usmdes = config.USMReadHeaders();
            int k = 0;
            foreach (Deskryptor d in usmdes)
            {
                show_usm[k] = d.Show;
                k++;
            }
            

            m_nowaLista.ListViewItemSorter = lvwColumnSorter;

            for (int i = 0; i < NazwaKolumny.Length; i++)
            {
                m_nowaLista.Columns.Add(NazwaKolumny[i], 0);
            }
            m_nowaLista.Dock = System.Windows.Forms.DockStyle.Fill;
            m_nowaLista.GridLines = true;
            m_nowaLista.Location = new System.Drawing.Point(3, 3);
            m_nowaLista.Name = name;
            m_nowaLista.Size = new System.Drawing.Size(988, 604);
            m_nowaLista.TabIndex = index;
            m_nowaLista.UseCompatibleStateImageBehavior = false;
            m_nowaLista.View = System.Windows.Forms.View.Details;
            m_nowaLista.GridLines = true;
            m_nowaLista.FullRowSelect = true;
            m_nowaLista.ListViewItemSorter = null;
            m_nowaLista.SetGroupState(ListViewGroupState.Collapsible);
            m_nowaLista.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(listViewFiles_ColumnClick);
            switch (rodzajLogu)
            {
                case 0:
                {
                    sprawdzenieWidocznoscikolumn(m_nowaLista,show_uplook);
                    nowaZakladka.Tag = "uplook";
                    break;
                }
                case 1:
                {
                    sprawdzenieWidocznoscikolumn(m_nowaLista, show_usm);
                    nowaZakladka.Tag = "usm";
                    break;
                }
            }

            // 
            // TabPages
            // 
            nowaZakladka.Location = new System.Drawing.Point(4, 22);
            nowaZakladka.Name = name;
            nowaZakladka.Padding = new System.Windows.Forms.Padding(3);
            nowaZakladka.Size = new System.Drawing.Size(994, 610);
            nowaZakladka.TabIndex = index;
            nowaZakladka.Text = "      " + name;
            nowaZakladka.UseVisualStyleBackColor = true;
            nowaZakladka.ToolTipText = Fullname;
            nowaZakladka.Tag = Fullname;
            nowaZakladka.Controls.Add(m_nowaLista);
        }

        #region Wlasciwosci

        public TabPage nowaZakladka
        {
            get
            {
                return m_nowaZakladka;
            }
        }

        public ListViewExtended nowaLista
        {
            get
            {
                return m_nowaLista;
            }
        }

        public string typRaportu
        {
            get { return m_typRaportu;}
            set { m_typRaportu = value;}
        }

        #endregion Wlasciwosci

        private void listViewFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            m_nowaLista.BeginUpdate();
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
            m_nowaLista.Sort();
            m_nowaLista.EndUpdate();
        }

        private void sprawdzenieWidocznoscikolumn(ListView ListViewTmp, bool[] show)
        {
            // Loop through and size each column header to fit the column header text.

                        int j = 0;
                        foreach (ColumnHeader ch in ListViewTmp.Columns)
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
