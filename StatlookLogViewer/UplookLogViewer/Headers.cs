﻿using System.IO; 

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
        private Descriptor[] m_Zbior_uplook_Deskryptors = new Descriptor[10];
        private Descriptor[] m_Zbior_usm_Deskryptors = new Descriptor[6];

        #endregion Members

        #region Constructors

        public Headers()
        {
            if (!File.Exists("config.xml"))
            {
                // Create a new configuration object
                // and initialize some variables
                Configuration c = new Configuration();

                // Serialize the configuration object to a file
                Configuration.Serialize("config.xml", c);

                // Read the configuration object from a file
                _config = Configuration.Deserialize("config.xml");
            }
            else
            {
                // Read the configuration object from a file
                _config = Configuration.Deserialize("config.xml");
            }

            m_uplook_Headers = _config.GetStatlookTextHeaders().Split(new char[] { ';' });
            m_usm_Headers = _config.GetUsmTextHeaders().Split(new char[] { ';' });

            //Utworzenie zbioru deskryptorów uplook
            for (int i = 0; i < m_uplook_Headers.Length; i++)
            {
                m_Zbior_uplook_Deskryptors[i] = new Descriptor(LogType.Statlook, m_uplook_Headers[i]);
            }
            //Utworzenie zbioru deskryptorów usm
            for (int i = 0; i < m_usm_Headers.Length; i++)
            {
                m_Zbior_usm_Deskryptors[i] = new Descriptor(LogType.Usm, m_usm_Headers[i]);
            }
        }

        #endregion Constructors

        #region Properties

        public Descriptor[] uplook_Deskryptor => _config.GetStatlookHeaders();

        public string[] uplook_Headers => m_uplook_Headers;

        public string uplook_Break => m_uplook_Break;

        public string uplook_Date => m_uplook_Date;

        public Descriptor[] usm_Deskryptor => m_Zbior_usm_Deskryptors;

        public string[] usm_Headers => m_usm_Headers;

        public string usm_Break => m_usm_Break;

        public string usm_Date => m_usm_Date;

        #endregion Properties

    }
}
