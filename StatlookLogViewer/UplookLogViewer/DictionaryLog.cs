using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace StatlookLogViewer
{
    class DictionaryLog
    {
        #region Zmienne

        private List<Descriptor> m_ListOfErrors = new List<Descriptor>();

        #endregion Zmienne

        #region Konstruktory

        public DictionaryLog()
        {
            m_ListOfErrors.Add(new Descriptor("80070424", "Brak zainstalowanej usługi: uplook system monitor."));
            m_ListOfErrors.Add(new Descriptor("80040154", "Brak zainstalowanej usługi: uplook system monitor."));
            m_ListOfErrors.Add(new Descriptor("ServerNeedsUpdate", "Nieaktualna wersja agenta lub serwera."));
            m_ListOfErrors.Add(new Descriptor("57P03", "The database system is shutting down."));
            m_ListOfErrors.Add(new Descriptor("1089", "Couldn't read information about process."));
            m_ListOfErrors.Add(new Descriptor("1060", "Couldn't save information about created process"));
        }

        #endregion Konstruktory

        #region Wlasciwosci

        public Descriptor[] GetListOfErrors()
        {
            Descriptor[] tmp_Array = m_ListOfErrors.ToArray();

            return tmp_Array;      
        }

        #endregion Wlasciwosci
    }
}
