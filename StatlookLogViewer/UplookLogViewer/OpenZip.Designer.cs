namespace StatlookLogViewer
{
    partial class OpenZip
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
            this.columnHeaderFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderPath1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonZipChoose = new System.Windows.Forms.Button();
            this.buttonZipCancel = new System.Windows.Forms.Button();
            this.labelChooseFiles = new System.Windows.Forms.Label();
            this.columnHeaderSeclect = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.columnHeaderLn1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.checkBoxSelectAll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // columnHeaderFileName
            // 
            this.columnHeaderFileName.Text = "Nazwa pliku";
            this.columnHeaderFileName.Width = 120;
            // 
            // columnHeaderDate
            // 
            this.columnHeaderDate.Text = "Data utworzenia";
            this.columnHeaderDate.Width = 120;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Rozmiar";
            // 
            // columnHeaderPath1
            // 
            this.columnHeaderPath1.Text = "Ścieżka";
            this.columnHeaderPath1.Width = 600;
            // 
            // buttonZipChoose
            // 
            this.buttonZipChoose.Location = new System.Drawing.Point(537, 433);
            this.buttonZipChoose.Name = "buttonZipChoose";
            this.buttonZipChoose.Size = new System.Drawing.Size(75, 23);
            this.buttonZipChoose.TabIndex = 2;
            this.buttonZipChoose.Text = "Choose";
            this.buttonZipChoose.UseVisualStyleBackColor = true;
            this.buttonZipChoose.Click += new System.EventHandler(this.buttonZipChoose_Click);
            // 
            // buttonZipCancel
            // 
            this.buttonZipCancel.Location = new System.Drawing.Point(618, 433);
            this.buttonZipCancel.Name = "buttonZipCancel";
            this.buttonZipCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonZipCancel.TabIndex = 2;
            this.buttonZipCancel.Text = "Cancel";
            this.buttonZipCancel.UseVisualStyleBackColor = true;
            this.buttonZipCancel.Click += new System.EventHandler(this.buttonZipCancel_Click);
            // 
            // labelChooseFiles
            // 
            this.labelChooseFiles.AutoSize = true;
            this.labelChooseFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelChooseFiles.Location = new System.Drawing.Point(9, 9);
            this.labelChooseFiles.Name = "labelChooseFiles";
            this.labelChooseFiles.Size = new System.Drawing.Size(145, 15);
            this.labelChooseFiles.TabIndex = 3;
            this.labelChooseFiles.Text = "Choose files to analyzing:";
            // 
            // columnHeaderSeclect
            // 
            this.columnHeaderSeclect.Text = "X";
            this.columnHeaderSeclect.Width = 25;
            // 
            // listViewFiles
            // 
            this.listViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderLn1,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeaderPath});
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.GridLines = true;
            this.listViewFiles.HideSelection = false;
            this.listViewFiles.Location = new System.Drawing.Point(2, 39);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(691, 388);
            this.listViewFiles.TabIndex = 4;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Details;
            this.listViewFiles.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewFiles_MouseClick);
            this.listViewFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewFiles_MouseDoubleClick);
            // 
            // columnHeaderLn1
            // 
            this.columnHeaderLn1.Text = "Lp.";
            this.columnHeaderLn1.Width = 30;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "File name";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Date";
            this.columnHeader2.Width = 120;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Size";
            // 
            // columnHeaderPath
            // 
            this.columnHeaderPath.Text = "Path";
            this.columnHeaderPath.Width = 500;
            // 
            // checkBoxSelectAll
            // 
            this.checkBoxSelectAll.AutoSize = true;
            this.checkBoxSelectAll.Location = new System.Drawing.Point(12, 437);
            this.checkBoxSelectAll.Name = "checkBoxSelectAll";
            this.checkBoxSelectAll.Size = new System.Drawing.Size(70, 17);
            this.checkBoxSelectAll.TabIndex = 5;
            this.checkBoxSelectAll.Text = "Select All";
            this.checkBoxSelectAll.UseVisualStyleBackColor = true;
            this.checkBoxSelectAll.Click += new System.EventHandler(this.checkBoxSelectAll_Click);
            // 
            // OpenZip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 462);
            this.Controls.Add(this.checkBoxSelectAll);
            this.Controls.Add(this.listViewFiles);
            this.Controls.Add(this.labelChooseFiles);
            this.Controls.Add(this.buttonZipCancel);
            this.Controls.Add(this.buttonZipChoose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "OpenZip";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Please select log source file";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeaderFileName;
        private System.Windows.Forms.ColumnHeader columnHeaderDate;
        private System.Windows.Forms.ColumnHeader columnHeaderSize;
        private System.Windows.Forms.ColumnHeader columnHeaderPath1;
        private System.Windows.Forms.Button buttonZipChoose;
        private System.Windows.Forms.Button buttonZipCancel;
        private System.Windows.Forms.Label labelChooseFiles;
        private System.Windows.Forms.ColumnHeader columnHeaderSeclect;
        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.ColumnHeader columnHeaderLn1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeaderPath;
        private System.Windows.Forms.CheckBox checkBoxSelectAll;
    }
}