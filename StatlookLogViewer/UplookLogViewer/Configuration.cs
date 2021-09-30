using StatlookLogViewer.Model;
using StatlookLogViewer.Model.Pattern;
using StatlookLogViewer.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace StatlookLogViewer
{
    [Serializable]
    public class Configuration
    {
        #region Constans


        private const string CONFIG_FILE_NAME = "config.xml";
        private const string LOG_DIRECTORY_PATH = "\\Statlook\\Logs\\";

        public const string LOG_FILE_EXTENSIONS = "Log files (*.log)|*.log|Text files (*.txt)|*.txt| Zip files (*.zip)|*.zip| All files (*.*)|*.*";
        public const string ZIP_FILE_EXTENSION = ".zip";

        #endregion Constans

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Configuration()
        {
            StatlookLogParser = new StatlookLogParser();
            UsmLogParser = new UsmLogParser();
        }

        #endregion Constructors

        #region Methods

        public static void SaveConfig(Configuration c) => Serialize(CONFIG_FILE_NAME, c);

        public LogPattern[] GetStatlookLogPatterns() => StatlookLogParser.GetLogPatterns().ToArray();

        public LogPattern[] GetUsmLogPatterns() => UsmLogParser.GetLogPatterns().ToArray();


        public void SetStatlookHeaderVisibility(string keyName, bool needToShow)
        {
            LogPattern logPattern = ((ILogParser)StatlookLogParser).GetHeaderByKeyName(keyName);

            if (logPattern != null)
                logPattern.Show = needToShow;
        }

        public void SetUsmHeaderVisibility(string keyName, bool needToShow)
        {
            LogPattern logPattern = ((ILogParser)UsmLogParser).GetHeaderByKeyName(keyName);

            if (logPattern != null)
                logPattern.Show = needToShow;
        }

        public static Configuration GetConfiguration()
        {
            if (!File.Exists(CONFIG_FILE_NAME))
            {
                // Create a new configuration object
                // and initialize some variables
                Configuration c = new Configuration();

                // Serialize the configuration object to a file
                Serialize(CONFIG_FILE_NAME, c);

                // Read the configuration object from a file
                return Deserialize(CONFIG_FILE_NAME);
            }
            else
            {
                // Read the configuration object from a file
                return Deserialize(CONFIG_FILE_NAME);
            }
        }

        private static void Serialize(string file, Configuration c)
        {
            XmlSerializer xs = new XmlSerializer(c.GetType());
            StreamWriter writer = File.CreateText(file);
            xs.Serialize(writer, c);
            writer.Flush();
            writer.Close();
        }

        private static Configuration Deserialize(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Configuration));
            StreamReader reader = File.OpenText(file);
            Configuration c = (Configuration)xs.Deserialize(reader);
            reader.Close();
            return c;
        }

        #endregion Methods

        #region Properties

        public StatlookLogParser StatlookLogParser { get; set; }
        public UsmLogParser UsmLogParser { get; set; }

        public string StatlookLogDirectory { get; set; } = LOG_DIRECTORY_PATH;
        public string UserDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + LOG_DIRECTORY_PATH;
        public string LogFileExtensions { get; set; } = "*.log;*.zip";

        #endregion Properties
    }
}

