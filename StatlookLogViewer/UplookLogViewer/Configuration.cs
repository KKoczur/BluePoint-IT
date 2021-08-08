using System;
using System.Collections.Generic;
using System.IO;

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
            Version = "1.0.1";
            AplusCLogDirectory = "\\Statlook\\Logs\\";
            AplusCUSMDirectory = "\\Statlook\\Logs\\";
            UserDirectory = "C:\\Users\\Karol\\Desktop\\Logs\\";
            FileExtensions = "*.log;*.zip";
            Uplook_Headers = "Date; Logger:; Type:; PID/TID:; Thread ID:; Description:; Exception:;   Message:;   Method:;   Stack:";
            Show_uplook = "false;true;true;true;true;true;true;true;true;true";
            Usm_Headers = "Date; Code:; Type:; Session:; PID/TID:; Description:";
            Show_usm = "false;false;true;false;false;true;";

            #region HeadersOfUplook
            UDate = "Date";
            ULogger = " Logger:";
            UType = " Type:";
            UProcess = " Process ID:";
            UThread = " Thread ID:";
            UDescription = " Description:";
            UException = " Exception:";
            UMessage = "   Message:";
            UMethod = "   Method:";
            UStack = "   Stack:";
            UDateVisible = true;
            ULoggerVisible = true;
            UTypeVisible = true;
            UProcessVisible = true;
            UThreadVisible = true;
            UDescriptionVisible = true;
            UExceptionVisible = true;
            UMessageVisible = true;
            UMethodVisible = true;
            UStackVisible = true;
            #endregion HeadersOfUplook

            #region HeadersOfUsm
            UsmDate = "Date";
            usmCode = " Code:";
            UsmType = " Type:";
            UsmSession = " Session:";
            UsmProcess = " Process ID:";
            UsmDescription = " Description:";
            UsmDateVisible = true;
            UsmCodeVisible = true;
            UsmTypeVisible = true;
            UsmSessionVisible = true;
            UsmProcessVisible = true;
            UsmDescriptionVisible = true;
          #endregion HeadersOfUsm
      }
      public static void Serialize(string file, Configuration c)
      {
         System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(c.GetType());
         StreamWriter writer = File.CreateText(file);
         xs.Serialize(writer, c);
         writer.Flush();
         writer.Close();
      }
      public static Configuration Deserialize(string file)
      {
         System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
         StreamReader reader = File.OpenText(file);
         Configuration c = (Configuration)xs.Deserialize(reader);
         reader.Close();
         return c;
      }
        public string Version { get; set; }
        public string AplusCLogDirectory { get; set; }
        public string AplusCUSMDirectory { get; set; }
        public string UserDirectory { get; set; }
        public string FileExtensions { get; set; }
        public string Uplook_Headers { get; set; }
        public string Show_uplook { get; set; }
        public string Usm_Headers { get; set; }
        public string Show_usm { get; set; }

        #region HeadersOfUplook
        public string UDate { get; set; }
        public string ULogger { get; set; }
        public string UType { get; set; }
        public string UProcess { get; set; }
        public string UThread { get; set; }
        public string UDescription { get; set; }
        public string UException { get; set; }
        public string UMessage { get; set; }
        public string UMethod { get; set; }
        public string UStack { get; set; }
        public bool UDateVisible { get; set; }
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
        public string UsmDate { get; set; }
        public string usmCode { get; set; }
        public string UsmType { get; set; }
        public string UsmSession { get; set; }
        public string UsmProcess { get; set; }
        public string UsmDescription { get; set; }
        public bool UsmDateVisible { get; set; }
        public bool UsmCodeVisible { get; set; }
        public bool UsmTypeVisible { get; set; }
        public bool UsmSessionVisible { get; set; }
        public bool UsmProcessVisible { get; set; }
        public bool UsmDescriptionVisible { get; set; }

        #endregion HeadersOfUsm

        public Descriptor[] GetStatlookHeaders()
        {
            var result = new List<Descriptor>()
            {
                new Descriptor("uDate", UDate, UDateVisible),
                new Descriptor("uLogger", ULogger, ULoggerVisible),
                new Descriptor("uType", UType, UTypeVisible),
                new Descriptor("uProcess", UProcess, UProcessVisible),
                new Descriptor("uThread", UThread, UThreadVisible),
                new Descriptor("uDescription", UDescription, UDescriptionVisible),
                new Descriptor("uException", UException, UExceptionVisible),
                new Descriptor("uMessage", UMessage, UMessageVisible),
                new Descriptor("uMethod", UMethod, UMethodVisible),
                new Descriptor("uStack", UStack, UStackVisible)
            };

          return result.ToArray();
      }
      public Descriptor[] GetUsmHeaders()
      {
          Descriptor[] usmD = new Descriptor[6];
          usmD[0] = new Descriptor("usmDate", UsmDate, UsmDateVisible);
          usmD[1] = new Descriptor("usmCode", usmCode, UsmCodeVisible);
          usmD[2] = new Descriptor("usmType", UsmType, UsmTypeVisible);
          usmD[3] = new Descriptor("usmSession", UsmSession, UsmSessionVisible);
          usmD[4] = new Descriptor("usmProcess", UsmProcess, UsmProcessVisible);
          usmD[5] = new Descriptor("usmDescription", UsmDescription, UsmDescriptionVisible);
          return usmD;
      }

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

