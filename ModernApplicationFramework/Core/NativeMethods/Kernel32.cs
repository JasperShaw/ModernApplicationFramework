using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using ModernApplicationFramework.Core.Platform.Enums;
using ModernApplicationFramework.Core.Platform.Structs;
using ModernApplicationFramework.Core.TrinetCoreNtfs;

namespace ModernApplicationFramework.Core.NativeMethods
{
    internal static class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int GetDriveTypeW(string nDrive);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int FormatMessage(
            int dwFlags,
            IntPtr lpSource,
            int dwMessageId,
            int dwLanguageId,
            StringBuilder lpBuffer,
            int nSize,
            IntPtr vaListArguments);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int GetFileAttributes(string fileName);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetFileSizeEx(SafeFileHandle handle, out LargeInteger size);

        [DllImport("kernel32.dll")]
        internal static extern int GetFileType(SafeFileHandle handle);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(
            string name,
            NativeFileAccess access,
            FileShare share,
            IntPtr security,
            FileMode mode,
            NativeFileFlags flags,
            IntPtr template);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteFile(string name);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool BackupRead(
            SafeFileHandle hFile,
            ref Win32StreamId pBuffer,
            int numberOfBytesToRead,
            out int numberOfBytesRead,
            [MarshalAs(UnmanagedType.Bool)] bool abort,
            [MarshalAs(UnmanagedType.Bool)] bool processSecurity,
            ref IntPtr context);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool BackupRead(
            SafeFileHandle hFile,
            SafeHGlobalHandle pBuffer,
            int numberOfBytesToRead,
            out int numberOfBytesRead,
            [MarshalAs(UnmanagedType.Bool)] bool abort,
            [MarshalAs(UnmanagedType.Bool)] bool processSecurity,
            ref IntPtr context);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool BackupSeek(
            SafeFileHandle hFile,
            int bytesToSeekLow,
            int bytesToSeekHigh,
            out int bytesSeekedLow,
            out int bytesSeekedHigh,
            ref IntPtr context);
    }
}