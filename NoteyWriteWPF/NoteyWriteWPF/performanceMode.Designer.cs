namespace NoteyWriteWPF
{
    partial class performanceMode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(performanceMode));
            this.rtbMain = new System.Windows.Forms.RichTextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tsGeneral = new System.Windows.Forms.ToolStrip();
            this.ddbFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.miNew = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.miSave = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.bBold = new System.Windows.Forms.ToolStripButton();
            this.bItalic = new System.Windows.Forms.ToolStripButton();
            this.bUnderline = new System.Windows.Forms.ToolStripButton();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.cbFontSize = new System.Windows.Forms.ToolStripComboBox();
            this.cbFont = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbMain
            // 
            this.rtbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbMain.Location = new System.Drawing.Point(0, 25);
            this.rtbMain.Name = "rtbMain";
            this.rtbMain.Size = new System.Drawing.Size(624, 416);
            this.rtbMain.TabIndex = 0;
            this.rtbMain.Text = "";
            this.rtbMain.SelectionChanged += new System.EventHandler(this.rtbMain_SelectionChanged);
            // 
            // tsGeneral
            // 
            this.tsGeneral.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ddbFile,
            this.toolStripSeparator1,
            this.cbFont,
            this.cbFontSize,
            this.toolStripSeparator2,
            this.bBold,
            this.bItalic,
            this.bUnderline});
            this.tsGeneral.Location = new System.Drawing.Point(0, 0);
            this.tsGeneral.Name = "tsGeneral";
            this.tsGeneral.Size = new System.Drawing.Size(624, 25);
            this.tsGeneral.TabIndex = 2;
            this.tsGeneral.Text = "toolStrip1";
            // 
            // ddbFile
            // 
            this.ddbFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ddbFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNew,
            this.miOpen,
            this.miSave,
            this.miSaveAs,
            this.miExit});
            this.ddbFile.Image = ((System.Drawing.Image)(resources.GetObject("ddbFile.Image")));
            this.ddbFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ddbFile.Name = "ddbFile";
            this.ddbFile.Size = new System.Drawing.Size(38, 22);
            this.ddbFile.Text = "File";
            // 
            // miNew
            // 
            this.miNew.Name = "miNew";
            this.miNew.Size = new System.Drawing.Size(190, 22);
            this.miNew.Text = "New";
            this.miNew.Click += new System.EventHandler(this.miNew_Click);
            // 
            // miOpen
            // 
            this.miOpen.Name = "miOpen";
            this.miOpen.Size = new System.Drawing.Size(190, 22);
            this.miOpen.Text = "Open...";
            this.miOpen.Click += new System.EventHandler(this.miOpen_Click);
            // 
            // miSave
            // 
            this.miSave.Name = "miSave";
            this.miSave.Size = new System.Drawing.Size(190, 22);
            this.miSave.Text = "Save";
            this.miSave.Click += new System.EventHandler(this.miSave_Click);
            // 
            // miSaveAs
            // 
            this.miSaveAs.Name = "miSaveAs";
            this.miSaveAs.Size = new System.Drawing.Size(190, 22);
            this.miSaveAs.Text = "Save As...";
            this.miSaveAs.Click += new System.EventHandler(this.miSaveAs_Click);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(190, 22);
            this.miExit.Text = "Exit to standard mode";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // bBold
            // 
            this.bBold.CheckOnClick = true;
            this.bBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bBold.Image = ((System.Drawing.Image)(resources.GetObject("bBold.Image")));
            this.bBold.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bBold.Name = "bBold";
            this.bBold.Size = new System.Drawing.Size(23, 22);
            this.bBold.Text = "toolStripButton1";
            this.bBold.CheckedChanged += new System.EventHandler(this.bBold_CheckedChanged);
            // 
            // bItalic
            // 
            this.bItalic.CheckOnClick = true;
            this.bItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bItalic.Image = ((System.Drawing.Image)(resources.GetObject("bItalic.Image")));
            this.bItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bItalic.Name = "bItalic";
            this.bItalic.Size = new System.Drawing.Size(23, 22);
            this.bItalic.Text = "toolStripButton1";
            this.bItalic.CheckedChanged += new System.EventHandler(this.bItalic_CheckedChanged);
            // 
            // bUnderline
            // 
            this.bUnderline.CheckOnClick = true;
            this.bUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bUnderline.Image = ((System.Drawing.Image)(resources.GetObject("bUnderline.Image")));
            this.bUnderline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bUnderline.Name = "bUnderline";
            this.bUnderline.Size = new System.Drawing.Size(23, 22);
            this.bUnderline.Text = "toolStripButton1";
            this.bUnderline.CheckedChanged += new System.EventHandler(this.bUnderline_CheckedChanged);
            // 
            // cbFontSize
            // 
            this.cbFontSize.DropDownWidth = 75;
            this.cbFontSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbFontSize.Items.AddRange(new object[] {
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "22",
            "24",
            "26",
            "28",
            "36",
            "48",
            "72",
            "102",
            "144",
            "288"});
            this.cbFontSize.MaxLength = 3;
            this.cbFontSize.Name = "cbFontSize";
            this.cbFontSize.Size = new System.Drawing.Size(75, 25);
            this.cbFontSize.Text = "8";
            this.cbFontSize.Click += new System.EventHandler(this.cbFontSize_Click);
            this.cbFontSize.TextChanged += new System.EventHandler(this.cbFontSize_TextChanged);
            // 
            // cbFont
            // 
            this.cbFont.Name = "cbFont";
            this.cbFont.Size = new System.Drawing.Size(150, 25);
            this.cbFont.Text = "Microsoft Sans Serif";
            this.cbFont.Click += new System.EventHandler(this.cbFont_Click);
            this.cbFont.TextChanged += new System.EventHandler(this.cbFont_TextChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // performanceMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.rtbMain);
            this.Controls.Add(this.tsGeneral);
            this.Name = "performanceMode";
            this.Text = "Performance Mode - NoteyWrite";
            this.tsGeneral.ResumeLayout(false);
            this.tsGeneral.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbMain;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStrip tsGeneral;
        private System.Windows.Forms.ToolStripDropDownButton ddbFile;
        private System.Windows.Forms.ToolStripMenuItem miNew;
        private System.Windows.Forms.ToolStripMenuItem miOpen;
        private System.Windows.Forms.ToolStripMenuItem miSave;
        private System.Windows.Forms.ToolStripMenuItem miSaveAs;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripButton bUnderline;
        private System.Windows.Forms.ToolStripButton bBold;
        private System.Windows.Forms.ToolStripButton bItalic;
        private System.Windows.Forms.ToolStripComboBox cbFontSize;
        private System.Windows.Forms.ToolStripComboBox cbFont;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}