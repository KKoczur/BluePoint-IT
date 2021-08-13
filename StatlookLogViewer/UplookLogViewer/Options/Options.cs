using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace StatlookLogViewer
{
    public partial class Options : Form
    {
        #region Members

        //private OCatalogs PanelCatalogs;
        private readonly Configuration _config;
        // Create a new configuration object
        // and initialize some variables
        private readonly Configuration c = new Configuration();
        private readonly string _osVersion;
        private readonly string _logMap;
        private readonly string _usmMap;

        #endregion Members

        public Options()
        {
            //Odczytanie konfiguracji zapisanej w pliku ustawień 
            if (!File.Exists("config.xml"))
            {
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

        _osVersion = System.Environment.OSVersion.Version.Major.ToString();
	    _logMap = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _config.StatlookLogDirectory;
        _usmMap = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _config.StatlookUsmLogDirectory;
        InitializeComponent();

        }

        private void buttonOptionsCancel_Click(object sender, EventArgs e)
        {
            // Serialize the configuration object to a file
            Configuration.Serialize("config.xml", _config);
            this.Close();
        }

        private void treeViewOptions_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Name == "NCatalogs")
            {
                this.oCatalogs.Dock = DockStyle.Fill;
                TextBox txUplook = (TextBox)(oCatalogs.Controls.Find("textBoxUplookCatalog", true))[0];
                txUplook.Text = _logMap;
                TextBox txUsm = (TextBox)(oCatalogs.Controls.Find("textBoxUSMCatalog", true))[0];
                txUsm.Text = _usmMap;
                TextBox txUser= (TextBox)(oCatalogs.Controls.Find("textBoxUserCatalog",true))[0];
                txUser.Text = _config.UserDirectory;
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
