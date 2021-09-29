
using System.Collections.Generic;

namespace StatlookLogViewer.Model.Pattern
{
    public class UsmLogPattern : ILogPattern
    {
        #region Constructors

        public UsmLogPattern( string keyName)
        {
            KeyName = keyName;
        }

        public UsmLogPattern( string keyName, string textPattern)
            : this( keyName)
        {
            TextPattern = textPattern;
        }

        public UsmLogPattern( string keyName, string textPattern, bool show)
            : this( keyName, textPattern)
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
