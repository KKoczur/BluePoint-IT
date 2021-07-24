namespace UplookLogViewer
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Ogólne");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Deskryptory");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.buttonOptionsSave = new System.Windows.Forms.Button();
            this.buttonOptionsCancel = new System.Windows.Forms.Button();
            this.splitContainerOptions = new System.Windows.Forms.SplitContainer();
            this.treeViewOptions = new System.Windows.Forms.TreeView();
            this.oCatalogs = new UplookLogViewer.OCatalogs();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerOptions)).BeginInit();
            this.splitContainerOptions.Panel1.SuspendLayout();
            this.splitContainerOptions.Panel2.SuspendLayout();
            this.splitContainerOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOptionsSave
            // 
            this.buttonOptionsSave.Location = new System.Drawing.Point(537, 433);
            this.buttonOptionsSave.Name = "buttonOptionsSave";
            this.buttonOptionsSave.Size = new System.Drawing.Size(75, 23);
            this.buttonOptionsSave.TabIndex = 0;
            this.buttonOptionsSave.Text = "Save";
            this.buttonOptionsSave.UseVisualStyleBackColor = true;
            this.buttonOptionsSave.Click += new System.EventHandler(this.buttonOptionsSave_Click);
            // 
            // buttonOptionsCancel
            // 
            this.buttonOptionsCancel.Location = new System.Drawing.Point(618, 433);
            this.buttonOptionsCancel.Name = "buttonOptionsCancel";
            this.buttonOptionsCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonOptionsCancel.TabIndex = 1;
            this.buttonOptionsCancel.Text = "Cancel";
            this.buttonOptionsCancel.UseVisualStyleBackColor = true;
            this.buttonOptionsCancel.Click += new System.EventHandler(this.buttonOptionsCancel_Click);
            // 
            // splitContainerOptions
            // 
            this.splitContainerOptions.Location = new System.Drawing.Point(12, 12);
            this.splitContainerOptions.Name = "splitContainerOptions";
            // 
            // splitContainerOptions.Panel1
            // 
            this.splitContainerOptions.Panel1.Controls.Add(this.treeViewOptions);
            // 
            // splitContainerOptions.Panel2
            // 
            this.splitContainerOptions.Panel2.Controls.Add(this.oCatalogs);
            this.splitContainerOptions.Size = new System.Drawing.Size(672, 415);
            this.splitContainerOptions.SplitterDistance = 208;
            this.splitContainerOptions.TabIndex = 2;
            // 
            // treeViewOptions
            // 
            this.treeViewOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewOptions.Location = new System.Drawing.Point(0, 0);
            this.treeViewOptions.Name = "treeViewOptions";
            treeNode1.Name = "NCatalogs";
            treeNode1.Text = "Ogólne";
            treeNode2.Name = "Node0";
            treeNode2.Text = "Deskryptory";
            this.treeViewOptions.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            this.treeViewOptions.Size = new System.Drawing.Size(208, 415);
            this.treeViewOptions.TabIndex = 0;
            this.treeViewOptions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewOptions_AfterSelect);
            // 
            // oCatalogs
            // 
            this.oCatalogs.BackColor = System.Drawing.SystemColors.Control;
            this.oCatalogs.Location = new System.Drawing.Point(16, 15);
            this.oCatalogs.Name = "oCatalogs";
            this.oCatalogs.Size = new System.Drawing.Size(430, 80);
            this.oCatalogs.TabIndex = 0;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 462);
            this.Controls.Add(this.splitContainerOptions);
            this.Controls.Add(this.buttonOptionsCancel);
            this.Controls.Add(this.buttonOptionsSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Options";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Options";
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