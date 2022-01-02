using StatlookLogViewer.Model.Pattern;
using System.Collections.Generic;

namespace StatlookLogViewer.Parser
{
    public class StatlookLogParser : ILogParser
    {
        public string UniqueLogKey { get; set; } = " Logger:";

        public string StartLogGroupEntry { get; set; } = "Date";

        public string EndLogGroupEntry { get; set; } = "----------------------------------------";

        public LogErrorPattern[] GetListOfErrors()
        {
            return new List<LogErrorPattern>()
            {
                new LogErrorPattern( "80070424", "Brak zainstalowanej usługi: uplook system monitor."),
                new LogErrorPattern( "80040154", "Brak zainstalowanej usługi: uplook system monitor."),
                new LogErrorPattern( "ServerNeedsUpdate", "Nieaktualna wersja agenta lub serwera."),
                new LogErrorPattern( "57P03", "The database system is shutting down."),
                new LogErrorPattern( "InvalidLoginSession", "Invalid login Session.")
            }.ToArray();
        }

        public IEnumerable<LogPattern> GetLogPatterns()
        {
            return new List<LogPattern>()
            {
                new LogPattern("uDate", "Date", true),
                new LogPattern("uLogger", " Logger:", true),
                new LogPattern("uType", " Type:", true),
                new LogPattern("uProcess", " PID/TID:", true),
                new LogPattern("uMessage", " Message:", true),
                new LogPattern("uException", " Exception:", true),
                new LogPattern("ueMessage", "   Message:", true),
                new LogPattern("ueMethod", "   Method:", true),
                new LogPattern("ueStack", "   Stack:", true),

                new LogPattern("uEvent", "Event=", false),
                new LogPattern("uDocumentId", "DocumentId=", false),
                new LogPattern("uBrowser", "Browser=", false),
                new LogPattern("uUrl", "Url=", false),
                new LogPattern("uTitle", "Title=", false),
                new LogPattern("uActive", "Active=", false),
            }.ToArray();
        }
    }
}
