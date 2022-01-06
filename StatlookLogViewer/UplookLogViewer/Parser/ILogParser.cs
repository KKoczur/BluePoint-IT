using StatlookLogViewer.Model.Pattern;
using System.Collections.Generic;
using System.Linq;

namespace StatlookLogViewer.Parser;

public interface ILogParser
{
    string UniqueLogKey { get; set; }

    string StartLogGroupEntry { get; set; }

    string EndLogGroupEntry { get; set; }

    IEnumerable<LogPattern> GetLogPatterns();

    LogErrorPattern[] GetListOfErrors();

    public IEnumerable<string> GetTextPatterns()
    {
        return GetLogPatterns().Select(item => item.TextPattern);
    }
}