﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 

namespace UplookLogViewer
{
    class Headers
    {
        #region Zmienne

        //Elementy składowe(deskryptory) pliku logów systemu uplook
        private string[] m_uplook_Headers;
        private string[] m_usm_Headers;
        private string m_uplook_Date = "Date";
        private string m_uplook_Break = "----------------------------------------";
        private string m_usm_Date = "Date";
        private string m_usm_Break = "----------------------------------------";
        private Deskryptor[] m_Zbior_uplook_Deskryptors = new Deskryptor[10];
        private Deskryptor[] m_Zbior_usm_Deskryptors = new Deskryptor[6];
        #endregion Zmienne

        #region Konstruktory

        public Headers()
        {

            Configuration config;
            if (!File.Exists("config.xml"))
            {
                // Create a new configuration object
                // and initialize some variables
                Configuration c = new Configuration();

                // Serialize the configuration object to a file
                Configuration.Serialize("config.xml", c);

                // Read the configuration object from a file
                config = Configuration.Deserialize("config.xml");
            }
            else
            {
                // Read the configuration object from a file
                config = Configuration.Deserialize("config.xml");
            }

            m_uplook_Headers = config.uplook_Headers.Split(new char[] { ';' });
            m_usm_Headers = config.usm_Headers.Split(new char[] { ';' });

            //Utworzenie zbioru deskryptorów uplook
            for (int i = 0; i < m_uplook_Headers.Length; i++)
            {
                m_Zbior_uplook_Deskryptors[i] = new Deskryptor(m_uplook_Headers[i]);
            }
            //Utworzenie zbioru deskryptorów usm
            for (int i = 0; i < m_usm_Headers.Length; i++)
            {
                m_Zbior_usm_Deskryptors[i] = new Deskryptor(m_usm_Headers[i]);
            }
        }

        #endregion Konstruktory

        #region Wlasciowsci

        public Deskryptor[] uplook_Deskryptor
        {
            get
            {
                return m_Zbior_uplook_Deskryptors;
            }

        }

        public string[] uplook_Headers
        {
            get
            {
                return m_uplook_Headers;
            }
        }

        public string uplook_Break
        {
            get
            {
                return m_uplook_Break;
            }
        }

        public string uplook_Date
        {
            get
            {
                return m_uplook_Date;
            }
        }

        public Deskryptor[] usm_Deskryptor
        {
            get
            {
                return m_Zbior_usm_Deskryptors;
            }

        }

        public string[] usm_Headers
        {
            get
            {
                return m_usm_Headers;
            }
        }

        public string usm_Break
        {
            get
            {
                return m_usm_Break;
            }
        }

        public string usm_Date
        {
            get
            {
                return m_usm_Date;
            }
        }

        #endregion Wlasciowsci

        #region Metody

        public string[] GetAllHeaders()
        {
            string [] T = new string [m_uplook_Headers.Length + m_usm_Headers.Length]; 
            m_uplook_Headers.CopyTo(T, 0);
            m_usm_Headers.CopyTo(T, m_uplook_Headers.Length);
            return T;
        }

        #endregion Metody
    }
}
