using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using ListViewGroupCollapse;
using System.Linq;

namespace StatlookLogViewer
{
    internal class LogLineCollection
    {
        #region Members

        private readonly List<LogLine> _logLineCollection = new List<LogLine>();

        private readonly List<string> _logLineGroupNameCollection = new List<string>();

        private readonly List<ListViewItem> _listViewItem = new List<ListViewItem>();

        private readonly Configuration _config;

        #endregion Members

        #region Constructors

        public LogLineCollection()
        {
            _config = Configuration.GetConfiguration();
        }

        #endregion Constructors

        #region Methods

        public LogTapPage LogAnalyze(string fileNameWithPath)
        {
            LogType logType = LogType.Default;

            string[] listOfHeaders = null;

            string allFileData = GetFileContent(fileNameWithPath);

            DetectLogType(allFileData, ref listOfHeaders, ref logType);

            LogTapPage newTabPage = CreateNewTabPage(fileNameWithPath, logType);

            ListViewExtended listViewExtended = newTabPage.ListViewExtended;

            StreamReader streamReader = new StreamReader(fileNameWithPath, Encoding.Default);

            LogLine logLine = new LogLine();

            string line;

            while ((line = streamReader.ReadLine()) != null)
            {
                //Wyrażenie regularne do sprawdzenia czy wpis logu nie zaczyna się od daty
                if (Regex.IsMatch(line, @"(?<rok>\d{4})\.(?<miesiac>\d{2})\.(?<dzien>\d{2})\b"))
                {
                    logLine = new LogLine();
                    line += ";";
                    line = line.Substring(0, line.IndexOf(";"));

                    //Dodanie do pojedynczej linii wartości kolumny: Date
                    logLine.AddLine(Configuration.STATLOOK_DATE, line, logType);

                    DateTime tmp = DateTime.Parse(line);

                    string MyHourTime = tmp.Hour.ToString();

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
                else if (!line.Contains(Configuration.STATLOOK_BREAK))
                {
                    for (int i = 1; i < listOfHeaders.Length; i++)
                    {
                        if (line.StartsWith(listOfHeaders[i]))
                        {
                            line += ";";
                            line = line.Remove(0, listOfHeaders[i].Length);
                            line = line.TrimStart();
                            line = line.Substring(0, line.IndexOf(";"));
                            logLine.AddLine(listOfHeaders[i], line, logType);
                            break;
                        }
                    }
                }
                else if (line.StartsWith(Configuration.STATLOOK_BREAK))
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

        private void AddLine(LogLine logLine)
        {
            _logLineCollection.Add(logLine);
            _logLineGroupNameCollection.Add(logLine.GroupName);
            _listViewItem.Add(logLine.ListViewItem);
        }

        private void DetectLogType(string allFileData, ref string[] listOfHeaders, ref LogType logType)
        {
            if (allFileData.Contains(_config.GetStatlookTextHeaders()[1]))
            {
                logType = (int)LogType.Statlook;
                listOfHeaders = _config.GetStatlookTextHeaders().Split(new char[] { ';' });
            }
            else if (allFileData.Contains(_config.GetUsmTextHeaders()[1]))
            {
                logType = LogType.Usm;
                listOfHeaders = _config.GetUsmTextHeaders().Split(new char[] { ';' });
            }
        }

        private LogTapPage CreateNewTabPage(string fileNameWithPath, LogType logType)
        {
            return new LogTapPage(0, fileNameWithPath, logType)
            {
                LogType = logType
            };
        }

        private ListViewItem[] GetListViewItem() => _listViewItem.ToArray();

        /// <summary>
        /// Get file content
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>File text content</returns>
        private string GetFileContent(string filePath)
        {
            string fileContent = null;

            try
            {
                using var streamReader = new StreamReader(filePath, Encoding.Default);

                fileContent = streamReader.ReadToEnd();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + exception.Message);
            }

            return fileContent;
        }

        #endregion Methods
    }
}
