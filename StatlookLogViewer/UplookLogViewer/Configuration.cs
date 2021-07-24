using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace StatlookLogViewer
{
    [Serializable]
    public class Configuration
    {
      string _Version;
      string _AplusCLogDirectory;
      string _AplusCUSMDirectory;
      string _UserDirectory;
      string _FileExtensions;
      string _uplook_Headers;
      string _show_statlook;  
      string _usm_Headers;
      string _show_usm;

      #region HeadersOfUplook
      string _uDate;
      string _uLogger;
      string _uType;
      string _uProcess;
      string _uThread;
      string _uDescription;
      string _uException;
      string _uMessage;
      string _uMethod;
      string _uStack;
      bool _uDateVisible;
      bool _uLoggerVisible;
      bool _uTypeVisible;
      bool _uProcessVisible;
      bool _uThreadVisible;
      bool _uDescriptionVisible;
      bool _uExceptionVisible;
      bool _uMessageVisible;
      bool _uMethodVisible;
      bool _uStackVisible;
      #endregion HeadersOfUplook

      #region HeadersOfUsm
      string _usmDate;
      string _usmCode;
      string _usmType;
      string _usmSession;
      string _usmProcess;
      string _usmDescription;
      bool _usmDateVisible;
      bool _usmCodeVisible;
      bool _usmTypeVisible;
      bool _usmSessionVisible;
      bool _usmProcessVisible;
      bool _usmDescriptionVisible;
      #endregion HeadersOfUsm

      public Configuration()
      {
          _Version = "1.0.1";
          _AplusCLogDirectory = "\\Statlook\\Logs\\";
          _AplusCUSMDirectory = "\\Statlook\\Logs\\";
          _UserDirectory = "C:\\Users\\Karol\\Desktop\\Logs\\";
          _FileExtensions= "*.log;*.zip";
          _uplook_Headers = "Date; Logger:; Type:; PID/TID:; Thread ID:; Description:; Exception:;   Message:;   Method:;   Stack:";
          _show_statlook = "false;true;true;true;true;true;true;true;true;true";
          _usm_Headers = "Date; Code:; Type:; Session:; PID/TID:; Description:";
          _show_usm = "false;false;true;false;false;true;";

        #region HeadersOfUplook
          _uDate = "Date";
          _uLogger = " Logger:";
          _uType = " Type:";
          _uProcess = " Process ID:";
          _uThread = " Thread ID:";
          _uDescription = " Description:";
          _uException = " Exception:";
          _uMessage = "   Message:";
          _uMethod = "   Method:";
          _uStack = "   Stack:";
          _uDateVisible = true;
          _uLoggerVisible = true;
          _uTypeVisible = true;
          _uProcessVisible = true;
          _uThreadVisible = true;
          _uDescriptionVisible = true;
          _uExceptionVisible = true;
          _uMessageVisible = true;
          _uMethodVisible = true;
          _uStackVisible = true;
          #endregion HeadersOfUplook

        #region HeadersOfUsm
          _usmDate="Date";
          _usmCode = " Code:";
          _usmType = " Type:";
          _usmSession = " Session:";
          _usmProcess = " Process ID:";
          _usmDescription = " Description:";
          _usmDateVisible = true;
          _usmCodeVisible = true;
          _usmTypeVisible = true;
          _usmSessionVisible = true;
          _usmProcessVisible = true;
          _usmDescriptionVisible=true;
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
      public string Version
      {
         get { return _Version; }
         set { _Version = value; }
      }
      public string AplusCLogDirectory
      {
          get { return _AplusCLogDirectory; }
          set { _AplusCLogDirectory = value; }
      }
      public string AplusCUSMDirectory
      {
          get { return _AplusCUSMDirectory; }
          set { _AplusCUSMDirectory = value; }
      }
      public string UserDirectory
      {
          get { return _UserDirectory; }
          set { _UserDirectory = value; }
      }
      public string FileExtensions
      {
          get { return _FileExtensions; }
          set { _FileExtensions = value; }
      }
      public string uplook_Headers
      {
          get { return _uplook_Headers; }
          set { _uplook_Headers = value; }
      }
      public string show_uplook
      {
          get { return _show_statlook; }
          set { _show_statlook = value; }
      }
      public string usm_Headers
      {
          get { return _usm_Headers; }
          set { _usm_Headers = value; }
      }
      public string show_usm
      {
          get { return _show_usm; }
          set { _show_usm = value; }
      }

      #region HeadersOfUplook
      public string UDate
      {
          get { return _uDate; }
          set { _uDate = value; }
      }
      public string ULogger
      {
          get { return _uLogger; }
          set { _uLogger = value; }
      }
      public string UType
      {
          get { return _uType; }
          set { _uType = value; }
      }
      public string UProcess
      {
          get { return _uProcess; }
          set { _uProcess = value; }
      }
      public string UThread
      {
          get { return _uThread; }
          set { _uThread = value; }
      }
      public string UDescription
      {
          get { return _uDescription; }
          set { _uDescription = value; }
      }
      public string UException
      {
          get { return _uException; }
          set { _uException = value; }
      }
      public string UMessage
      {
          get { return _uMessage; }
          set { _uMessage = value; }
      }
      public string UMethod
      {
          get { return _uMethod; }
          set { _uMethod = value; }
      }
      public string UStack
      {
          get { return _uStack; }
          set { _uStack = value; }
      }
      public bool UDateVisible
      {
          get { return _uDateVisible; }
          set { _uDateVisible = value; }
      }
      public bool ULoggerVisible
      {
          get { return _uLoggerVisible; }
          set { _uLoggerVisible = value; }
      }
      public bool UTypeVisible
      {
          get { return _uTypeVisible; }
          set { _uTypeVisible = value; }
      }
      public bool UProcessVisible
      {
          get { return _uProcessVisible; }
          set { _uProcessVisible = value; }
      }
      public bool UThreadVisible
      {
          get { return _uThreadVisible; }
          set { _uThreadVisible = value; }
      }
      public bool UDescriptionVisible
      {
          get { return _uDescriptionVisible; }
          set { _uDescriptionVisible = value; }
      }
      public bool UExceptionVisible
      {
          get { return _uExceptionVisible; }
          set { _uExceptionVisible = value; }
      }
      public bool UMessageVisible
      {
          get { return _uMessageVisible; }
          set { _uMessageVisible = value; }
      }
      public bool UMethodVisible
      {
          get { return _uMethodVisible; }
          set { _uMethodVisible = value; }
      }
      public bool UStackVisible
      {
          get { return _uStackVisible; }
          set { _uStackVisible = value; }
      }
      #endregion HeadersOfUplook

      #region HeadersOfUsm
      public string usmDate
      {
          get { return _usmDate; }
          set { _usmDate = value; }
      }
      public string usmCode
      {
          get { return _usmCode; }
          set { _usmCode = value; }
      }
      public string usmType
      {
          get { return _usmType; }
          set { _usmType = value; }
      }
      public string usmSession
      {
          get { return _usmSession; }
          set { _usmSession = value; }
      }
      public string usmProcess
      {
          get { return _usmProcess; }
          set { _usmProcess = value; }
      }
      public string usmDescription
      {
          get { return _usmDescription; }
          set { _usmDescription = value; }
      }
      public bool usmDateVisible
        {
          get { return _usmDateVisible; }
          set { _usmDateVisible = value; }
        }
      public bool usmCodeVisible
        {
          get { return _usmCodeVisible; }
          set { _usmCodeVisible = value; }
        }
      public bool usmTypeVisible
        {
          get { return _usmTypeVisible; }
          set { _usmTypeVisible = value; }
        }
      public bool usmSessionVisible
        {
          get { return _usmSessionVisible; }
          set { _usmSessionVisible = value; }
        }
      public bool usmProcessVisible
        {
          get { return _usmProcessVisible; }
          set { _usmProcessVisible = value; }
        }
      public bool usmDescriptionVisible
        {
          get { return _usmDescriptionVisible; }
          set { _usmDescriptionVisible = value; }
        }
      #endregion HeadersOfUsm

      public Descriptor[] UReadHeaders()
      {
          Descriptor[] uD = new Descriptor[10];
          uD[0] = new Descriptor("uDate", UDate, UDateVisible);
          uD[1] = new Descriptor("uLogger", ULogger, ULoggerVisible);
          uD[2] = new Descriptor("uType", UType, UTypeVisible);
          uD[3] = new Descriptor("uProcess", UProcess, UProcessVisible);
          uD[4] = new Descriptor("uThread", UThread, UThreadVisible);
          uD[5] = new Descriptor("uDescription", UDescription, UDescriptionVisible);
          uD[6] = new Descriptor("uException", UException, UExceptionVisible);
          uD[7] = new Descriptor("uMessage", UMessage, UMessageVisible);
          uD[8] = new Descriptor("uMethod", UMethod, UMethodVisible);
          uD[9] = new Descriptor("uStack", UStack, UStackVisible);
          return uD;
      }
      public Descriptor[] USMReadHeaders()
      {
          Descriptor[] usmD = new Descriptor[6];
          usmD[0] = new Descriptor("usmDate", usmDate, usmDateVisible);
          usmD[1] = new Descriptor("usmCode", usmCode, usmCodeVisible);
          usmD[2] = new Descriptor("usmType", usmType, usmTypeVisible);
          usmD[3] = new Descriptor("usmSession", usmSession, usmSessionVisible);
          usmD[4] = new Descriptor("usmProcess", usmProcess, usmProcessVisible);
          usmD[5] = new Descriptor("usmDescription", usmDescription, usmDescriptionVisible);
          return usmD;
      }

      public void uShow(string NameOfHeder,bool Show)
      {
          //if(NameOfHeder=="uDate")
          //{
          //  uDateVisible = Show;
          //}
          //else 
		  if(NameOfHeder=="uLogger")
          {
              ULoggerVisible=Show;
          }
          else if(NameOfHeder=="uType")
          {
              UTypeVisible=Show;
          }
          else if(NameOfHeder=="uProcess")
          {
              UProcessVisible=Show;
          }
          else if(NameOfHeder=="uThread")
          {
              UThreadVisible=Show;
          }
          else if (NameOfHeder=="uDescription")
          {
              UDescriptionVisible=Show;
          }
          else if (NameOfHeder=="uException")
          {
              UExceptionVisible=Show;
          }
          else if (NameOfHeder=="uMessage")
          {
              UMessageVisible=Show;
          }
          else if (NameOfHeder=="uMethod")
          {
              UMethodVisible=Show;
          }
          else if (NameOfHeder=="uStack")
          {
              UStackVisible=Show;
          }

      }
      public void usmShow(string NameOfHeder, bool Show)
      {
          //if (NameOfHeder == "usmDate")
          //{
          //    usmDateVisible = Show;
          //}
          //else 
		  if (NameOfHeder == "usmCode")
          {
              usmCodeVisible = Show;
          }
          else if (NameOfHeder == "usmType")
          {
              usmTypeVisible = Show;
          }
          else if (NameOfHeder == "usmSession")
          {
              usmSessionVisible = Show;
          }
          else if (NameOfHeder == "usmProcess")
          {
              usmProcessVisible = Show;
          }
          else if (NameOfHeder == "usmDescription")
          {
              usmDescriptionVisible = Show;
          }         
      }

   }
}

