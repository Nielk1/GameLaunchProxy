using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GameLaunchProxy
{
    public class LaunchBoxLibrary
    {
        private XmlDocument doc;
        private XmlElement root;
        private Settings settings;

        public LaunchBoxLibrary(string launchBoxLibrary, Settings settings)
        {
            doc = new XmlDocument();
            doc.Load(launchBoxLibrary);
            root = doc["LaunchBox"];
            this.settings = settings;
        }

        public List<GameNameData> GetGameData()
        {
            List<GameNameData> retVal = new List<GameNameData>();

            int total = root.ChildNodes.Count;
            int counter = 0;

            foreach (XmlNode xnode in root.ChildNodes)
            {
                OnProgress(counter, total);
                if (xnode.Name == "Game")
                {
                    string fullPath = xnode["ApplicationPath"].InnerText;
                    string title = xnode["Title"].InnerText;
                    string platform = xnode["Platform"].InnerText;
                    bool hide = xnode["Hide"].InnerText == "true";

                    List<string> innerFilenames = new List<string>();

                    if (!fullPath.StartsWith("steam://") && !hide)
                    {
                        switch (Path.GetExtension(fullPath))
                        {
                            case ".7z":
                            case ".zip":
                                try
                                {
                                    if (!string.IsNullOrWhiteSpace(settings.Core.SevenZipLib) && File.Exists(settings.Core.SevenZipLib))
                                    {
                                        SevenZipExtractor.SetLibraryPath(settings.Core.SevenZipLib);
                                        SevenZipExtractor engine = new SevenZipExtractor(fullPath);
                                        foreach (var item in engine.ArchiveFileData)
                                        {
                                            innerFilenames.Add(item.FileName);
                                        }
                                    }
                                }
                                catch { }
                                break;
                        }


                        if (innerFilenames.Count > 0)
                        {
                            foreach (string innerName in innerFilenames)
                            {
                                GameNameData dat = new GameNameData()
                                {
                                    OuterFileFullPath = fullPath,
                                    OuterFileName = Path.GetFileName(fullPath),
                                    InnerFileName = innerName,
                                    Title = title,
                                    Platform = platform
                                };

                                retVal.Add(dat);
                            }
                        }
                        else
                        {
                            GameNameData dat = new GameNameData()
                            {
                                OuterFileFullPath = fullPath,
                                OuterFileName = Path.GetFileName(fullPath),
                                Title = title,
                                Platform = platform
                            };

                            retVal.Add(dat);
                        }
                    }
                }
                counter++;
                OnProgress(counter, total);
            }
            
            return retVal;
        }

        public delegate void ProgressEventHandler(object sender, int counter, int total);
        public event ProgressEventHandler Progress;
        public virtual void OnProgress(int counter, int total)
        {
            var progressEvent = Progress;
            if (progressEvent != null)
            {
                progressEvent(this, counter, total);
            }
        }
    }
}