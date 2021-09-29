using StatlookLogViewer.Model.Pattern;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace StatlookLogViewer.Parser
{
    public interface ILogParser
    {
        string StartLogGroupEntry { get; set; }

        string EndLogGroupEntry { get; set; }

        IEnumerable<ILogPattern> GetLogPatterns();

        LogErrorPattern[] GetListOfErrors();

        public ILogPattern GetHeaderByKeyName(string keyName)
        {
            return GetLogPatterns().ToList().Find(item => item.KeyName == keyName);
        }

        public IEnumerable<string> GetTextPatterns() => GetLogPatterns().Select(item => item.TextPattern);
    }
}
