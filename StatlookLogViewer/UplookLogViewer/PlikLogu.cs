﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using ListViewGroupCollapse;

namespace StatlookLogViewer
{
    internal class PlikLogu
    {
        #region Members

        private readonly ArrayList _logLineCollection = new ArrayList();

        private readonly ArrayList _logLineGroupNameCollection = new ArrayList();

        private readonly List <ListViewItem> _listViewItem = new List<ListViewItem>(); 

        private NewPage _newPage= new NewPage();

        #endregion Members

        #region Methods

        public void AddLine(LogLine logLine)
        {
            _logLineCollection.Add(logLine);
            _logLineGroupNameCollection.Add(logLine.GroupName);
            _listViewItem.Add(logLine.ListViewItem);
        }


        public ListViewItem[] GetListViewItem() => _listViewItem.ToArray();

        public NewPage analizeUplookLog(string SafeFileName, string FileName, string dataUtworzenia, LogHeader logHeader)
        {
            string allData = null;
            StreamReader streamReader;

            try
            {
              streamReader = new StreamReader(SafeFileName, Encoding.Default);
              allData = streamReader.ReadToEnd();
              streamReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }

            LogType logType = LogType.Default;

            string[] listOfHeaders = null;

            if(allData.Contains(logHeader.GetStatlookTextHeaders()[1]))
            {
                logType = (int)LogType.Statlook;
                listOfHeaders = logHeader.GetStatlookTextHeaders();               
            }
            else if (allData.Contains(logHeader.GetUsmTextHeaders()[1]))
            {
                logType = LogType.Usm;
                listOfHeaders = logHeader.GetUsmTextHeaders();
            }

            _newPage = new NewPage(0, FileName, SafeFileName, listOfHeaders, dataUtworzenia, logType)
            {
                LogType = logType
            };

            ListViewExtended ListViewTmp = _newPage.ListViewExtended;
             
             StreamReader plikAnalize_Sec = new StreamReader(SafeFileName, UTF8Encoding.Default);

            //Utworzenie obiektu przechowującego zbiór linii przetworzonych pliku logu 
            PlikLogu PlikLogu = new PlikLogu();

            LogLine logLine = new LogLine();

            string line;

            while ((line = plikAnalize_Sec.ReadLine()) != null)
            {
                //Wyrażenie regularne do sprawdzenia czy wpis logu nie zaczyna się od daty
                if (Regex.IsMatch(line, @"(?<rok>\d{4})\.(?<miesiac>\d{2})\.(?<dzien>\d{2})\b"))
                {
                    #region if_1
                    logLine = new LogLine();
                    line += ";";
                    line = line.Substring(0, line.IndexOf(";"));

                    //Dodanie do pojedynczej linii wartości kolumny: Date
                    logLine.AddLine(logLine.Headers.StatlookHeaderDate, line, logType);

                    DateTime tmp = DateTime.Parse(line);
                    string MyHourTime = tmp.Hour.ToString();

                    ListViewGroup tmp_Group = new ListViewGroup(logLine.GroupName, HorizontalAlignment.Left);
                    if (ListViewTmp.Groups.Count == 0)
                    {
                        ListViewTmp.Groups.Add(tmp_Group);
                        logLine.ListViewItem.Group = tmp_Group;
                        logLine.ListViewItem.Group.Name = tmp_Group.ToString(); /**/
                    }
                    else
                    {
                        if (ListViewTmp.Groups[ListViewTmp.Groups.Count - 1].Name.Equals(tmp_Group.ToString()))
                        {
                            logLine.ListViewItem.Group = ListViewTmp.Groups[ListViewTmp.Groups.Count - 1];
                            logLine.ListViewItem.Group.Name = ListViewTmp.Groups[ListViewTmp.Groups.Count - 1].Name; /**/
                        }
                        else
                        {
                            ListViewTmp.Groups.Add(tmp_Group);
                            logLine.ListViewItem.Group = tmp_Group;
                            logLine.ListViewItem.Group.Name = tmp_Group.ToString();
                        }
                    }

                    #endregion if_1
                }

                //Wykonaj jeśli linia nie zawiera znacznika przerwy 
                else if (!line.Contains(logHeader.StatlookHeaderBreak))
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
                else if (line.StartsWith(logHeader.StatlookHeaderBreak))
                {

                    //Wykonaj jeśli linia zawiera znacznika przerwy 
                    //Dodanie pojedynczej linii do pliku wynikowego analizy 
                    PlikLogu.AddLine(logLine);
                }

            }

            ListViewTmp.BeginUpdate();
            ListViewTmp.SuspendLayout();
            //dodanie całego zakresu danych 
            ListViewTmp.Items.AddRange(PlikLogu.GetListViewItem());
            ListViewTmp.EndUpdate();        
            ListViewTmp.ResumeLayout();

            return _newPage;
        }

        #endregion Methods
    }
}
