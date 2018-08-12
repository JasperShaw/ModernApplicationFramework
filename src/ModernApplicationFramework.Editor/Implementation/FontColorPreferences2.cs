using System;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.Editor.Implementation
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FontColorPreferences2
    {
        public IntPtr pguidColorService;
        public object pColorTable;
        public IntPtr hRegularViewFont;
        public IntPtr hBoldViewFont;
        public IntPtr pguidFontCategory;
        public IntPtr pguidColorCategory;
    }
}