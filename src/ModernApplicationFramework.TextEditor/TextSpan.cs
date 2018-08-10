using System.Runtime.InteropServices;

namespace ModernApplicationFramework.TextEditor
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