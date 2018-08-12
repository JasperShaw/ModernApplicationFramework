using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.TextManager
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NewHiddenRegion
    {
        public int iType;
        public uint dwBehavior;
        public uint dwState;
        public TextSpan tsHiddenText;
        public string pszBanner;
        public uint dwClient;
    }
}