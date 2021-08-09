using System;
using System.IO;
using System.Xml.Serialization;

namespace StatlookLogViewer
{
    [Serializable]
    public class Configuration
    {
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

        public static void Serialize(string file, Configuration c)
        {
             XmlSerializer xs = new XmlSerializer(c.GetType());
             StreamWriter writer = File.CreateText(file);
             xs.Serialize(writer, c);
             writer.Flush();
             writer.Close();
        }

        public static Configuration Deserialize(string file)
        {
             XmlSerializer xs = new XmlSerializer(typeof(Configuration));
             StreamReader reader = File.OpenText(file);
             Configuration c = (Configuration)xs.Deserialize(reader);
             reader.Close();
             return c;
        }

        public Descriptor[] GetStatlookHeaders() => DescriptorCollection.GetStatlookHeaders();

        public Descriptor[] GetUsmHeaders() => DescriptorCollection.GetUsmHeaders();

        public void SetHeaderVisibility(string headerKeyName, bool needToShow)
        {
            Descriptor descriptor = DescriptorCollection.GetHeaderByKeyName(headerKeyName);

            if (descriptor != null)
                descriptor.Show = needToShow;
        }

        #endregion Methods

        public DescriptorCollection DescriptorCollection { get; set; }

        public string Version { get; set; } = "1.0.1";
        public string StatlookLogDirectory { get; set; } = "\\Statlook\\Logs\\";
        public string StatlookUsmLogDirectory { get; set; }= "\\Statlook\\Logs\\";
        public string UserDirectory { get; set; } = "C:\\Users\\Karol\\Desktop\\Logs\\";
        public string LogFileExtensions { get; set; } = "*.log;*.zip";
        public string StatlookHeaders { get; set; } = "Date; Logger:; Type:; PID/TID:; Thread ID:; Description:; Exception:;   Message:;   Method:;   Stack:";
        public string Show_uplook { get; set; } = "false;true;true;true;true;true;true;true;true;true";
        public string Usm_Headers { get; set; } = "Date; Code:; Type:; Session:; PID/TID:; Description:";
        public string Show_usm { get; set; } = "false;false;true;false;false;true;";
    }
}

