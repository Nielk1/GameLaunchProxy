using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLaunchProxy
{
    public partial class EditForm : Form
    {
        AboutBox1 about = new AboutBox1();

        Settings settings;

        string selectedKey;
        ProgramSettings selectedItem;
        SteamShortcutSettings selectedShortcut;

        int FontGroupHeight = 100;

        bool ignoreAggressiveFocusChanges = false;
        bool ignoreSteamShortcutFields = false;

        public EditForm()
        {
            InitializeComponent();

            this.Text = String.Format("{0} ({1})", this.Text, Assembly.GetExecutingAssembly().GetName().Version.ToString());

            FontGroupHeight = gbFonts.Height;

            try
            {
                SteamContext.Init();
            }
            catch { }

            LoadSettings();
            UpdateLaunchOptionsList();
            UpdateSteamShortcutList();
            LoadProgramItem(null);
        }

        ~EditForm()
        {
            try
            {
                SteamContext.Shutdown();
            }
            catch { }
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about.ShowDialog();
        }

        #region ProgramList
        private void UpdateLaunchOptionsList()
        {
            lbPrograms.BeginUpdate();
            lbPrograms.Items.Clear();
            settings.Programs.Keys.ToList().ForEach(dr =>
            {
                lbPrograms.Items.Add(dr);
            });
            lbPrograms.EndUpdate();
        }
        private void lbPrograms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbPrograms.SelectedIndex > -1)
            {
                LoadProgramItem((string)lbPrograms.SelectedItem);
            }
            else
            {
                LoadProgramItem(null);
            }
        }
        private void LoadProgramItem(string _selectedItem)
        {
            if (_selectedItem != null && settings.Programs.ContainsKey(_selectedItem))
            {
                selectedItem = settings.Programs[_selectedItem];
                selectedKey = _selectedItem;
                UpdateFontList();
                UpdateAggressiveFocus();
            }
            else
            {
                selectedItem = null;
                selectedKey = null;
                UpdateFontList();
                UpdateAggressiveFocus();
            }

            if (selectedItem != null)
            {
                lbFont.Items.Clear();
                selectedItem.Fonts.ForEach(dr => lbFont.Items.Add(dr));

                btnRemoveProgram.Enabled = true;

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

                btnRemoveProgram.Enabled = false;

                // fonts
                lbFont.Enabled = false;
                btnAddFont.Enabled = false;
                btnRemoveFont.Enabled = false;

                // aggressive focus
                cbAggressiveFocus.Enabled = false;
            }
        }
        private void btnAddProgram_Click(object sender, EventArgs e)
        {
            if (ofdOpenProgram.ShowDialog() == DialogResult.OK)
            {
                if (!settings.Programs.ContainsKey(ofdOpenProgram.FileName))
                {
                    settings.Programs.Add(ofdOpenProgram.FileName, new ProgramSettings());
                    SaveSettings();
                    UpdateLaunchOptionsList();
                }
            }
        }
        private void btnRemoveProgram_Click(object sender, EventArgs e)
        {
            if (selectedItem != null)
            {
                settings.Programs.Remove(selectedKey);
                SaveSettings();
                UpdateLaunchOptionsList();
                LoadProgramItem(null);
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

        #region SteamShortcutList
        private void UpdateSteamShortcutList()
        {
            lbSteamShortcuts.BeginUpdate();
            lbSteamShortcuts.Items.Clear();
            settings.SteamShortcuts.ToList().ForEach(dr =>
            {
                lbSteamShortcuts.Items.Add(dr);
            });
            lbSteamShortcuts.EndUpdate();
        }
        private void lbSteamShortcuts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbSteamShortcuts.SelectedIndex > -1)
            {
                LoadSteamShortcutItem(lbSteamShortcuts.SelectedIndex);
            }
            else
            {
                LoadSteamShortcutItem(-1);
            }
        }
        private void LoadSteamShortcutItem(int _selectedItem)
        {
            if (_selectedItem >= 0 && settings.SteamShortcuts.Count > _selectedItem)
            {
                selectedShortcut = settings.SteamShortcuts[_selectedItem];
                //selectedKey = _selectedItem;
            }
            else
            {
                selectedShortcut = null;
                //selectedKey = null;
            }

            ignoreSteamShortcutFields = true;
            if (selectedShortcut != null)
            {
                btnRemoveSteamShortcut.Enabled = true;

                txtSteamShortcutName.Text = selectedShortcut.Name;
                txtSteamShortcutTarget.Text = selectedShortcut.LaunchPath;
                txtSteamShortcutShortcut.Text = selectedShortcut.ID.ToString();
                txtLaunchBoxPath.Text = selectedShortcut.LaunchPath.Replace("-steamproxy", "-steamproxystart") + " -steamproxyname \"<Name for Steam>\" <original emulator command line>";

                txtSteamShortcutName.Enabled = true;
                txtSteamShortcutTarget.Enabled = true;
                txtSteamShortcutShortcut.Enabled = true;
            }
            else
            {
                btnRemoveSteamShortcut.Enabled = false;

                txtSteamShortcutName.Enabled = false;
                txtSteamShortcutTarget.Enabled = false;
                txtSteamShortcutShortcut.Enabled = false;

                txtSteamShortcutName.Text = null;
                txtSteamShortcutTarget.Text = null;
                txtSteamShortcutShortcut.Text = null;
                txtLaunchBoxPath.Text = null;
            }
            ignoreSteamShortcutFields = false;
        }
        private void btnAddNewSteamShortcut_Click(object sender, EventArgs e)
        {
            settings.SteamShortcuts.Add(new SteamShortcutSettings());
            SaveSettings();
            UpdateSteamShortcutList();
        }
        private void btnRemoveSteamShortcut_Click(object sender, EventArgs e)
        {
            if (selectedShortcut != null)
            {
                settings.SteamShortcuts.Remove(selectedShortcut);
                SaveSettings();
                UpdateSteamShortcutList();
                LoadSteamShortcutItem(-1);
            }
        }
        #endregion SteamShortcutList

        #region Edit Steam Shortcut
        private void txtSteamShortcutName_TextChanged(object sender, EventArgs e)
        {
            if(!ignoreSteamShortcutFields)
            {
                if(selectedShortcut != null)
                {
                    selectedShortcut.Name = txtSteamShortcutName.Text;
                    SaveSettings();
                    UpdateSteamShortcutList();
                }
            }
        }
        private void txtSteamShortcutTarget_TextChanged(object sender, EventArgs e)
        {
            if (!ignoreSteamShortcutFields)
            {
                if (selectedShortcut != null)
                {
                    selectedShortcut.LaunchPath = txtSteamShortcutTarget.Text;
                    SaveSettings();
                }
            }
        }
        private void txtSteamShortcutShortcut_TextChanged(object sender, EventArgs e)
        {
            if (!ignoreSteamShortcutFields)
            {
                if (selectedShortcut != null)
                {
                    try
                    {
                        selectedShortcut.ID = UInt64.Parse(txtSteamShortcutShortcut.Text);
                    }
                    catch
                    {
                        txtSteamShortcutShortcut.Text = selectedShortcut.ID.ToString();
                    }
                    SaveSettings();
                }
            }
        }
        #endregion Edit Steam Shortcut
    }
}
