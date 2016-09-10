using GameLaunchProxy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteamProxy
{
    class Program
    {
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

        struct SteamProxyData
        {
            public string Program;
            public string Args;
            public UInt64 ShortcutID;

            public int? AggressiveFocus;
        }

        // Used for logging only
        public static string instanceID;

        // Various settings constructs for running logic
        static Settings settings;

        static void Main(string[] args)
        {
            LoadSettings();

            LogMessage($"Start");
            LogMessage($"Version {Assembly.GetExecutingAssembly().GetName().Version.ToString()}");
            //LogMessage($"Args[{args.Length}]");
            //for (int x = 0; x < args.Length; x++) LogMessage($"[{x}]\t{args[x]}");
            LogMessage($"Raw Command Line\t{Environment.CommandLine}");

            if (File.Exists("steamproxy.json"))
            {
                SteamProxyData steamProxy = JsonConvert.DeserializeObject<SteamProxyData>(File.ReadAllText("steamproxy.json"));
                File.Delete("steamproxy.json");

                LogMessage($"steamproxy.json");
                LogMessage(JsonConvert.SerializeObject(steamProxy));

                //ProgramSettings programSettings = LoadProgramSettings(steamProxy.Program);
                //StartProgram(programSettings, steamProxy.Program, steamProxy.Args); // and wait for it to finish
                StartProgram(steamProxy.Program, steamProxy.Args, steamProxy.AggressiveFocus); // and wait for it to finish
            }
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

        private static void LogMessage(string message)
        {
            if (instanceID != null) File.AppendAllText("GameLaunchProxy.log", instanceID + "\t" + message + "\r\n");
        }
    }
}
