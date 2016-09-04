using GameLaunchProxy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteamProxy
{
    class Program
    {
        struct SteamProxyData
        {
            public string Program;
            public string Args;
            public UInt64 ShortcutID;
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
                StartProgramTemporarySimplified(steamProxy.Program, steamProxy.Args); // and wait for it to finish
            }
        }

        private static void StartProgramTemporarySimplified(string programPath, string programArgs)
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
