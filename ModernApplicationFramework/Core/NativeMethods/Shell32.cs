using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Core.Platform.Structs;

namespace ModernApplicationFramework.Core.NativeMethods
{
    internal static class Shell32
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            IntPtr pbc, ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv);


        [DllImport("shell32", CallingConvention = CallingConvention.StdCall)]
        internal static extern int SHAppBarMessage(int dwMessage, ref Appbardata pData);
    }
}