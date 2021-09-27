
using System.Collections.Generic;

namespace StatlookLogViewer.Model.Pattern
{
    public class StatlookLogPattern : ILogPattern
    {
        #region Constructors

        public StatlookLogPattern(LogType logType, string keyName)
        {
            LogType = logType;
            KeyName = keyName;
        }

        public StatlookLogPattern(LogType logType, string keyName, string textPattern)
            : this(logType, keyName)
        {
            TextPattern = textPattern;
        }

        public StatlookLogPattern(LogType logType, string keyName, string textPattern, bool show)
            : this(logType, keyName, textPattern)
        {
            Show = show;
        }

        #endregion Constructors

        #region Properties

        public LogType LogType { get; set; }

        /// <summary>
        /// Key name of descriptor
        /// </summary>
        public string KeyName { set; get; }

        /// <summary>
        /// Text pattern
        /// </summary>
        public string TextPattern { set; get; }

        /// <summary>
        /// Need to show
        /// </summary>
        public bool Show { set; get; }

        #endregion Properties

        #region Methods

        public static ILogPattern[] CreateLogPatterns()
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

        #endregion Methods

    }
}
