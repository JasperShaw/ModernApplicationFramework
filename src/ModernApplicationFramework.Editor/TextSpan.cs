using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TextSpan
    {
        public int iStartIndex;
        public int iStartLine;
        public int iEndIndex;
        public int iEndLine;
    }
}