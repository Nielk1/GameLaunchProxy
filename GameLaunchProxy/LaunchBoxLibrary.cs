using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GameLaunchProxy
{
    public class LaunchBoxLibrary
    {
        private XmlDocument doc;
        private XmlElement root;

        public LaunchBoxLibrary(string launchBoxLibrary)
        {
            doc = new XmlDocument();
            doc.Load(launchBoxLibrary);
            root = doc["LaunchBox"];
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

                    if (!fullPath.StartsWith("steam://") && !hide)
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