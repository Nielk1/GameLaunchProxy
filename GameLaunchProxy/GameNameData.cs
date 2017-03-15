using System.Collections.Generic;

namespace GameLaunchProxy
{
    public class GameNameData
    {
        public string GUID { get; set; }
        public string Title { get; set; }
        public string Platform { get; set; }

        public bool Equals(GameNameData other)
        {
            return (GUID == other.GUID);
        }
    }
}