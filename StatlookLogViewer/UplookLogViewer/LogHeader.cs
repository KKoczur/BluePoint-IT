using System.IO; 

namespace StatlookLogViewer
{
    internal class LogHeader
    {
        #region Members

        /// <summary>
        /// Obiekt konfiguracji
        /// </summary>
        private readonly Configuration _config;

        private string m_uplook_Date = "Date";
        private string m_uplook_Break = "----------------------------------------";
        private string m_usm_Date = "Date";
        private string m_usm_Break = "----------------------------------------";

        #endregion Members

        #region Constructors

        public LogHeader()
        {
            _config = Configuration.GetConfiguration();
        }

        #endregion Constructors

        #region Properties

        public string uplook_Break => m_uplook_Break;

        public string uplook_Date => m_uplook_Date;

        public string usm_Break => m_usm_Break;

        public string usm_Date => m_usm_Date;

        #endregion Properties

        #region Methods

        public Descriptor[] GetStatlookDescriptors() => _config.GetStatlookDescriptors();

        public Descriptor[] GetUsmDescriptors() => _config.GetUsmDescriptors();

        public string[] GetStatlookTextHeaders() => _config.GetStatlookTextHeaders().Split(new char[] { ';' });

        public string[] GetUsmTextHeaders() => _config.GetUsmTextHeaders().Split(new char[] { ';' });

        #endregion Methods
    }
}
