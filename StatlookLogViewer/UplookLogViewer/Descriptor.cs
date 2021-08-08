
namespace StatlookLogViewer
{
    public class Descriptor
    {
        #region Constructors

        public Descriptor(LogType logType, string name)
        {
            LogType = logType;
            Name = name;
        }

        public Descriptor(LogType logType,string name, string value)
            :this(logType, name)
        {
            Value = value;
        }

        public Descriptor(LogType logType, string name, string value,bool show)
            :this (logType, name, value)
        {
            Show = show;
        }

        #endregion Constructors

        #region Properties

        public LogType LogType { get; set; }

        /// <summary>
        /// Name of descriptor
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Value of descriptor
        /// </summary>
        public string Value { set; get; }

        /// <summary>
        /// Need to show
        /// </summary>
        public bool Show { set; get; }

        #endregion Properties

    }
}
