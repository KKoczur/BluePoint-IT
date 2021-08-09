using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace StatlookLogViewer
{
    internal class Linia
    {

        #region Members

        private string _groupName;

        private ListViewGroup _listViewGroup = new ListViewGroup();

        #endregion Members

        #region Properties

        public int NumerLinii { set; get; }

        public Headers Headers { get; } = new Headers();

        public string GroupName => _groupName;

        public ListViewItem ListViewItem { get; } = new ListViewItem();

        #endregion Properties

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

                    Descriptor[] ListaBledow = SlownikErrors.GetListOfAllErrors();

                    foreach (Descriptor Des in Headers.uplook_Deskryptor)
                    {
                        if (Des.KeyName == tmp_Header)
                        {
                            if (tmp_Header == Headers.uplook_Date)
                            {
                                DateTime tmp_DateTime = DateTime.Parse(tmp_Value);
                                myHourTime = tmp_DateTime.Hour.ToString();
                                //Określenie nazwę grupy do której ma należeć linia
                                if (myHourTime.Length < 2)
                                {
                                    tmp_NazwaGrupy = "0" + myHourTime + ":00-" + "0" + myHourTime + ":59";
                                }
                                else
                                {
                                    tmp_NazwaGrupy = myHourTime + ":00-" + myHourTime + ":59";
                                }
                                _groupName = tmp_NazwaGrupy;
                                    ListViewItem.Text = tmp_Value;
                                _listViewGroup = new ListViewGroup(_groupName, HorizontalAlignment.Left);
                                    ListViewItem.Group = _listViewGroup;
                                    ListViewItem.Group.Name = _groupName; /**/
                                    ListViewItem.Group.Header = _groupName;
                                Des.HeaderText = tmp_Value;
                                break;
                            }
                                /*if (tmp_Value.Contains("Error"))
                                {
                                    m_ListViewItem.UseItemStyleForSubItems = false;
                                    m_ListViewItem.SubItems.Add(tmp_Value, System.Drawing.Color.Red, System.Drawing.Color.White, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238))));
                                }*/
                                ListViewItem.SubItems.Add(tmp_Value);
                            
                        if (Regex.IsMatch(tmp_Value, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b został uruchomiony.") || Regex.IsMatch(tmp_Value, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b started"))
                        {
                                    ListViewItem.Group.Header = ListViewItem.Group.Name + " (" + tmp_Value + ")"; /**/
                        }
                        else
                        {
                            foreach (Descriptor des in ListaBledow)
                            {
                                if (tmp_Value.Contains(des.KeyName))
                                {
                                    if (!ListViewItem.Group.Header.Contains(des.HeaderText))
                                    {
                                                ListViewItem.Group.Header += " ( " + des.HeaderText + " )";
                                    }
                                }
                            }
                        }
                        Des.HeaderText = tmp_Value;
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
                Descriptor[] ListaBledow = SlownikErrors.GetListOfAllErrors();
                foreach (Descriptor Des in Headers.usm_Deskryptor)
                {
                    if (Des.KeyName == tmp_Header)
                    {
                        if (tmp_Header == Headers.uplook_Date)
                        {
                            DateTime tmp_DateTime = DateTime.Parse(tmp_Value);
                            myHourTime = tmp_DateTime.Hour.ToString();
                            //Określenie nazwę grupy do której ma należeć linia
                            if (myHourTime.Length < 2)
                            {
                                tmp_NazwaGrupy = "0" + myHourTime + ":00-" + "0" + myHourTime + ":59";
                            }
                            else
                            {
                                tmp_NazwaGrupy = myHourTime + ":00-" + myHourTime + ":59";
                            }
                            _groupName = tmp_NazwaGrupy;
                                    ListViewItem.Text = tmp_Value;
                            _listViewGroup = new ListViewGroup(_groupName, HorizontalAlignment.Left);
                                    ListViewItem.Group = _listViewGroup;
                                    ListViewItem.Group.Name = _groupName; /**/
                                    ListViewItem.Group.Header = _groupName;
                            Des.HeaderText = tmp_Value;
                            break;
                        }
                                ListViewItem.SubItems.Add(tmp_Value);
                        if (Regex.IsMatch(tmp_Value, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b został uruchomiony.") || Regex.IsMatch(tmp_Value, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b started"))
                        {
                                    ListViewItem.Group.Header = ListViewItem.Group.Name + " (" + tmp_Value + ")"; /**/
                        }
                        else
                        {
                            foreach (Descriptor des in ListaBledow)
                            {
                                if (tmp_Value.Contains(des.KeyName))
                                {
                                    if (!ListViewItem.Group.Header.Contains(des.HeaderText))
                                    {
                                                ListViewItem.Group.Header += " ( " + des.HeaderText + " )";
                                    }
                                }
                            }
                        }
                        Des.HeaderText = tmp_Value;
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

    