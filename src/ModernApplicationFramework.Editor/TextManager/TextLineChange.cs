using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.TextManager
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TextLineChange
    {
        public int iStartIndex;
        public int iStartLine;
        public int iOldEndIndex;
        public int iOldEndLine;
        public int iNewEndIndex;
        public int iNewEndLine;
    }
}