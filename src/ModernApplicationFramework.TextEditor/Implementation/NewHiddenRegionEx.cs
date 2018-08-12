using System;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Implementation
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct NewHiddenRegionEx
    {
        public int iType;
        public uint dwBehavior;
        public uint dwState;
        public TextSpan tsHiddenText;
        public string pszBanner;
        public uint dwClient;
        public uint dwLength;
        public IntPtr pBannerAttr;
    }
}