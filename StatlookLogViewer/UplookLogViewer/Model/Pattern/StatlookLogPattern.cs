
using System.Collections.Generic;

namespace StatlookLogViewer.Model.Pattern
{
    public class StatlookLogPattern : ILogPattern
    {
        #region Constructors

        public StatlookLogPattern( string keyName)
        {
            KeyName = keyName;
        }

        public StatlookLogPattern( string keyName, string textPattern)
            : this( keyName)
        {
            TextPattern = textPattern;
        }

        public StatlookLogPattern(string keyName, string textPattern, bool show)
            : this( keyName, textPattern)
        {
            Show = show;
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

        #endregion Properties

    }
}
