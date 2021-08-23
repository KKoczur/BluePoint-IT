
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

        public Descriptor(LogType logType, string keyName, string rowCaption)
            : this(logType, keyName)
        {
            RowCaption = rowCaption;
        }

        public Descriptor(LogType logType, string keyName, string rowCaption, bool show)
            : this(logType, keyName, rowCaption)
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
        /// Row caption
        /// </summary>
        public string RowCaption { set; get; }

        /// <summary>
        /// Need to show
        /// </summary>
        public bool Show { set; get; }

        #endregion Properties

    }
}
