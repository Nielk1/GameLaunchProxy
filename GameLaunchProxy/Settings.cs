using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameLaunchProxy
{
    public class Settings
    {
        public Dictionary<string, ProgramSettings> Programs;
        public List<SteamShortcutSettings> SteamShortcuts;

        public Settings()
        {
            Programs = new Dictionary<string, ProgramSettings>();
            SteamShortcuts = new List<SteamShortcutSettings>();
        }
    }
    public class ProgramSettings
    {
        public List<string> Fonts;
        public int? AggressiveFocus;

        public ProgramSettings()
        {
            Fonts = new List<string>();
        }
    }
    public class SteamShortcutSettings
    {
        public string LaunchPath;
        public string Name;
        public UInt64 ID;

        public SteamShortcutSettings()
        {
            Name = Guid.NewGuid().ToString();
            LaunchPath = $"\"{AssemblyDirectory}\" -steamproxy {Name.Split('-').First()}";
            ID = 0;
        }

        public override string ToString()
        {
            return Name;
        }


        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetFullPath(path);
            }
        }
    }
}
