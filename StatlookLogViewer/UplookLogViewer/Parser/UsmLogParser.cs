using StatlookLogViewer.Model.Pattern;
using System.Collections.Generic;

namespace StatlookLogViewer.Parser;

public class UsmLogParser : ILogParser
{
    public string UniqueLogKey { get; set; } = " Code:";

    public string StartLogGroupEntry { get; set; } = "Date";

    public string EndLogGroupEntry { get; set; } = "----------------------------------------";

    public LogErrorPattern[] GetListOfErrors()
    {
        return new List<LogErrorPattern>
        {
            new("1089", "Couldn't read information about process."),
            new("1060", "Couldn't save information about created process")
        }.ToArray();
    }

    public IEnumerable<LogPattern> GetLogPatterns()
    {
        return new List<LogPattern>
        {
            new("usmDate", "Date", true),
            new("usmCode", " Code:", true),
            new("usmType", " Type:", true,true),
            new("usmSession", " Session:", true),
            new("usmProcess", " Process ID:", true),
            new("usmDescription", " Description:", true)
        }.ToArray();
    }
}