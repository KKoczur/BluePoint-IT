
using System.Collections.Generic;

namespace StatlookLogViewer.Model.Pattern
{
    public class UsmLogPattern : ILogPattern
    {
        #region Constructors

        public UsmLogPattern(LogType logType, string keyName)
        {
            LogType = logType;
            KeyName = keyName;
        }

        public UsmLogPattern(LogType logType, string keyName, string textPattern)
            : this(logType, keyName)
        {
            TextPattern = textPattern;
        }

        public UsmLogPattern(LogType logType, string keyName, string textPattern, bool show)
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
