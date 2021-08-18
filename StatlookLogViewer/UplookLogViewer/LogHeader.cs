
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

        public string StatlookHeaderDate => Configuration.STATLOOK_DATE;

        public string StatlookHeaderBreak => Configuration.STATLOOK_BREAK;

        public string UsmHeaderDate => Configuration.USM_DATE;

        public string UsmHeaderBreak => Configuration.USM_BREAK;

        #endregion Properties

        #region Methods

        public Descriptor[] GetStatlookDescriptors() => _config.GetStatlookDescriptors();

        public Descriptor[] GetUsmDescriptors() => _config.GetUsmDescriptors();

        public string[] GetStatlookTextHeaders() => _config.GetStatlookTextHeaders().Split(new char[] { ';' });

        public string[] GetUsmTextHeaders() => _config.GetUsmTextHeaders().Split(new char[] { ';' });

        #endregion Methods
    }
}
