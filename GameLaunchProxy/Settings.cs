using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.HashFunction;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameLaunchProxy
{
    public class Settings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Dictionary<string, ProgramSettings> Programs;
        public CoreSettings Core;

        public Dictionary<string, string> PlatformRenames;

        public bool logging;

        public Settings()
        {
            Programs = new Dictionary<string, ProgramSettings>();
            Core = new CoreSettings();

            PlatformRenames = new Dictionary<string, string>();

            logging = false;

            Core.PropertyChanged += Core_PropertyChanged;
        }

        private void Core_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Core." + e.PropertyName);
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class CoreSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _SteamShortcutFilePath;
        public string SteamShortcutFilePath
        {
            get
            {
                return _SteamShortcutFilePath;
            }
            set
            {
                _SteamShortcutFilePath = value;
                OnPropertyChanged("SteamShortcutFilePath");
            }
        }

        private string _LaunchBoxLibrary;
        public string LaunchBoxLibrary
        {
            get
            {
                return _LaunchBoxLibrary;
            }
            set
            {
                _LaunchBoxLibrary = value;
                OnPropertyChanged("LaunchBoxLibrary");
            }
        }

        private string _SevenZipLib;
        public string SevenZipLib
        {
            get
            {
                return _SevenZipLib;
            }
            set
            {
                _SevenZipLib = value;
                OnPropertyChanged("SevenZipLib");
            }
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
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
