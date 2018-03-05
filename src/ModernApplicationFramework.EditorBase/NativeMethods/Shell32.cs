using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Native.Platform.Structs;

namespace ModernApplicationFramework.EditorBase.NativeMethods
{
    internal static class Shell32
    {
        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref ShFileinfo psfi, uint cbSizeFileInfo, uint uFlags);
    }
}
