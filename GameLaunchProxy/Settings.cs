using System;
using System.Collections.Generic;
using System.Data.HashFunction;
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

        public bool logging;

        public Settings()
        {
            Programs = new Dictionary<string, ProgramSettings>();
            SteamShortcuts = new List<SteamShortcutSettings>();
            logging = false;
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

        CRC.Setting crcSetting = new CRC.Setting(32, 0x04C11DB7, 0xffffffff, true, true, 0xffffffff);

        public SteamShortcutSettings()
        {
            Name = Guid.NewGuid().ToString();
            LaunchPath = $"\"{AssemblyDirectory}\" -steamproxyactivate {Name.Split('-').First()}";
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

        public UInt64 GenerateGameID()
        {
            CRC algorithm = new CRC(crcSetting);
            string crc_input = LaunchPath + Name;
            UInt64 high_32 = BitConverter.ToUInt32(algorithm.ComputeHash(crc_input.ToCharArray().Select(dr => (byte)dr).ToArray()), 0) | 0x80000000;
            UInt64 full_64 = (high_32 << 32) | 0x02000000;
            return full_64;
        }
    }
}
