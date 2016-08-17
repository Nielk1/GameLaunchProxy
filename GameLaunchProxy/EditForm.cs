using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLaunchProxy
{
    public partial class EditForm : Form
    {
        AboutBox1 about = new AboutBox1();

        Settings settings;

        ProgramSettings selectedItem;

        int FontGroupHeight = 100;

        bool ignoreAggressiveFocusChanges = false;

        public EditForm()
        {
            InitializeComponent();

            FontGroupHeight = gbFonts.Height;

            LoadSettings();
            UpdateList();
            LoadItem(null);
        }

        private void LoadSettings()
        {
            if (File.Exists("settings.json"))
            {
                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));
            }
            else
            {
                settings = new Settings();
            }
        }

        private void SaveSettings()
        {
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
        }

        private void UpdateList()
        {
            lbPrograms.BeginUpdate();
            lbPrograms.Items.Clear();
            settings.Programs.Keys.ToList().ForEach(dr =>
            {
                lbPrograms.Items.Add(dr);
            });
            lbPrograms.EndUpdate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdOpenProgram.ShowDialog() == DialogResult.OK)
            {
                if (!settings.Programs.ContainsKey(ofdOpenProgram.FileName))
                {
                    settings.Programs.Add(ofdOpenProgram.FileName, new ProgramSettings());
                    SaveSettings();
                    UpdateList();
                }
            }
        }

        #region ProgramList
        private void lbPrograms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbPrograms.SelectedIndex > -1)
            {
                LoadItem((string)lbPrograms.SelectedItem);
            }
            else
            {
                LoadItem(null);
            }
        }
        private void LoadItem(string _selectedItem)
        {
            if (_selectedItem != null && settings.Programs.ContainsKey(_selectedItem))
            {
                selectedItem = settings.Programs[_selectedItem];
                UpdateFontList();
                UpdateAggressiveFocus();
            }
            else
            {
                selectedItem = null;
                UpdateFontList();
                UpdateAggressiveFocus();
            }

            if (selectedItem != null)
            {
                lbFont.Items.Clear();
                selectedItem.Fonts.ForEach(dr => lbFont.Items.Add(dr));

                // fonts
                lbFont.Enabled = true;
                btnAddFont.Enabled = true;
                btnRemoveFont.Enabled = true;

                // aggressive focus
                cbAggressiveFocus.Enabled = true;
            }
            else
            {
                lbFont.Items.Clear();

                // fonts
                lbFont.Enabled = false;
                btnAddFont.Enabled = false;
                btnRemoveFont.Enabled = false;

                // aggressive focus
                cbAggressiveFocus.Enabled = false;
            }
        }
        #endregion ProgramList

        #region FontPanel
        private void UpdateFontList()
        {
            lbFont.BeginUpdate();
            lbFont.Items.Clear();
            if (selectedItem != null)
            {
                selectedItem.Fonts.ForEach(dr =>
                {
                    lbFont.Items.Add(dr);
                });
            }
            gbFonts.Height = FontGroupHeight + (lbFont.ItemHeight * lbFont.Items.Count);
            lbFont.EndUpdate();
            tlpMainGrid.Refresh();
        }
        private void btnAddFont_Click(object sender, EventArgs e)
        {
            if (ofdAddFont.ShowDialog() == DialogResult.OK)
            {
                foreach (string fontFilename in ofdAddFont.FileNames)
                {
                    if (File.Exists(fontFilename))
                    {
                        if (!selectedItem.Fonts.Contains(fontFilename))
                        {
                            selectedItem.Fonts.Add(fontFilename);
                        }
                    }
                }
                SaveSettings();
                UpdateFontList();
            }
        }
        private void btnRemoveFont_Click(object sender, EventArgs e)
        {
            if (lbFont.SelectedIndex > -1)
            {
                string selected = (string)lbFont.SelectedItem;
                selectedItem.Fonts.Remove(selected);
                UpdateFontList();
            }
        }
        #endregion FontPanel

        #region AggressiveFocus
        private void UpdateAggressiveFocus()
        {
            ignoreAggressiveFocusChanges = true;
            if (selectedItem != null)
            {
                cbAggressiveFocus.Checked = selectedItem.AggressiveFocus.HasValue;
                if (selectedItem.AggressiveFocus.HasValue)
                {
                    nudAggressiveFocus.Enabled = true;
                    nudAggressiveFocus.Value = selectedItem.AggressiveFocus.Value;
                }
            }
            else
            {
                cbAggressiveFocus.Checked = false;
            }
            ignoreAggressiveFocusChanges = false;
        }
        private void cbAggressiveFocus_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreAggressiveFocusChanges)
            {
                nudAggressiveFocus.Enabled = cbAggressiveFocus.Checked;
                selectedItem.AggressiveFocus = nudAggressiveFocus.Enabled ? (int?)nudAggressiveFocus.Value : null;
                SaveSettings();
                UpdateAggressiveFocus();
            }
        }
        private void nudAggressiveFocus_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreAggressiveFocusChanges)
            {
                selectedItem.AggressiveFocus = nudAggressiveFocus.Enabled ? (int?)nudAggressiveFocus.Value : null;
                SaveSettings();
                UpdateAggressiveFocus();
            }
        }
        #endregion AggressiveFocus

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about.ShowDialog();
        }
    }
}
