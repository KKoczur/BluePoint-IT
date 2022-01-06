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
                new( "80070424", "Brak zainstalowanej usługi: uplook system monitor."),
                new( "80040154", "Brak zainstalowanej usługi: uplook system monitor."),
                new( "ServerNeedsUpdate", "Nieaktualna wersja agenta lub serwera."),
                new( "57P03", "The database system is shutting down."),
                new( "InvalidLoginSession", "Invalid login Session.")
            }.ToArray();
        }

        public IEnumerable<LogPattern> GetLogPatterns()
        {
            return new List<LogPattern>()
            {
                new("uDate", "Date", true),
                new("uLogger", " Logger:", true),
                new("uType", " Type:", true),
                new("uProcess", " PID/TID:", true),
                new("uMessage", " Message:", true),
                new("uException", " Exception:", true),
                new("ueMessage", "   Message:", true),
                new("ueMethod", "   Method:", true),
                new("ueStack", "   Stack:", true),

                new("uEvent", "Event=", false),
                new("uDocumentId", "DocumentId=", false),
                new("uBrowser", "Browser=", false),
                new("uUrl", "Url=", false),
                new("uTitle", "Title=", false),
                new("uActive", "Active=", false),
            }.ToArray();
        }
    }
}
