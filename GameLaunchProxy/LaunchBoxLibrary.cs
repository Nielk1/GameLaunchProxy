using SevenZip;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GameLaunchProxy
{
    public class LaunchBoxLibrary
    {
        private List<XmlDocument> docs;
        private List<XmlElement> roots;
        private Settings settings;

        public LaunchBoxLibrary(string launchBoxLibrary, Settings settings)
        {
            docs = new List<XmlDocument>();
            roots = new List<XmlElement>();

            if (File.Exists(launchBoxLibrary))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(launchBoxLibrary);
                XmlElement root = doc["LaunchBox"];
                docs.Add(doc);
                roots.Add(root);
            }else if(Directory.Exists(launchBoxLibrary))
            {
                Directory.EnumerateFiles(Path.Combine(launchBoxLibrary, "Platforms")).ToList().ForEach(file =>
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(file);
                    XmlElement root = doc["LaunchBox"];
                    docs.Add(doc);
                    roots.Add(root);
                });
            }
            this.settings = settings;
        }

        public List<GameNameData> GetGameData()
        {
            List<GameNameData> retVal = new List<GameNameData>();

            //int total = root.ChildNodes.Count;
            int total = roots.Sum(dr => dr.ChildNodes.Count);
            int counter = 0;

            for (int index = 0; index < docs.Count; index++)
            {
                if (EditForm.KillWorker) return null;
                XmlDocument doc = docs[index];
                XmlElement root = roots[index];

                foreach (XmlNode xnode in root.ChildNodes)
                {
                    if (EditForm.KillWorker) return null;
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