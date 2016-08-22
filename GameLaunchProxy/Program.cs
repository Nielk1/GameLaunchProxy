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

        struct SteamProxy
        {
            public string Program;
            public string Args;
            public UInt64 ShortcutID;
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
            //LogMessage($"Args[{args.Length}]");
            //for (int x = 0; x < args.Length; x++) LogMessage($"[{x}]\t{args[x]}");
            LogMessage($"Raw Command Line\t{Environment.CommandLine}");

            if (args.Length > 0)
            {
                if (args.Contains("-steamproxysetup"))
                {
                    SteamProxySetup(args);
                }
                else if (args.Contains("-steamproxyactivate"))
                {
                    SteamProxyActivate(args);
                }
                else
                {
                    GenericProxy(args);
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

        private static void SteamProxySetup(string[] args)
        {
            LogMessage($"Mode SteamProxySetup");
            using (var mutex = new Mutex(false, mutex_id + "SteamProxySetup"))
            {
                try
                {
                    #region Mutex for SteamProxySetup
                    try
                    {
                        if (!mutex.WaitOne(TimeSpan.FromSeconds(5), false))
                        {
                            LogMessage($"Mutex Aquisition Failed");
                            MessageBox.Show("GameLaunchProxy is already running in SteamProxySetup mode", "Steam Proxy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(0);
                        }
                    }
                    catch (AbandonedMutexException)
                    {
                        // Log the fact the mutex was abandoned in another process, it will still get aquired
                    }
                    #endregion Mutex for SteamProxySetup

                    #region Read -steamproxysetup
                    int CLI_steamproxysetup_Index = 0;
                    for (; CLI_steamproxysetup_Index < args.Length; CLI_steamproxysetup_Index++)
                    {
                        if (args[CLI_steamproxysetup_Index] == "-steamproxysetup")
                            break;
                    }
                    CLI_steamproxysetup_Index++;
                    string CLI_steamproxysetup = null;
                    if (CLI_steamproxysetup_Index < args.Length)
                    {
                        CLI_steamproxysetup = args[CLI_steamproxysetup_Index];
                    }
                    // steamShortcutID is now known
                    #endregion Read -steamproxysetup

                    #region Read -steamproxyname
                    int CLI_steamproxyname_Index = 0;
                    for (; CLI_steamproxyname_Index < args.Length; CLI_steamproxyname_Index++)
                    {
                        if (args[CLI_steamproxyname_Index] == "-steamproxyname")
                            break;
                    }
                    CLI_steamproxyname_Index++;
                    string CLI_steamproxyname = null;
                    if (CLI_steamproxyname_Index < args.Length)
                    {
                        CLI_steamproxyname = args[CLI_steamproxyname_Index];
                    }
                    // steamShortcutName is now known
                    #endregion Read -steamproxyname

                    #region Read -steamproxyhold REMOVED
                    //bool steamProxyHold = false;
                    //if (args.Contains("-steamproxyhold"))
                    //{
                    //    steamProxyHold = true;
                    //}
                    #endregion Read -steamproxyhold REMOVED

                    #region Read -steamproxyforcebigpicture
                    bool CLI_steamproxyforcebigpicture = false;
                    if (args.Contains("-steamproxyforcebigpicture"))
                    {
                        CLI_steamproxyforcebigpicture = true;
                    }
                    #endregion Read -steamproxyforcebigpicture

                    #region Read -steamproxyclosebigpicture
                    bool CLI_steamproxyclosebigpicture = false;
                    if (args.Contains("-steamproxyclosebigpicture"))
                    {
                        CLI_steamproxyclosebigpicture = true;
                    }
                    #endregion Read -steamproxyclosebigpicture

                    // need to be sure we have a working ID
                    if (CLI_steamproxysetup != null && CLI_steamproxysetup.Length > 0)
                    {
                        // clean unneeded CLI args
                        int argCounter = 0;
                        string rawCommandLine = Environment.CommandLine;
                        List<string> rawArgList = Regex.Matches(rawCommandLine, @"[\""].+?[\""]|[^ ]+")
                                                        .Cast<Match>()
                                                        .Select(m => m.Value)
                                                        .Skip(1)
                                                        .ToList();
                        List<string> argList = rawArgList.Select(arg =>
                        {
                            if ((argCounter == CLI_steamproxysetup_Index)
                            || (argCounter == (CLI_steamproxysetup_Index - 1))
                            || (argCounter == CLI_steamproxyname_Index)
                            || (argCounter == (CLI_steamproxyname_Index - 1)))
                            {
                                argCounter++;
                                return null;
                            }
                            argCounter++;
                            if (arg == "-steamproxyhold") return null; // still trimming this old shortcut but we should remove this later
                            if (arg == "-steamproxyforcebigpicture") return null;
                            if (arg == "-steamproxyclosebigpicture") return null;
                            return arg;
                        }).Where(arg => arg != null).ToList();

                        #region Adjust Shortcut Name
                        if (!string.IsNullOrWhiteSpace(CLI_steamproxyname))
                        {
                            if (CLI_steamproxyname.Contains("%cleanromname%"))
                            {
                                LogMessage($"-steamproxyname Contains %cleanromname%");

                                // assuming last argument is rom path
                                string cleanromname = Path.GetFileNameWithoutExtension(argList.Last().Trim('"'));
                                while (CLI_steamproxyname.Contains("%cleanromname%"))
                                {
                                    CLI_steamproxyname = CLI_steamproxyname.Replace("%cleanromname%", cleanromname);
                                }

                                LogMessage($"New -steamproxyname\t{CLI_steamproxyname}");
                            }
                        }
                        #endregion Adjust Shortcut Name

                        LogMessage($"Looking for shortcut in database that ends with -steamproxyactivate {CLI_steamproxysetup}");
                        SteamShortcutSettings knownShortcut = settings.SteamShortcuts.Where(shortDat => shortDat.LaunchPath.ToLowerInvariant().EndsWith($"-steamproxyactivate {CLI_steamproxysetup}".ToLowerInvariant())).FirstOrDefault();

                        if (knownShortcut != null)
                        {
                            LogMessage($"Found Shortcut");

                            // note, this ID is invalid if the name has changed since the shortcut was made, so if steam integration is offline this won't work
                            UInt64 steamShortcutId = knownShortcut.ID;

                            // try to init steamworks
                            try { SteamContext.Init(); } catch { }

                            // set shortcut name
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(CLI_steamproxyname))
                                {
                                    SteamContext.SetShortcutName(steamShortcutId, CLI_steamproxysetup, CLI_steamproxyname); // agressivly try to find that shortcut
                                    LogMessage($"set shortcut name\t{CLI_steamproxyname}");
                                }
                            }
                            catch { }

                            // get the new shortcut ID due to the rename
                            try { steamShortcutId = SteamContext.FindShortcut(steamShortcutId, CLI_steamproxysetup); } catch { }

                            // save data about launch item into temporary data
                            SteamProxy steamProxy = new SteamProxy()
                            {
                                Program = argList[0].StartsWith("\"") && argList[0].EndsWith("\"") ? argList[0].Substring(1, argList[0].Length - 2) : argList[0],
                                Args = string.Join(" ", argList.Skip(1)),
                                ShortcutID = steamShortcutId
                            };
                            File.WriteAllText("steamproxy.json", JsonConvert.SerializeObject(steamProxy));

                            LogMessage($"steamproxy.json contents");
                            LogMessage(JsonConvert.SerializeObject(steamProxy));


                            ProgramSettings programSettings = LoadProgramSettings(steamProxy.Program);
                            List<string> AddedFonts = SetupFonts(programSettings);
                            SetupBigPicture(CLI_steamproxyforcebigpicture);

                            LogMessage($"start\tsteam://rungameid/{steamShortcutId}");
                            Process.Start($"steam://rungameid/{steamShortcutId}");

                            Process LookingForProc = null;
                            for (int tries = 0; tries < 10; tries++)
                            {
                                Thread.Sleep(1000);

                                Process[] procs = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(argList[0].Trim('"')));
                                LookingForProc = procs.FirstOrDefault();

                                if (LookingForProc != null) break;
                            }
                            if (LookingForProc != null)
                            {
                                LogMessage($"found process to wait on\t{LookingForProc.ProcessName}");

                                Thread.Sleep(1000);
                                LookingForProc.WaitForExit();
                                Thread.Sleep(1000);


                                //for(;;)
                                //{
                                //    Thread.Sleep(1000);
                                //    if (!LookingForProc.IsRunning())
                                //    {
                                //        break;
                                //    }
                                //    //LogMessage($"Proc Still Here");
                                //}



                                LogMessage($"ended\tsteam://rungameid/{steamShortcutId}");
                            }
                            else
                            {
                                LogMessage($"failed to catch process from steam");
                            }

                            Thread.Sleep(1000);

                            CleanupBigPicture(CLI_steamproxyclosebigpicture);

                            try
                            {
                                if (steamShortcutId > 0)
                                {
                                    SteamContext.SetShortcutName(steamShortcutId, CLI_steamproxysetup, knownShortcut.Name);
                                    LogMessage($"set shortcut name\t{knownShortcut.Name}");
                                }
                            }
                            catch { }

                            CleanupFonts(AddedFonts);
                        }
                        else
                        {
                            LogMessage($"Didn't find Shortcut, nothing to launch in Steam");
                            MessageBox.Show($"Could not find shortcut in Proxy DB with ID {CLI_steamproxysetup}", "Steam Proxy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        LogMessage($"-steamproxysetup missing ID");
                        MessageBox.Show("-steamproxysetup missing ID", "Steam Proxy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
        private static void SteamProxyActivate(string[] args)
        {
            LogMessage($"Start Mode SteamProxyActivate");

            using (var mutex = new Mutex(false, mutex_id + "SteamProxyActivate"))
            {
                try
                {
                    #region Mutex for SteamProxyActivate
                    try
                    {
                        if (!mutex.WaitOne(TimeSpan.FromSeconds(5), false))
                        {
                            LogMessage($"Mutex Aquisition Failed");
                            MessageBox.Show("GameLaunchProxy is already running in SteamProxyActivate mode", "Steam Proxy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(0);
                        }
                    }
                    catch (AbandonedMutexException)
                    {
                        // Log the fact the mutex was abandoned in another process, it will still get aquired
                    }
                    #endregion Mutex for SteamProxyActivate

                    if (File.Exists("steamproxy.json"))
                    {
                        #region Read -steamproxyactivate
                        int CLI_steamproxyactivate_Index = 0;
                        for (; CLI_steamproxyactivate_Index < args.Length; CLI_steamproxyactivate_Index++)
                        {
                            if (args[CLI_steamproxyactivate_Index] == "-steamproxyactivate")
                                break;
                        }
                        CLI_steamproxyactivate_Index++;
                        string CLI_steamproxyactivate = null;
                        if (CLI_steamproxyactivate_Index < args.Length)
                        {
                            CLI_steamproxyactivate = args[CLI_steamproxyactivate_Index];
                        }
                        #endregion Read -steamproxyactivate

                        if (CLI_steamproxyactivate != null)
                        {
                            SteamShortcutSettings steamShortcutSetting = settings.SteamShortcuts.Where(shortDat => shortDat.LaunchPath.ToLowerInvariant().EndsWith($"-steamproxyactivate {CLI_steamproxyactivate}".ToLowerInvariant())).FirstOrDefault();

                            SteamProxy steamProxy = JsonConvert.DeserializeObject<SteamProxy>(File.ReadAllText("steamproxy.json"));
                            File.Delete("steamproxy.json");

                            LogMessage($"steamproxy.json");
                            LogMessage(JsonConvert.SerializeObject(steamProxy));

                            ProgramSettings programSettings = LoadProgramSettings(steamProxy.Program);
                            StartProgram(programSettings, steamProxy.Program, steamProxy.Args); // and wait for it to finish
                        }
                        else
                        {
                            LogMessage($"Could not find shortcut in Proxy DB with ID, how on earth could this happen?");
                            MessageBox.Show($"Could not find shortcut in Proxy DB with ID {CLI_steamproxyactivate}", "Steam Proxy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("steamproxy.json file not found", "Steam Proxy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }

            LogMessage($"End Mode SteamProxyActivate");
        }
        private static void GenericProxy(string[] args)
        {
            LogMessage($"Start Mode GenericProxy");

            string rawCommandLine = Environment.CommandLine;
            List<string> argList = Regex.Matches(rawCommandLine, @"[\""].+?[\""]|[^ ]+")
                                        .Cast<Match>()
                                        .Select(m => m.Value)
                                        .Skip(1)
                                        .ToList();

            string programPath = argList[0].StartsWith("\"") && argList[0].EndsWith("\"") ? argList[0].Substring(1, argList[0].Length - 2) : argList[0];
            string programArgs = string.Join(" ", argList.Skip(1));

            LogMessage($"Program\t{programPath}");
            LogMessage($"Args\t{programArgs}");

            if (File.Exists(programPath))
            {
                ProgramSettings programSettings = LoadProgramSettings(programPath);
                List<string> AddedFonts = SetupFonts(programSettings);
                StartProgram(programSettings, programPath, programArgs); // and wait for it to finish
                CleanupFonts(AddedFonts);
            }

            LogMessage($"End Mode GenericProxy");
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
                Thread.Sleep(1000);
            }
        }
        private static void CleanupBigPicture(bool performAction)
        {
            //close big picture
            if (performAction)
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
                }
                else
                {
                    LogMessage($"Couldn't find Big Picture window :(");
                }
            }
        }

        private static void LogMessage(string message)
        {
            if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", instanceID + "\t" + message + "\r\n");
        }
        private static void StartProgram(ProgramSettings programSetting, string programPath, string programArgs)
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

            if (programSetting != null)
            {
                if (programSetting.AggressiveFocus.HasValue)
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
                    } while (secondsOfAggression <= programSetting.AggressiveFocus.Value);
                }
            }

            runningProc.WaitForExit();
            /*for(;;)
            {
                if(!runningProc.IsRunning())
                {
                    break;
                }
                if (alreadyCleaning)
                {
                    if (!runningProc.IsRunning())
                    {
                        runningProc.Close();
                    }
                    break;
                }
                Thread.Sleep(1000);
            }*/

            LogMessage($"Ended Program Normally");
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
}
