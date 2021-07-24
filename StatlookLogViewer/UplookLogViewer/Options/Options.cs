using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace UplookLogViewer
{
    public partial class Options : Form
    {
        /// <summary> Zmienne
        /// 
        /// </summary>
        #region Zmienne
        //private OCatalogs PanelCatalogs;
        private Configuration config;
        // Create a new configuration object
        // and initialize some variables
        public Configuration c = new Configuration();
        private string OSVersion;
        private string LogDirectory;
        private string USMDirectory;

        #endregion Zmienne

        public Options()
        {
            //Odczytanie konfiguracji zapisanej w pliku ustawień 
            if (!File.Exists("config.xml"))
            {
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
        OSVersion = System.Environment.OSVersion.Version.Major.ToString();
	    LogDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + config.AplusCLogDirectory;
        USMDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + config.AplusCUSMDirectory;
        InitializeComponent();

        }

        private void buttonOptionsCancel_Click(object sender, EventArgs e)
        {
            // Serialize the configuration object to a file
            Configuration.Serialize("config.xml", config);
            this.Close();
        }

        private void treeViewOptions_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name == "NCatalogs")
            {
                this.oCatalogs.Dock = System.Windows.Forms.DockStyle.Fill;
                TextBox txUplook = (TextBox)(oCatalogs.Controls.Find("textBoxUplookCatalog", true))[0];
                txUplook.Text = LogDirectory;
                TextBox txUsm = (TextBox)(oCatalogs.Controls.Find("textBoxUSMCatalog", true))[0];
                txUsm.Text = USMDirectory;
                TextBox txUser= (TextBox)(oCatalogs.Controls.Find("textBoxUserCatalog",true))[0];
                txUser.Text = config.UserDirectory;
                oCatalogs.Visible = true;
            }
            else
            {
                oCatalogs.Visible = false;
            }
        }

        private void buttonOptionsSave_Click(object sender, EventArgs e)
        {
            TextBox tx = (TextBox)(oCatalogs.Controls.Find("textBoxUserCatalog", true))[0];
            c.UserDirectory = tx.Text;
            // Serialize the configuration object to a file
            Configuration.Serialize("config.xml", c);
            this.Close();
        }
    }
}
