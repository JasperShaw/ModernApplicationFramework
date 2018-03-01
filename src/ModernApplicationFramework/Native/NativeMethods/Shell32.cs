using System;
using System.Runtime.InteropServices;
using System.Text;
using ModernApplicationFramework.Native.Platform.Structs;
using ModernApplicationFramework.Native.Shell;

namespace ModernApplicationFramework.Native.NativeMethods
{
    internal static class Shell32
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            IntPtr pbc, ref Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SHBrowseForFolder(ref BrowseInfo lpbi);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SHGetPathFromIDList(IntPtr pidl, StringBuilder pszPath);

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, Environment.SpecialFolder nFolder, ref IntPtr ppidl);


        [DllImport("shell32", CallingConvention = CallingConvention.StdCall)]
        internal static extern int SHAppBarMessage(int dwMessage, ref Appbardata pData);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern int ExtractIconEx(string szFileName, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, int nIcons);

        [DllImport("shell32.dll", PreserveSig = false)]
        public static extern IMalloc SHGetMalloc();
    }
}