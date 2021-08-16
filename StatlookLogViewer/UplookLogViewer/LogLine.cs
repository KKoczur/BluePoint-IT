using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace StatlookLogViewer
{
    internal class LogLine
    {
        #region Members

        private string _groupName;

        private ListViewGroup _listViewGroup = new ListViewGroup();

        private readonly Descriptor[] _listOfAllErrors = (new DictionaryLog()).GetListOfAllErrors();

        #endregion Members

        #region Properties

        public LogHeader Headers { get; } = new LogHeader();

        public string GroupName => _groupName;

        public ListViewItem ListViewItem { get; } = new ListViewItem();

        #endregion Properties

        #region Methods

        public void AddLine(string header, string value, LogType logType)
        {
            string tmp_Header= header;
            string tmp_Value= value;

            switch (logType)
            {
                case LogType.Statlook:
                {
                    foreach (Descriptor Des in Headers.GetStatlookDescriptors())
                    {
                        if (Des.HeaderText == tmp_Header)
                        {
                            if (tmp_Header == Headers.StatlookHeaderDate)
                            {
                                DateTime tmp_DateTime = DateTime.Parse(tmp_Value);
                                    string myHourTime = tmp_DateTime.Hour.ToString();
                                    string tmp_NazwaGrupy;
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
                            foreach (Descriptor des in _listOfAllErrors)
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

                case LogType.Usm:
                {

                foreach (Descriptor Des in Headers.GetUsmDescriptors())
                {
                    if (Des.HeaderText == tmp_Header)
                    {
                        if (tmp_Header == Headers.StatlookHeaderDate)
                        {
                            DateTime tmp_DateTime = DateTime.Parse(tmp_Value);

                                    string myHourTime = tmp_DateTime.Hour.ToString();
                                    string tmp_NazwaGrupy;
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
                            foreach (Descriptor des in _listOfAllErrors)
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
            }
        }


        #endregion Methods
    }
}

    