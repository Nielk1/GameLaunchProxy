namespace GameLaunchProxy
{
    partial class AboutBox1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox1));
            this.okButton = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.multiPanel1 = new Liron.Windows.Forms.MultiPanel();
            this.Page_1 = new Liron.Windows.Forms.MultiPanelPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.labelCompanyName = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.Page_2 = new Liron.Windows.Forms.MultiPanelPage();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.Page_3 = new Liron.Windows.Forms.MultiPanelPage();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.Page_4 = new Liron.Windows.Forms.MultiPanelPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Page_5 = new Liron.Windows.Forms.MultiPanelPage();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.multiPanel1.SuspendLayout();
            this.Page_1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.Page_2.SuspendLayout();
            this.Page_3.SuspendLayout();
            this.Page_4.SuspendLayout();
            this.Page_5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.okButton.Location = new System.Drawing.Point(12, 145);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(128, 31);
            this.okButton.TabIndex = 30;
            this.okButton.Text = "&OK";
            // 
            // radioButton1
            // 
            this.radioButton1.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.radioButton1.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.radioButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.radioButton1.Location = new System.Drawing.Point(146, 9);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(47, 25);
            this.radioButton1.TabIndex = 34;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "About";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButtonPage_CheckedChanged);
            // 
            // multiPanel1
            // 
            this.multiPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.multiPanel1.Controls.Add(this.Page_1);
            this.multiPanel1.Controls.Add(this.Page_2);
            this.multiPanel1.Controls.Add(this.Page_3);
            this.multiPanel1.Controls.Add(this.Page_4);
            this.multiPanel1.Controls.Add(this.Page_5);
            this.multiPanel1.Location = new System.Drawing.Point(146, 40);
            this.multiPanel1.Name = "multiPanel1";
            this.multiPanel1.SelectedPage = this.Page_1;
            this.multiPanel1.Size = new System.Drawing.Size(546, 136);
            this.multiPanel1.TabIndex = 32;
            // 
            // Page_1
            // 
            this.Page_1.Controls.Add(this.tableLayoutPanel1);
            this.Page_1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Page_1.Location = new System.Drawing.Point(0, 0);
            this.Page_1.Name = "Page_1";
            this.Page_1.Size = new System.Drawing.Size(546, 136);
            this.Page_1.TabIndex = 0;
            this.Page_1.Text = "Page_1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.labelProductName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelVersion, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxDescription, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelCompanyName, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelCopyright, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(546, 136);
            this.tableLayoutPanel1.TabIndex = 32;
            // 
            // labelProductName
            // 
            this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelProductName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.labelProductName.Location = new System.Drawing.Point(0, 0);
            this.labelProductName.Margin = new System.Windows.Forms.Padding(0);
            this.labelProductName.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(546, 17);
            this.labelProductName.TabIndex = 19;
            this.labelProductName.Text = "Product Name";
            this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.labelVersion.Location = new System.Drawing.Point(0, 20);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(0);
            this.labelVersion.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(546, 17);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "Version";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.textBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.textBoxDescription.Location = new System.Drawing.Point(0, 80);
            this.textBoxDescription.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDescription.Size = new System.Drawing.Size(546, 56);
            this.textBoxDescription.TabIndex = 23;
            this.textBoxDescription.TabStop = false;
            this.textBoxDescription.Text = "Description";
            // 
            // labelCompanyName
            // 
            this.labelCompanyName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCompanyName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.labelCompanyName.Location = new System.Drawing.Point(0, 60);
            this.labelCompanyName.Margin = new System.Windows.Forms.Padding(0);
            this.labelCompanyName.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelCompanyName.Name = "labelCompanyName";
            this.labelCompanyName.Size = new System.Drawing.Size(546, 17);
            this.labelCompanyName.TabIndex = 22;
            this.labelCompanyName.Text = "Company Name";
            this.labelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCopyright
            // 
            this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCopyright.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.labelCopyright.Location = new System.Drawing.Point(0, 40);
            this.labelCopyright.Margin = new System.Windows.Forms.Padding(0);
            this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 17);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(546, 17);
            this.labelCopyright.TabIndex = 21;
            this.labelCopyright.Text = "Copyright";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Page_2
            // 
            this.Page_2.Controls.Add(this.textBox3);
            this.Page_2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Page_2.Location = new System.Drawing.Point(0, 0);
            this.Page_2.Name = "Page_2";
            this.Page_2.Size = new System.Drawing.Size(546, 136);
            this.Page_2.TabIndex = 1;
            this.Page_2.Text = "Page_2";
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.textBox3.Location = new System.Drawing.Point(0, 0);
            this.textBox3.Margin = new System.Windows.Forms.Padding(0);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox3.Size = new System.Drawing.Size(546, 136);
            this.textBox3.TabIndex = 32;
            this.textBox3.Text = resources.GetString("textBox3.Text");
            // 
            // Page_3
            // 
            this.Page_3.Controls.Add(this.textBox2);
            this.Page_3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Page_3.Location = new System.Drawing.Point(0, 0);
            this.Page_3.Name = "Page_3";
            this.Page_3.Size = new System.Drawing.Size(546, 136);
            this.Page_3.TabIndex = 2;
            this.Page_3.Text = "Page_3";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.textBox2.Location = new System.Drawing.Point(0, 0);
            this.textBox2.Margin = new System.Windows.Forms.Padding(0);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(546, 136);
            this.textBox2.TabIndex = 32;
            this.textBox2.Text = resources.GetString("textBox2.Text");
            // 
            // Page_4
            // 
            this.Page_4.Controls.Add(this.textBox1);
            this.Page_4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Page_4.Location = new System.Drawing.Point(0, 0);
            this.Page_4.Name = "Page_4";
            this.Page_4.Size = new System.Drawing.Size(546, 136);
            this.Page_4.TabIndex = 3;
            this.Page_4.Text = "Page_4";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(546, 136);
            this.textBox1.TabIndex = 32;
            this.textBox1.Text = "https://github.com/Nielk1/open-steamworks";
            // 
            // Page_5
            // 
            this.Page_5.Controls.Add(this.textBox4);
            this.Page_5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Page_5.Location = new System.Drawing.Point(0, 0);
            this.Page_5.Name = "Page_5";
            this.Page_5.Size = new System.Drawing.Size(546, 136);
            this.Page_5.TabIndex = 4;
            this.Page_5.Text = "Page_5";
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.textBox4.Location = new System.Drawing.Point(0, 0);
            this.textBox4.Margin = new System.Windows.Forms.Padding(0);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox4.Size = new System.Drawing.Size(546, 136);
            this.textBox4.TabIndex = 33;
            this.textBox4.Text = "https://github.com/EasyAsABC123/Keyboard";
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Image = global::GameLaunchProxy.Properties.Resources.Nielk1_eyes_128;
            this.logoPictureBox.Location = new System.Drawing.Point(12, 9);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(128, 128);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logoPictureBox.TabIndex = 29;
            this.logoPictureBox.TabStop = false;
            this.logoPictureBox.MouseEnter += new System.EventHandler(this.logoPictureBox_MouseEnter);
            this.logoPictureBox.MouseLeave += new System.EventHandler(this.logoPictureBox_MouseLeave);
            // 
            // radioButton2
            // 
            this.radioButton2.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton2.AutoSize = true;
            this.radioButton2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.radioButton2.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.radioButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.radioButton2.Location = new System.Drawing.Point(199, 9);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(66, 25);
            this.radioButton2.TabIndex = 35;
            this.radioButton2.Text = "Json.NET";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButtonPage_CheckedChanged);
            // 
            // radioButton3
            // 
            this.radioButton3.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton3.AutoSize = true;
            this.radioButton3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.radioButton3.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.radioButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.radioButton3.Location = new System.Drawing.Point(271, 9);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(53, 25);
            this.radioButton3.TabIndex = 36;
            this.radioButton3.Text = "CRC32";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButtonPage_CheckedChanged);
            // 
            // radioButton4
            // 
            this.radioButton4.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton4.AutoSize = true;
            this.radioButton4.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.radioButton4.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.radioButton4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.radioButton4.Location = new System.Drawing.Point(330, 9);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(77, 25);
            this.radioButton4.TabIndex = 37;
            this.radioButton4.Text = "Steam4NET";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.Click += new System.EventHandler(this.radioButtonPage_CheckedChanged);
            // 
            // radioButton5
            // 
            this.radioButton5.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton5.AutoSize = true;
            this.radioButton5.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(54)))), ((int)(((byte)(54)))));
            this.radioButton5.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.radioButton5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButton5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.radioButton5.Location = new System.Drawing.Point(413, 9);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(64, 25);
            this.radioButton5.TabIndex = 38;
            this.radioButton5.Text = "Keyboard";
            this.radioButton5.UseVisualStyleBackColor = true;
            this.radioButton5.Click += new System.EventHandler(this.radioButtonPage_CheckedChanged);
            // 
            // AboutBox1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.ClientSize = new System.Drawing.Size(705, 188);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton3);
            this.Controls.Add(this.radioButton4);
            this.Controls.Add(this.radioButton5);
            this.Controls.Add(this.multiPanel1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.logoPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox1";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AboutBox1";
            this.multiPanel1.ResumeLayout(false);
            this.Page_1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.Page_2.ResumeLayout(false);
            this.Page_2.PerformLayout();
            this.Page_3.ResumeLayout(false);
            this.Page_3.PerformLayout();
            this.Page_4.ResumeLayout(false);
            this.Page_4.PerformLayout();
            this.Page_5.ResumeLayout(false);
            this.Page_5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private Liron.Windows.Forms.MultiPanel multiPanel1;
        private Liron.Windows.Forms.MultiPanelPage Page_1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label labelCompanyName;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private Liron.Windows.Forms.MultiPanelPage Page_2;
        private Liron.Windows.Forms.MultiPanelPage Page_3;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private Liron.Windows.Forms.MultiPanelPage Page_4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton radioButton5;
        private Liron.Windows.Forms.MultiPanelPage Page_5;
        private System.Windows.Forms.TextBox textBox4;
    }
}
