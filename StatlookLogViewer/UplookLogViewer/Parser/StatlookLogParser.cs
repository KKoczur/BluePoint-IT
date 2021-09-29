using StatlookLogViewer.Model.Pattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatlookLogViewer.Parser
{
    public class StatlookLogParser : ILogParser
    {
        public LogErrorPattern[] GetListOfErrors()
        {
            return new List<LogErrorPattern>()
            { 
                new LogErrorPattern( "80070424", "Brak zainstalowanej usługi: uplook system monitor."),
                new LogErrorPattern( "80040154", "Brak zainstalowanej usługi: uplook system monitor."),
                new LogErrorPattern( "ServerNeedsUpdate", "Nieaktualna wersja agenta lub serwera."),
                new LogErrorPattern( "57P03", "The database system is shutting down.")
            }.ToArray();
        }

        public IEnumerable<ILogPattern> GetLogPatterns()
        {
            return new List<ILogPattern>()
            {
                new StatlookLogPattern("uDate", "Date", true),
                new StatlookLogPattern("uLogger", " Logger:", true),
                new StatlookLogPattern("uType", " Type:", true),
                new StatlookLogPattern("uProcess", " Process ID:", true),
                new StatlookLogPattern("uThread", " Thread ID:", true),
                new StatlookLogPattern("uDescription", " Description:", true),
                new StatlookLogPattern("uException", " Exception:", true),
                new StatlookLogPattern("uMessage", "   Message:", true),
                new StatlookLogPattern("uMethod", "   Method:", true),
                new StatlookLogPattern("uStack", "   Stack:", true),

                new StatlookLogPattern("uEvent", "Event=", false),
                new StatlookLogPattern("uDocumentId", "DocumentId=", false),
                new StatlookLogPattern("uBrowser", "Browser=", false),
                new StatlookLogPattern("uUrl", "Url=", false),
                new StatlookLogPattern("uTitle", "Title=", false),
                new StatlookLogPattern("uActive", "Active=", false),
            }.ToArray();
        }
    }
}
