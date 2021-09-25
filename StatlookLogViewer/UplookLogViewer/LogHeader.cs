
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

        public ILogPattern[] GetStatlookDescriptors() => _config.GetStatlookDescriptors();

        public ILogPattern[] GetUsmDescriptors() => _config.GetUsmDescriptors();

        public string[] GetStatlookTextHeaders() => _config.GetStatlookTextHeaders().Split(new char[] { ';' });

        public string[] GetUsmTextHeaders() => _config.GetUsmTextHeaders().Split(new char[] { ';' });

        #endregion Methods
    }
}
