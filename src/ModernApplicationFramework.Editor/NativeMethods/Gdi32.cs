using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Editor.Interop;

namespace ModernApplicationFramework.Editor.NativeMethods
{
    internal static class Gdi32
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern int EnumFontFamiliesEx(IntPtr hdc, ref Logfont lpLogfont, NativeMethods.FontEnumProc lpEnumFontFamExProc, IntPtr lParam, uint dwFlags);
    }
}
