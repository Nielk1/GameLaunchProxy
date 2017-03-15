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
using System.Text.RegularExpressions;
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

        RenamePlatformDialog dlgRenamePlatformDialog;

        int FontGroupHeight = 100;

        bool ignoreAggressiveFocusChanges = false;

        Thread worker = null;
        static internal volatile bool KillWorker = false;

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
            loggingToolStripMenuItem.Checked = settings.logging;
            LoadProgramItem(null);


            dlgRenamePlatformDialog = new RenamePlatformDialog();


            btnSteamUserDataFind.Enabled = true;// SteamContext.GetInstance().CanGetUserShortcutFile;
            settingsFirstLoad();

            txtLaunchBoxLibrary.Text = settings.Core.LaunchBoxLibrary;
            txt7z.Text = settings.Core.SevenZipLib;

            settings.PropertyChanged += settings_PropertyChanged;

            UpdateTxtFrontEndShortcut();
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
                    btnScrapeLaunchBox.Enabled = settings.Core.LaunchBoxLibrary != null
                          && (File.Exists(settings.Core.LaunchBoxLibrary) || ((Path.GetFileName(settings.Core.LaunchBoxLibrary) == "Data")
                                                                               && Directory.Exists(settings.Core.LaunchBoxLibrary)))
                          && (worker == null || !worker.IsAlive);
                    break;
                case "Core.SevenZipLib":
                    txt7z.TextChanged -= txt7z_TextChanged;
                    txt7z.Text = settings.Core.SevenZipLib;
                    txt7z.TextChanged += txt7z_TextChanged;
                    //btn7z.Enabled = settings.Core.SevenZipLib != null && File.Exists(settings.Core.SevenZipLib) && (worker == null || !worker.IsAlive);
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
                if(Path.GetFileName(ofdLaunchBoxLibrary.FileName) == "LaunchBox.xml")
                {
                    string DataPath = Path.Combine(Path.GetDirectoryName(ofdLaunchBoxLibrary.FileName), "Data");
                    if(Directory.Exists(DataPath))
                    {
                        settings.Core.LaunchBoxLibrary = DataPath;
                    }
                    else
                    {
                        settings.Core.LaunchBoxLibrary = ofdLaunchBoxLibrary.FileName;
                    }
                }
                else
                {
                    string DataPath = Path.GetDirectoryName(ofdLaunchBoxLibrary.FileName);
                    if (Directory.Exists(DataPath))
                    {
                        settings.Core.LaunchBoxLibrary = DataPath;
                    }
                }
            }
        }
        private void txtLaunchBoxLibrary_TextChanged(object sender, EventArgs e)
        {
            settings.Core.LaunchBoxLibrary = txtLaunchBoxLibrary.Text;
        }

        private void btn7z_Click(object sender, EventArgs e)
        {
            if (ofd7z.ShowDialog() == DialogResult.OK)
            {
                settings.Core.SevenZipLib = ofd7z.FileName;
            }
        }
        private void txt7z_TextChanged(object sender, EventArgs e)
        {
            settings.Core.SevenZipLib = txt7z.Text;
        }
        #endregion CoreTab

        #region SteamShortcutNamesTab
        private void btnScrapeLaunchBox_Click(object sender, EventArgs e)
        {
            StartLaunchBoxScrape(settings.Core.LaunchBoxLibrary);
        }

        private void StartLaunchBoxScrape(string dataFilePath)
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
                    LaunchBoxLibrary lib = new LaunchBoxLibrary(dataFilePath, settings);
                    DateTime time = DateTime.UtcNow;

                    lib.Progress += delegate (object _sender, int counter, int total)
                    {
                        if (this == null || this.IsDisposed || KillWorker) return;

                        MethodInvoker mi = new MethodInvoker(() =>
                        {
                            pbScrapeLaunchBox.Style = ProgressBarStyle.Blocks;
                            pbScrapeLaunchBox.Maximum = total;
                            pbScrapeLaunchBox.Value = counter;

                            {
                                TimeSpan elap = DateTime.UtcNow - time;

                                int percent = (int)(((double)(pbScrapeLaunchBox.Value - pbScrapeLaunchBox.Minimum) /
                                (double)(pbScrapeLaunchBox.Maximum - pbScrapeLaunchBox.Minimum)) * 100);

                                TimeSpan? remain = counter > 0 ? (TimeSpan?)TimeSpan.FromMilliseconds((elap.TotalMilliseconds * total / counter) - elap.TotalMilliseconds) : null;

                                using (Graphics gr = pbScrapeLaunchBox.CreateGraphics())
                                {
                                    gr.DrawString(percent.ToString() + "%" + (remain.HasValue ? " (" + remain.Value.ToString("hh\\:mm\\:ss") + " est remaining)" : string.Empty),
                                        SystemFonts.DefaultFont,
                                        Brushes.Black,
                                        new PointF(pbScrapeLaunchBox.Width / 2 - (gr.MeasureString(percent.ToString() + "%",
                                            SystemFonts.DefaultFont).Width / 2.0F),
                                        pbScrapeLaunchBox.Height / 2 - (gr.MeasureString(percent.ToString() + "%",
                                            SystemFonts.DefaultFont).Height / 2.0F)));
                                }
                            }
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

                    List<GameNameData> knownData;
                    if (File.Exists("names_launchbox.json"))
                    {
                        knownData = JsonConvert.DeserializeObject<List<GameNameData>>(File.ReadAllText("names_launchbox.json"));
                    }
                    else
                    {
                        knownData = new List<GameNameData>();
                    }

                    gameDat = lib.GetGameData();
                    if (KillWorker) return;

                    Dictionary<string, List<GameNameData>> idMapDat = new Dictionary<string, List<GameNameData>>();
                    gameDat.ForEach(dr =>
                    {
                        if (!idMapDat.ContainsKey(dr.GUID)) idMapDat.Add(dr.GUID, new List<GameNameData>());
                        idMapDat[dr.GUID].Add(dr);
                    });
                    gameDat = gameDat.Union(knownData.Where(dr =>
                    {
                        return !idMapDat[dr.GUID].Any(dx => dx.Equals(dr)); // I don't trust the hash code enough
                    })).ToList();

                    File.WriteAllText("names_launchbox.json", JsonConvert.SerializeObject(gameDat));
                    this.Invoke((MethodInvoker)delegate
                    {
                        pbScrapeLaunchBox.Value = 0;
                        pbScrapeLaunchBox.Maximum = 0;
                        worker = null;
                        pbScrapeLaunchBox.Enabled = false;
                        btnScrapeLaunchBox.Enabled = settings.Core.LaunchBoxLibrary != null
                                                  && (File.Exists(settings.Core.LaunchBoxLibrary) || (   (Path.GetFileName(settings.Core.LaunchBoxLibrary) == "Data")
                                                                                                       && Directory.Exists(settings.Core.LaunchBoxLibrary)))
                                                  && (worker == null || !worker.IsAlive);
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

        private void btnClearLaunchBoxCache_Click(object sender, EventArgs e)
        {
            if (File.Exists("names_launchbox.json"))
            {
                File.Delete("names_launchbox.json");
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

        private void btnAddAllProxyShortcutsWithPlatformPrefix_Click(object sender, EventArgs e)
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
                        if (settings.PlatformRenames.ContainsKey(dr.Platform))
                        {
                            platform = settings.PlatformRenames[dr.Platform];
                        }
                        if (!string.IsNullOrWhiteSpace(platform))
                        {
                            title = @"[" + platform + @"] " + title;
                        }
                    }
                    return title;
                }).Distinct().Select(name => new SteamShortcut(name, proxyPath, Path.GetDirectoryName(proxyPath), proxyPath, string.Empty, true, true, false, new List<string>() { "GameLaunchProxy" })).ToList();
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

        private void loggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings.logging = !settings.logging;
            loggingToolStripMenuItem.Checked = settings.logging;
            SaveSettings();
        }

        private void txtEmulator_TextChanged(object sender, EventArgs e)
        {
            UpdateTxtFrontEndShortcut();
        }

        private void txtCommand_TextChanged(object sender, EventArgs e)
        {
            UpdateTxtFrontEndShortcut();
        }

        private void rbProxyType_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTxtFrontEndShortcut();
        }

        private void UpdateTxtFrontEndShortcut()
        {
            string proxyPath;
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                proxyPath = Path.GetFullPath(path);
                proxyPath = Path.Combine(Path.GetDirectoryName(proxyPath), "GameLaunchProxy.exe");
            }

            string emulator = txtEmulator.Text.Trim().Trim('"');
            string command = txtCommand.Text.Trim();
            string shortEmulatorName = Path.GetFileName(emulator);
            if(command.StartsWith(shortEmulatorName))
            {
                command = command.Replace(shortEmulatorName, "").Trim();
            }

            txtFrontEndShortcut.Text = string.Empty;
            txtFrontEndShortcut.AppendText("\"" + proxyPath + "\"", Color.Red);
            if (rbProxySteam.Checked) txtFrontEndShortcut.AppendText(" -steam", Color.Black);
            if (rbProxyBigPicture.Checked) txtFrontEndShortcut.AppendText(" -steambigpicture", Color.Black);
            txtFrontEndShortcut.AppendText(" -gameid %gameid% -name \"%gamename% (%platformname%)\" -fallbackname \"%platformname%\" -proxy ", Color.Black);
            txtFrontEndShortcut.AppendText("\"" + emulator + "\"", Color.Blue);
            txtFrontEndShortcut.AppendText(" " + command, Color.Green);
        }

        private void btnScrapeLaunchBox_MouseDown(object sender, MouseEventArgs e)
        {
            if(btnScrapeLaunchBox.Enabled)
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        {
                            if (settings.Core.LaunchBoxLibrary != null && settings.Core.LaunchBoxLibrary.Length > 0 && Path.GetFileName(settings.Core.LaunchBoxLibrary) == "Data")
                            {
                                cmsPlatforms.Items.Clear();
                                string platformdir = Path.Combine(settings.Core.LaunchBoxLibrary, "Platforms");
                                string[] platforms = Directory.GetFiles(platformdir);
                                if (platforms.Length > 0)
                                {
                                    platforms.ToList().ForEach(platFile =>
                                    {
                                        ToolStripItem newItem = cmsPlatforms.Items.Add(Path.GetFileNameWithoutExtension(platFile));
                                        string platFile_ = platFile;
                                        newItem.Click += delegate (object sender_, EventArgs e_)
                                        {
                                            StartLaunchBoxScrape(platFile_);
                                        };
                                    });
                                    cmsPlatforms.Show(this, new Point(e.X, e.Y));//places the menu at the pointer position
                                }
                            }
                        }
                        break;
                }
        }

        private void EditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            KillWorker = true;
            while (worker != null && worker.IsAlive)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
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

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}
