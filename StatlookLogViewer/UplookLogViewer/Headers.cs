using System.IO; 

namespace StatlookLogViewer
{
    class Headers
    {
        #region Members

        /// <summary>
        /// Obiekt konfiguracji
        /// </summary>
        private readonly Configuration _config;

        //Elementy składowe(deskryptory) pliku logów systemu uplook
        private string[] m_uplook_Headers;
        private string[] m_usm_Headers;
        private string m_uplook_Date = "Date";
        private string m_uplook_Break = "----------------------------------------";
        private string m_usm_Date = "Date";
        private string m_usm_Break = "----------------------------------------";

        #endregion Members

        #region Constructors

        public Headers()
        {
            _config = Configuration.GetConfiguration();

            m_uplook_Headers = _config.GetStatlookTextHeaders().Split(new char[] { ';' });
            m_usm_Headers = _config.GetUsmTextHeaders().Split(new char[] { ';' });
        }

        #endregion Constructors

        #region Properties

        public Descriptor[] uplook_Deskryptor => _config.GetStatlookDescriptors();

        public string[] uplook_Headers => m_uplook_Headers;

        public string uplook_Break => m_uplook_Break;

        public string uplook_Date => m_uplook_Date;

        public Descriptor[] usm_Deskryptor => _config.GetUsmDescriptors();

        public string[] usm_Headers => m_usm_Headers;

        public string usm_Break => m_usm_Break;

        public string usm_Date => m_usm_Date;

        #endregion Properties
    }
}
