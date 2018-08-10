using System.Runtime.InteropServices.ComTypes;

namespace ModernApplicationFramework.Text.NativeMethods
{
    internal static class NativeMethods
    {
        internal struct ByHandleFileInformation
        {
            public FILETIME CreationTime;
            public uint FileAttributes;
            public uint FileIndexHigh;
            public uint FileIndexLow;
            public uint FileSizeHigh;
            public uint FileSizeLow;
            public FILETIME LastAccessTime;
            public FILETIME LastWriteTime;
            public uint NumberOfLinks;
            public uint VolumeSerialNumber;
        }
    }
}