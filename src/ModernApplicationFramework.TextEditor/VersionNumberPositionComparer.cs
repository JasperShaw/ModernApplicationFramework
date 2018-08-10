using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    internal class VersionNumberPositionComparer : IComparer<VersionNumberPosition>
    {
        public static VersionNumberPositionComparer Instance = new VersionNumberPositionComparer();

        public int Compare(VersionNumberPosition x, VersionNumberPosition y)
        {
            return x.VersionNumber - y.VersionNumber;
        }
    }
}