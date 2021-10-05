using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using StatlookLogViewer.Model.Pattern;
using StatlookLogViewer.Parser;

namespace StatlookLogViewer
{
    internal class SingleLogLine
    {
        #region Members

        private string _groupName;

        private ListViewGroup _listViewGroup = new();

        #endregion Members

        #region Properties

        public string GroupName => _groupName;

        public ListViewItem ListViewItem { get; } = new ListViewItem();

        #endregion Properties

        #region Methods

        public void AnalyzeLine(string lineCaption, string lineValue, ILogParser logParser)
        {
            foreach (LogPattern logPattern in logParser.GetLogPatterns())
            {
                if (string.Compare(logPattern.TextPattern, lineCaption, true) != 0)
                    continue;

                if (lineCaption != logParser.StartLogGroupEntry)
                {
                    ListViewItem.SubItems.Add(lineValue);

                    if (Regex.IsMatch(lineValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b został uruchomiony.") || Regex.IsMatch(lineValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b started"))
                    {
                        ListViewItem.Group.Header = $"{ListViewItem.Group.Name} ({lineValue})";
                    }
                    else
                    {
                        foreach (LogErrorPattern logErrorPattern in logParser.GetListOfErrors())
                        {
                            if (lineValue.Contains(logErrorPattern.ErrorTextPattern) && !ListViewItem.Group.Header.Contains(logErrorPattern.ErrorReason))
                            {
                                ListViewItem.Group.Header += " ( " + logErrorPattern.ErrorReason + " )";
                            }
                        }
                    }


                    break;
                }

                 DateTime.TryParse(lineValue,out DateTime dateTime);

                ListViewItem.Text = lineValue;
                _groupName = GetNameOfGroupByHourTime(dateTime);
                _listViewGroup = new ListViewGroup(_groupName, HorizontalAlignment.Left);
                ListViewItem.Group = _listViewGroup;
                ListViewItem.Group.Name = _groupName;
                ListViewItem.Group.Header = _groupName;
                break;
            }
        }

        private static string GetNameOfGroupByHourTime(DateTime dateTime)
        {
            string hourTime = dateTime.Hour.ToString();
            string hourPart = hourTime.Length < 2 ? $"0{hourTime}:00-0{hourTime}:59" : $"{hourTime}:00-{hourTime}:59";

            return $"{dateTime.ToShortDateString()} ({hourPart})";
        }


        #endregion Methods
    }
}

