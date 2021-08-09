using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace StatlookLogViewer
{
    [Serializable]
    public class Configuration
    {
      /// <summary>
      /// Constructor
      /// </summary>
      public Configuration()
      {
           DescriptorCollection = new DescriptorCollection();
      }

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
        public string Version { get; set; } = "1.0.1";
        public string StatlookLogDirectory { get; set; } = "\\Statlook\\Logs\\";
        public string StatlookUsmLogDirectory { get; set; }= "\\Statlook\\Logs\\";
        public string UserDirectory { get; set; } = "C:\\Users\\Karol\\Desktop\\Logs\\";
        public string LogFileExtensions { get; set; } = "*.log;*.zip";
        public string StatlookHeaders { get; set; } = "Date; Logger:; Type:; PID/TID:; Thread ID:; Description:; Exception:;   Message:;   Method:;   Stack:";
        public string Show_uplook { get; set; } = "false;true;true;true;true;true;true;true;true;true";
        public string Usm_Headers { get; set; } = "Date; Code:; Type:; Session:; PID/TID:; Description:";
        public string Show_usm { get; set; } = "false;false;true;false;false;true;";

        public DescriptorCollection DescriptorCollection { get; set; } 

        #region HeadersOfUplook

        public bool ULoggerVisible { get; set; }
        public bool UTypeVisible { get; set; }
        public bool UProcessVisible { get; set; }
        public bool UThreadVisible { get; set; }
        public bool UDescriptionVisible { get; set; }
        public bool UExceptionVisible { get; set; }
        public bool UMessageVisible { get; set; }
        public bool UMethodVisible { get; set; }
        public bool UStackVisible { get; set; }
        #endregion HeadersOfUplook

        #region HeadersOfUsm
        public bool UsmCodeVisible { get; set; }
        public bool UsmTypeVisible { get; set; }
        public bool UsmSessionVisible { get; set; }
        public bool UsmProcessVisible { get; set; }
        public bool UsmDescriptionVisible { get; set; }

        #endregion HeadersOfUsm


        public Descriptor[] GetStatlookHeaders() => DescriptorCollection.GetStatlookHeaders();

        public Descriptor[] GetUsmHeaders() => DescriptorCollection.GetUsmHeaders();


      public void uShow(string NameOfHeder,bool Show)
      {
            switch (NameOfHeder)
            {
                case "uLogger":
                    ULoggerVisible = Show;
                    break;
                case "uType":
                    UTypeVisible = Show;
                    break;
                case "uProcess":
                    UProcessVisible = Show;
                    break;
                case "uThread":
                    UThreadVisible = Show;
                    break;
                case "uDescription":
                    UDescriptionVisible = Show;
                    break;
                case "uException":
                    UExceptionVisible = Show;
                    break;
                case "uMessage":
                    UMessageVisible = Show;
                    break;
                case "uMethod":
                    UMethodVisible = Show;
                    break;
                case "uStack":
                    UStackVisible = Show;
                    break;
                default:
                    break;
            }
        }

        public void usmShow(string NameOfHeder, bool Show)
        {
            switch (NameOfHeder)
            {
                case "usmCode":
                    UsmCodeVisible = Show;
                    break;
                case "usmType":
                    UsmTypeVisible = Show;
                    break;
                case "usmSession":
                    UsmSessionVisible = Show;
                    break;
                case "usmProcess":
                    UsmProcessVisible = Show;
                    break;
                case "usmDescription":
                    UsmDescriptionVisible = Show;
                    break;
                default:
                    break;
            }
        }
    }
}

