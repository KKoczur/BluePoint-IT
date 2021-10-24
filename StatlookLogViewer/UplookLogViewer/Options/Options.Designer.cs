namespace StatlookLogViewer
{
    partial class Options
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.buttonOptionsSave = new System.Windows.Forms.Button();
            this.buttonOptionsCancel = new System.Windows.Forms.Button();
            this.splitContainerOptions = new System.Windows.Forms.SplitContainer();
            this.treeViewOptions = new System.Windows.Forms.TreeView();
            this.oCatalogs = new StatlookLogViewer.OCatalogs();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerOptions)).BeginInit();
            this.splitContainerOptions.Panel1.SuspendLayout();
            this.splitContainerOptions.Panel2.SuspendLayout();
            this.splitContainerOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOptionsSave
            // 
            resources.ApplyResources(this.buttonOptionsSave, "buttonOptionsSave");
            this.buttonOptionsSave.Name = "buttonOptionsSave";
            this.buttonOptionsSave.UseVisualStyleBackColor = true;
            this.buttonOptionsSave.Click += new System.EventHandler(this.buttonOptionsSave_Click);
            // 
            // buttonOptionsCancel
            // 
            resources.ApplyResources(this.buttonOptionsCancel, "buttonOptionsCancel");
            this.buttonOptionsCancel.Name = "buttonOptionsCancel";
            this.buttonOptionsCancel.UseVisualStyleBackColor = true;
            this.buttonOptionsCancel.Click += new System.EventHandler(this.buttonOptionsCancel_Click);
            // 
            // splitContainerOptions
            // 
            resources.ApplyResources(this.splitContainerOptions, "splitContainerOptions");
            this.splitContainerOptions.Name = "splitContainerOptions";
            // 
            // splitContainerOptions.Panel1
            // 
            resources.ApplyResources(this.splitContainerOptions.Panel1, "splitContainerOptions.Panel1");
            this.splitContainerOptions.Panel1.Controls.Add(this.treeViewOptions);
            // 
            // splitContainerOptions.Panel2
            // 
            resources.ApplyResources(this.splitContainerOptions.Panel2, "splitContainerOptions.Panel2");
            this.splitContainerOptions.Panel2.Controls.Add(this.oCatalogs);
            // 
            // treeViewOptions
            // 
            resources.ApplyResources(this.treeViewOptions, "treeViewOptions");
            this.treeViewOptions.Name = "treeViewOptions";
            this.treeViewOptions.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("treeViewOptions.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("treeViewOptions.Nodes1")))});
            this.treeViewOptions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewOptions_AfterSelect);
            // 
            // oCatalogs
            // 
            resources.ApplyResources(this.oCatalogs, "oCatalogs");
            this.oCatalogs.BackColor = System.Drawing.SystemColors.Control;
            this.oCatalogs.Name = "oCatalogs";
            // 
            // Options
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerOptions);
            this.Controls.Add(this.buttonOptionsCancel);
            this.Controls.Add(this.buttonOptionsSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Options";
            this.splitContainerOptions.Panel1.ResumeLayout(false);
            this.splitContainerOptions.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerOptions)).EndInit();
            this.splitContainerOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOptionsSave;
        private System.Windows.Forms.Button buttonOptionsCancel;
        private System.Windows.Forms.SplitContainer splitContainerOptions;
        private System.Windows.Forms.TreeView treeViewOptions;
        private OCatalogs oCatalogs;
    }
}