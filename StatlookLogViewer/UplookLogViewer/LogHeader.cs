namespace StatlookLogViewer
{
    internal class LogHeader
    {
        #region Consts

        private const string STATLOOK_DATE = "Date";
        private const string USM_DATE = "Date";
        private const string STATLOOK_BREAK = "----------------------------------------";
        private const string USM_BREAK = "----------------------------------------";

        #endregion Consts

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

        public string StatlookHeaderDate => STATLOOK_DATE;

        public string StatlookHeaderBreak => STATLOOK_BREAK;

        public string UsmHeaderDate => USM_DATE;

        public string UsmHeaderBreak => USM_BREAK;

        #endregion Properties

        #region Methods

        public Descriptor[] GetStatlookDescriptors() => _config.GetStatlookDescriptors();

        public Descriptor[] GetUsmDescriptors() => _config.GetUsmDescriptors();

        public string[] GetStatlookTextHeaders() => _config.GetStatlookTextHeaders().Split(new char[] { ';' });

        public string[] GetUsmTextHeaders() => _config.GetUsmTextHeaders().Split(new char[] { ';' });

        #endregion Methods
    }
}
