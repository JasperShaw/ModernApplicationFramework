using System.Collections.Generic;
using ModernApplicationFramework.TextEditor;

namespace ModernApplicationFramework.Modules.Editor.Text.Implementation
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