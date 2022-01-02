using StatlookLogViewer.Parser;
using StatlookLogViewer.Views;
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

            LogListViewItem logListViewItem = new();

            List<ListViewGroup> groups = new();

            foreach (string line in allFileLines)
            {
                //Wyrażenie regularne do sprawdzenia czy wpis logu nie zaczyna się od daty
                if (Regex.IsMatch(line, @"(?<rok>\d{4})\.(?<miesiac>\d{2})\.(?<dzien>\d{2})\b"))
                {
                    logListViewItem = new LogListViewItem();

                    string normalizeLine = line + ";";
                    normalizeLine = normalizeLine[..normalizeLine.IndexOf(";")];

                    // Dodanie do pojedynczej linii wartości kolumny: Date
                    logListViewItem.AnalyzeLine(logParser.StartLogGroupEntry, normalizeLine, logParser);

                    ListViewGroup listViewGroup = logListViewItem.Group;

                    if (groups.Count == 0)
                    {
                        groups.Add(listViewGroup);
                    }
                    else
                    {
                        ListViewGroup lastListViewGroup = groups.Last();

                        if (string.Compare(lastListViewGroup.Header, listViewGroup.Header) == 0)
                        {
                            logListViewItem.Group = lastListViewGroup;
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
                            logListViewItem.AnalyzeLine(textPattern, normalizeSingleLine, logParser);
                            break;
                        }
                    }
                }
                else if (line.StartsWith(logParser.EndLogGroupEntry))
                {
                    //Wykonaj jeśli linia zawiera znacznika nowej grupy 
                    //Dodanie pojedynczej linii do pliku wynikowego analizy 
                    listViewItemCollection.Add(logListViewItem);
                }
            }

            return (groups, listViewItemCollection, logParser);
        }

        #endregion Methods
    }
}
