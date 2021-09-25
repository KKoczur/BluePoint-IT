
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

    }
}
