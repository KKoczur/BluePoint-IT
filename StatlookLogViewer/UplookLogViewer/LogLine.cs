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

        public void AddLine(string rowCaption, string rowValue, LogType logType)
        {
            switch (logType)
            {
                case LogType.Statlook:
                    {
                        foreach (Descriptor descriptor in Headers.GetStatlookDescriptors())
                        {
                            if (string.Compare(descriptor.RowCaption, rowCaption, true) != 0)
                            {
                                continue;
                            }

                            if (rowCaption != Headers.StatlookHeaderDate)
                            {
                                /*if (tmp_Value.Contains("Error"))
                                                            {
                                                                m_ListViewItem.UseItemStyleForSubItems = false;
                                                                m_ListViewItem.SubItems.Add(tmp_Value, System.Drawing.Color.Red, System.Drawing.Color.White, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238))));
                                                            }*/
                                ListViewItem.SubItems.Add(rowValue);

                                if (Regex.IsMatch(rowValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b został uruchomiony.") || Regex.IsMatch(rowValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b started"))
                                {
                                    ListViewItem.Group.Header = $"{ListViewItem.Group.Name} ({rowValue})";
                                }
                                else
                                {
                                    foreach (Descriptor des in _listOfAllErrors)
                                    {
                                        if (rowValue.Contains(des.KeyName))
                                        {
                                            if (!ListViewItem.Group.Header.Contains(des.RowCaption))
                                            {
                                                ListViewItem.Group.Header += " ( " + des.RowCaption + " )";
                                            }
                                        }
                                    }
                                }

                                descriptor.RowCaption = rowValue;

                                break;
                            }

                            DateTime tmp_DateTime = DateTime.Parse(rowValue);
                            string myHourTime = tmp_DateTime.Hour.ToString();
                            string tmp_NazwaGrupy;

                            //Określenie nazwę grupy do której ma należeć linia
                            if (myHourTime.Length < 2)
                            {
                                tmp_NazwaGrupy = $"0{myHourTime}:00-0{myHourTime}:59";
                            }
                            else
                            {
                                tmp_NazwaGrupy = $"{myHourTime}:00-{myHourTime}:59";
                            }

                            _groupName = tmp_NazwaGrupy;
                            ListViewItem.Text = rowValue;
                            _listViewGroup = new ListViewGroup(_groupName, HorizontalAlignment.Left);
                            ListViewItem.Group = _listViewGroup;
                            ListViewItem.Group.Name = _groupName;
                            ListViewItem.Group.Header = _groupName;
                            descriptor.RowCaption = rowValue;
                            break;
                        }

                        break;
                    }

                case LogType.Usm:
                    {
                        foreach (Descriptor Des in Headers.GetUsmDescriptors())
                        {
                            if (Des.RowCaption != rowCaption)
                            {
                                continue;
                            }

                            if (rowCaption != Headers.StatlookHeaderDate)
                            {
                                ListViewItem.SubItems.Add(rowValue);

                                if (Regex.IsMatch(rowValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b został uruchomiony.") || Regex.IsMatch(rowValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b started"))
                                {
                                    ListViewItem.Group.Header = ListViewItem.Group.Name + " (" + rowValue + ")"; /**/
                                }
                                else
                                {
                                    foreach (Descriptor des in _listOfAllErrors)
                                    {
                                        if (rowValue.Contains(des.KeyName))
                                        {
                                            if (!ListViewItem.Group.Header.Contains(des.RowCaption))
                                            {
                                                ListViewItem.Group.Header += " ( " + des.RowCaption + " )";
                                            }
                                        }
                                    }
                                }

                                Des.RowCaption = rowValue;
                                break;
                            }

                            DateTime tmp_DateTime = DateTime.Parse(rowValue);

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
                            ListViewItem.Text = rowValue;
                            _listViewGroup = new ListViewGroup(_groupName, HorizontalAlignment.Left);
                            ListViewItem.Group = _listViewGroup;
                            ListViewItem.Group.Name = _groupName; /**/
                            ListViewItem.Group.Header = _groupName;
                            Des.RowCaption = rowValue;
                            break;
                        }
                        break;
                    }

                default:
                    break;
            }
        }


        #endregion Methods
    }
}

