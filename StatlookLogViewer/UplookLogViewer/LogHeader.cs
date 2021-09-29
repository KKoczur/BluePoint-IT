
using StatlookLogViewer.Model;
using StatlookLogViewer.Model.Pattern;
using System.Linq;

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

        public ILogPattern[] GetStatlookLogPatterns() => _config.GetStatlookLogPatterns().ToArray();

        public ILogPattern[] GetUsmLogPatterns() => _config.GetUsmLogPatterns().ToArray();

        public string[] GetStatlookTextHeaders() => _config.GetStatlookTextPatterns().Split(new char[] { ';' });

        public string[] GetUsmTextHeaders() => _config.GetUsmTextPatterns().Split(new char[] { ';' });

        #endregion Methods
    }
}
