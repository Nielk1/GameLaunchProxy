namespace GameLaunchProxy
{
    partial class EditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAddProgram = new System.Windows.Forms.Button();
            this.btnRemoveProgram = new System.Windows.Forms.Button();
            this.lbPrograms = new System.Windows.Forms.ListBox();
            this.tlpMainGrid = new System.Windows.Forms.TableLayoutPanel();
            this.gbFonts = new System.Windows.Forms.GroupBox();
            this.btnRemoveFont = new System.Windows.Forms.Button();
            this.btnAddFont = new System.Windows.Forms.Button();
            this.lbFont = new System.Windows.Forms.ListBox();
            this.gbAggressiveFocus = new System.Windows.Forms.GroupBox();
            this.nudAggressiveFocus = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cbAggressiveFocus = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ofdOpenProgram = new System.Windows.Forms.OpenFileDialog();
            this.ofdAddFont = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnAddNewSteamShortcut = new System.Windows.Forms.Button();
            this.btnRemoveSteamShortcut = new System.Windows.Forms.Button();
            this.lbSteamShortcuts = new System.Windows.Forms.ListBox();
            this.txtSteamShortcutName = new System.Windows.Forms.TextBox();
            this.txtSteamShortcutTarget = new System.Windows.Forms.TextBox();
            this.txtSteamShortcutShortcut = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLaunchBoxPath = new System.Windows.Forms.TextBox();
            this.loggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tlpMainGrid.SuspendLayout();
            this.gbFonts.SuspendLayout();
            this.gbAggressiveFocus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAggressiveFocus)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1MinSize = 150;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.tlpMainGrid);
            this.splitContainer1.Panel2MinSize = 400;
            this.splitContainer1.Size = new System.Drawing.Size(670, 264);
            this.splitContainer1.SplitterDistance = 150;
            this.splitContainer1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnAddProgram);
            this.panel1.Controls.Add(this.btnRemoveProgram);
            this.panel1.Controls.Add(this.lbPrograms);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(150, 264);
            this.panel1.TabIndex = 1;
            // 
            // btnAddProgram
            // 
            this.btnAddProgram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddProgram.Location = new System.Drawing.Point(75, 241);
            this.btnAddProgram.Name = "btnAddProgram";
            this.btnAddProgram.Size = new System.Drawing.Size(75, 23);
            this.btnAddProgram.TabIndex = 2;
            this.btnAddProgram.Text = "Add New";
            this.btnAddProgram.UseVisualStyleBackColor = true;
            this.btnAddProgram.Click += new System.EventHandler(this.btnAddProgram_Click);
            // 
            // btnRemoveProgram
            // 
            this.btnRemoveProgram.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveProgram.Enabled = false;
            this.btnRemoveProgram.Location = new System.Drawing.Point(0, 241);
            this.btnRemoveProgram.Name = "btnRemoveProgram";
            this.btnRemoveProgram.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveProgram.TabIndex = 1;
            this.btnRemoveProgram.Text = "Remove";
            this.btnRemoveProgram.UseVisualStyleBackColor = true;
            this.btnRemoveProgram.Click += new System.EventHandler(this.btnRemoveProgram_Click);
            // 
            // lbPrograms
            // 
            this.lbPrograms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbPrograms.FormattingEnabled = true;
            this.lbPrograms.HorizontalScrollbar = true;
            this.lbPrograms.Location = new System.Drawing.Point(0, 0);
            this.lbPrograms.Name = "lbPrograms";
            this.lbPrograms.Size = new System.Drawing.Size(150, 238);
            this.lbPrograms.TabIndex = 0;
            this.lbPrograms.SelectedIndexChanged += new System.EventHandler(this.lbPrograms_SelectedIndexChanged);
            // 
            // tlpMainGrid
            // 
            this.tlpMainGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpMainGrid.AutoSize = true;
            this.tlpMainGrid.ColumnCount = 1;
            this.tlpMainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpMainGrid.Controls.Add(this.gbFonts, 0, 0);
            this.tlpMainGrid.Controls.Add(this.gbAggressiveFocus, 0, 1);
            this.tlpMainGrid.Location = new System.Drawing.Point(3, 3);
            this.tlpMainGrid.Name = "tlpMainGrid";
            this.tlpMainGrid.RowCount = 2;
            this.tlpMainGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMainGrid.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMainGrid.Size = new System.Drawing.Size(519, 121);
            this.tlpMainGrid.TabIndex = 2;
            // 
            // gbFonts
            // 
            this.gbFonts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFonts.Controls.Add(this.btnRemoveFont);
            this.gbFonts.Controls.Add(this.btnAddFont);
            this.gbFonts.Controls.Add(this.lbFont);
            this.gbFonts.Location = new System.Drawing.Point(3, 3);
            this.gbFonts.Name = "gbFonts";
            this.gbFonts.Size = new System.Drawing.Size(513, 59);
            this.gbFonts.TabIndex = 1;
            this.gbFonts.TabStop = false;
            this.gbFonts.Text = "Install Temporary Fonts";
            // 
            // btnRemoveFont
            // 
            this.btnRemoveFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveFont.Enabled = false;
            this.btnRemoveFont.Location = new System.Drawing.Point(351, 30);
            this.btnRemoveFont.Name = "btnRemoveFont";
            this.btnRemoveFont.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveFont.TabIndex = 2;
            this.btnRemoveFont.Text = "Remove";
            this.btnRemoveFont.UseVisualStyleBackColor = true;
            this.btnRemoveFont.Click += new System.EventHandler(this.btnRemoveFont_Click);
            // 
            // btnAddFont
            // 
            this.btnAddFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddFont.Enabled = false;
            this.btnAddFont.Location = new System.Drawing.Point(432, 30);
            this.btnAddFont.Name = "btnAddFont";
            this.btnAddFont.Size = new System.Drawing.Size(75, 23);
            this.btnAddFont.TabIndex = 1;
            this.btnAddFont.Text = "Add";
            this.btnAddFont.UseVisualStyleBackColor = true;
            this.btnAddFont.Click += new System.EventHandler(this.btnAddFont_Click);
            // 
            // lbFont
            // 
            this.lbFont.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbFont.Enabled = false;
            this.lbFont.FormattingEnabled = true;
            this.lbFont.Location = new System.Drawing.Point(6, 19);
            this.lbFont.Name = "lbFont";
            this.lbFont.Size = new System.Drawing.Size(501, 4);
            this.lbFont.TabIndex = 0;
            // 
            // gbAggressiveFocus
            // 
            this.gbAggressiveFocus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbAggressiveFocus.Controls.Add(this.nudAggressiveFocus);
            this.gbAggressiveFocus.Controls.Add(this.label1);
            this.gbAggressiveFocus.Controls.Add(this.cbAggressiveFocus);
            this.gbAggressiveFocus.Location = new System.Drawing.Point(3, 68);
            this.gbAggressiveFocus.Name = "gbAggressiveFocus";
            this.gbAggressiveFocus.Size = new System.Drawing.Size(513, 50);
            this.gbAggressiveFocus.TabIndex = 2;
            this.gbAggressiveFocus.TabStop = false;
            this.gbAggressiveFocus.Text = "Aggressive Focus";
            // 
            // nudAggressiveFocus
            // 
            this.nudAggressiveFocus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudAggressiveFocus.Enabled = false;
            this.nudAggressiveFocus.Location = new System.Drawing.Point(430, 17);
            this.nudAggressiveFocus.Name = "nudAggressiveFocus";
            this.nudAggressiveFocus.Size = new System.Drawing.Size(77, 20);
            this.nudAggressiveFocus.TabIndex = 2;
            this.nudAggressiveFocus.ValueChanged += new System.EventHandler(this.nudAggressiveFocus_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(338, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Period (seconds)";
            // 
            // cbAggressiveFocus
            // 
            this.cbAggressiveFocus.AutoSize = true;
            this.cbAggressiveFocus.Enabled = false;
            this.cbAggressiveFocus.Location = new System.Drawing.Point(7, 20);
            this.cbAggressiveFocus.Name = "cbAggressiveFocus";
            this.cbAggressiveFocus.Size = new System.Drawing.Size(56, 17);
            this.cbAggressiveFocus.TabIndex = 0;
            this.cbAggressiveFocus.Text = "Active";
            this.cbAggressiveFocus.UseVisualStyleBackColor = true;
            this.cbAggressiveFocus.CheckedChanged += new System.EventHandler(this.cbAggressiveFocus_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(684, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loggingToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // ofdOpenProgram
            // 
            this.ofdOpenProgram.Filter = "Program|*.exe";
            // 
            // ofdAddFont
            // 
            this.ofdAddFont.Multiselect = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(684, 296);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(676, 270);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Launch Options";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(676, 270);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Steam Shortcut";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panel2);
            this.splitContainer2.Panel1MinSize = 150;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.txtLaunchBoxPath);
            this.splitContainer2.Panel2.Controls.Add(this.label7);
            this.splitContainer2.Panel2.Controls.Add(this.label6);
            this.splitContainer2.Panel2.Controls.Add(this.label5);
            this.splitContainer2.Panel2.Controls.Add(this.label4);
            this.splitContainer2.Panel2.Controls.Add(this.label3);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Panel2.Controls.Add(this.txtSteamShortcutShortcut);
            this.splitContainer2.Panel2.Controls.Add(this.txtSteamShortcutTarget);
            this.splitContainer2.Panel2.Controls.Add(this.txtSteamShortcutName);
            this.splitContainer2.Panel2MinSize = 400;
            this.splitContainer2.Size = new System.Drawing.Size(670, 264);
            this.splitContainer2.SplitterDistance = 150;
            this.splitContainer2.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnAddNewSteamShortcut);
            this.panel2.Controls.Add(this.btnRemoveSteamShortcut);
            this.panel2.Controls.Add(this.lbSteamShortcuts);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(150, 264);
            this.panel2.TabIndex = 2;
            // 
            // btnAddNewSteamShortcut
            // 
            this.btnAddNewSteamShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNewSteamShortcut.Location = new System.Drawing.Point(75, 241);
            this.btnAddNewSteamShortcut.Name = "btnAddNewSteamShortcut";
            this.btnAddNewSteamShortcut.Size = new System.Drawing.Size(75, 23);
            this.btnAddNewSteamShortcut.TabIndex = 2;
            this.btnAddNewSteamShortcut.Text = "Add New";
            this.btnAddNewSteamShortcut.UseVisualStyleBackColor = true;
            this.btnAddNewSteamShortcut.Click += new System.EventHandler(this.btnAddNewSteamShortcut_Click);
            // 
            // btnRemoveSteamShortcut
            // 
            this.btnRemoveSteamShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveSteamShortcut.Enabled = false;
            this.btnRemoveSteamShortcut.Location = new System.Drawing.Point(0, 241);
            this.btnRemoveSteamShortcut.Name = "btnRemoveSteamShortcut";
            this.btnRemoveSteamShortcut.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveSteamShortcut.TabIndex = 1;
            this.btnRemoveSteamShortcut.Text = "Remove";
            this.btnRemoveSteamShortcut.UseVisualStyleBackColor = true;
            this.btnRemoveSteamShortcut.Click += new System.EventHandler(this.btnRemoveSteamShortcut_Click);
            // 
            // lbSteamShortcuts
            // 
            this.lbSteamShortcuts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbSteamShortcuts.FormattingEnabled = true;
            this.lbSteamShortcuts.HorizontalScrollbar = true;
            this.lbSteamShortcuts.Location = new System.Drawing.Point(0, 0);
            this.lbSteamShortcuts.Name = "lbSteamShortcuts";
            this.lbSteamShortcuts.Size = new System.Drawing.Size(150, 238);
            this.lbSteamShortcuts.TabIndex = 0;
            this.lbSteamShortcuts.SelectedIndexChanged += new System.EventHandler(this.lbSteamShortcuts_SelectedIndexChanged);
            // 
            // txtSteamShortcutName
            // 
            this.txtSteamShortcutName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSteamShortcutName.Enabled = false;
            this.txtSteamShortcutName.Location = new System.Drawing.Point(83, 3);
            this.txtSteamShortcutName.Name = "txtSteamShortcutName";
            this.txtSteamShortcutName.Size = new System.Drawing.Size(428, 20);
            this.txtSteamShortcutName.TabIndex = 0;
            this.txtSteamShortcutName.TextChanged += new System.EventHandler(this.txtSteamShortcutName_TextChanged);
            // 
            // txtSteamShortcutTarget
            // 
            this.txtSteamShortcutTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSteamShortcutTarget.Enabled = false;
            this.txtSteamShortcutTarget.Location = new System.Drawing.Point(83, 29);
            this.txtSteamShortcutTarget.Name = "txtSteamShortcutTarget";
            this.txtSteamShortcutTarget.Size = new System.Drawing.Size(428, 20);
            this.txtSteamShortcutTarget.TabIndex = 1;
            this.txtSteamShortcutTarget.TextChanged += new System.EventHandler(this.txtSteamShortcutTarget_TextChanged);
            // 
            // txtSteamShortcutShortcut
            // 
            this.txtSteamShortcutShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSteamShortcutShortcut.Enabled = false;
            this.txtSteamShortcutShortcut.Location = new System.Drawing.Point(182, 55);
            this.txtSteamShortcutShortcut.Name = "txtSteamShortcutShortcut";
            this.txtSteamShortcutShortcut.Size = new System.Drawing.Size(329, 20);
            this.txtSteamShortcutShortcut.TabIndex = 2;
            this.txtSteamShortcutShortcut.TextChanged += new System.EventHandler(this.txtSteamShortcutShortcut_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Target";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Shortcut";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(80, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "steam://rungameid/";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(331, 39);
            this.label6.TabIndex = 7;
            this.label6.Text = "Add the Target as a third party game on Steam.\r\nBe sure the Name and Target match" +
    " between Steam and the above.\r\nYou may customize these values but they must matc" +
    "h.";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 242);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Path for Front End";
            // 
            // txtLaunchBoxPath
            // 
            this.txtLaunchBoxPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLaunchBoxPath.Location = new System.Drawing.Point(102, 239);
            this.txtLaunchBoxPath.Name = "txtLaunchBoxPath";
            this.txtLaunchBoxPath.ReadOnly = true;
            this.txtLaunchBoxPath.Size = new System.Drawing.Size(409, 20);
            this.txtLaunchBoxPath.TabIndex = 9;
            // 
            // loggingToolStripMenuItem
            // 
            this.loggingToolStripMenuItem.Name = "loggingToolStripMenuItem";
            this.loggingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loggingToolStripMenuItem.Text = "Logging";
            this.loggingToolStripMenuItem.Click += new System.EventHandler(this.loggingToolStripMenuItem_Click);
            // 
            // EditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 320);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(700, 358);
            this.Name = "EditForm";
            this.Text = "Game Launch Proxy";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tlpMainGrid.ResumeLayout(false);
            this.gbFonts.ResumeLayout(false);
            this.gbAggressiveFocus.ResumeLayout(false);
            this.gbAggressiveFocus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAggressiveFocus)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox lbPrograms;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog ofdOpenProgram;
        private System.Windows.Forms.GroupBox gbFonts;
        private System.Windows.Forms.Button btnRemoveFont;
        private System.Windows.Forms.Button btnAddFont;
        private System.Windows.Forms.ListBox lbFont;
        private System.Windows.Forms.OpenFileDialog ofdAddFont;
        private System.Windows.Forms.TableLayoutPanel tlpMainGrid;
        private System.Windows.Forms.GroupBox gbAggressiveFocus;
        private System.Windows.Forms.NumericUpDown nudAggressiveFocus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbAggressiveFocus;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAddProgram;
        private System.Windows.Forms.Button btnRemoveProgram;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnAddNewSteamShortcut;
        private System.Windows.Forms.Button btnRemoveSteamShortcut;
        private System.Windows.Forms.ListBox lbSteamShortcuts;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSteamShortcutShortcut;
        private System.Windows.Forms.TextBox txtSteamShortcutTarget;
        private System.Windows.Forms.TextBox txtSteamShortcutName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtLaunchBoxPath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolStripMenuItem loggingToolStripMenuItem;
    }
}

