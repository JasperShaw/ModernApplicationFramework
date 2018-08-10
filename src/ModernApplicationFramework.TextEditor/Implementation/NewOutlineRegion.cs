using System.Runtime.InteropServices;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NewOutlineRegion
    {
        public uint dwState;
        public TextSpan tsHiddenText;
    }
}