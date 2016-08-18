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

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args.Contains("-steamproxystart"))
                {
                    int ProxyInstructionIndex = 0;
                    for (;ProxyInstructionIndex<args.Length;ProxyInstructionIndex++)
                    {
                        if (args[ProxyInstructionIndex] == "-steamproxystart")
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


                    // need to be sure we have a working ID
                    if (ProxyID != null && ProxyID.Length > 0)
                    {
                        // clean unneeded CLI args
                        int argCounter = 0;
                        List<string> argList = args.Select(arg =>
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
                            if (arg.Contains(" "))
                            {
                                return "\"" + Regex.Replace(arg, @"(\\+)$", @"$1$1") + "\"";
                            }
                            return arg;
                        }).Where(arg => arg != null).ToList();




                        Settings settings;

                        if (File.Exists("settings.json"))
                        {
                            settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));
                        }
                        else
                        {
                            settings = new Settings();
                        }

                        SteamShortcutSettings knownShortcut = settings.SteamShortcuts.Where(shortDat => shortDat.LaunchPath.ToLowerInvariant().EndsWith($"-steamproxy {ProxyID}".ToLowerInvariant())).FirstOrDefault();


                        if (knownShortcut != null)
                        {
                            UInt64 steamShortcutId = knownShortcut.ID; // note, this ID is invalid if the name has changed since the shortcut was made, so if steam integration is offline this won't work

                            string oldShortcutName = string.Empty;
                            try
                            {
                                SteamContext.Init();
                                oldShortcutName = SteamContext.GetShortcutName(steamShortcutId, ProxyID);
                            }
                            catch { }

                            try
                            {
                                if (newShortcutName.Length > 0)
                                {
                                    SteamContext.SetShortcutName(steamShortcutId, ProxyID, newShortcutName); // agressivly try to find that shortcut
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
                                steamShortcutId + "\r\n" + // steam's id
                                //ProxyID + "\r\n" + // proxy id
                                oldShortcutName); // old name

                            Process.Start($"steam://rungameid/{steamShortcutId}");
                        }
                    }
                    else
                    {
                        // failed, throw a message box here that the steam shortcut was not found
                    }
                }
                else if (args.Contains("-steamproxy"))
                {
                    if (File.Exists("steamproxy.data"))
                    {
                        int ProxyInstructionIndex = 0;
                        for (; ProxyInstructionIndex < args.Length; ProxyInstructionIndex++)
                        {
                            if (args[ProxyInstructionIndex] == "-steamproxy")
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
                            Settings settings;

                            if (File.Exists("settings.json"))
                            {
                                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));
                            }
                            else
                            {
                                settings = new Settings();
                            }

                            SteamShortcutSettings knownShortcut = settings.SteamShortcuts.Where(shortDat => shortDat.LaunchPath.ToLowerInvariant().EndsWith($"-steamproxy {ProxyID}".ToLowerInvariant())).FirstOrDefault();

                            


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
                            string shortcutName = lines.Length >= 4 ? lines[3] : null;

                            StartProgram(programPath, programArgs); // and wait for it to finish

                            try
                            {
                                if (steamShortcutID > 0 && shortcutName.Length > 0)
                                {
                                    SteamContext.Init();
                                    SteamContext.SetShortcutName(steamShortcutID, ProxyID, shortcutName);
                                }
                            }
                            catch { }
                        }
                    }
                }
                else
                {
                    List<string> argList = args.Select(arg =>
                    {
                        if (arg.Contains(" "))
                        {
                            return "\"" + Regex.Replace(arg, @"(\\+)$", @"$1$1") + "\"";
                        }
                        return arg;
                    }).ToList();

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
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new EditForm());
            }
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
                var process = Process.Start(startInfo);
                process.WaitForExit();
            }
        }
    }
}
