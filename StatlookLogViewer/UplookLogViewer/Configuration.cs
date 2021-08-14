using System;
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

        #endregion Constans

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public Configuration()
        {
           DescriptorCollection = new DescriptorCollection();
        }

        #endregion Constructors

        #region Methods

        public static void SaveConfig(Configuration c) => Serialize(CONFIG_FILE_NAME, c);

        public Descriptor[] GetStatlookHeaders() => DescriptorCollection.GetStatlookHeaders();

        public string GetStatlookTextHeaders() => string.Join (";", DescriptorCollection.GetStatlookHeaders().Select(item => item.HeaderText));

        public Descriptor[] GetUsmHeaders() => DescriptorCollection.GetUsmHeaders();

        public string GetUsmTextHeaders() => string.Join(";", DescriptorCollection.GetUsmHeaders().Select(item => item.HeaderText));

        public void SetHeaderVisibility(string headerKeyName, bool needToShow)
        {
            Descriptor descriptor = DescriptorCollection.GetHeaderByKeyName(headerKeyName);

            if (descriptor != null)
                descriptor.Show = needToShow;
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

        static void Serialize(string file, Configuration c)
        {
            XmlSerializer xs = new XmlSerializer(c.GetType());
            StreamWriter writer = File.CreateText(file);
            xs.Serialize(writer, c);
            writer.Flush();
            writer.Close();
        }

        static Configuration Deserialize(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Configuration));
            StreamReader reader = File.OpenText(file);
            Configuration c = (Configuration)xs.Deserialize(reader);
            reader.Close();
            return c;
        }

        #endregion Methods

        #region Properties

        public DescriptorCollection DescriptorCollection { get; set; }

        public string StatlookLogDirectory { get; set; } = "\\Statlook\\Logs\\";
        public string StatlookUsmLogDirectory { get; set; }= "\\Statlook\\Logs\\";
        public string UserDirectory { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Statlook\\Logs\\";
        public string LogFileExtensions { get; set; } = "*.log;*.zip";

        #endregion Properties
    }
}

