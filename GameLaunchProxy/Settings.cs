using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLaunchProxy
{
    public class Settings
    {
        public Dictionary<string, ProgramSettings> Programs;

        public Settings()
        {
            Programs = new Dictionary<string, ProgramSettings>();
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
}
