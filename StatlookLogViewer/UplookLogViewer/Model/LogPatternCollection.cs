using StatlookLogViewer.Model;
using StatlookLogViewer.Model.Pattern;
using System.Collections.Generic;
using System.Linq;

namespace StatlookLogViewer.Model
{
    public class LogPatternCollection
    {
        #region Members

        private readonly List<ILogPattern> _logPatterns = new List<ILogPattern>();

        #endregion Members

        #region Constructors

        public LogPatternCollection()
        {
            _logPatterns.AddRange(CreateStatlookHeaders());
            _logPatterns.AddRange(CreateUsmHeaders());
        }

        #endregion Constructors

        #region Methods

        public ILogPattern[] GetStatlookDescriptors() => _logPatterns.Where(item => item.LogType == LogType.Statlook).ToArray();

        public ILogPattern[] GetUsmDescriptors() => _logPatterns.Where(item => item.LogType == LogType.Usm).ToArray();

        public ILogPattern GetHeaderByKeyName(string keyName) => _logPatterns.Find(item => item.KeyName == keyName);

        private ILogPattern[] CreateStatlookHeaders()
        {
            const LogType logType = LogType.Statlook;

            var result = new List<ILogPattern>()
            {
                new StatlookLogPattern(logType,"uDate", "Date", true),
                new StatlookLogPattern(logType,"uLogger", " Logger:", true),
                new StatlookLogPattern(logType,"uType", " Type:", true),
                new StatlookLogPattern(logType,"uProcess", " Process ID:", true),
                new StatlookLogPattern(logType,"uThread", " Thread ID:", true),
                new StatlookLogPattern(logType,"uDescription", " Description:", true),
                new StatlookLogPattern(logType,"uException", " Exception:", true),
                new StatlookLogPattern(logType,"uMessage", "   Message:", true),
                new StatlookLogPattern(logType,"uMethod", "   Method:", true),
                new StatlookLogPattern(logType,"uStack", "   Stack:", true),

                new StatlookLogPattern(logType,"uEvent", "Event=", false),
                new StatlookLogPattern(logType,"uDocumentId", "DocumentId=", false),
                new StatlookLogPattern(logType,"uBrowser", "Browser=", false),
                new StatlookLogPattern(logType,"uUrl", "Url=", false),
                new StatlookLogPattern(logType,"uTitle", "Title=", false),
                new StatlookLogPattern(logType,"uActive", "Active=", false),
            };

            return result.ToArray();
        }

        private ILogPattern[] CreateUsmHeaders()
        {
            const LogType logType = LogType.Usm;

            var result = new List<ILogPattern>()
            {
                new UsmLogPattern(logType, "usmDate", "Date", true),
                new UsmLogPattern(logType, "usmCode", " Code:", true),
                new UsmLogPattern(logType, "usmType", " Type:", true),
                new UsmLogPattern(logType, "usmSession", " Session:", true),
                new UsmLogPattern(logType, "usmProcess", " Process ID:", true),
                new UsmLogPattern(logType, "usmDescription", " Description:", true)
             };

            return result.ToArray();
        }

        #endregion Methods
    }
}
