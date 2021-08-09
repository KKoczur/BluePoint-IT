
namespace StatlookLogViewer
{
    public class Descriptor
    {
        #region Constructors

        public Descriptor(LogType logType, string keyName)
        {
            LogType = logType;
            KeyName = keyName;
        }

        public Descriptor(LogType logType, string keyName, string headerText)
            :this(logType, keyName)
        {
            HeaderText = headerText;
        }

        public Descriptor(LogType logType, string keyName, string headerText, bool show)
            :this (logType, keyName, headerText)
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
        /// Header of descriptor
        /// </summary>
        public string HeaderText { set; get; }

        /// <summary>
        /// Need to show
        /// </summary>
        public bool Show { set; get; }


        #endregion Properties

    }
}
