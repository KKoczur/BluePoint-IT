using StatlookLogViewer.Model.Pattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatlookLogViewer.Parser
{
    public class UsmLogParser : ILogParser
    {
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

        public IEnumerable<ILogPattern> GetLogPatterns()
        {
            return new List<ILogPattern>()
            {
                new UsmLogPattern( "usmDate", "Date", true),
                new UsmLogPattern( "usmCode", " Code:", true),
                new UsmLogPattern( "usmType", " Type:", true),
                new UsmLogPattern( "usmSession", " Session:", true),
                new UsmLogPattern( "usmProcess", " Process ID:", true),
                new UsmLogPattern( "usmDescription", " Description:", true)
             }.ToArray();
        }
    }
}
