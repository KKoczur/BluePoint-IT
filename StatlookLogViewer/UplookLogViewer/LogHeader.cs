
using StatlookLogViewer.Model;
using StatlookLogViewer.Model.Pattern;

namespace StatlookLogViewer
{
    internal class LogHeader
    {

        #region Members

        /// <summary>
        /// Obiekt konfiguracji
        /// </summary>
        private readonly Configuration _config;

        #endregion Members

        #region Constructors

        public LogHeader()
        {
            _config = Configuration.GetConfiguration();
        }

        #endregion Constructors

        #region Properties


        #endregion Properties

        #region Methods

        public ILogPattern[] GetStatlookLogPatterns() => _config.GetStatlookLogPatterns();

        public ILogPattern[] GetUsmLogPatterns() => _config.GetUsmLogPatterns();

        public string[] GetStatlookTextHeaders() => _config.GetStatlookTextPatterns().Split(new char[] { ';' });

        public string[] GetUsmTextHeaders() => _config.GetUsmTextPatterns().Split(new char[] { ';' });

        #endregion Methods
    }
}
