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
using System.Threading;
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

        RenamePlatformDialog dlgRenamePlatformDialog;

        int FontGroupHeight = 100;

        bool ignoreAggressiveFocusChanges = false;
        bool ignoreSteamShortcutFields = false;

        Thread worker = null;

        public EditForm()
        {
            InitializeComponent();

            this.Text = String.Format("{0} ({1})", this.Text, Assembly.GetExecutingAssembly().GetName().Version.ToString());

            FontGroupHeight = gbFonts.Height;

            //try
            //{
            //    SteamContext.Init();
            //}
            //catch { }

            LoadSettings();
            UpdateLaunchOptionsList();
            UpdateSteamShortcutList();
            loggingToolStripMenuItem.Checked = settings.logging;
            LoadProgramItem(null);


            dlgRenamePlatformDialog = new RenamePlatformDialog();


            btnSteamUserDataFind.Enabled = true;// SteamContext.GetInstance().CanGetUserShortcutFile;
            settingsFirstLoad();

            txtLaunchBoxLibrary.Text = settings.Core.LaunchBoxLibrary;

            settings.PropertyChanged += settings_PropertyChanged;
        }

        ~EditForm()
        {
            //try
            //{
            //    SteamContext.Shutdown();
            //}
            //catch { }
            SteamContext.GetInstance().Shutdown();
        }

        #region MenuStrip
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about.ShowDialog();
        }
        #endregion MenuStrip

        #region Settings
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

        private void settings_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "Core.SteamShortcutFilePath":
                    txtSteamUserData.TextChanged -= txtSteamUserData_TextChanged;
                    txtSteamUserData.Text = settings.Core.SteamShortcutFilePath;
                    txtSteamUserData.TextChanged += txtSteamUserData_TextChanged;
                    break;
                case "Core.LaunchBoxLibrary":
                    txtLaunchBoxLibrary.TextChanged -= txtLaunchBoxLibrary_TextChanged;
                    txtLaunchBoxLibrary.Text = settings.Core.LaunchBoxLibrary;
                    txtLaunchBoxLibrary.TextChanged += txtLaunchBoxLibrary_TextChanged;
                    btnScrapeLaunchBox.Enabled = settings.Core.LaunchBoxLibrary != null && File.Exists(settings.Core.LaunchBoxLibrary) && (worker == null || !worker.IsAlive);
                    break;
            }
            SaveSettings();
        }

        private void settingsFirstLoad()
        {
            txtSteamUserData.Text = settings.Core.SteamShortcutFilePath;
            txtLaunchBoxLibrary.Text = settings.Core.LaunchBoxLibrary;
        }
        #endregion Settings

        #region CoreTab
        private void btnSteamUserDataFind_Click(object sender, EventArgs e)
        {
            try
            {
                string path = SteamContext.GetInstance().GetUserShortcutFile();
                if (path != null)
                {
                    settings.Core.SteamShortcutFilePath = path;
                }
            }
            catch { }
        }
        private void btnSteamUserDataBrowse_Click(object sender, EventArgs e)
        {
            if (ofdSteamUserDataShortcuts.ShowDialog() == DialogResult.OK)
            {
                settings.Core.SteamShortcutFilePath = ofdSteamUserDataShortcuts.FileName;
            }
        }
        private void txtSteamUserData_TextChanged(object sender, EventArgs e)
        {
            settings.Core.SteamShortcutFilePath = txtSteamUserData.Text;
        }

        private void btnLaunchBoxLibraryBrowse_Click(object sender, EventArgs e)
        {
            if (ofdLaunchBoxLibrary.ShowDialog() == DialogResult.OK)
            {
                settings.Core.LaunchBoxLibrary = ofdLaunchBoxLibrary.FileName;
            }
        }
        private void txtLaunchBoxLibrary_TextChanged(object sender, EventArgs e)
        {
            settings.Core.LaunchBoxLibrary = txtLaunchBoxLibrary.Text;
        }
        #endregion CoreTab

        #region SteamShortcutNamesTab
        private void btnScrapeLaunchBox_Click(object sender, EventArgs e)
        {
            if (worker == null || !worker.IsAlive)
            {
                btnScrapeLaunchBox.Enabled = false;
                pbScrapeLaunchBox.Enabled = true;
                List<GameNameData> gameDat = null;

                pbScrapeLaunchBox.Maximum = 100;
                pbScrapeLaunchBox.Value = 100;
                pbScrapeLaunchBox.Style = ProgressBarStyle.Marquee;

                worker = new Thread(() =>
                {
                    LaunchBoxLibrary lib = new LaunchBoxLibrary(settings.Core.LaunchBoxLibrary);

                    lib.Progress += delegate (object _sender, int counter, int total)
                    {
                        MethodInvoker mi = new MethodInvoker(() =>
                        {
                            pbScrapeLaunchBox.Style = ProgressBarStyle.Blocks;
                            pbScrapeLaunchBox.Maximum = total;
                            pbScrapeLaunchBox.Value = counter;
                        });
                        if (pbScrapeLaunchBox.InvokeRequired)
                        {
                            pbScrapeLaunchBox.Invoke(mi);
                        }
                        else
                        {
                            mi.Invoke();
                        }
                    };

                    gameDat = lib.GetGameData();
                    File.WriteAllText("names_launchbox.json", JsonConvert.SerializeObject(gameDat));
                    this.Invoke((MethodInvoker)delegate
                    {
                        pbScrapeLaunchBox.Value = 0;
                        pbScrapeLaunchBox.Maximum = 0;
                        worker = null;
                        pbScrapeLaunchBox.Enabled = false;
                        btnScrapeLaunchBox.Enabled = settings.Core.LaunchBoxLibrary != null && File.Exists(settings.Core.LaunchBoxLibrary) && (worker == null || !worker.IsAlive);
                    });


                    /*MethodInvoker miDone = new MethodInvoker(() =>
                    {
                        worker = null;
                        btnScrapeLaunchBox.Enabled = settings.Core.LaunchBoxLibrary != null && File.Exists(settings.Core.LaunchBoxLibrary) && (worker == null || !worker.IsAlive);
                    });
                    if (btnScrapeLaunchBox.InvokeRequired)
                    {
                        btnScrapeLaunchBox.Invoke(miDone);
                    }
                    else
                    {
                        miDone.Invoke();
                    }*/
                });
                worker.Start();
            }
        }

        private void btnRemoveAllProxyShortcuts_Click(object sender, EventArgs e)
        {
            if (settings.Core.SteamShortcutFilePath != null && File.Exists(settings.Core.SteamShortcutFilePath))
            {
                string proxyPath;
                {
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    proxyPath = Path.GetFullPath(path);
                    proxyPath = Path.Combine(Path.GetDirectoryName(proxyPath), "SteamProxy.exe");
                }

                List<SteamShortcut> shortcuts = SteamContext.GetInstance().GetShortcutsForExe(proxyPath, settings.Core.SteamShortcutFilePath);
                SteamContext.GetInstance().RemoveShortcuts(shortcuts, settings.Core.SteamShortcutFilePath);
            }
        }

        private void btnAddAllProxyShortcutsWithPlatform_Click(object sender, EventArgs e)
        {
            if (settings.Core.SteamShortcutFilePath != null && File.Exists(settings.Core.SteamShortcutFilePath))
            {
                string proxyPath;
                {
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    proxyPath = Path.GetFullPath(path);
                    proxyPath = Path.Combine(Path.GetDirectoryName(proxyPath), "SteamProxy.exe");
                }

                List<GameNameData> launchbox_names;
                if (File.Exists("names_launchbox.json"))
                {
                    launchbox_names = JsonConvert.DeserializeObject<List<GameNameData>>(File.ReadAllText("names_launchbox.json"));
                }
                else
                {
                    launchbox_names = new List<GameNameData>();
                }


                
                List<SteamShortcut> newData = launchbox_names.Select(dr =>
                {
                    string title = dr.Title;
                    if (!string.IsNullOrWhiteSpace(dr.Platform))
                    {
                        string platform = dr.Platform;
                        if(settings.PlatformRenames.ContainsKey(dr.Platform))
                        {
                            platform = settings.PlatformRenames[dr.Platform];
                        }
                        if (!string.IsNullOrWhiteSpace(platform))
                        {
                            title += @" (" + platform + @")";
                        }
                    }
                    return title;
                }).Distinct().Select(name => new SteamShortcut(name, proxyPath, Path.GetDirectoryName(proxyPath), proxyPath, string.Empty, true, true, false, new List<string>() { "GameLaunchProxy" })).ToList();
                if(newData.Count > 0)
                {
                    try
                    {
                        SteamContext.GetInstance().AddShortcuts(newData, settings.Core.SteamShortcutFilePath);
                    }
                    catch (SteamException)
                    {
                        MessageBox.Show("Error adding shortcuts to running Steam instance.\r\nNote: some shortcuts may have been added.\r\nIf error persists, please close Steam to utilize alternative methods.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnAddDefault_Click(object sender, EventArgs e)
        {
            if (settings.Core.SteamShortcutFilePath != null && File.Exists(settings.Core.SteamShortcutFilePath))
            {
                string proxyPath;
                {
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    proxyPath = Path.GetFullPath(path);
                    proxyPath = Path.Combine(Path.GetDirectoryName(proxyPath), "SteamProxy.exe");
                }

                List<SteamShortcut> newData = new List<SteamShortcut>();
                newData.Add(new SteamShortcut("SteamProxy", proxyPath, Path.GetDirectoryName(proxyPath), proxyPath, string.Empty, true, true, false, new List<string>() { "GameLaunchProxy" }));
                if (newData.Count > 0)
                {
                    try
                    {
                        SteamContext.GetInstance().AddShortcuts(newData, settings.Core.SteamShortcutFilePath);
                    }
                    catch (SteamException)
                    {
                        MessageBox.Show("Error adding shortcuts to running Steam instance.\r\nNote: some shortcuts may have been added.\r\nIf error persists, please close Steam to utilize alternative methods.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnAddPlatforms_Click(object sender, EventArgs e)
        {
            if (settings.Core.SteamShortcutFilePath != null && File.Exists(settings.Core.SteamShortcutFilePath))
            {
                string proxyPath;
                {
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    proxyPath = Path.GetFullPath(path);
                    proxyPath = Path.Combine(Path.GetDirectoryName(proxyPath), "SteamProxy.exe");
                }

                List<SteamShortcut> newData = settings.PlatformRenames.Values.Where(dr => !string.IsNullOrWhiteSpace(dr)).Distinct()
                    .Select(name => new SteamShortcut(name, proxyPath, Path.GetDirectoryName(proxyPath), proxyPath, string.Empty, true, true, false, new List<string>() { "GameLaunchProxy" })).ToList();
                if (newData.Count > 0)
                {
                    try
                    {
                        SteamContext.GetInstance().AddShortcuts(newData, settings.Core.SteamShortcutFilePath);
                    }
                    catch (SteamException)
                    {
                        MessageBox.Show("Error adding shortcuts to running Steam instance.\r\nNote: some shortcuts may have been added.\r\nIf error persists, please close Steam to utilize alternative methods.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnLoadPlatformNameAdjustment_Click(object sender, EventArgs e)
        {
            RefreshPlatformNameAdjustmenList();
        }
        private void RefreshPlatformNameAdjustmenList()
        {
            int selectedIndex = lbPlatformNameAdjustment.SelectedIndex;

            List<GameNameData> launchbox_names = JsonConvert.DeserializeObject<List<GameNameData>>(File.ReadAllText("names_launchbox.json"));
            List<string> launchbox_platforms = launchbox_names.Select(dr => dr.Platform).Where(dr => !string.IsNullOrWhiteSpace(dr)).Distinct().ToList();

            launchbox_platforms.ForEach(plat =>
            {
                if (!settings.PlatformRenames.ContainsKey(plat))
                    settings.PlatformRenames[plat] = plat;
            });

            lbPlatformNameAdjustment.BeginUpdate();
            lbPlatformNameAdjustment.Items.Clear();
            settings.PlatformRenames.OrderBy(dr => dr.Key).ToList().ForEach(dr => lbPlatformNameAdjustment.Items.Add(new PlatformRenameItem(dr.Key, dr.Value)));
            lbPlatformNameAdjustment.EndUpdate();

            SaveSettings();

            if (selectedIndex < lbPlatformNameAdjustment.Items.Count)
            {
                lbPlatformNameAdjustment.SelectedIndex = selectedIndex;
            }
            else
            {
                lbPlatformNameAdjustment.SelectedIndex = lbPlatformNameAdjustment.Items.Count - 1;
            }
        }
        private void btnEditPlatformNameAdjustment_Click(object sender, EventArgs e)
        {
            object selectedItem = lbPlatformNameAdjustment.SelectedItem;
            if(selectedItem != null)
            {
                PlatformRenameItem selectedKey = (PlatformRenameItem)selectedItem;
                if(settings.PlatformRenames.ContainsKey(selectedKey.key))
                {
                    dlgRenamePlatformDialog.PlatformName = selectedKey.value;
                    if(dlgRenamePlatformDialog.ShowDialog() == DialogResult.OK)
                    {
                        selectedKey.value = dlgRenamePlatformDialog.PlatformName;
                        settings.PlatformRenames[selectedKey.key] = dlgRenamePlatformDialog.PlatformName;
                        SaveSettings();
                        lbPlatformNameAdjustment.Items[lbPlatformNameAdjustment.SelectedIndex] = lbPlatformNameAdjustment.SelectedItem;
                        //RefreshPlatformNameAdjustmenList();
                    }
                }
                else
                {
                    RefreshPlatformNameAdjustmenList();
                }
            }
        }
        private void btnRemovePlatformNameAdjustment_Click(object sender, EventArgs e)
        {
            object selectedItem = lbPlatformNameAdjustment.SelectedItem;
            if (selectedItem != null)
            {
                PlatformRenameItem selectedKey = (PlatformRenameItem)selectedItem;
                if (settings.PlatformRenames.ContainsKey(selectedKey.key))
                {
                    settings.PlatformRenames.Remove(selectedKey.key);
                    SaveSettings();
                    lbPlatformNameAdjustment.Items.Remove(selectedKey);
                }
                else
                {
                    RefreshPlatformNameAdjustmenList();
                }
            }
        }
        #endregion SteamShortcutNamesTab


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
                txtLaunchBoxPath.Text = selectedShortcut.LaunchPath.Replace("-steamproxyactivate", "-steamproxysetup") + " -steamproxyname \"<Name for Steam>\" <original emulator command line>";

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
                    selectedShortcut.ID = selectedShortcut.GenerateGameID();
                    ignoreSteamShortcutFields = true;
                    txtSteamShortcutShortcut.Text = selectedShortcut.ID.ToString();
                    ignoreSteamShortcutFields = false;
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
                    selectedShortcut.ID = selectedShortcut.GenerateGameID();
                    ignoreSteamShortcutFields = true;
                    txtSteamShortcutShortcut.Text = selectedShortcut.ID.ToString();
                    ignoreSteamShortcutFields = false;
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

        private void loggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings.logging = !settings.logging;
            loggingToolStripMenuItem.Checked = settings.logging;
            SaveSettings();
        }
    }

    internal class PlatformRenameItem
    {
        public string key;
        public string value;

        public PlatformRenameItem(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public override string ToString()
        {
            return key.PadRight(40) + "\t=>\t" + value.PadLeft(40);
        }
    }
}
