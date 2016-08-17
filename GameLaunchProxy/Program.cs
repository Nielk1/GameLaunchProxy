using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
                string programPath = args.Last();
                if(File.Exists(programPath))
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

                    ProgramSettings setting = settings.Programs[programPath];
                    if(setting != null)
                    {
                        List<string> AddedFonts = new List<string>();
                        setting.Fonts.ForEach(font =>
                        {
                            int addedCount = AddFontResource(font);
                            if(addedCount > 0)
                            {
                                AddedFonts.Add(font);
                            }
                        });




                        {
                            var process = Process.Start(programPath);

                            if(setting.AggressiveFocus.HasValue)
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
                        var process = Process.Start(programPath);
                        process.WaitForExit();
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
    }
}
