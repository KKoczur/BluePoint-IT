using StatlookLogViewer.Parser;
using StatlookLogViewer.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StatlookLogViewer
{
    internal static class LogAnalyzer
    {
        #region Methods

        public static LogTapPage GetLogTapePage(string filePath)
        {

            var (groups, listViewItems, logParser) = AnalyzeLogFile(filePath);

            return new LogTapPage(filePath, logParser, groups, listViewItems);

        }

        private static (List<ListViewGroup> groups, List<ListViewItem> listViewItems, ILogParser logParser) AnalyzeLogFile(string filePath)
        {
            var (logParser, allFileLines) = LogParserTools.DetectLogParser(filePath);

            List<ListViewItem> listViewItemCollection = new();

            ListViewItem currentListViewItem = new();

            List<ListViewGroup> groups = new();

            foreach (var line in allFileLines)
            {
                //Wyrażenie regularne do sprawdzenia czy wpis logu nie zaczyna się od daty
                if (DateTime.TryParse(line, out _))
                {
                    currentListViewItem = new ListViewItem();

                    var normalizeLine = line + ";";
                    normalizeLine = normalizeLine[..normalizeLine.IndexOf(";", StringComparison.Ordinal)];

                    // Dodanie do pojedynczej linii wartości kolumny: Date
                    AnalyzeLine(currentListViewItem, logParser.StartLogGroupEntry, normalizeLine, logParser);

                    var listViewGroup = currentListViewItem.Group;

                    if (groups.Count == 0)
                    {
                        groups.Add(listViewGroup);
                    }
                    else
                    {
                        var lastListViewGroup = groups.Last();

                        if (string.CompareOrdinal(lastListViewGroup.Header, listViewGroup.Header) == 0)
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
                else if (line != null && !line.Contains(logParser.EndLogGroupEntry))
                {
                    foreach (var textPattern in logParser.GetTextPatterns())
                    {
                        if (!line.StartsWith(textPattern)) continue;

                        var normalizeSingleLine = line + ";";
                        normalizeSingleLine = normalizeSingleLine.Remove(0, textPattern.Length);
                        normalizeSingleLine = normalizeSingleLine.TrimStart();
                        normalizeSingleLine = normalizeSingleLine[..normalizeSingleLine.IndexOf(";", StringComparison.Ordinal)];
                        AnalyzeLine(currentListViewItem, textPattern, normalizeSingleLine, logParser);
                        break;
                    }
                }
                else if (line != null && line.StartsWith(logParser.EndLogGroupEntry))
                {
                    //Wykonaj jeśli linia zawiera znacznika nowej grupy 
                    //Dodanie pojedynczej linii do pliku wynikowego analizy 
                    listViewItemCollection.Add(currentListViewItem);
                }
            }

            return (groups, listViewItemCollection, logParser);
        }

        private static void AnalyzeLine(ListViewItem listViewItem, string lineCaption, string lineValue, ILogParser logParser)
        {
            foreach (var logPattern in logParser.GetLogPatterns())
            {
                if (string.Compare(logPattern.TextPattern, lineCaption, StringComparison.OrdinalIgnoreCase) != 0)
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
                        foreach (var logErrorPattern in logParser.GetListOfErrors())
                        {
                            if (lineValue.Contains(logErrorPattern.ErrorTextPattern) && !listViewItem.Group.Header.Contains(logErrorPattern.ErrorReason))
                            {
                                listViewItem.Group.Header += " ( " + logErrorPattern.ErrorReason + " )";
                            }
                        }
                    }


                    break;
                }

                _ = DateTime.TryParse(lineValue, out var dateTime);

                listViewItem.Text = lineValue;

                listViewItem.Group = new ListViewGroup(GetNameOfGroupByHourTime(dateTime), HorizontalAlignment.Left)
                {
                    CollapsedState = ListViewGroupCollapsedState.Collapsed
                };

                break;
            }
        }

        private static string GetNameOfGroupByHourTime(DateTime dateTime)
        {
            var hourTime = dateTime.Hour.ToString();
            var hourPart = hourTime.Length < 2 ? $"0{hourTime}:00-0{hourTime}:59" : $"{hourTime}:00-{hourTime}:59";

            return $"{dateTime.ToShortDateString()} ({hourPart})";
        }

        #endregion Methods
    }
}
