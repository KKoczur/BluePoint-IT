using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UplookLogViewer
{
    public class Deskryptor
    {
        #region Zmienne

        //Nazwa obiektu deksryptora
        private string m_Name;
        //Wartość obiektu deskryptora
        private string m_Value;
        //Widocznosc
        private bool m_Show;

        #endregion Zmienne
        
        #region Konstruktory

        public Deskryptor()
        {

        }

        public Deskryptor(string tmp_Name)
        {
            m_Name = tmp_Name;
        }

        public Deskryptor(string tmp_Name, string tmp_Value)
        {
            m_Name = tmp_Name;
            m_Value = tmp_Value;
        }

        public Deskryptor(string tmp_Name, string tmp_Value,bool tmp_Show)
        {
            m_Name = tmp_Name;
            m_Value = tmp_Value;
            m_Show = tmp_Show;
        }

        #endregion Konstruktory

        #region Wlasciwosci

        public string Name
        {
            set{m_Name = value;}
            get{return m_Name;}
        }
        public string Value
        {
            set{m_Value = value;}
            get{return m_Value;}
        }
        public bool Show
        {
            set { m_Show = value; }
            get { return m_Show; }
        }
        #endregion Wlasciwosci

    }
}
