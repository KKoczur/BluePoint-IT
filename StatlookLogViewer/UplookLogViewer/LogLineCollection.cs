using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using StatlookLogViewer.Views;
using StatlookLogViewer.Model;
using System.Runtime.InteropServices.WindowsRuntime;
using StatlookLogViewer.Parser;
using StatlookkLogViewer.Tools;

namespace StatlookLogViewer
{
    internal class LogLineCollection
    {
        #region Members

        private readonly List<ListViewItem> _listViewItem = new();

        #endregion Members

        #region Constructors

        public LogLineCollection()
        {
        }

        #endregion Constructors

        #region Methods

        public LogTapPage AnalyzeLogFile(string filePath)
        {
            (ILogParser, string[]) logParserDetectorResult = DetectLogParser(filePath);

            ILogParser logParser = logParserDetectorResult.Item1;

            LogListViewItem logListViewItem = new();

            List<ListViewGroup> group = new();

            foreach (string line in logParserDetectorResult.Item2)
            {
                //Wyrażenie regularne do sprawdzenia czy wpis logu nie zaczyna się od daty
                if (Regex.IsMatch(line, @"(?<rok>\d{4})\.(?<miesiac>\d{2})\.(?<dzien>\d{2})\b"))
                {
                    logListViewItem = new LogListViewItem();

                    string normalizeLine = line + ";";
                    normalizeLine = normalizeLine.Substring(0, normalizeLine.IndexOf(";"));

                    // Dodanie do pojedynczej linii wartości kolumny: Date
                    logListViewItem.AnalyzeLine(logParser.StartLogGroupEntry, normalizeLine, logParser);

                    ListViewGroup listViewGroup = logListViewItem.Group;

                    if (group.Count == 0)
                    {
                        group.Add(listViewGroup);
                    }
                    else
                    {
                        ListViewGroup lastListViewGroup = group.Last();

                        if (string.Compare(lastListViewGroup.Header, listViewGroup.Header) == 0)
                        {
                            logListViewItem.Group = lastListViewGroup;
                        }
                        else
                        {
                            group.Add(listViewGroup);
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
                            normalizeSingleLine = normalizeSingleLine.Substring(0, normalizeSingleLine.IndexOf(";"));
                            logListViewItem.AnalyzeLine(textPattern, normalizeSingleLine, logParser);
                            break;
                        }
                    }
                }
                else if (line.StartsWith(logParser.EndLogGroupEntry))
                {
                    //Wykonaj jeśli linia zawiera znacznika przerwy 
                    //Dodanie pojedynczej linii do pliku wynikowego analizy 
                    AddLogListViewItem(logListViewItem);
                }
            }

            LogTapPage newTabPage = CreateNewTabPage(filePath, logParser);

            newTabPage.SetListViewGroups(group);

            ListViewItem[] listViewItemCollection = GetListViewItem();

            newTabPage.SetListViewItems(listViewItemCollection);

            return newTabPage;
        }

        private void AddLogListViewItem(LogListViewItem logListViewItem)
        {
            if (logListViewItem != null)
                _listViewItem.Add(logListViewItem);
        }

        private static (ILogParser,string[]) DetectLogParser(string filePath)
        {
           string[] allFileLines= IOTools.ReadAllLines(filePath);

            ILogParser logParser = null;

            foreach (KeyValuePair<string, ILogParser> kvp in GetLogParserMap())
            {
                foreach (string line in allFileLines)
                {
                    if (line.Contains(kvp.Key))
                    {
                        logParser = kvp.Value;
                        return (logParser, allFileLines);
                    }
                }
            }

            return (logParser, allFileLines);
           
        }

        private static Dictionary<string, ILogParser> GetLogParserMap()
        {
            var statlookLogParser = new StatlookLogParser();
            var usmLogParser = new UsmLogParser();

            var logParserMap = new Dictionary<string, ILogParser>
            {
                { statlookLogParser.UniqueLogKey, statlookLogParser },
                { usmLogParser.UniqueLogKey, usmLogParser }
            };
            return logParserMap;
        }

        private static LogTapPage CreateNewTabPage(string filePath, ILogParser logParser)
        {
            return new LogTapPage(0, filePath, logParser)
            {
                LogParser = logParser
            };
        }

        private ListViewItem[] GetListViewItem() => _listViewItem.ToArray();



        #endregion Methods
    }
}
