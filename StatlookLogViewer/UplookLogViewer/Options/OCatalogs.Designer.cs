namespace StatlookLogViewer
{
    partial class OCatalogs
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxCatalogs = new System.Windows.Forms.GroupBox();
            this.buttonUserCatalog = new System.Windows.Forms.Button();
            this.textBoxUserCatalog = new System.Windows.Forms.TextBox();
            this.labelUserCatalog = new System.Windows.Forms.Label();
            this.folderBrowserDialogOptionsC = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBoxFilesType = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxUSMCatalog = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxUplookCatalog = new System.Windows.Forms.TextBox();
            this.groupBoxCatalogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxCatalogs
            // 
            this.groupBoxCatalogs.Controls.Add(this.buttonUserCatalog);
            this.groupBoxCatalogs.Controls.Add(this.textBoxUplookCatalog);
            this.groupBoxCatalogs.Controls.Add(this.textBoxUSMCatalog);
            this.groupBoxCatalogs.Controls.Add(this.textBoxUserCatalog);
            this.groupBoxCatalogs.Controls.Add(this.label2);
            this.groupBoxCatalogs.Controls.Add(this.label1);
            this.groupBoxCatalogs.Controls.Add(this.labelUserCatalog);
            this.groupBoxCatalogs.Location = new System.Drawing.Point(4, 4);
            this.groupBoxCatalogs.Name = "groupBoxCatalogs";
            this.groupBoxCatalogs.Size = new System.Drawing.Size(457, 188);
            this.groupBoxCatalogs.TabIndex = 0;
            this.groupBoxCatalogs.TabStop = false;
            this.groupBoxCatalogs.Text = "Katalogi:";
            // 
            // buttonUserCatalog
            // 
            this.buttonUserCatalog.Location = new System.Drawing.Point(420, 150);
            this.buttonUserCatalog.Name = "buttonUserCatalog";
            this.buttonUserCatalog.Size = new System.Drawing.Size(29, 23);
            this.buttonUserCatalog.TabIndex = 2;
            this.buttonUserCatalog.Text = "...";
            this.buttonUserCatalog.UseVisualStyleBackColor = true;
            this.buttonUserCatalog.Click += new System.EventHandler(this.ButtonUserCatalog_Click);
            // 
            // textBoxUserCatalog
            // 
            this.textBoxUserCatalog.Location = new System.Drawing.Point(23, 152);
            this.textBoxUserCatalog.Name = "textBoxUserCatalog";
            this.textBoxUserCatalog.Size = new System.Drawing.Size(391, 20);
            this.textBoxUserCatalog.TabIndex = 1;
            // 
            // labelUserCatalog
            // 
            this.labelUserCatalog.AutoSize = true;
            this.labelUserCatalog.Location = new System.Drawing.Point(6, 136);
            this.labelUserCatalog.Name = "labelUserCatalog";
            this.labelUserCatalog.Size = new System.Drawing.Size(186, 13);
            this.labelUserCatalog.TabIndex = 0;
            this.labelUserCatalog.Text = "Domyślny katalog logów użytkownika:";
            // 
            // groupBoxFilesType
            // 
            this.groupBoxFilesType.Location = new System.Drawing.Point(7, 198);
            this.groupBoxFilesType.Name = "groupBoxFilesType";
            this.groupBoxFilesType.Size = new System.Drawing.Size(454, 103);
            this.groupBoxFilesType.TabIndex = 1;
            this.groupBoxFilesType.TabStop = false;
            this.groupBoxFilesType.Text = "Typy plików";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Domyślny katalog logów uplook system monitor:";
            // 
            // textBoxUSMCatalog
            // 
            this.textBoxUSMCatalog.Location = new System.Drawing.Point(23, 95);
            this.textBoxUSMCatalog.Name = "textBoxUSMCatalog";
            this.textBoxUSMCatalog.ReadOnly = true;
            this.textBoxUSMCatalog.Size = new System.Drawing.Size(391, 20);
            this.textBoxUSMCatalog.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Domyślny katalog logów uplook-a:";
            // 
            // textBoxUplookCatalog
            // 
            this.textBoxUplookCatalog.Location = new System.Drawing.Point(23, 43);
            this.textBoxUplookCatalog.Name = "textBoxUplookCatalog";
            this.textBoxUplookCatalog.ReadOnly = true;
            this.textBoxUplookCatalog.Size = new System.Drawing.Size(391, 20);
            this.textBoxUplookCatalog.TabIndex = 1;
            // 
            // OCatalogs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxFilesType);
            this.Controls.Add(this.groupBoxCatalogs);
            this.Name = "OCatalogs";
            this.Size = new System.Drawing.Size(464, 415);
            this.groupBoxCatalogs.ResumeLayout(false);
            this.groupBoxCatalogs.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxCatalogs;
        private System.Windows.Forms.Label labelUserCatalog;
        private System.Windows.Forms.Button buttonUserCatalog;
        private System.Windows.Forms.TextBox textBoxUserCatalog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogOptionsC;
        private System.Windows.Forms.GroupBox groupBoxFilesType;
        private System.Windows.Forms.TextBox textBoxUplookCatalog;
        private System.Windows.Forms.TextBox textBoxUSMCatalog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;

    }
}
