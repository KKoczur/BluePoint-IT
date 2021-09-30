using System;
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

        private readonly List<SingleLogLine> _logLineCollection = new List<SingleLogLine>();

        private readonly List<ListViewItem> _listViewItem = new List<ListViewItem>();

        #endregion Members

        #region Constructors

        public LogLineCollection()
        {
        }

        #endregion Constructors

        #region Methods

        public LogTapPage AnalyzeLogFile(string fileFullName)
        {
            ILogParser logParser = DetectLogType(fileFullName);

            LogTapPage newTabPage = CreateNewTabPage(fileFullName, logParser);

            ListViewExtended listViewExtended = newTabPage.ListViewExtended;

            SingleLogLine logLine = new SingleLogLine();

            foreach (string singleLine in File.ReadAllLines(fileFullName))
            {
                //Wyrażenie regularne do sprawdzenia czy wpis logu nie zaczyna się od daty
                if (Regex.IsMatch(singleLine, @"(?<rok>\d{4})\.(?<miesiac>\d{2})\.(?<dzien>\d{2})\b"))
                {
                    logLine = new SingleLogLine();
                    string normalizeSingleLine = singleLine + ";";
                    normalizeSingleLine = normalizeSingleLine.Substring(0, normalizeSingleLine.IndexOf(";"));

                    // Dodanie do pojedynczej linii wartości kolumny: Date
                    logLine.AddLine(logParser.StartLogGroupEntry, normalizeSingleLine, logParser);

                    ListViewGroup tmp_Group = new ListViewGroup(logLine.GroupName, HorizontalAlignment.Left);

                    if (listViewExtended.Groups.Count == 0)
                    {
                        listViewExtended.Groups.Add(tmp_Group);
                        logLine.ListViewItem.Group = tmp_Group;
                        logLine.ListViewItem.Group.Name = tmp_Group.ToString();
                    }
                    else
                    {
                        if (listViewExtended.Groups[listViewExtended.Groups.Count - 1].Name.Equals(tmp_Group.ToString()))
                        {
                            logLine.ListViewItem.Group = listViewExtended.Groups[listViewExtended.Groups.Count - 1];
                            logLine.ListViewItem.Group.Name = listViewExtended.Groups[listViewExtended.Groups.Count - 1].Name;
                        }
                        else
                        {
                            listViewExtended.Groups.Add(tmp_Group);
                            logLine.ListViewItem.Group = tmp_Group;
                            logLine.ListViewItem.Group.Name = tmp_Group.ToString();
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
                    AddLine(logLine);
                }

            }

            listViewExtended.BeginUpdate();
            listViewExtended.SuspendLayout();
            try
            {

                //dodanie całego zakresu danych 
                listViewExtended.Items.AddRange(GetListViewItem());
            }
            finally
            {

                listViewExtended.EndUpdate();
                listViewExtended.ResumeLayout();
            }

            return newTabPage;
        }

        private void AddLine(SingleLogLine logLine)
        {
            _logLineCollection.Add(logLine);
            _listViewItem.Add(logLine.ListViewItem);
        }

        private ILogParser DetectLogType(string fileFullName)
        {
            var statlookLogParser = new StatlookLogParser();
            var usmLogParser = new UsmLogParser();

            var logParserMap = new Dictionary<string, ILogParser>
            {
                { statlookLogParser.UniqueLogKey, statlookLogParser },
                { usmLogParser.UniqueLogKey, usmLogParser }
            };

            string allFileData = IOTools.ReadAllFileText(fileFullName);

            ILogParser logParser = null;

            foreach (KeyValuePair<string, ILogParser> kvp in logParserMap)
            {
                if (allFileData.Contains(kvp.Key))
                {
                    logParser = kvp.Value;
                    break;
                }
            }

            return logParser;
        }

        private LogTapPage CreateNewTabPage(string fileNameWithPath, ILogParser logParser)
        {
            return new LogTapPage(0, fileNameWithPath, logParser)
            {
                LogParser = logParser
            };
        }

        private ListViewItem[] GetListViewItem() => _listViewItem.ToArray();



        #endregion Methods
    }
}
