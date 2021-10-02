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
            (ILogParser,string[]) logParserDetectorResult = DetectLogParser(filePath);

            ILogParser logParser = logParserDetectorResult.Item1;

            LogTapPage newTabPage = CreateNewTabPage(filePath, logParser);

            ListViewExtended listViewExtended = newTabPage.ListViewExtended;

            SingleLogLine logLine = new();

            List<ListViewGroup> group = new(); 

            foreach (string singleLine in logParserDetectorResult.Item2)
            {
                //Wyrażenie regularne do sprawdzenia czy wpis logu nie zaczyna się od daty
                if (Regex.IsMatch(singleLine, @"(?<rok>\d{4})\.(?<miesiac>\d{2})\.(?<dzien>\d{2})\b"))
                {
                    logLine = new SingleLogLine();
                    string normalizeSingleLine = singleLine + ";";
                    normalizeSingleLine = normalizeSingleLine.Substring(0, normalizeSingleLine.IndexOf(";"));

                    // Dodanie do pojedynczej linii wartości kolumny: Date
                    logLine.AddLine(logParser.StartLogGroupEntry, normalizeSingleLine, logParser);

                    ListViewGroup listViewGroup = new(logLine.GroupName, HorizontalAlignment.Left);

                    if (group.Count == 0)
                    {
                        group.Add(listViewGroup);
                        logLine.ListViewItem.Group = listViewGroup;
                    }
                    else
                    {
                        ListViewGroup lastListViewGroup = group.Last();

                        if (string.Compare(lastListViewGroup.Header, listViewGroup.Header)==0)
                        {
                            logLine.ListViewItem.Group = lastListViewGroup;
                        }
                        else
                        {
                            group.Add(listViewGroup);
                            logLine.ListViewItem.Group = listViewGroup;
                        }
                    }
                }

                //Wykonaj jeśli linia nie zawiera znacznika przerwy 
                else if (!singleLine.Contains(logParser.EndLogGroupEntry))
                {
                    foreach (string textPattern in (logParser as ILogParser).GetTextPatterns())
                    {
                        if (singleLine.StartsWith(textPattern))
                        {
                            string normalizeSingleLine = singleLine + ";";
                            normalizeSingleLine = normalizeSingleLine.Remove(0, textPattern.Length);
                            normalizeSingleLine = normalizeSingleLine.TrimStart();
                            normalizeSingleLine = normalizeSingleLine.Substring(0, normalizeSingleLine.IndexOf(";"));
                            logLine.AddLine(textPattern, normalizeSingleLine, logParser);
                            break;
                        }
                    }
                }
                else if (singleLine.StartsWith(logParser.EndLogGroupEntry))
                {
                    //Wykonaj jeśli linia zawiera znacznika przerwy 
                    //Dodanie pojedynczej linii do pliku wynikowego analizy 
                    AddSingleLine(logLine);
                }
            }

            listViewExtended.Groups.AddRange(group.ToArray());

            SetListViewItems(listViewExtended);

            return newTabPage;
        }

        private void SetListViewItems(ListViewExtended listViewExtended)
        {
            listViewExtended.BeginUpdate();
            listViewExtended.SuspendLayout();
            try
            {
                ListViewItem[] listViewItemCollection = GetListViewItem();

                listViewExtended.Items.AddRange(listViewItemCollection);
            }
            finally
            {
                listViewExtended.EndUpdate();
                listViewExtended.ResumeLayout();
            }
        }

        private void AddSingleLine(SingleLogLine logLine)
        {
            _listViewItem.Add(logLine.ListViewItem);
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
