using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using ListViewGroupCollapse;
using Yoramo.GuiLib;

namespace StatlookLogViewer
{
    class PlikLogu
    {
        #region Zmienne

        private ArrayList m_ZbiorLinii = new ArrayList();
        private ArrayList m_Grupy = new ArrayList();
        private Linia m_Linia = new Linia();
        private List <ListViewItem> m_ListViewItem = new List<ListViewItem>(); 
        private ArrayList m_ListViewGroup = new ArrayList();
        private string typRaportu;
        private LogType _typeOfLog;
        private NewPage nowaKarta= new NewPage();
        //private Headers uplookDeskryptor;

        #endregion Zmienne

        #region Konstruktory

        public PlikLogu()
        {
            //uplookDeskryptor = _uplookDeskryptor;
        }

        #endregion Konstruktory

        #region Metody

        public void DodajLinie(Linia tmp_Linia)
        {
            m_ZbiorLinii.Add(tmp_Linia);
            m_Grupy.Add(tmp_Linia.GroupName);
            m_ListViewItem.Add(tmp_Linia.ListViewItem);
        }

        public int IloscLinii()
        {
            return m_ZbiorLinii.Count;
        }

        public ListViewItem[] GetListViewItem()
        {
            return m_ListViewItem.ToArray();
        }

        public NewPage analizeUplookLog(string SafeFileName, string FileName, string dataUtworzenia, Headers _uplookDeskryptor)
        {
            Headers uplookDeskryptor = _uplookDeskryptor;
            string[] choose_Headers;
            string allData=null;
            StreamReader plikFirstAnalize;
            StreamReader plikAnalize;
            //NewPage nowaKarta;
            try
            {
              plikFirstAnalize = new StreamReader(SafeFileName, UTF8Encoding.Default);
              plikAnalize = new StreamReader(SafeFileName, UTF8Encoding.Default);
              allData = plikAnalize.ReadToEnd();
              plikAnalize.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }

            List<String> m_ListOfHeaders = new List<String>();
            int numer = 0;
            if(allData.Contains(uplookDeskryptor.uplook_Headers[1]))
            {
                for (int i = 0; i < uplookDeskryptor.uplook_Headers.Length; i++)
                {
                    m_ListOfHeaders.Add(uplookDeskryptor.uplook_Headers[i]);
                    numer = 1;
                    _typeOfLog=(int)LogType.Statlook;
                    typRaportu = LogType.Statlook.ToString();
                }

                         
            }
            else if (allData.Contains(uplookDeskryptor.usm_Headers[1]))
            {
                for (int i = 0; i < uplookDeskryptor.usm_Headers.Length; i++)
                {
                    m_ListOfHeaders.Add(uplookDeskryptor.usm_Headers[i]);
                    numer = 2;
                    _typeOfLog = LogType.Usm;
                    typRaportu = LogType.Usm.ToString();
                }

             }
             else
             {
             }
             choose_Headers = m_ListOfHeaders.ToArray();
            nowaKarta = new NewPage(0, FileName, SafeFileName, choose_Headers, dataUtworzenia, _typeOfLog)
            {
                TypeOfReport = typRaportu
            };

            ListViewExtended ListViewTmp = nowaKarta.ListViewExtended;
             
            //Otwarcie pliku po raz drugi do odczytu danych
             StreamReader plikFirstAnalize_Sec = new StreamReader(SafeFileName, UTF8Encoding.Default);
             StreamReader plikAnalize_Sec = new StreamReader(SafeFileName, UTF8Encoding.Default);
             string line = null;
             string MyHourTime = null;
            
            //Utworzenie obiektu przechowującego zbiór linii przetworzonych pliku logu 
            PlikLogu PlikLogu = new PlikLogu();
            Linia NowaLinia = new Linia();
            #region pętla
            while ((line = plikAnalize_Sec.ReadLine()) != null)
            {
                //Wyrażenie regularne do sprawdzenia czy wpis logu nie zaczyna się od daty
                if (Regex.IsMatch(line, @"(?<rok>\d{4})\.(?<miesiac>\d{2})\.(?<dzien>\d{2})\b"))
                {
                 #region if_1
                    NowaLinia = new Linia();
                    line = line + ";";
                    line = line.Substring(0, line.IndexOf(";"));
                    
                    //Dodanie do pojedynczej linii wartości kolumny: Date
                    NowaLinia.AddLine(NowaLinia.Headers.uplook_Date, line, numer);
                    DateTime tmp = DateTime.Parse(line);
                    MyHourTime = tmp.Hour.ToString();
                    ListViewGroup tmp_Group = new ListViewGroup(NowaLinia.GroupName, HorizontalAlignment.Left);
                    if (ListViewTmp.Groups.Count == 0)
                    {
                        ListViewTmp.Groups.Add(tmp_Group);
                        NowaLinia.ListViewItem.Group = tmp_Group;
                        NowaLinia.ListViewItem.Group.Name = tmp_Group.ToString(); /**/
                    }
                    else
                    {
                        if (ListViewTmp.Groups[ListViewTmp.Groups.Count - 1].Name.Equals(tmp_Group.ToString()))
                        {
                            NowaLinia.ListViewItem.Group = ListViewTmp.Groups[ListViewTmp.Groups.Count - 1];
                            NowaLinia.ListViewItem.Group.Name = ListViewTmp.Groups[ListViewTmp.Groups.Count - 1].Name; /**/
                        }
                        else
                        {
                            ListViewTmp.Groups.Add(tmp_Group);
                            NowaLinia.ListViewItem.Group = tmp_Group;
                            NowaLinia.ListViewItem.Group.Name = tmp_Group.ToString();
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
                            line = line + ";";
                            line = line.Remove(0, choose_Headers[i].Length);
                            line = line.TrimStart();
                            line = line.Substring(0, line.IndexOf(";"));
                            NowaLinia.AddLine(choose_Headers[i], line, numer);
                            break;
                         }
                     }
                }

                 //Wykonaj jeśli linia zawiera znacznika przerwy 
                 else if (line.StartsWith(uplookDeskryptor.uplook_Break))
                 {
                    //Dodanie pojedynczej linii do pliku wynikowego analizy 
                    PlikLogu.DodajLinie(NowaLinia);
                 }

            }
            
            #endregion pętla

            ListViewTmp.BeginUpdate();
            ListViewTmp.SuspendLayout();
            //dodanie całego zakresu danych 
            ListViewTmp.Items.AddRange(PlikLogu.GetListViewItem());
            ListViewTmp.EndUpdate();        
            ListViewTmp.ResumeLayout();
            return nowaKarta;
        }

        #endregion Metody
    }
}
