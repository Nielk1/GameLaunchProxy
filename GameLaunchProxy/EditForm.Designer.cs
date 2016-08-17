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
            this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ofdOpenProgram = new System.Windows.Forms.OpenFileDialog();
            this.ofdAddFont = new System.Windows.Forms.OpenFileDialog();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tlpMainGrid.SuspendLayout();
            this.gbFonts.SuspendLayout();
            this.gbAggressiveFocus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAggressiveFocus)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbPrograms);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.tlpMainGrid);
            this.splitContainer1.Panel2MinSize = 400;
            this.splitContainer1.Size = new System.Drawing.Size(684, 288);
            this.splitContainer1.SplitterDistance = 155;
            this.splitContainer1.TabIndex = 0;
            // 
            // lbPrograms
            // 
            this.lbPrograms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbPrograms.FormattingEnabled = true;
            this.lbPrograms.HorizontalScrollbar = true;
            this.lbPrograms.Location = new System.Drawing.Point(0, 0);
            this.lbPrograms.Name = "lbPrograms";
            this.lbPrograms.Size = new System.Drawing.Size(155, 288);
            this.lbPrograms.TabIndex = 0;
            this.lbPrograms.SelectedIndexChanged += new System.EventHandler(this.lbPrograms_SelectedIndexChanged);
            // 
            // tlpMainGrid
            // 
            this.tlpMainGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpMainGrid.AutoSize = true;
            this.tlpMainGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
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
            this.addNewToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // addNewToolStripMenuItem
            // 
            this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
            this.addNewToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addNewToolStripMenuItem.Text = "&Add New";
            this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // ofdOpenProgram
            // 
            this.ofdOpenProgram.Filter = "Program|*.exe";
            // 
            // ofdAddFont
            // 
            this.ofdAddFont.Multiselect = true;
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // EditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 312);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = Properties.Resources.GameLaunchProxy;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(700, 350);
            this.Name = "EditForm";
            this.Text = "Game Launch Proxy";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tlpMainGrid.ResumeLayout(false);
            this.gbFonts.ResumeLayout(false);
            this.gbAggressiveFocus.ResumeLayout(false);
            this.gbAggressiveFocus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAggressiveFocus)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox lbPrograms;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
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
    }
}

