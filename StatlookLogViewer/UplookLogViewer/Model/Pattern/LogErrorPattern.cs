
namespace StatlookLogViewer.Model.Pattern
{
    public class LogErrorPattern
    {
        #region Constructors

        public LogErrorPattern(LogType logType, string errorTextPattern)
        {
            LogType = logType;
            ErrorTextPattern = errorTextPattern;
        }

        public LogErrorPattern(LogType logType, string errorTextPattern, string errorReason)
            : this(logType, errorTextPattern)
        {
            ErrorReason = errorReason;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Log type
        /// </summary>
        public LogType LogType { get; set; }

        /// <summary>
        /// Text pattern
        /// </summary>
        public string ErrorTextPattern { set; get; }

        /// <summary>
        /// Error reason
        /// </summary>
        public string ErrorReason { set; get; }


        #endregion Properties

    }
}
