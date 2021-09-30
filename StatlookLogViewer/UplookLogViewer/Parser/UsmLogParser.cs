using StatlookLogViewer.Model.Pattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatlookLogViewer.Parser
{
    public class UsmLogParser : ILogParser
    {
        public string UniqueLogKey { get; set; } = " Code:";

        public string StartLogGroupEntry { get; set; } = "Date";

        public string EndLogGroupEntry { get; set; } = "----------------------------------------";

        public LogErrorPattern[] GetListOfErrors()
        {
            return new List<LogErrorPattern>()
            {
                new LogErrorPattern("1089", "Couldn't read information about process."),
                new LogErrorPattern( "1060", "Couldn't save information about created process")
            }.ToArray();
        }

        public IEnumerable<LogPattern> GetLogPatterns()
        {
            return new List<LogPattern>()
            {
                new LogPattern( "usmDate", "Date", true),
                new LogPattern( "usmCode", " Code:", true),
                new LogPattern( "usmType", " Type:", true),
                new LogPattern( "usmSession", " Session:", true),
                new LogPattern( "usmProcess", " Process ID:", true),
                new LogPattern( "usmDescription", " Description:", true)
             }.ToArray();
        }
    }
}
