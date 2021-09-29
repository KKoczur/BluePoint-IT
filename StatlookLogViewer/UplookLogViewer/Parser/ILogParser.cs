using StatlookLogViewer.Model.Pattern;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace StatlookLogViewer.Parser
{
    public interface ILogParser
    {
        IEnumerable<ILogPattern> GetLogPatterns();

        LogErrorPattern[] GetListOfErrors();

        public ILogPattern GetHeaderByKeyName(string keyName)
        {
            return GetLogPatterns().ToList().Find(item => item.KeyName == keyName);
        }
    }
}
