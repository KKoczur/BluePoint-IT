using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace StatlookLogViewer
{
    class Linia
    {
        #region Zmienne

        private int m_NumerLinii;
        private Headers m_Headers = new Headers();
        private string m_GroupName;
        private ListViewItem m_ListViewItem = new ListViewItem();
        private ListViewGroup m_ListViewGroup = new ListViewGroup();

        #endregion Zmienne

        #region Konstruktory

        public Linia()
        {
        }

        #endregion Konstruktory 

        #region Wlasciwosci

        public int NumerLinii
        {
            set
            {
                m_NumerLinii = value;
            }
            get
            {
                return m_NumerLinii;
            }
        }

        public Headers Headers
        {
            get
            {
                return m_Headers;
            }
        }

        public string GroupName
        {
            get
            {
                return m_GroupName;
            }
        }

        public ListViewItem ListViewItem
        {
            get
            {
                return m_ListViewItem;
            }

        }

        #endregion Wlasciowsci

        #region Metody

        public void AddLine(string Header, string Value, int numer)
        {
            string tmp_Header= Header;
            string tmp_Value= Value;
            switch (numer)
            {
                #region uplook_Log
                case 1:
                {
                    string tmp_NazwaGrupy = null;
                    string myHourTime = null;
                    DictionaryLog SlownikErrors = new DictionaryLog();
                    Descriptor[] ListaBledow = SlownikErrors.GetListOfErrors();
                    foreach (Descriptor Des in m_Headers.uplook_Deskryptor)
                    {
                        if (Des.Name == tmp_Header)
                        {
                            if (tmp_Header == m_Headers.uplook_Date)
                            {
                                DateTime tmp_DateTime = DateTime.Parse(tmp_Value);
                                myHourTime = tmp_DateTime.Hour.ToString();
                                //Określenie nazwę grupy do której ma należeć linia
                                if (myHourTime.Length < 2)
                                {
                                    tmp_NazwaGrupy = "0" + myHourTime.ToString() + ":00-" + "0" + myHourTime.ToString() + ":59";
                                }
                                else
                                {
                                    tmp_NazwaGrupy = myHourTime.ToString() + ":00-" + myHourTime.ToString() + ":59";
                                }
                                m_GroupName = tmp_NazwaGrupy;
                                m_ListViewItem.Text = tmp_Value;
                                m_ListViewGroup = new ListViewGroup(m_GroupName, HorizontalAlignment.Left);
                                m_ListViewItem.Group = m_ListViewGroup;
                                m_ListViewItem.Group.Name = m_GroupName; /**/
                                m_ListViewItem.Group.Header = m_GroupName;
                                Des.Value = tmp_Value;
                                break;
                            }
                            /*if (tmp_Value.Contains("Error"))
                            {
                                m_ListViewItem.UseItemStyleForSubItems = false;
                                m_ListViewItem.SubItems.Add(tmp_Value, System.Drawing.Color.Red, System.Drawing.Color.White, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238))));
                            }*/
                            m_ListViewItem.SubItems.Add(tmp_Value);
                            
                        if (Regex.IsMatch(tmp_Value, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b został uruchomiony.") || Regex.IsMatch(tmp_Value, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b started"))
                        {
                            m_ListViewItem.Group.Header = m_ListViewItem.Group.Name + " (" + tmp_Value + ")"; /**/
                        }
                        else
                        {
                            foreach (Descriptor des in ListaBledow)
                            {
                                if (tmp_Value.Contains(des.Name))
                                {
                                    if (!m_ListViewItem.Group.Header.Contains(des.Value))
                                    {
                                        m_ListViewItem.Group.Header += " ( " + des.Value + " )";
                                    }
                                }
                            }
                        }
                        Des.Value = tmp_Value;
                        break;
                    }
                    }
                        break;
                    }
                        #endregion uplook_Log

                #region usm_Log
             case 2:
             {
                string tmp_NazwaGrupy = null;
                string myHourTime = null;
                DictionaryLog SlownikErrors = new DictionaryLog();
                Descriptor[] ListaBledow = SlownikErrors.GetListOfErrors();
                foreach (Descriptor Des in m_Headers.usm_Deskryptor)
                {
                    if (Des.Name == tmp_Header)
                    {
                        if (tmp_Header == m_Headers.uplook_Date)
                        {
                            DateTime tmp_DateTime = DateTime.Parse(tmp_Value);
                            myHourTime = tmp_DateTime.Hour.ToString();
                            //Określenie nazwę grupy do której ma należeć linia
                            if (myHourTime.Length < 2)
                            {
                                tmp_NazwaGrupy = "0" + myHourTime.ToString() + ":00-" + "0" + myHourTime.ToString() + ":59";
                            }
                            else
                            {
                                tmp_NazwaGrupy = myHourTime.ToString() + ":00-" + myHourTime.ToString() + ":59";
                            }
                            m_GroupName = tmp_NazwaGrupy;
                            m_ListViewItem.Text = tmp_Value;
                            m_ListViewGroup = new ListViewGroup(m_GroupName, HorizontalAlignment.Left);
                            m_ListViewItem.Group = m_ListViewGroup;
                            m_ListViewItem.Group.Name = m_GroupName; /**/
                            m_ListViewItem.Group.Header = m_GroupName;
                            Des.Value = tmp_Value;
                            break;
                        }
                        m_ListViewItem.SubItems.Add(tmp_Value);
                        if (Regex.IsMatch(tmp_Value, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b został uruchomiony.") || Regex.IsMatch(tmp_Value, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b started"))
                        {
                            m_ListViewItem.Group.Header = m_ListViewItem.Group.Name + " (" + tmp_Value + ")"; /**/
                        }
                        else
                        {
                            foreach (Descriptor des in ListaBledow)
                            {
                                if (tmp_Value.Contains(des.Name))
                                {
                                    if (!m_ListViewItem.Group.Header.Contains(des.Value))
                                    {
                                        m_ListViewItem.Group.Header += " ( " + des.Value + " )";
                                    }
                                }
                            }
                        }
                        Des.Value = tmp_Value;
                        break;
                    }
                }
                break;
               }
               #endregion usm_Log
            }

        }


        #endregion Metody
    }
}

    