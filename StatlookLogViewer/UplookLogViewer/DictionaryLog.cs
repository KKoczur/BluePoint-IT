using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace UplookLogViewer
{
    class DictionaryLog
    {
        #region Zmienne

        private List<Deskryptor> m_ListOfErrors = new List<Deskryptor>();

        #endregion Zmienne

        #region Konstruktory

        public DictionaryLog()
        {
            m_ListOfErrors.Add(new Deskryptor("80070424", "Brak zainstalowanej usługi: uplook system monitor."));
            m_ListOfErrors.Add(new Deskryptor("80040154", "Brak zainstalowanej usługi: uplook system monitor."));
            m_ListOfErrors.Add(new Deskryptor("ServerNeedsUpdate", "Nieaktualna wersja agenta lub serwera."));
            m_ListOfErrors.Add(new Deskryptor("57P03", "The database system is shutting down."));
            m_ListOfErrors.Add(new Deskryptor("1089", "Couldn't read information about process."));
            m_ListOfErrors.Add(new Deskryptor("1060", "Couldn't save information about created process"));
        }

        #endregion Konstruktory

        #region Wlasciwosci

        public Deskryptor[] GetListOfErrors()
        {
            Deskryptor[] tmp_Array = m_ListOfErrors.ToArray();

            return tmp_Array;      
        }

        #endregion Wlasciwosci
    }
}
