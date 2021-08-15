using System;
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

        private LogType _typeOfLog;

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
            string[] choose_Headers;
            string allData=null;
            StreamReader plikFirstAnalize;
            StreamReader plikAnalize;

            try
            {
              plikFirstAnalize = new StreamReader(SafeFileName, Encoding.Default);
              plikAnalize = new StreamReader(SafeFileName, Encoding.Default);
              allData = plikAnalize.ReadToEnd();
              plikAnalize.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }

            var listOfHeaders = new List<string>();

            int numer = 0;

            if(allData.Contains(logHeader.GetStatlookTextHeaders()[1]))
            {
                foreach (var item in logHeader.GetStatlookTextHeaders())
                {
                    listOfHeaders.Add(item);
                    numer = 1;
                    _typeOfLog = (int)LogType.Statlook;
                }


            }
            else if (allData.Contains(logHeader.GetUsmTextHeaders()[1]))
            {
                foreach (var item2 in logHeader.GetUsmTextHeaders())
                {
                    listOfHeaders.Add(item2);
                    numer = 2;
                    _typeOfLog = LogType.Usm;
                }

            }

             choose_Headers = listOfHeaders.ToArray();

            _newPage = new NewPage(0, FileName, SafeFileName, choose_Headers, dataUtworzenia, _typeOfLog)
            {
                TypeOfReport = _typeOfLog.ToString()
            };

            ListViewExtended ListViewTmp = _newPage.ListViewExtended;
             
             StreamReader plikAnalize_Sec = new StreamReader(SafeFileName, UTF8Encoding.Default);

            //Utworzenie obiektu przechowującego zbiór linii przetworzonych pliku logu 
            PlikLogu PlikLogu = new PlikLogu();

            LogLine logLine = new LogLine();

            string line;

            #region pętla

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
                    logLine.AddLine(logLine.Headers.uplook_Date, line, numer);
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
                else if (!line.Contains(uplookDeskryptor.uplook_Break))
                {
                    for (int i = 1; i < choose_Headers.Length; i++)
                    {
                        if (line.StartsWith(choose_Headers[i]))
                        {
                            line += ";";
                            line = line.Remove(0, choose_Headers[i].Length);
                            line = line.TrimStart();
                            line = line.Substring(0, line.IndexOf(";"));
                            logLine.AddLine(choose_Headers[i], line, numer);
                            break;
                        }
                    }
                }
                else if (line.StartsWith(uplookDeskryptor.uplook_Break))
                {

                    //Wykonaj jeśli linia zawiera znacznika przerwy 
                    //Dodanie pojedynczej linii do pliku wynikowego analizy 
                    PlikLogu.AddLine(logLine);
                }

            }

            #endregion pętla

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
