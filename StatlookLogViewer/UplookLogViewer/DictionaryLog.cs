using System.Collections.Generic;

namespace StatlookLogViewer
{
    internal class DictionaryLog
    {
        #region Members

        private readonly List<Descriptor> _listOfErrors = new List<Descriptor>();

        #endregion Members

        #region Constructors

        public DictionaryLog()
        {
            _listOfErrors.Add(new Descriptor(LogType.Statlook, "80070424", "Brak zainstalowanej usługi: uplook system monitor."));
            _listOfErrors.Add(new Descriptor(LogType.Statlook, "80040154", "Brak zainstalowanej usługi: uplook system monitor."));
            _listOfErrors.Add(new Descriptor(LogType.Statlook, "ServerNeedsUpdate", "Nieaktualna wersja agenta lub serwera."));
            _listOfErrors.Add(new Descriptor(LogType.Statlook, "57P03", "The database system is shutting down."));
            _listOfErrors.Add(new Descriptor(LogType.Usm, "1089", "Couldn't read information about process."));
            _listOfErrors.Add(new Descriptor(LogType.Usm, "1060", "Couldn't save information about created process"));
        }

        #endregion Constructors

        #region Methods

        public Descriptor[] GetListOfAllErrors() => _listOfErrors.ToArray();

        #endregion Methods
    }
}
