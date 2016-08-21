using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLaunchProxy
{
    static class Program
    {
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

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hwnd);


        // unique id for global mutex - Global prefix means it is global to the machine
        const string mutex_id = "Global\\{0C497414-1C8C-4E44-9A29-01E80B9B6556}";

        static string instanceID;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Settings settings;

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
            if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tStart\r\n");
            if (args.Length > 0)
            {
                if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tArgs[{args.Length}]\r\n");
                if (instanceID != null) for (int x = 0; x < args.Length; x++) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\t[{x}]\t{args[x]}\r\n");
                if (args.Contains("-steamproxysetup"))
                {


                    using (var mutex = new Mutex(false, mutex_id + "SteamProxySetup"))
                    {
                        try
                        {
                            try
                            {
                                if (!mutex.WaitOne(TimeSpan.FromSeconds(5), false))
                                {
                                    if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tSteamProxySetup Mutex Aquisition Failed\r\n");
                                    MessageBox.Show("GameLaunchProxy is already running in SteamProxySetup mode", "Steam Proxy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Environment.Exit(0);
                                }
                            }
                            catch (AbandonedMutexException)
                            {
                                // Log the fact the mutex was abandoned in another process, it will still get aquired
                            }










                            int ProxyInstructionIndex = 0;
                            for (; ProxyInstructionIndex < args.Length; ProxyInstructionIndex++)
                            {
                                if (args[ProxyInstructionIndex] == "-steamproxysetup")
                                    break;
                            }
                            ProxyInstructionIndex++;
                            string ProxyID = null;
                            if (ProxyInstructionIndex < args.Length)
                            {
                                ProxyID = args[ProxyInstructionIndex];
                            }
                            // steamShortcutID is now known



                            int ProxyNameInstructionIndex = 0;
                            for (; ProxyNameInstructionIndex < args.Length; ProxyNameInstructionIndex++)
                            {
                                if (args[ProxyNameInstructionIndex] == "-steamproxyname")
                                    break;
                            }
                            ProxyNameInstructionIndex++;
                            string newShortcutName = null;
                            if (ProxyNameInstructionIndex < args.Length)
                            {
                                newShortcutName = args[ProxyNameInstructionIndex];
                            }
                            // steamShortcutName is now known

                            bool steamProxyHold = false;
                            if (args.Contains("-steamproxyhold"))
                            {
                                steamProxyHold = true;
                            }

                            // need to be sure we have a working ID
                            if (ProxyID != null && ProxyID.Length > 0)
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
                                    if ((argCounter == ProxyInstructionIndex)
                                    || (argCounter == (ProxyInstructionIndex - 1))
                                    || (argCounter == ProxyNameInstructionIndex)
                                    || (argCounter == (ProxyNameInstructionIndex - 1)))
                                    {
                                        argCounter++;
                                        return null;
                                    }
                                    argCounter++;
                                    if (arg == "-steamproxyhold") return null;
                                    return arg;
                                }).Where(arg => arg != null).ToList();

                                if (!string.IsNullOrWhiteSpace(newShortcutName))
                                {
                                    if (newShortcutName.Contains("%cleanromname%"))
                                    {
                                        // assuming last argument is rom path
                                        string cleanromname = Path.GetFileNameWithoutExtension(argList.Last().Trim('"'));
                                        while (newShortcutName.Contains("%cleanromname%"))
                                        {
                                            newShortcutName.Replace("%cleanromname%", cleanromname);
                                        }
                                    }
                                }

                                SteamShortcutSettings knownShortcut = settings.SteamShortcuts.Where(shortDat => shortDat.LaunchPath.ToLowerInvariant().EndsWith($"-steamproxyactivate {ProxyID}".ToLowerInvariant())).FirstOrDefault();


                                if (knownShortcut != null)
                                {
                                    UInt64 steamShortcutId = knownShortcut.ID; // note, this ID is invalid if the name has changed since the shortcut was made, so if steam integration is offline this won't work

                                    try
                                    {
                                        SteamContext.Init();
                                    }
                                    catch { }

                                    try
                                    {
                                        if (!string.IsNullOrWhiteSpace(newShortcutName))
                                        {
                                            SteamContext.SetShortcutName(steamShortcutId, ProxyID, newShortcutName); // agressivly try to find that shortcut
                                            if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tset shortcut name\t{newShortcutName}\r\n");
                                        }
                                    }
                                    catch { }

                                    try
                                    {
                                        // if we've managed to rename the shortcut, it's ID changed.  We need that.
                                        steamShortcutId = SteamContext.FindShortcut(steamShortcutId, ProxyID);
                                    }
                                    catch { }

                                    // save data about launch item into temporary data
                                    File.WriteAllText("steamproxy.data",
                                        argList[0] + "\r\n" + // program name
                                        string.Join(" ", argList.Skip(1)) + "\r\n" + // arguments
                                        steamShortcutId + "\r\n");

                                    if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tsteamproxy.data contents\r\n");
                                    if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tprogram:\t{argList[0]}\r\n");
                                    if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\targuments:\t{string.Join(" ", argList.Skip(1))}\r\n");
                                    if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tsteamid:\t{steamShortcutId}\r\n");

                                    if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tstart\tsteam://rungameid/{steamShortcutId}\r\n");
                                    Process.Start($"steam://rungameid/{steamShortcutId}");

                                    if (steamProxyHold)
                                    {
                                        Thread.Sleep(10 * 1000);

                                        Process[] procs = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(argList[0].Trim('"')));
                                        Process proc = procs.FirstOrDefault();
                                        if (proc != null)
                                        {
                                            proc.WaitForExit();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // failed, throw a message box here that the steam shortcut was not found
                            }



                        }
                        finally
                        {
                            mutex.ReleaseMutex();
                        }
                    }
                }
                else if (args.Contains("-steamproxyactivate"))
                {
                    using (var mutex = new Mutex(false, mutex_id + "SteamProxyActivate"))
                    {
                        try
                        {
                            try
                            {
                                if (!mutex.WaitOne(TimeSpan.FromSeconds(5), false))
                                {
                                    if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tSteamProxyActivate Mutex Aquisition Failed\r\n");
                                    MessageBox.Show("GameLaunchProxy is already running in SteamProxyActivate mode", "Steam Proxy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Environment.Exit(0);
                                }
                            }
                            catch (AbandonedMutexException)
                            {
                                // Log the fact the mutex was abandoned in another process, it will still get aquired
                            }







                            if (File.Exists("steamproxy.data"))
                            {
                                int ProxyInstructionIndex = 0;
                                for (; ProxyInstructionIndex < args.Length; ProxyInstructionIndex++)
                                {
                                    if (args[ProxyInstructionIndex] == "-steamproxyactivate")
                                        break;
                                }
                                ProxyInstructionIndex++;
                                string ProxyID = null;
                                if (ProxyInstructionIndex < args.Length)
                                {
                                    ProxyID = args[ProxyInstructionIndex];
                                }

                                if (ProxyID != null)
                                {
                                    SteamShortcutSettings knownShortcut = settings.SteamShortcuts.Where(shortDat => shortDat.LaunchPath.ToLowerInvariant().EndsWith($"-steamproxyactivate {ProxyID}".ToLowerInvariant())).FirstOrDefault();




                                    string[] lines = File.ReadAllLines("steamproxy.data");
                                    File.Delete("steamproxy.data");

                                    string programPath = lines[0];
                                    string programArgs = lines[1];
                                    UInt64 steamShortcutID = 0;
                                    try
                                    {
                                        steamShortcutID = UInt64.Parse(lines[2]);
                                    }
                                    catch { };
                                    //string ProxyID = lines[3];

                                    if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tsteamproxy.data [{lines.Length}]\r\n");
                                    if (instanceID != null) for (int z = 0; z < lines.Length; z++) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\t[{z}]\t{lines[z]}\r\n");

                                    StartProgram(programPath, programArgs); // and wait for it to finish

                                    try
                                    {
                                        if (steamShortcutID > 0)
                                        {
                                            SteamContext.Init();
                                            SteamContext.SetShortcutName(steamShortcutID, ProxyID, knownShortcut.Name);
                                            if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tset shortcut name\t{knownShortcut.Name}\r\n");
                                        }
                                    }
                                    catch { }
                                }
                            }
                            else
                            {
                                MessageBox.Show("steamproxy.data file not found", "Steam Proxy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }



                        }
                        finally
                        {
                            mutex.ReleaseMutex();
                        }
                    }
                }
                else
                {
                    string rawCommandLine = Environment.CommandLine;
                    List<string> argList = Regex.Matches(rawCommandLine, @"[\""].+?[\""]|[^ ]+")
                                                .Cast<Match>()
                                                .Select(m => m.Value)
                                                .Skip(1)
                                                .ToList();

                    string programPath = argList[0];
                    string programArgs = string.Join(" ", argList.Skip(1));


                    if (File.Exists(programPath))
                    {
                        StartProgram(programPath, programArgs);
                    }
                }
            }
            else
            {
                if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tStart GUI\r\n");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new EditForm());
                if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tEnd GUI\r\n");
            }
            if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tEnd\r\n");
        }

        private static void StartProgram(string programPath, string programArgs)
        {
            Settings settings;

            if (File.Exists("settings.json"))
            {
                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));
            }
            else
            {
                settings = new Settings();
            }

            ProgramSettings setting = null;
            if (settings.Programs.ContainsKey(programPath))
            {
                setting = settings.Programs[programPath];
            }
            if (setting != null)
            {
                List<string> AddedFonts = new List<string>();
                setting.Fonts.ForEach(font =>
                {
                    int addedCount = AddFontResource(font);
                    if (addedCount > 0)
                    {
                        AddedFonts.Add(font);
                    }
                });




                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.CreateNoWindow = false;
                    startInfo.UseShellExecute = false;
                    startInfo.FileName = programPath.Trim('"');
                    startInfo.Arguments = programArgs;
                    startInfo.WorkingDirectory = Path.GetDirectoryName(programPath.Trim('"'));

                    if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tStarting Program\r\n");
                    if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tFileName:\t{startInfo.FileName}\r\n");
                    if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tArguments:\t{startInfo.Arguments}\r\n");
                    if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tWorkingDirectory:\t{startInfo.WorkingDirectory}\r\n");

                    var process = Process.Start(startInfo);
                    process.WaitForExit();

                    if (setting.AggressiveFocus.HasValue)
                    {
                        int secondsOfAggression = 0;
                        do
                        {
                            {
                                IntPtr hwnd = process.MainWindowHandle;
                                if (hwnd == IntPtr.Zero)
                                {
                                    //the window is hidden so try to restore it before setting focus.
                                    ShowWindow(process.Handle, ShowWindowEnum.Restore);
                                }

                                //set user the focus to the window
                                SetForegroundWindow(process.MainWindowHandle);
                            }
                            Thread.Sleep(1000);
                            secondsOfAggression++;
                        } while (secondsOfAggression <= setting.AggressiveFocus.Value);
                    }

                    process.WaitForExit();
                    if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tEnded Program\r\n");
                }




                AddedFonts.ForEach(font =>
                {
                    bool success = RemoveFontResource(font);
                });
            }
            else
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = programPath.Trim('"');
                startInfo.Arguments = programArgs;
                startInfo.WorkingDirectory = Path.GetDirectoryName(programPath.Trim('"'));

                if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tStarting Program\r\n");
                if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tFileName:\t{startInfo.FileName}\r\n");
                if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tArguments:\t{startInfo.Arguments}\r\n");
                if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tWorkingDirectory:\t{startInfo.WorkingDirectory}\r\n");

                var process = Process.Start(startInfo);
                process.WaitForExit();
                if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", $"{instanceID}\tEnded Program\r\n");
            }
        }
    }
}
