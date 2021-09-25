using StatlookLogViewer.Model.Pattern;
using System.Collections.Generic;

namespace StatlookLogViewer.Model
{
    internal class ErrorCollection
    {
        #region Members

        private readonly List<LogErrorPattern> _listOfErrors = new List<LogErrorPattern>();

        #endregion Members

        #region Constructors

        public ErrorCollection()
        {
            _listOfErrors.Add(new LogErrorPattern(LogType.Statlook, "80070424", "Brak zainstalowanej usługi: uplook system monitor."));
            _listOfErrors.Add(new LogErrorPattern(LogType.Statlook, "80040154", "Brak zainstalowanej usługi: uplook system monitor."));
            _listOfErrors.Add(new LogErrorPattern(LogType.Statlook, "ServerNeedsUpdate", "Nieaktualna wersja agenta lub serwera."));
            _listOfErrors.Add(new LogErrorPattern(LogType.Statlook, "57P03", "The database system is shutting down."));
            _listOfErrors.Add(new LogErrorPattern(LogType.Usm, "1089", "Couldn't read information about process."));
            _listOfErrors.Add(new LogErrorPattern(LogType.Usm, "1060", "Couldn't save information about created process"));
        }

        #endregion Constructors

        #region Methods

        public LogErrorPattern[] GetListOfAllErrors() => _listOfErrors.ToArray();

        #endregion Methods
    }
}
