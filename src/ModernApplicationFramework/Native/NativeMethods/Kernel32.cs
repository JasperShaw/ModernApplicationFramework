using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Native.Platform.Structs;
using ModernApplicationFramework.Native.Shell;
using ModernApplicationFramework.Native.TrinetCoreNtfs;

namespace ModernApplicationFramework.Native.NativeMethods
{
    internal static class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int GetDriveTypeW(string nDrive);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentThread();

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

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint FormatMessage([MarshalAs(UnmanagedType.U4)] FormatMessageFlags dwFlags, IntPtr lpSource,
            uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer,
            uint nSize, string[] Arguments);

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

        [DllImport("kernel32", SetLastError = true),
         ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeModuleHandle LoadLibraryEx(
            string lpFileName,
            IntPtr hFile,
            LoadLibraryExFlags dwFlags
        );

        [DllImport("kernel32.dll"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern void ReleaseActCtx(IntPtr hActCtx);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetThreadPriority(IntPtr hThread, NativeMethods.ThreadPriority nPriority);
    }
}