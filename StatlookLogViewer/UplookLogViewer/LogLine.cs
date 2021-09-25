using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using StatlookLogViewer.Model;
using StatlookLogViewer.Model.Pattern;

namespace StatlookLogViewer
{
    internal class LogLine
    {
        #region Members

        private string _groupName;

        private ListViewGroup _listViewGroup = new ListViewGroup();

        private readonly LogErrorPattern[] _listOfAllErrors = (new ErrorCollection()).GetListOfAllErrors();

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
                        foreach (StatlookLogPattern descriptor in Headers.GetStatlookDescriptors())
                        {
                            if (string.Compare(descriptor.TextPattern, rowCaption, true) != 0)
                                continue;

                            if (rowCaption != Configuration.STATLOOK_DATE)
                            {
                                ListViewItem.SubItems.Add(rowValue);

                                if (Regex.IsMatch(rowValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b został uruchomiony.") || Regex.IsMatch(rowValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b started"))
                                {
                                    ListViewItem.Group.Header = $"{ListViewItem.Group.Name} ({rowValue})";
                                }
                                else
                                {
                                    foreach (LogErrorPattern logErrorPattern in _listOfAllErrors)
                                    {
                                        if (rowValue.Contains(logErrorPattern.ErrorTextPattern))
                                        {
                                            if (!ListViewItem.Group.Header.Contains(logErrorPattern.ErrorReason))
                                            {
                                                ListViewItem.Group.Header += " ( " + logErrorPattern.ErrorReason + " )";
                                            }
                                        }
                                    }
                                }

                                descriptor.TextPattern = rowValue;

                                break;
                            }

                            DateTime tmp_DateTime = DateTime.Parse(rowValue);
                            string myHourTime = tmp_DateTime.Hour.ToString();

                            _groupName = GetNameOfGroupByHourTime(myHourTime);
                            ListViewItem.Text = rowValue;
                            _listViewGroup = new ListViewGroup(_groupName, HorizontalAlignment.Left);
                            ListViewItem.Group = _listViewGroup;
                            ListViewItem.Group.Name = _groupName;
                            ListViewItem.Group.Header = _groupName;
                            descriptor.TextPattern = rowValue;
                            break;
                        }

                        break;
                    }

                case LogType.Usm:
                    {
                        foreach (StatlookLogPattern logPattern in Headers.GetUsmDescriptors())
                        {
                            if (logPattern.TextPattern != rowCaption)
                            {
                                continue;
                            }

                            if (rowCaption != Configuration.STATLOOK_DATE)
                            {
                                ListViewItem.SubItems.Add(rowValue);

                                if (Regex.IsMatch(rowValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b został uruchomiony.") || Regex.IsMatch(rowValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b started"))
                                {
                                    ListViewItem.Group.Header = ListViewItem.Group.Name + " (" + rowValue + ")"; /**/
                                }
                                else
                                {
                                    foreach (LogErrorPattern logErrorPattern in _listOfAllErrors)
                                    {
                                        if (rowValue.Contains(logErrorPattern.ErrorTextPattern))
                                        {
                                            if (!ListViewItem.Group.Header.Contains(logErrorPattern.ErrorReason))
                                            {
                                                ListViewItem.Group.Header += " ( " + logErrorPattern.ErrorReason + " )";
                                            }
                                        }
                                    }
                                }

                                logPattern.TextPattern = rowValue;
                                break;
                            }

                            DateTime tmp_DateTime = DateTime.Parse(rowValue);

                            string myHourTime = tmp_DateTime.Hour.ToString();

                            _groupName = GetNameOfGroupByHourTime(myHourTime);
                            ListViewItem.Text = rowValue;
                            _listViewGroup = new ListViewGroup(_groupName, HorizontalAlignment.Left);
                            ListViewItem.Group = _listViewGroup;
                            ListViewItem.Group.Name = _groupName; /**/
                            ListViewItem.Group.Header = _groupName;
                            logPattern.TextPattern = rowValue;
                            break;
                        }
                        break;
                    }

                default:
                    break;
            }
        }

        private string GetNameOfGroupByHourTime(string myHourTime)
        {
            return myHourTime.Length < 2 ? $"0{myHourTime}:00-0{myHourTime}:59" : $"{myHourTime}:00-{myHourTime}:59"; ;
        }


        #endregion Methods
    }
}

