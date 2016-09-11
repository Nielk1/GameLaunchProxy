using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace GameLaunchProxy
{
    static class Program
    {
        #region Externals
        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        public static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        [DllImport("gdi32.dll", EntryPoint = "RemoveFontResourceW", SetLastError = true)]
        public static extern bool RemoveFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);
        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };

        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);
        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);
        const uint WM_GETTEXT = 0x000D;
        #endregion Externals

        #region Data Structures
        struct WindowData
        {
            public Process proc;
            public IntPtr win;
            public Rect rec;
        }

        struct SteamProxyData
        {
            public string Program;
            public string Args;
            public UInt64 ShortcutID;

            public int? AggressiveFocus;
        }
        #endregion Data Structures

        // unique id for global mutex - Global prefix means it is global to the machine
        // const string mutex_id = "Global\\{0C497414-1C8C-4E44-9A29-01E80B9B6556}";
        // This mutex prevents the program from running more than once, it is suffixed with a function name
        const string mutex_id = "{0C497414-1C8C-4E44-9A29-01E80B9B6556}";

        // Used for logging only
        public static string instanceID;
        
        // Various settings constructs for running logic
        static Settings settings;


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            LoadSettings();

            LogMessage($"Start");
            LogMessage($"Version {Assembly.GetExecutingAssembly().GetName().Version.ToString()}");
            //LogMessage($"Args[{args.Length}]");
            //for (int x = 0; x < args.Length; x++) LogMessage($"[{x}]\t{args[x]}");
            LogMessage($"Raw Command Line\t{Environment.CommandLine}");

            if (args.Length > 0)
            {
                if(args.Contains("-proxy"))
                {
                    // anything after the command -proxy is treated as part of the proxy.

                    #region Clean Up Stuck Old Run
                    CleanupLaunchedGame();
                    #endregion Clean Up Stuck Old Run

                    #region Break Up Arguments
                    string rawCommandLine = Environment.CommandLine;
                    List<string> rawArgList = Regex.Matches(rawCommandLine, @"[\""].+?[\""]|[^ ]+")
                                                    .Cast<Match>()
                                                    .Select(m => m.Value)
                                                    .Skip(1)
                                                    .ToList();
                    int proxyFlagIndex = rawArgList.IndexOf("-proxy");
                    List<string> ProxyArgs = rawArgList.Take(proxyFlagIndex).ToList();
                    List<string> EmulatorArgs = rawArgList.Skip(proxyFlagIndex + 1).ToList();
                    #endregion Break Up Arguments

                    #region Process Arguments
                    bool steam = ProxyArgs.Contains("-steam");
                    bool bigpicture = false;
                    if(ProxyArgs.Contains("-steambigpicture"))
                    {
                        steam = true;
                        bigpicture = true;
                    }
                    string rom = null;
                    int indexOfRomArg = -1;
                    if ((indexOfRomArg = ProxyArgs.IndexOf("-rom")) > -1)
                    {
                        if (ProxyArgs.Count > (indexOfRomArg + 1))
                        {
                            rom = ProxyArgs[indexOfRomArg + 1].Trim('"');
                        }
                    }
                    else
                    {
                        string possibleRom = EmulatorArgs.Last().Trim('"');
                        if (possibleRom.IndexOfAny(Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()).ToArray()) > -1)
                        {
                            rom = possibleRom;
                        }
                        else if (possibleRom.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
                        {
                            rom = possibleRom;
                        }
                    }
                    string name = null;
                    int indexOfNameArg = -1;
                    if ((indexOfNameArg = ProxyArgs.IndexOf("-name")) > -1)
                    {
                        if (ProxyArgs.Count > (indexOfNameArg + 1))
                        {
                            name = ProxyArgs[indexOfNameArg + 1];
                        }
                    }
                    string fallBackName = null;
                    int indexOfFallBackNameArg = -1;
                    if ((indexOfFallBackNameArg = ProxyArgs.IndexOf("-fallbackname")) > -1)
                    {
                        if (ProxyArgs.Count > (indexOfFallBackNameArg + 1))
                        {
                            fallBackName = ProxyArgs[indexOfFallBackNameArg + 1];
                        }
                    }
                    #endregion Process Arguments

                    // lookup an entry to do work for, swap out dictionary system for a raw list with a regex scan.
                    // Sure it's slow but such customziation is rare.

                    GameRestoreData gameRestoreData = new GameRestoreData();

                    if (steam)
                    {
                        #region Clean Up Names
                        string cleanname = Path.GetFileNameWithoutExtension(rom);
                        string cleanplatformname = string.Empty;
                        if (name.Contains("%gamename%") || name.Contains("%platformname%") || fallBackName.Contains("%gamename%") || fallBackName.Contains("%platformname%"))
                        {
                            List<GameNameData> launchbox_names;
                            if (File.Exists("names_launchbox.json"))
                            {
                                launchbox_names = JsonConvert.DeserializeObject<List<GameNameData>>(File.ReadAllText("names_launchbox.json"));
                            }
                            else
                            {
                                launchbox_names = new List<GameNameData>();
                            }

                            List<GameNameData> possibleNameMatches = new List<GameNameData>();
                            possibleNameMatches.AddRange(launchbox_names.Where(dr => dr.OuterFileFullPath != null && dr.OuterFileFullPath == rom).ToList());
                            possibleNameMatches.AddRange(launchbox_names.Where(dr => dr.OuterFileName != null && dr.OuterFileName == Path.GetFileName(rom)).ToList());
                            possibleNameMatches.AddRange(launchbox_names.Where(dr => dr.InnerFileName != null && dr.InnerFileName == Path.GetFileName(rom)).ToList());
                            possibleNameMatches.AddRange(launchbox_names.Where(dr => dr.OuterFileName != null && Path.GetFileNameWithoutExtension(dr.OuterFileName) == Path.GetFileNameWithoutExtension(rom)).ToList());
                            possibleNameMatches.AddRange(launchbox_names.Where(dr => dr.InnerFileName != null && Path.GetFileNameWithoutExtension(dr.InnerFileName) == Path.GetFileNameWithoutExtension(rom)).ToList());

                            LogMessage($"Possible Name Matches:");
                            foreach (GameNameData item in possibleNameMatches)
                            {
                                LogMessage($"\t{item.Title}\t{item.Platform}\t{item.OuterFileFullPath}\t{item.OuterFileName}\t{item.InnerFileName}");
                            }

                            GameNameData gameNameData = possibleNameMatches.FirstOrDefault();
                            if (gameNameData != null)
                            {
                                cleanname = gameNameData.Title;
                                cleanplatformname = gameNameData.Platform;
                            }
                        }

                        if(!string.IsNullOrWhiteSpace(cleanplatformname))
                        {
                            if(settings.PlatformRenames.ContainsKey(cleanplatformname))
                            {
                                cleanplatformname = settings.PlatformRenames[cleanplatformname];
                            }
                        }

                        if (name == null)
                        {
                            name = "%gamename%";
                        }
                        name = name.Trim('"');
                        if (name.Contains("%gamename%") || name.Contains("%platformname%"))
                        {
                            LogMessage($"-name Contains %gamename% or %platformname%");


                            while (name.Contains("%gamename%"))
                            {
                                name = name.Replace("%gamename%", cleanname);
                            }
                            while (name.Contains("%platformname%"))
                            {
                                name = name.Replace("%platformname%", cleanplatformname);
                            }
                            LogMessage($"New -name\t{name}");
                        }

                        if (!string.IsNullOrWhiteSpace(fallBackName))
                        {
                            fallBackName = fallBackName.Trim('"');
                            if (fallBackName.Contains("%gamename%") || fallBackName.Contains("%platformname%"))
                            {
                                LogMessage($"-fallbackname Contains %gamename% or %platformname%");

                                while (fallBackName.Contains("%gamename%"))
                                {
                                    fallBackName = fallBackName.Replace("%gamename%", cleanname);
                                }
                                while (fallBackName.Contains("%platformname%"))
                                {
                                    fallBackName = fallBackName.Replace("%platformname%", cleanplatformname);
                                }
                                LogMessage($"New -fallbackname\t{fallBackName}");
                            }
                        }
                        #endregion Clean Up Names

                        #region Find Shortcut
                        LogMessage($"Looking for steam shortcut");
                        string proxyPath;
                        {
                            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                            UriBuilder uri = new UriBuilder(codeBase);
                            string path = Uri.UnescapeDataString(uri.Path);
                            proxyPath = Path.GetFullPath(path);
                            proxyPath = Path.Combine(Path.GetDirectoryName(proxyPath), "SteamProxy.exe");
                        }

                        LogMessage($"Shotcuts must path to \"{proxyPath}\"");

                        List<Tuple<string, bool>> names = new List<Tuple<string, bool>>();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            names.Add(new Tuple<string, bool>(name, false));
                            names.Add(new Tuple<string, bool>("SteamProxy", true));
                        }
                        if (!string.IsNullOrWhiteSpace(fallBackName))
                            names.Add(new Tuple<string, bool>(fallBackName, false));
                        names.Add(new Tuple<string, bool>("SteamProxy", false));

                        LogMessage($"Search Patterns:");
                        foreach(Tuple<string, bool> item in names)
                        {
                            LogMessage($"\t{item.Item1}\t" + (item.Item2 ? "<rename>" : string.Empty));
                        }

                        UInt64 steamShortcutId = 0;
                        for (int x = 0; x < names.Count && steamShortcutId == 0; x++)
                        {
                            steamShortcutId = SteamContext.GetInstance().GetShortcutID(names[x].Item1, proxyPath, settings.Core.SteamShortcutFilePath);
                            if (steamShortcutId != 0 && names[x].Item2)
                            {
                                //UInt64 oldSteamShortcutId = steamShortcutId;
                                //string oldSteamShortcutName = names[x].Item1;
                                steamShortcutId = SteamContext.GetInstance().RenameLiveShortcut(steamShortcutId, name);

                                if (steamShortcutId != 0)
                                {
                                    gameRestoreData.SteamShortcutID = steamShortcutId;
                                    gameRestoreData.OldSteamShortcutname = names[x].Item1;
                                    File.WriteAllText("GameRestoreData.json", JsonConvert.SerializeObject(gameRestoreData));
                                    break; // loop will terminate anyway but why not
                                }
                            }
                        }
                        LogMessage($"Done looking for steam shortcut");
                        #endregion Find Shortcut

                        if (steamShortcutId == 0)
                        {
                            StringBuilder message = new StringBuilder();
                            message.AppendLine("Could not find a fitting Steam shortcut");
                            message.AppendLine($"EXE: {proxyPath}");
                            message.AppendLine($"Names: {string.Join(", ", names.Select(dr => "\"" + dr + "\"").ToArray())}");

                            MessageBox.Show(message.ToString(), "Couldn't find usable Steam Shortcut", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // create steamproxy.json file
                        // save data about launch item into temporary data
                        SteamProxyData steamProxy = new SteamProxyData()
                        {
                            Program = EmulatorArgs[0].StartsWith("\"") && EmulatorArgs[0].EndsWith("\"") ? EmulatorArgs[0].Substring(1, EmulatorArgs[0].Length - 2) : EmulatorArgs[0],
                            Args = string.Join(" ", EmulatorArgs.Skip(1)),
                            ShortcutID = steamShortcutId
                        };

                        ProgramSettings programSettings = LoadProgramSettings(steamProxy.Program);

                        gameRestoreData.AddedFonts = SetupFonts(programSettings);
                        File.WriteAllText("GameRestoreData.json", JsonConvert.SerializeObject(gameRestoreData));
                        LogMessage($"GameRestoreData.json\t{JsonConvert.SerializeObject(gameRestoreData)}");
                        steamProxy.AggressiveFocus = programSettings != null ? programSettings.AggressiveFocus : null;
                        File.WriteAllText("steamproxy.json", JsonConvert.SerializeObject(steamProxy));
                        LogMessage($"steamproxy.json\t{JsonConvert.SerializeObject(steamProxy)}");

                        bool AlreadyInBigPicture = SteamContext.GetInstance().BigPicturePID != 0;
                        SetupBigPicture(bigpicture && !AlreadyInBigPicture);

                        LogMessage($"start\tsteam://rungameid/{steamShortcutId}");
                        Process.Start($"steam://rungameid/{steamShortcutId}");

                        Process LookingForProc = null;
                        for (int tries = 0; tries < 10; tries++)
                        {
                            Thread.Sleep(1000);

                            Process[] procs = Process.GetProcessesByName("SteamProxy");
                            //Process[] procs = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(argList[0].Trim('"')));
                            LookingForProc = procs.FirstOrDefault();

                            if (LookingForProc != null) break;
                        }
                        if (LookingForProc != null)
                        {
                            LogMessage($"found process to wait on\t{LookingForProc.ProcessName}");

                            Thread.Sleep(1000);
                            //LookingForProc.WaitForExit();
                            //Thread.Sleep(1000);


                            for(;;)
                            {
                                Thread.Sleep(1000);
                                if (!LookingForProc.IsRunning())
                                {
                                    break;
                                }
                                //LogMessage($"Proc Still Here");
                            }



                            LogMessage($"ended\tsteam://rungameid/{steamShortcutId}");
                        }
                        else
                        {
                            LogMessage($"failed to catch process from steam");
                        }

                        Thread.Sleep(1000);

                        CleanupBigPicture(bigpicture, AlreadyInBigPicture);

                        CleanupLaunchedGame();
                    }
                    else
                    {
                        string programPath = EmulatorArgs[0].StartsWith("\"") && EmulatorArgs[0].EndsWith("\"") ? EmulatorArgs[0].Substring(1, EmulatorArgs[0].Length - 2) : EmulatorArgs[0];
                        string programArgs = string.Join(" ", EmulatorArgs.Skip(1));

                        LogMessage($"Program\t{programPath}");
                        LogMessage($"Args\t{programArgs}");

                        if (File.Exists(programPath))
                        {
                            ProgramSettings programSettings = LoadProgramSettings(programPath);

                            gameRestoreData.AddedFonts = SetupFonts(programSettings);
                            File.WriteAllText("GameRestoreData.json", JsonConvert.SerializeObject(gameRestoreData));

                            StartProgram(programPath, programArgs, programSettings != null ? programSettings.AggressiveFocus : null); // and wait for it to finish
                        }
                    }
                }
            }
            else
            {
                LogMessage($"Start GUI");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new EditForm());
                LogMessage($"End GUI");
            }
            LogMessage($"End");
        }

        private static void LogMessage(string message)
        {
            if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", instanceID + "\t" + message + "\r\n");
        }
        private static void StartProgram(string programPath, string programArgs, int? AggressiveFocus)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = programPath.Trim('"');
            startInfo.Arguments = programArgs;
            startInfo.WorkingDirectory = Path.GetDirectoryName(programPath.Trim('"'));

            LogMessage($"Starting Program");
            LogMessage($"FileName:\t{startInfo.FileName}");
            LogMessage($"Arguments:\t{startInfo.Arguments}");
            LogMessage($"WorkingDirectory:\t{startInfo.WorkingDirectory}");

            Process runningProc = Process.Start(startInfo);

            if (AggressiveFocus.HasValue)
            {
                int secondsOfAggression = 0;
                do
                {
                    {
                        IntPtr hwnd = runningProc.MainWindowHandle;
                        if (hwnd == IntPtr.Zero)
                        {
                            //the window is hidden so try to restore it before setting focus.
                            ShowWindow(runningProc.Handle, ShowWindowEnum.Restore);
                        }

                        //set user the focus to the window
                        SetForegroundWindow(runningProc.MainWindowHandle);
                    }
                    Thread.Sleep(1000);
                    secondsOfAggression++;
                } while (secondsOfAggression <= AggressiveFocus.Value);
            }

            //runningProc.WaitForExit();
            for (;;)
            {
                Thread.Sleep(1000);
                if (!runningProc.IsRunning())
                {
                    break;
                }
                //LogMessage($"Proc Still Here");
            }

            LogMessage($"Ended Program Normally");
        }

        private static void CleanupLaunchedGame()
        {
            if (File.Exists("GameRestoreData.json"))
            {
                LogMessage("Restoring old Steam shortcut name");

                GameRestoreData restoreData = JsonConvert.DeserializeObject<GameRestoreData>(File.ReadAllText("GameRestoreData.json"));

                CleanupFonts(restoreData.AddedFonts);

                UInt64 shortcutID = restoreData.SteamShortcutID;
                if(shortcutID > 0 && !string.IsNullOrWhiteSpace(restoreData.OldSteamShortcutname))
                try
                {
                    SteamContext.GetInstance().RenameLiveShortcut(shortcutID, restoreData.OldSteamShortcutname);
                }
                catch
                {
                    LogMessage("Failed to restore Steam shortcut name");
                    MessageBox.Show("Error occured trying to restore shortcut name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                File.Delete("GameRestoreData.json");
            }
        }
      
        private static void LoadSettings()
        {
            if (File.Exists("settings.json"))
            {
                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));
            }
            else
            {
                settings = new Settings();
            }

            if (settings.logging)
            {
                instanceID = Guid.NewGuid().ToString();
            }
        }
        private static ProgramSettings LoadProgramSettings(string programPath)
        {
            ProgramSettings programSetting = null;
            if (settings.Programs.ContainsKey(programPath))
            {
                programSetting = settings.Programs[programPath];
            }
            return programSetting;
        }

        private static List<string> SetupFonts(ProgramSettings programSetting)
        {
            List<string> AddedFonts = new List<string>();
            if (programSetting != null)
            {
                programSetting.Fonts.ForEach(font =>
                {
                    int addedCount = AddFontResource(font);
                    if (addedCount > 0)
                    {
                        AddedFonts.Add(font);
                    }
                });
            }
            return AddedFonts;
        }
        private static void CleanupFonts(List<string> AddedFonts)
        {
            if (AddedFonts != null)
            {
                AddedFonts.ForEach(font =>
                {
                    bool success = RemoveFontResource(font);
                });
            }
        }

        private static void SetupBigPicture(bool performAction)
        {
            if (performAction)
            {
                LogMessage($"start\tsteam://open/bigpicture");
                Process.Start($"steam://open/bigpicture");
                for(int t=0;t<100;t++)
                {
                    Thread.Sleep(100);
                    if (SteamContext.GetInstance().BigPicturePID > 0)
                        break;
                }
            }
        }
        private static void CleanupBigPicture(bool performAction, bool minimize = false)
        {
            //close big picture
            if (performAction)
            {
                int bigPictureID = SteamContext.GetInstance().BigPicturePID;
                if (bigPictureID != 0)
                {
                    Process bigProc = Process.GetProcessById(bigPictureID);

                    if (bigProc != null)
                    {
                        LogMessage($"Trying to close BigPicture, this is 'fun'");
                        IntPtr possibleBigPicture = GetForegroundWindow();

                        List<WindowData> handles = new List<WindowData>();
                        Process[] procs2 = Process.GetProcessesByName("steam");
                        foreach (Process proc in procs2)
                        {
                            foreach (ProcessThread thread in proc.Threads)
                                EnumThreadWindows(thread.Id, (hWnd, lParam) => { handles.Add(new WindowData() { proc = proc, win = hWnd }); return true; }, IntPtr.Zero);
                        }
                        LogMessage($"Found {handles.Count} possible Steam windows");

                        // if the active window is in the set, we're done
                        var activeWindowAndSteam = handles.Where(winDat => winDat.win == possibleBigPicture);
                        if (activeWindowAndSteam.Count() == 1)
                        {
                            handles = activeWindowAndSteam.ToList();
                            LogMessage($"Active window is steam window, assume it's big picture");
                        }

                        if (handles.Count > 1)
                        {
                            handles = handles.Where(windowHandle =>
                            {
                                StringBuilder message = new StringBuilder(1000);
                                SendMessage(windowHandle.win, WM_GETTEXT, message.Capacity, message);
                                return message.ToString() == "Steam";
                            }).ToList();

                            LogMessage($"Narrowed to {handles.Count} possible Steam windows");



                            // there's too many windows still, try getting the biggest window
                            if (handles.Count > 1)
                            {
                                handles.ForEach(winDat =>
                                {
                                    winDat.rec = new Rect();
                                    GetWindowRect(winDat.win, ref winDat.rec);
                                });

                                int Area = handles.Max(winDat => (winDat.rec.Right - winDat.rec.Left) * (winDat.rec.Bottom - winDat.rec.Top));
                                handles = handles.Where(winDat => ((winDat.rec.Right - winDat.rec.Left) * (winDat.rec.Bottom - winDat.rec.Top)) == Area).ToList();
                                LogMessage($"Grabbing biggest Steam windows, {handles.Count} found");

                                if (handles.Count > 1)
                                {
                                    var qry = handles.Where(winDat => winDat.rec.Left == 0 && winDat.rec.Top == 0);
                                    if (qry.Count() == 1)
                                    {
                                        handles = qry.ToList();
                                        LogMessage($"Last resort, if there's one at 0,0 we're using it");
                                    }
                                }
                            }
                        }

                        if (handles.Count > 0)
                        {
                            LogMessage($"Sending Big Picture Alt+F4");
                            IntPtr WindowToFind = handles.First().win;

                            Keyboard.Key key = new Keyboard.Key(Keyboard.Messaging.VKeys.KEY_F4);
                            Keyboard.Messaging.ForegroundKeyPressAll(WindowToFind, key, true, false, false);



                            // force parent process to front
                            try
                            {
                                var myId = Process.GetCurrentProcess().Id;
                                var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", myId);
                                var search = new ManagementObjectSearcher("root\\CIMV2", query);
                                var results = search.Get().GetEnumerator();
                                results.MoveNext();
                                var queryObj = results.Current;
                                var parentId = (uint)queryObj["ParentProcessId"];
                                var parent = Process.GetProcessById((int)parentId);




                                int secondsOfAggression = 0;
                                do
                                {
                                    {
                                        IntPtr hwnd = parent.MainWindowHandle;
                                        if (hwnd == IntPtr.Zero)
                                        {
                                            //the window is hidden so try to restore it before setting focus.
                                            ShowWindow(parent.Handle, ShowWindowEnum.Restore);
                                        }

                                        //set user the focus to the window
                                        SetForegroundWindow(parent.MainWindowHandle);
                                    }
                                    Thread.Sleep(1000);
                                    secondsOfAggression++;
                                } while (secondsOfAggression <= 1);
                            }
                            catch { }
                        }
                        else
                        {
                            LogMessage($"Couldn't find Big Picture window :(");
                        }
                    }
                }
            }
        }
    }

    public static class ProcessExtensions
    {
        public static bool IsRunning(this Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            try
            {
                Process.GetProcessById(process.Id);
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }
    }

    public class GameRestoreData
    {
        public UInt64 SteamShortcutID { get; set; }
        public string OldSteamShortcutname { get; set; }
        public List<string> AddedFonts { get; set; }
    }
}
