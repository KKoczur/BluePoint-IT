
namespace StatlookLogViewer.Model.Pattern
{
    public class LogErrorPattern
    {
        #region Constructors

        public LogErrorPattern(string errorTextPattern)
        {
            ErrorTextPattern = errorTextPattern;
        }

        public LogErrorPattern(string errorTextPattern, string errorReason)
            : this(errorTextPattern)
        {
            ErrorReason = errorReason;
        }

        #endregion Constructors

        #region Properties

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
