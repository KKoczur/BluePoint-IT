using StatlookLogViewer.Model.Pattern;
using StatlookLogViewer.Parser;
using StatlookLogViewer.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StatlookLogViewer
{
    internal class LogLineCollection
    {
        #region Constructors

        public LogLineCollection()
        {
        }

        #endregion Constructors

        #region Methods

        public LogTapPage GetLogTapePage(string filePath)
        {

            (List<ListViewGroup> groups, List<ListViewItem> listViewItems, ILogParser logParser) = AnalyzeLogFile(filePath);

            return new LogTapPage(filePath, logParser, groups, listViewItems);

        }

        public (List<ListViewGroup> groups, List<ListViewItem> listViewItems, ILogParser logParser) AnalyzeLogFile(string filePath)
        {
            (ILogParser, string[]) logParserDetectorResult = LogParserTools.DetectLogParser(filePath);

            ILogParser logParser = logParserDetectorResult.Item1;

            string[] allFileLines = logParserDetectorResult.Item2;

            List<ListViewItem> listViewItemCollection = new();

            ListViewItem currentListViewItem = new();

            List<ListViewGroup> groups = new();

            foreach (string line in allFileLines)
            {
                //Wyrażenie regularne do sprawdzenia czy wpis logu nie zaczyna się od daty
                if (Regex.IsMatch(line, @"(?<rok>\d{4})\.(?<miesiac>\d{2})\.(?<dzien>\d{2})\b"))
                {
                    currentListViewItem = new ListViewItem();

                    string normalizeLine = line + ";";
                    normalizeLine = normalizeLine[..normalizeLine.IndexOf(";")];

                    // Dodanie do pojedynczej linii wartości kolumny: Date
                    AnalyzeLine(currentListViewItem, logParser.StartLogGroupEntry, normalizeLine, logParser);

                    ListViewGroup listViewGroup = currentListViewItem.Group;

                    if (groups.Count == 0)
                    {
                        groups.Add(listViewGroup);
                    }
                    else
                    {
                        ListViewGroup lastListViewGroup = groups.Last();

                        if (string.Compare(lastListViewGroup.Header, listViewGroup.Header) == 0)
                        {
                            currentListViewItem.Group = lastListViewGroup;
                        }
                        else
                        {
                            groups.Add(listViewGroup);
                        }
                    }
                }

                //Wykonaj jeśli linia nie zawiera znacznika przerwy 
                else if (!line.Contains(logParser.EndLogGroupEntry))
                {
                    foreach (string textPattern in logParser.GetTextPatterns())
                    {
                        if (line.StartsWith(textPattern))
                        {
                            string normalizeSingleLine = line + ";";
                            normalizeSingleLine = normalizeSingleLine.Remove(0, textPattern.Length);
                            normalizeSingleLine = normalizeSingleLine.TrimStart();
                            normalizeSingleLine = normalizeSingleLine[..normalizeSingleLine.IndexOf(";")];
                            AnalyzeLine(currentListViewItem, textPattern, normalizeSingleLine, logParser);
                            break;
                        }
                    }
                }
                else if (line.StartsWith(logParser.EndLogGroupEntry))
                {
                    //Wykonaj jeśli linia zawiera znacznika nowej grupy 
                    //Dodanie pojedynczej linii do pliku wynikowego analizy 
                    listViewItemCollection.Add(currentListViewItem);
                }
            }

            return (groups, listViewItemCollection, logParser);
        }

        static void AnalyzeLine(ListViewItem listViewItem, string lineCaption, string lineValue, ILogParser logParser)
        {
            foreach (LogPattern logPattern in logParser.GetLogPatterns())
            {
                if (string.Compare(logPattern.TextPattern, lineCaption, true) != 0)
                    continue;

                if (lineCaption != logParser.StartLogGroupEntry)
                {
                    listViewItem.SubItems.Add(lineValue);

                    if (Regex.IsMatch(lineValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b został uruchomiony.") || Regex.IsMatch(lineValue, @"(?<NR_1>\d{1})\.(?<NR_2>\d{1})\.(?<NR_3>\d{1})\b started"))
                    {
                        listViewItem.Group.Header = $"{listViewItem.Group.Name} ({lineValue})";
                    }
                    else
                    {
                        foreach (LogErrorPattern logErrorPattern in logParser.GetListOfErrors())
                        {
                            if (lineValue.Contains(logErrorPattern.ErrorTextPattern) && !listViewItem.Group.Header.Contains(logErrorPattern.ErrorReason))
                            {
                                listViewItem.Group.Header += " ( " + logErrorPattern.ErrorReason + " )";
                            }
                        }
                    }


                    break;
                }

                _ = DateTime.TryParse(lineValue, out DateTime dateTime);

                listViewItem.Text = lineValue;

                listViewItem.Group = new ListViewGroup(GetNameOfGroupByHourTime(dateTime), HorizontalAlignment.Left)
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
