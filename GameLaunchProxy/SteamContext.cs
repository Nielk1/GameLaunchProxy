using Microsoft.Win32;
using Steam4NET;
using System;
using System.Collections.Generic;
using System.Data.HashFunction;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using HSteamPipe = System.Int32;
//using HSteamUser = System.Int32;

namespace GameLaunchProxy
{
    class SteamException : Exception
    {
        public SteamException(string msg)
            : base(msg)
        {
        }
    }
    public class SteamContext
    {
        private Int32 Pipe { get; set; }
        private Int32 User { get; set; }

        private ISteamClient017 SteamClient { get; set; }
        private ISteamUser017 SteamUser { get; set; }

        private IClientEngine ClientEngine { get; set; }
        private IClientShortcuts ClientShortcuts { get; set; }

        /// <summary>
        /// Get Big Picture PID from Registy
        /// </summary>
        /// <value>
        /// Window ID of Big Picture, 0 if not in use
        /// </value>
        /// <remarks>
        /// Same as Steam's PID if in big picture, else 0
        /// </remarks>
        public Int32 BigPicturePID
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam");
                if (key != null)
                {
                    return (Int32)(key.GetValue("BigPictureInForeground"));
                }
                return 0;
            }
        }
        /// <summary>
        /// Get Steam Process ID
        /// </summary>
        /// <value>
        /// Steam Process ID
        /// </value>
        public Int32 SteamPID
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam\\ActiveProcess");
                if (key != null)
                {
                    return (Int32)(key.GetValue("pid"));
                }
                return 0;
            }
        }

        /// <summary>
        /// Is Steam4NET Loaded?
        /// </summary>
        public bool IsSteam4NETLoaded
        {
            get
            {
                return AppDomain.CurrentDomain
                                .GetAssemblies()
                                .Where(a => a.FullName.Contains("Steam4NET"))
                                .Select(a => String.Format(
                                    CultureInfo.InvariantCulture,
                                    "[{0}] {1}",
                                    a.GlobalAssemblyCache,
                                    a.FullName
                                )).ToArray().Count() > 0;
            }
        }
        /// <summary>
        /// Steam is loaded
        /// </summary>
        public bool IsSteamLoaded { get; private set; }
        /// <summary>
        /// Steam client is loaded
        /// </summary>
        public bool IsSteamClientLoaded { get; private set; }

        /// <summary>
        /// SteamClient Interface Exists
        /// </summary>
        public bool HasSteamClientInterface { get { return SteamClient != null; } }
        /// <summary>
        /// SteamUser Interface Exists
        /// </summary>
        public bool HasSteamUserInterface { get { return SteamUser != null; } }

        /// <summary>
        /// ClientEngine Interface Exists
        /// </summary>
        public bool HasClientEngineInterface { get { return ClientEngine != null; } }
        /// <summary>
        /// ClientShortcuts Interface Exists
        /// </summary>
        public bool HasClientShortcutsInterface { get { return ClientShortcuts != null; } }

        /// <summary>
        /// Current active user
        /// </summary>
        public UInt32 CurrentUserID
        {
            get
            {
                try
                {
                    if (IsSteam4NETLoaded && HasClientShortcutsInterface)
                    {
                        CSteamID id = SteamUser.GetSteamID();
                        string installPath = Steamworks.GetInstallPath();
                        if (id != null && installPath != null && id.AccountID > 0)
                        {
                            return id.AccountID;
                        }
                    }
                }
                catch { }

                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam\\ActiveProcess");
                if (key != null)
                {
                    object _possibleValue = key.GetValue("ActiveUser");
                    if (_possibleValue != null)
                    {
                        UInt32 possibleValue = (UInt32)_possibleValue;
                        if(possibleValue > 0)
                        {
                            return possibleValue;
                        }
                    }
                }

                return 0;
            }
        }

        /*public bool CanGetUserShortcutFile
        {
            get
            {
                return IsSteam4NETLoaded && SteamUser != null;
            }
        }*/



        private static readonly object CoreInstanceMutex = new object();
        private static SteamContext CoreInstance;
        private SteamContext()
        {
            Init(); // attempt a free init
        }
        ~SteamContext()
        {
            Shutdown();
        }
        public static SteamContext GetInstance()
        {
            lock(CoreInstanceMutex)
            {
                if (CoreInstance == null)
                    CoreInstance = new SteamContext();
                return CoreInstance;
            }
        }

        public void Init()
        {
            if(!IsSteam4NETLoaded)
                return;

            if (SteamPID == 0)
                return;

            if (!Steamworks.LoadSteam())
                return;
            IsSteamLoaded = true;

            if (Steamworks.LoadSteamClient())
                IsSteamClientLoaded = true;

            if (IsSteamLoaded)
            {
                if (SteamClient == null)
                {
                    SteamClient = Steamworks.CreateInterface<ISteamClient017>();
                }

                if (SteamClient != null)
                {
                    #region Pipe
                    if (Pipe == 0)
                    {
                        Pipe = SteamClient.CreateSteamPipe();

                        if (Pipe == 0)
                        {
                            throw new SteamException("Unable to create steam pipe.");
                        }
                    }
                    #endregion Pipe
                    #region User
                    if (User == 0 || User == -1)
                    {
                        User = SteamClient.ConnectToGlobalUser(Pipe);

                        if (User == 0 || User == -1)
                        {
                            throw new SteamException("Unable to connect to global user.");
                        }
                    }
                    #endregion User
                    if (Pipe > 0 && User > 0)
                    {
                        #region SteamUser
                        if (SteamUser == null)
                        {
                            SteamUser = SteamClient.GetISteamUser<ISteamUser017>(User, Pipe);
                        }
                        #endregion SteamUser
                    }
                }
            }

            if (IsSteamClientLoaded)
            {
                if (ClientEngine == null)
                {
                    ClientEngine = Steamworks.CreateInterface<IClientEngine>();
                }

                if (ClientEngine != null)
                {
                    if (ClientShortcuts == null)
                    {
                        ClientShortcuts = ClientEngine.GetIClientShortcuts<IClientShortcuts>(User, Pipe);
                    }
                }
            }
        }
        public void Shutdown()
        {
            //CallbackDispatcher.StopDispatchThread(Pipe);

            SteamUser = null;
            if (ClientEngine != null && User != 0 && User != -1)
            {
                ClientEngine.ReleaseUser(Pipe, User);
            }
            if (ClientEngine != null && Pipe != 0)
            {
                ClientEngine.BReleaseSteamPipe(Pipe);
            }

            ClientShortcuts = null;
            if (SteamClient != null && User != 0 && User != -1)
            {
                SteamClient.ReleaseUser(Pipe, User);
            }
            if (SteamClient != null && Pipe != 0)
            {
                SteamClient.BReleaseSteamPipe(Pipe);
            }
        }

        /// <summary>
        /// User Data Folder
        /// </summary>
        public string GetUserShortcutFile()
        {
            uint userid = CurrentUserID;

            if (IsSteam4NETLoaded)
            {
                try
                {
                    string installPath = Steamworks.GetInstallPath();
                    string shortcutFile = Path.Combine(installPath, @"userdata", userid.ToString(), @"config", @"shortcuts.vdf");
                    if (File.Exists(shortcutFile))
                        return shortcutFile;
                }
                catch { }
            }

            {
                string installPath = null;
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Valve\\Steam");
                if (key == null)
                {
                    key = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Valve\\Steam");
                }
                if (key != null)
                {
                    installPath = key.GetValue("InstallPath").ToString();
                }
                if (string.IsNullOrWhiteSpace(installPath))
                {
                    key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam");
                    if (key != null)
                    {
                        installPath = key.GetValue("SteamPath").ToString();
                    }
                }
                if (installPath == null)
                    return null;

                {
                    string basePath = Path.Combine(installPath, @"userdata");
                    if (userid == 0)
                    {
                        string path = Directory.GetDirectories(basePath).OrderByDescending(dr => new DirectoryInfo(dr).LastAccessTimeUtc).FirstOrDefault();
                        if (path != null)
                        {
                            string shortcutFile = Path.Combine(path, @"config", @"shortcuts.vdf");
                            if (File.Exists(shortcutFile))
                                return shortcutFile;
                        }
                    }
                    {
                        string shortcutFile = Path.Combine(basePath, userid.ToString(), @"config", @"shortcuts.vdf");
                        if (File.Exists(shortcutFile))
                            return shortcutFile;
                    }
                }
            }

            return null;
        }

        public bool AddShortcuts(List<SteamShortcut> shortcuts, string shortcutFilePath)
        {
            if (SteamPID > 0)
            {
                if (IsSteam4NETLoaded && HasClientShortcutsInterface)
                {
                    try
                    {
                        //int counter = 0;
                        shortcuts.ForEach(shortcut =>
                        {
                            uint shortcutid;
                            if (shortcut.OpenVR)
                            {
                                shortcutid = ClientShortcuts.AddOpenVRShortcut(shortcut.appname, shortcut.exe, shortcut.icon, shortcut.ShortcutPath);
                            }
                            else
                            {
                                shortcutid = ClientShortcuts.AddShortcut(
                                    shortcut.appname,
                                    shortcut.exe.Trim('"'), // system adds quotes automaticly
                                    shortcut.icon.Trim('"'), // system adds quotes automaticly
                                    shortcut.ShortcutPath.Trim('"'));
                            }
                            ClientShortcuts.SetShortcutHidden(shortcutid, shortcut.hidden);
                            ClientShortcuts.SetShortcutIcon(shortcutid, shortcut.icon);
                            ClientShortcuts.SetAllowDesktopConfig(shortcutid, shortcut.AllowDesktopConfig);
                            shortcut.tags.ForEach(tag => ClientShortcuts.AddShortcutUserTag(shortcutid, tag));
                            //counter++;

                            //if(counter % 200 == 0)
                            //{
                            //    Thread.Sleep(1000);
                            //}
                        });
                        return true;
                    }
                    catch
                    {
                        throw new SteamException("Error Adding Shortcut");
                        //return false;
                    }
                }
                else
                {
                    return false; // can't access API, can't edit file as Steam is open
                }
            }
            else
            {
                if (shortcutFilePath == null)
                    return false;

                VPropertyCollection ShortcutFile = SteamShortcutDataFile.Read(shortcutFilePath);

                VPropertyCollection shortcutVData = (VPropertyCollection)ShortcutFile["shortcuts"];

                shortcuts.Distinct().ToList().ForEach(shortcut =>
                {
                    VPropertyCollection shortcutData = new VPropertyCollection();
                    shortcutData["appname"] = shortcut.appname; // name
                    shortcutData["exe"] = shortcut.exe; // full path
                    shortcutData["StartDir"] = shortcut.StartDir; // working folder
                    shortcutData["icon"] = shortcut.icon; // icon path
                    shortcutData["ShortcutPath"] = shortcut.ShortcutPath; // empty
                    shortcutData["IsHidden"] = shortcut.hidden ? 1 : 0; // 
                    shortcutData["AllowDesktopConfig"] = shortcut.AllowDesktopConfig ? 1 : 0; // 
                    shortcutData["OpenVR"] = shortcut.OpenVR ? 1 : 0; // 

                    VPropertyCollection tagData = new VPropertyCollection();
                    shortcut.tags.ForEach(tag => tagData.Add(tag));

                    shortcutData["tags"] = tagData; // 

                    shortcutVData.Add(shortcutData);
                });

                File.Copy(shortcutFilePath, shortcutFilePath + DateTime.UtcNow.ToString(".yyyyMMddHHmmss"));
                SteamShortcutDataFile.Write(shortcutFilePath, ShortcutFile);
                return true;
            }
        }

        public UInt64 RenameLiveShortcut(UInt64 shortcutID, string name)
        {
            if (SteamPID > 0)
            {
                if (IsSteam4NETLoaded && HasClientShortcutsInterface)
                {
                    try
                    {
                        UInt32 appID = ClientShortcuts.GetAppIDForGameID(new CGameID(shortcutID));
                        if (appID > 0)
                        {
                            string exe = ClientShortcuts.GetShortcutExeByAppID(appID);
                            ClientShortcuts.SetShortcutAppName(appID, name);
                            return SteamShortcut.GetShortcutID(exe, name);
                        }
                    }
                    catch { }
                }
            }
            return 0;
        }

        public UInt64 GetShortcutID(string appname, string exe, string shortcutFilePath)
        {
            if (SteamPID > 0)
            {
                if (IsSteam4NETLoaded && HasClientShortcutsInterface)
                {
                    try
                    {
                        uint CountShortcuts = ClientShortcuts.GetShortcutCount();
                        for (uint x = 0; x < CountShortcuts; x++)
                        {
                            string steamexe = ClientShortcuts.GetShortcutExeByIndex(x).Trim('"');
                            string steamappname = ClientShortcuts.GetShortcutAppNameByIndex(x);
                            if (exe == steamexe
                              && appname == steamappname)
                            {
                                UInt64 shortcutID = SteamShortcut.GetShortcutID(ClientShortcuts.GetShortcutExeByIndex(x), appname);
                                UInt32 appID = ClientShortcuts.GetAppIDForGameID(new CGameID(shortcutID));
                                if (appID > 0)
                                {
                                    return shortcutID;
                                }
                            }
                        }
                        //return 0;
                    }
                    catch { }
                }
            }
            {
                if (shortcutFilePath == null)
                    return 0;

                VPropertyCollection ShortcutFile = SteamShortcutDataFile.Read(shortcutFilePath);

                VPropertyCollection shortcutVData = (VPropertyCollection)ShortcutFile["shortcuts"];

                foreach (VProperty _shortcutData in shortcutVData.Properties)
                {
                    VPropertyCollection shortcutData = (VPropertyCollection)_shortcutData.Value;
                    if (exe == ((VStringToken)shortcutData["exe"]).Value.Trim('"')
                      && appname == ((VStringToken)shortcutData["appname"]).Value)
                    {
                        return SteamShortcut.GetShortcutID(exe, appname);
                    }
                }

                return 0;
            }
        }

        public List<SteamShortcut> GetShortcutsForExe(string exe, string shortcutFilePath)
        {
            List<SteamShortcut> shortcuts = new List<SteamShortcut>();

            if (SteamPID > 0)
            {
                if (IsSteam4NETLoaded && HasClientShortcutsInterface)
                {
                    try
                    {
                        uint CountShortcuts = ClientShortcuts.GetShortcutCount();
                        for (uint x = 0; x < CountShortcuts; x++)
                        {
                            if(exe == ClientShortcuts.GetShortcutExeByIndex(x).Trim('"'))
                            {
                                string appname = ClientShortcuts.GetShortcutAppNameByIndex(x);
                                UInt32 appID = ClientShortcuts.GetAppIDForGameID(new CGameID(SteamShortcut.GetShortcutID(ClientShortcuts.GetShortcutExeByIndex(x), appname)));
                                if (appID > 0)
                                {
                                    string StartDir = ClientShortcuts.GetShortcutStartDirByAppID(appID);
                                    string icon = ClientShortcuts.GetShortcutIconByAppID(appID);
                                    string ShortcutPath = ClientShortcuts.GetShortcutPathByAppID(appID);
                                    bool hidden = ClientShortcuts.BIsShortcutHiddenByAppID(appID);
                                    bool AllowDesktopConfig = ClientShortcuts.BAllowDesktopConfigByAppID(appID);
                                    bool OpenVR = ClientShortcuts.BIsOpenVRShortcutByIndex(x);
                                    List<string> tags = new List<string>();

                                    uint CountTags = ClientShortcuts.GetShortcutUserTagCountByIndex(x);
                                    for (uint y = 0; y < CountTags; y++)
                                    {
                                        tags.Add(ClientShortcuts.GetShortcutUserTagByIndex(x, y));
                                    }

                                    shortcuts.Add(new SteamShortcut(appname, exe, StartDir, icon, ShortcutPath, hidden, AllowDesktopConfig, OpenVR, tags));
                                }
                            }
                        }
                        return shortcuts;
                    }
                    catch
                    {
                        throw new SteamException("Error Finding Shortcuts");
                        //return null;
                    }
                }
                else
                {
                    return null; // can't access API, can't edit file as Steam is open
                }
            }
            else
            {
                if (shortcutFilePath == null)
                    return null;

                VPropertyCollection ShortcutFile = SteamShortcutDataFile.Read(shortcutFilePath);

                VPropertyCollection shortcutVData = (VPropertyCollection)ShortcutFile["shortcuts"];

                shortcutVData.Properties.ForEach(_shortcutData =>
                {
                    VPropertyCollection shortcutData = (VPropertyCollection)_shortcutData.Value;
                    if (exe == ((VStringToken)shortcutData["exe"]).Value.Trim('"'))
                    {
                        string appname = ((VStringToken)shortcutData["appname"]).Value;
                        //string exe = ((VStringToken)shortcutData["exe"]).Value;
                        string StartDir = ((VStringToken)shortcutData["StartDir"]).Value;
                        string icon = ((VStringToken)shortcutData["icon"]).Value;
                        string ShortcutPath = ((VStringToken)shortcutData["ShortcutPath"]).Value;
                        bool hidden = ((VIntToken)shortcutData["IsHidden"]).Value > 0;
                        bool AllowDesktopConfig = ((VIntToken)shortcutData["AllowDesktopConfig"]).Value > 0;
                        bool OpenVR = ((VIntToken)shortcutData["OpenVR"]).Value > 0;
                        List<string> tags = new List<string>();

                        VPropertyCollection tagData = ((VPropertyCollection)shortcutData["tags"]);
                        tagData.Properties.ForEach(dr => tags.Add(((VStringToken)dr.Value).Value));

                        shortcuts.Add(new SteamShortcut(appname, exe, StartDir, icon, ShortcutPath, hidden, AllowDesktopConfig, OpenVR, tags));
                    }
                });

                return shortcuts;
            }
        }

        /// <summary>
        /// Remove shortcuts by matching the exe and appname
        /// </summary>
        /// <param name="shortcuts"></param>
        /// <param name="shortcutFilePath"></param>
        /// <returns>Count removed shortcuts</returns>
        public int RemoveShortcuts(List<SteamShortcut> shortcuts, string shortcutFilePath)
        {
            int count = 0;
            if (SteamPID > 0)
            {
                if (IsSteam4NETLoaded && HasClientShortcutsInterface)
                {
                    try
                    {
                        shortcuts.ForEach(shortcut =>
                        {
                            UInt32 appid = ClientShortcuts.GetAppIDForGameID(new CGameID(shortcut.GetShortcutID()));
                            if (appid > 0)
                            {
                                ClientShortcuts.RemoveShortcut(appid);
                            }
                            //if (count % 200 == 0)
                            //{
                            //    Thread.Sleep(1000);
                            //}
                        });
                        count++;
                        return count;
                    }
                    catch
                    {
                        throw new SteamException("Error Removing Shortcut");
                        //return 0;
                    }
                }
                else
                {
                    return 0; // can't access API, can't edit file as Steam is open
                }
            }
            else
            {
                if (shortcutFilePath == null)
                    return 0;

                VPropertyCollection ShortcutFile = SteamShortcutDataFile.Read(shortcutFilePath);

                VPropertyCollection shortcutVData = (VPropertyCollection)ShortcutFile["shortcuts"];

                List<string> idsToRemove = new List<string>();
                shortcutVData.Properties.ForEach(dr =>
                {
                    VPropertyCollection shortcutData = (VPropertyCollection)dr.Value;
                    shortcuts.Distinct().ToList().ForEach(shortcut =>
                    {
                        if ( (((VStringToken)shortcutData["appname"]).Value + ((VStringToken)shortcutData["exe"]).Value)
                          == (shortcut.appname + shortcut.exe))
                        {
                            idsToRemove.Add(dr.Key);
                        }
                    });
                });

                idsToRemove.OrderByDescending(dr => int.Parse(dr)).ToList().ForEach(dr =>
                {
                    shortcutVData.Remove(dr);
                    count++;
                });

                File.Copy(shortcutFilePath, shortcutFilePath + DateTime.UtcNow.ToString(".yyyyMMddHHmmss"));
                File.Delete(shortcutFilePath);
                //File.Create(shortcutFilePath);
                SteamShortcutDataFile.Write(shortcutFilePath, ShortcutFile);

                return count;
            }
        }















        public void SetShortcutName(UInt64 shortcutID, string steamShortcutID, string newName)
        {
            if (!IsSteam4NETLoaded)
            {
                throw new SteamException("Unable to load Steam4NET library.");
            }

            try
            {
                uint shortCutCount = ClientShortcuts.GetShortcutCount();
                for (uint index = 0; index < shortCutCount; index++)
                {
                    uint appID = ClientShortcuts.GetShortcutAppIDByIndex(index);

                    CGameID gameID = ClientShortcuts.GetGameIDForAppID(appID);
                    if (shortcutID == gameID.ConvertToUint64()
                      || ClientShortcuts.GetShortcutExeByIndex(index).Contains($"-steamproxyactivate {steamShortcutID}"))
                    {
                        string name = ClientShortcuts.GetShortcutAppNameByAppID(appID);
                        ClientShortcuts.SetShortcutAppName(appID, newName);
                        return;
                    }
                }
            }
            catch { }
        }

        public string GetShortcutName(UInt64 shortcutID, string steamShortcutID)
        {
            if (!IsSteam4NETLoaded)
            {
                throw new SteamException("Unable to load Steam4NET library.");
            }

            try
            {
                uint shortCutCount = ClientShortcuts.GetShortcutCount();
                for (uint index = 0; index < shortCutCount; index++)
                {
                    uint appID = ClientShortcuts.GetShortcutAppIDByIndex(index);

                    CGameID gameID = ClientShortcuts.GetGameIDForAppID(appID);
                    if (shortcutID == gameID.ConvertToUint64()
                      || ClientShortcuts.GetShortcutExeByIndex(index).EndsWith($"-steamproxyactivate {steamShortcutID}"))
                    {
                        return ClientShortcuts.GetShortcutAppNameByAppID(appID);
                    }
                }
            }
            catch { }
            return null;
        }

        public UInt64 FindShortcut(UInt64 shortcutID, string steamShortcutID)
        {
            if (!IsSteam4NETLoaded)
            {
                throw new SteamException("Unable to load Steam4NET library.");
            }

            try
            {
                uint shortCutCount = ClientShortcuts.GetShortcutCount();
                for (uint index = 0; index < shortCutCount; index++)
                {
                    uint appID = ClientShortcuts.GetShortcutAppIDByIndex(index);

                    CGameID gameID = ClientShortcuts.GetGameIDForAppID(appID);
                    if (shortcutID == gameID.ConvertToUint64()
                      || ClientShortcuts.GetShortcutExeByIndex(index).EndsWith($"-steamproxyactivate {steamShortcutID}"))
                    {
                        return gameID.ConvertToUint64();
                    }
                }
            }
            catch { }
            return 0;
        }
    }

    public class SteamShortcut
    {
        //"\""
        private string _exe;
        private string _StartDir;
        private string _icon;

        public string appname { get; set; }
        public string exe { get { return "\"" + _exe.Trim('"') + "\""; } set { _exe = value; } }
        public string StartDir { get { return "\"" + _StartDir.Trim('"') + "\""; } set { _StartDir = value; } }
        public string icon { get { return string.IsNullOrEmpty(_icon) ? null : "\"" + _icon.Trim('"') + "\""; } set { _icon = value; } }
        public string ShortcutPath { get; set; }
        public bool hidden { get; set; }
        public bool AllowDesktopConfig { get; set; }
        public bool OpenVR { get; set; }
        public List<string> tags { get; private set; }

        private static readonly CRC.Setting crcSetting = new CRC.Setting(32, 0x04C11DB7, 0xffffffff, true, true, 0xffffffff);

        public SteamShortcut(string appname, string exe, string StartDir, string icon = null, string ShortcutPath = null, bool hidden = false, bool AllowDesktopConfig = true, bool OpenVR = false, List<string> tags = null)
        {
            this.appname = appname;
            this.exe = exe;
            this.StartDir = StartDir;
            this.icon = icon;
            this.ShortcutPath = ShortcutPath;
            this.hidden = hidden;
            this.AllowDesktopConfig = AllowDesktopConfig;
            this.OpenVR = OpenVR;
            this.tags = tags ?? new List<string>();
        }

        public static UInt64 GetShortcutID(string exe, string appname)
        {
            CRC algorithm = new CRC(crcSetting);
            string crc_input = exe + appname;
            UInt64 high_32 = BitConverter.ToUInt32(algorithm.ComputeHash(Encoding.UTF8.GetBytes(crc_input).Select(dr => (byte)dr).ToArray()), 0) | 0x80000000;
            UInt64 full_64 = (high_32 << 32) | 0x02000000;
            return full_64;
        }

        public UInt64 GetShortcutID()
        {
            return GetShortcutID(exe, appname);
        }
    }
}
