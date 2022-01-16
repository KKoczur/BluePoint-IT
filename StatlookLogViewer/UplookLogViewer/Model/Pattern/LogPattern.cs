namespace StatlookLogViewer.Model.Pattern
{
    public class LogPattern
    {
        #region Constructors

        public LogPattern(string keyName)
        {
            KeyName = keyName;
        }

        public LogPattern(string keyName, string textPattern)
            : this(keyName)
        {
            TextPattern = textPattern;
        }

        public LogPattern(string keyName, string textPattern, bool show, bool isLogType = false)
            : this(keyName, textPattern)
        {
            Show = show;
            IsLogType = isLogType;
        }

        #endregion Constructors

        #region Properties

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

        public bool IsLogType { set; get; }

        #endregion Properties
    }
}
