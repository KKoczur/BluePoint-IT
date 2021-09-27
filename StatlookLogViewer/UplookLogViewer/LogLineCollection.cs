using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using StatlookLogViewer.Views;
using StatlookLogViewer.Model;
using System.Runtime.InteropServices.WindowsRuntime;

namespace StatlookLogViewer
{
    internal class LogLineCollection
    {
        #region Members

        private readonly List<LogLine> _logLineCollection = new List<LogLine>();

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

        public LogTapPage AnalyzeLogFile(string fileFullName)
        {
            string[] logTextPatterns = null;

            LogType logType = DetectLogType(fileFullName, ref logTextPatterns);

            LogTapPage newTabPage = CreateNewTabPage(fileFullName, logType);

            ListViewExtended listViewExtended = newTabPage.ListViewExtended;

            LogLine logLine = new LogLine();

            foreach (string singleLine in File.ReadAllLines(fileFullName))
            {
                //Wyrażenie regularne do sprawdzenia czy wpis logu nie zaczyna się od daty
                if (Regex.IsMatch(singleLine, @"(?<rok>\d{4})\.(?<miesiac>\d{2})\.(?<dzien>\d{2})\b"))
                {
                    logLine = new LogLine();
                    string normalizeSingleLine = singleLine + ";";
                    normalizeSingleLine = normalizeSingleLine.Substring(0, normalizeSingleLine.IndexOf(";"));

                    // Dodanie do pojedynczej linii wartości kolumny: Date
                    logLine.AddLine(Configuration.STATLOOK_DATE, normalizeSingleLine, logType);

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
                else if (!singleLine.Contains(Configuration.STATLOOK_BREAK))
                {
                    for (int i = 1; i < logTextPatterns.Length; i++)
                    {
                        if (singleLine.StartsWith(logTextPatterns[i]))
                        {
                            string normalizeSingleLine = singleLine + ";";
                            normalizeSingleLine = normalizeSingleLine.Remove(0, logTextPatterns[i].Length);
                            normalizeSingleLine = normalizeSingleLine.TrimStart();
                            normalizeSingleLine = normalizeSingleLine.Substring(0, normalizeSingleLine.IndexOf(";"));
                            logLine.AddLine(logTextPatterns[i], normalizeSingleLine, logType);
                            break;
                        }
                    }
                }
                else if (singleLine.StartsWith(Configuration.STATLOOK_BREAK))
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
            _listViewItem.Add(logLine.ListViewItem);
        }

        private LogType DetectLogType(string fileFullName, ref string[] logTextPatterns)
        {
            string allFileData = ReadAllFileText(fileFullName);

            LogType logType = LogType.Default;

            if (allFileData.Contains(_config.GetStatlookTextPatterns()[1]))
            {
                logType = (int)LogType.Statlook;
                logTextPatterns = _config.GetStatlookTextPatterns().Split(new char[] { ';' });
            }
            else if (allFileData.Contains(_config.GetUsmTextPatterns()[1]))
            {
                logType = LogType.Usm;
                logTextPatterns = _config.GetUsmTextPatterns().Split(new char[] { ';' });
            }

            return logType;
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
        /// Get file text content
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>File text content</returns>
        private string ReadAllFileText(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + exception.Message);
                return string.Empty;
            }
        }

        #endregion Methods
    }
}
