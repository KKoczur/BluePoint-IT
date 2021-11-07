using StatlookLogViewer.Model.Pattern;
using StatlookLogViewer.Parser;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StatlookLogViewer
{
    internal class LogListViewItem : ListViewItem
    {
        #region Methods

        public void AnalyzeLine(string lineCaption, string lineValue, ILogParser logParser)
        {
            foreach (LogPattern logPattern in logParser.GetLogPatterns())
            {
                if (string.Compare(logPattern.TextPattern, lineCaption, true) != 0)
                    continue;

                if (lineCaption != logParser.StartLogGroupEntry)
                {
                    SubItems.Add(lineValue);

                    if (Regex.IsMatch(lineValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b został uruchomiony.") || Regex.IsMatch(lineValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b started"))
                    {
                        Group.Header = $"{Group.Name} ({lineValue})";
                    }
                    else
                    {
                        foreach (LogErrorPattern logErrorPattern in logParser.GetListOfErrors())
                        {
                            if (lineValue.Contains(logErrorPattern.ErrorTextPattern) && !Group.Header.Contains(logErrorPattern.ErrorReason))
                            {
                                Group.Header += " ( " + logErrorPattern.ErrorReason + " )";
                            }
                        }
                    }


                    break;
                }

                _ = DateTime.TryParse(lineValue, out DateTime dateTime);

                Text = lineValue;

                Group = new ListViewGroup(GetNameOfGroupByHourTime(dateTime), HorizontalAlignment.Left)
                {
                    CollapsedState = ListViewGroupCollapsedState.Collapsed
                };

                break;
            }
        }

        static string GetNameOfGroupByHourTime(DateTime dateTime)
        {
            string hourTime = dateTime.Hour.ToString();
            string hourPart = hourTime.Length < 2 ? $"0{hourTime}:00-0{hourTime}:59" : $"{hourTime}:00-{hourTime}:59";

            return $"{dateTime.ToShortDateString()} ({hourPart})";
        }


        #endregion Methods
    }
}

