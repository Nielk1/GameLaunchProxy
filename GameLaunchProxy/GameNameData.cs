using System.Collections.Generic;

namespace GameLaunchProxy
{
    public class GameNameData
    {
        public string OuterFileFullPath { get; set; }
        public string OuterFileName { get; set; }
        public string InnerFileName { get; set; }
        public string Title { get; set; }
        public string Platform { get; set; }

        public bool Equals(GameNameData other)
        {
            return (InnerFileName == other.InnerFileName)
                && (OuterFileFullPath == other.OuterFileFullPath)
                && (OuterFileName == other.OuterFileName);
            //&& (x.Platform == y.InnerFileName)
            //&& (x.Title == y.InnerFileName);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                if (InnerFileName != null) hash = hash * 23 + InnerFileName.GetHashCode();
                if (OuterFileFullPath != null) hash = hash * 23 + OuterFileFullPath.GetHashCode();
                if (OuterFileName != null) hash = hash * 23 + OuterFileName.GetHashCode();
                return hash;
            }
        }
    }
}