using System;
using System.Windows.Forms;


namespace StatlookLogViewer
{
    public partial class Options : Form
    {
        #region Members

        //private OCatalogs PanelCatalogs;

        /// <summary>
        /// Configuration object
        /// </summary>
        private readonly Configuration _config;

        private readonly string _osVersion;
        private readonly string _logMap;
        private readonly string _usmMap;

        #endregion Members

        public Options()
        {
            InitializeComponent();

            _config = Configuration.GetConfiguration();

            _osVersion = System.Environment.OSVersion.Version.Major.ToString();
	        _logMap = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _config.StatlookLogDirectory;
            _usmMap = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _config.StatlookUsmLogDirectory;
        }

        private void buttonOptionsCancel_Click(object sender, EventArgs e)
        {
            // Serialize the configuration object to a file
            Configuration.SaveConfig(_config);
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
            _config.UserDirectory = tx.Text;

            // Serialize the configuration object to a file
            Configuration.SaveConfig(_config);

            this.Close();
        }
    }
}
