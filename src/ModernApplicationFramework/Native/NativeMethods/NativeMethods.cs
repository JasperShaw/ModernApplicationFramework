﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Win32.SafeHandles;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Native.Platform.Structs;
using ModernApplicationFramework.Native.Shell;
using ModernApplicationFramework.Native.TrinetCoreNtfs;
using Point = System.Windows.Point;

namespace ModernApplicationFramework.Native.NativeMethods
{
    internal static class NativeMethods
    {
        private const int WsExDlgmodalframe = 0x0001;
        private const int SwpNosize = 0x0001;
        private const int SwpNomove = 0x0002;
        private const int SwpNozorder = 0x0004;
        private const int SwpFramechanged = 0x0020;

        internal const int MaxPath = 256;
        private const string LongPathPrefix = @"\\?\";
        internal const char StreamSeparator = ':';
        internal const int DefaultBufferSize = 0x1000;
        private const int ErrorFileNotFound = 2;

        private static readonly char[] InvalidStreamNameChars =
            Path.GetInvalidFileNameChars().Where(c => c < 1 || c > 31).ToArray();

        internal static int NotifyOwnerActivate => User32.RegisterWindowMessage(
            "NOTIFYOWNERACTIVATE{A982313C-756C-4da9-8BD0-0C375A45784B}");

        public static bool IsWindowsVistaOrLater => Environment.OSVersion.Platform == PlatformID.Win32NT &&
                                                    Environment.OSVersion.Version >= new Version(6, 0, 6000);

        public static bool IsWindowsXpOrLater => Environment.OSVersion.Platform == PlatformID.Win32NT &&
                                                 Environment.OSVersion.Version >= new Version(5, 1, 2600);


        internal static unsafe ModifierKeys ModifierKeys
        {
            get
            {
                byte* lpKeyState = stackalloc byte[256];
                User32.GetKeyboardState(lpKeyState);
                var modifierKeys = ModifierKeys.None;
                if ((lpKeyState[16] & 128) == 128)
                    modifierKeys |= ModifierKeys.Shift;
                if ((lpKeyState[17] & 128) == 128)
                    modifierKeys |= ModifierKeys.Control;
                if ((lpKeyState[18] & 128) == 128)
                    modifierKeys |= ModifierKeys.Alt;
                if ((lpKeyState[91] & 128) == 128 || (lpKeyState[92] & 128) == 128)
                    modifierKeys |= ModifierKeys.Windows;
                return modifierKeys;
            }
        }

        [DllImport("user32.dll")]
        internal static extern short GetAsyncKeyState(ushort virtualKeyCode);

        internal static bool IsLeftButtonPressed()
        {
            return IsKeyPressed(1);
        }

        internal static bool IsRightButtonPressed()
        {
            return IsKeyPressed(2);
        }

        internal static IShellItem CreateItemFromParsingName(string path)
        {
            object item;
            var guid = new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe"); // IID_IShellItem
            var hr = Shell32.SHCreateItemFromParsingName(path, IntPtr.Zero, ref guid, out item);
            if (hr != 0)
                throw new Win32Exception(hr);
            return (IShellItem) item;
        }

        internal static Windowplacement GetWindowPlacement(IntPtr hwnd)
        {
            var lpwndpl = new Windowplacement();
            if (User32.GetWindowPlacement(hwnd, lpwndpl))
                return lpwndpl;
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        internal static void RemoveIcon(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var extendedStyle = User32.GetWindowLong(hwnd, (int) Gwl.Exstyle);
            SetWindowLong(hwnd, Gwl.Exstyle, extendedStyle | WsExDlgmodalframe);
            User32.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SwpNomove | SwpNosize | SwpNozorder | SwpFramechanged);
        }

        internal static int SetWindowLong(IntPtr hWnd, Gwl nIndex, int dwNewLong)
        {
            return User32.SetWindowLong(hWnd, (int) nIndex, dwNewLong);
        }

        internal static int CombineRgn(IntPtr hrnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, CombineMode combineMode)
        {
            return Gdi32.CombineRgn(hrnDest, hrgnSrc1, hrgnSrc2, (int) combineMode);
        }

        internal static Point GetCursorPos()
        {
            var point1 = new Platform.Structs.Point
            {
                X = 0,
                Y = 0
            };
            var point2 = new Point();
            if (!User32.GetCursorPos(ref point1))
                return point2;
            point2.X = point1.X;
            point2.Y = point1.Y;
            return point2;
        }

        internal static void SetOwner(IntPtr childHandle, IntPtr ownerHandle)
        {
            User32.SetWindowLong(
                childHandle,
                -8,
                ownerHandle.ToInt32());
        }

        internal static int GetXlParam(int lParam)
        {
            return LoWord(lParam);
        }

        internal static int GetYlParam(int lParam)
        {
            return HiWord(lParam);
        }

        internal static bool IsKeyPressed(int vKey)
        {
            return User32.GetKeyState(vKey) < 0;
        }

        internal static int HiWord(int value)
        {
            return (short) (value >> 16);
        }

        internal static int LoWord(int value)
        {
            return (short) (value & ushort.MaxValue);
        }

        internal static IntPtr MakeParam(int lowWord, int highWord)
        {
            return new IntPtr((lowWord & ushort.MaxValue) | (highWord << 16));
        }

        internal static int GetScWparam(IntPtr wParam)
        {
            return (int) wParam & 65520;
        }

        private static int MakeHrFromErrorCode(int errorCode)
        {
            return -2147024896 | errorCode;
        }

        private static string GetErrorMessage(int errorCode)
        {
            var lpBuffer = new StringBuilder(0x200);
            return 0 != Kernel32.FormatMessage(0x3200, IntPtr.Zero, errorCode, 0, lpBuffer, lpBuffer.Capacity,
                       IntPtr.Zero)
                ? lpBuffer.ToString()
                : $"Unknown error: {errorCode}";
        }

        private static void ThrowIoError(int errorCode, string path)
        {
            switch (errorCode)
            {
                case 0:
                {
                    break;
                }
                case 2: // File not found
                {
                    if (string.IsNullOrEmpty(path))
                        throw new FileNotFoundException();
                    throw new FileNotFoundException(null, path);
                }
                case 3: // Directory not found
                {
                    if (string.IsNullOrEmpty(path))
                        throw new DirectoryNotFoundException();
                    throw new DirectoryNotFoundException($"Could not find a part of the path {path}");
                }
                case 5: // Access denied
                {
                    if (string.IsNullOrEmpty(path))
                        throw new UnauthorizedAccessException();
                    throw new UnauthorizedAccessException($"Access to the path '{path}' was denied.");
                }
                case 15: // Drive not found
                {
                    if (string.IsNullOrEmpty(path))
                        throw new DriveNotFoundException();
                    throw new DriveNotFoundException(
                        $"Could not find the drive '{path}'. The drive might not be ready or might not be mapped.");
                }
                case 32: // Sharing violation
                {
                    if (string.IsNullOrEmpty(path))
                        throw new IOException(GetErrorMessage(errorCode), MakeHrFromErrorCode(errorCode));
                    throw new IOException(
                        $"The process cannot access the file '{path}' because it is being used by another process.");
                }
                case 80: // File already exists
                {
                    if (!string.IsNullOrEmpty(path))
                        throw new IOException($"The file '{path}' already exists.");
                    break;
                }
                case 87: // Invalid parameter
                {
                    throw new IOException(GetErrorMessage(errorCode), MakeHrFromErrorCode(errorCode));
                }
                case 183: // File or directory already exists
                {
                    if (!string.IsNullOrEmpty(path))
                        throw new IOException(
                            $"Cannot create '{path}' because a file or directory with the same name already exists.");
                    break;
                }
                case 206: // Path too long
                {
                    throw new PathTooLongException();
                }
                case 995: // Operation cancelled
                {
                    throw new OperationCanceledException();
                }
                default:
                {
                    Marshal.ThrowExceptionForHR(MakeHrFromErrorCode(errorCode));
                    break;
                }
            }
        }

        internal static void ThrowLastIoError(string path)
        {
            var errorCode = Marshal.GetLastWin32Error();
            if (0 != errorCode)
            {
                var hr = Marshal.GetHRForLastWin32Error();
                if (0 <= hr)
                    throw new Win32Exception(errorCode);
                ThrowIoError(errorCode, path);
            }
        }

        internal static NativeFileAccess ToNative(this FileAccess access)
        {
            NativeFileAccess result = 0;
            if (FileAccess.Read == (FileAccess.Read & access))
                result |= NativeFileAccess.GenericRead;
            if (FileAccess.Write == (FileAccess.Write & access))
                result |= NativeFileAccess.GenericWrite;
            return result;
        }

        internal static string BuildStreamPath(string filePath, string streamName)
        {
            var result = filePath;
            if (!String.IsNullOrEmpty(filePath))
            {
                if (1 == result.Length)
                    result = ".\\" + result;
                result += StreamSeparator + streamName + StreamSeparator + "$DATA";
                if (MaxPath <= result.Length)
                    result = LongPathPrefix + result;
            }

            return result;
        }

        internal static void ValidateStreamName(string streamName)
        {
            if (!String.IsNullOrEmpty(streamName) && -1 != streamName.IndexOfAny(InvalidStreamNameChars))
                throw new ArgumentException("The specified stream name contains invalid characters.");
        }

        internal static int SafeGetFileAttributes(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var result = Kernel32.GetFileAttributes(name);
            if (-1 == result)
            {
                var errorCode = Marshal.GetLastWin32Error();
                if (ErrorFileNotFound != errorCode)
                    ThrowLastIoError(name);
            }

            return result;
        }

        internal static bool SafeDeleteFile(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var result = Kernel32.DeleteFile(name);
            if (!result)
            {
                var errorCode = Marshal.GetLastWin32Error();
                if (ErrorFileNotFound != errorCode)
                    ThrowLastIoError(name);
            }

            return result;
        }

        internal static SafeFileHandle SafeCreateFile(string path, NativeFileAccess access, FileShare share,
            IntPtr security, FileMode mode, NativeFileFlags flags,
            IntPtr template)
        {
            var result = Kernel32.CreateFile(path, access, share, security, mode, flags, template);
            if (!result.IsInvalid && 1 != Kernel32.GetFileType(result))
            {
                result.Dispose();
                throw new NotSupportedException($"The specified file name '{path}' is not a disk-based file.");
            }

            return result;
        }

        private static long GetFileSize(string path, SafeFileHandle handle)
        {
            var result = 0L;
            if (null != handle && !handle.IsInvalid)
            {
                LargeInteger value;
                if (Kernel32.GetFileSizeEx(handle, out value))
                    result = value.ToInt64();
                else
                    ThrowLastIoError(path);
            }

            return result;
        }

        internal static long GetFileSize(string path)
        {
            var result = 0L;
            if (!String.IsNullOrEmpty(path))
                using (
                    var handle = SafeCreateFile(path, NativeFileAccess.GenericRead, FileShare.Read,
                        IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero))
                {
                    result = GetFileSize(path, handle);
                }

            return result;
        }

        internal static IList<Win32StreamInfo> ListStreams(string filePath)
        {
            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            if (-1 != filePath.IndexOfAny(Path.GetInvalidPathChars()))
                throw new ArgumentException("The specified stream name contains invalid characters.", nameof(filePath));

            var result = new List<Win32StreamInfo>();

            using (
                var hFile = SafeCreateFile(filePath, NativeFileAccess.GenericRead, FileShare.Read,
                    IntPtr.Zero, FileMode.Open, NativeFileFlags.BackupSemantics, IntPtr.Zero))
            using (var hName = new StreamName())
            {
                if (!hFile.IsInvalid)
                {
                    var streamId = new Win32StreamId();
                    var dwStreamHeaderSize = Marshal.SizeOf(streamId);
                    var finished = false;
                    var context = IntPtr.Zero;
                    int bytesRead;

                    try
                    {
                        while (!finished)
                            // Read the next stream header:
                            if (
                                !Kernel32.BackupRead(hFile, ref streamId, dwStreamHeaderSize, out bytesRead, false,
                                    false,
                                    ref context))
                            {
                                finished = true;
                            }
                            else if (dwStreamHeaderSize != bytesRead)
                            {
                                finished = true;
                            }
                            else
                            {
                                // Read the stream name:
                                string name;
                                if (0 >= streamId.StreamNameSize)
                                {
                                    name = null;
                                }
                                else
                                {
                                    hName.EnsureCapacity(streamId.StreamNameSize);
                                    if (
                                        !Kernel32.BackupRead(hFile, hName.MemoryBlock, streamId.StreamNameSize,
                                            out bytesRead, false, false, ref context))
                                    {
                                        name = null;
                                        finished = true;
                                    }
                                    else
                                    {
                                        // Unicode chars are 2 bytes:
                                        name = hName.ReadStreamName(bytesRead >> 1);
                                    }
                                }

                                // Add the stream info to the result:
                                if (!String.IsNullOrEmpty(name))
                                    result.Add(new Win32StreamInfo
                                    {
                                        StreamType = (FileStreamType) streamId.StreamId,
                                        StreamAttributes = (FileStreamAttributes) streamId.StreamAttributes,
                                        StreamSize = streamId.Size.ToInt64(),
                                        StreamName = name
                                    });

                                // Skip the contents of the stream:
                                int bytesSeekedLow, bytesSeekedHigh;
                                if (!finished
                                    && !Kernel32.BackupSeek(hFile, streamId.Size.Low, streamId.Size.High,
                                        out bytesSeekedLow, out bytesSeekedHigh, ref context))
                                    finished = true;
                            }
                    }
                    finally
                    {
                        // Abort the backup:
                        Kernel32.BackupRead(hFile, hName.MemoryBlock, 0, out bytesRead, true, false, ref context);
                    }
                }
            }

            return result;
        }

        internal static Monitorinfo MonitorInfoFromWindow(Window window)
        {
            var interop = new WindowInteropHelper(window);
            interop.EnsureHandle();
            var handle = interop.Handle;
            var hMonitor = User32.MonitorFromWindow(handle, 2);
            var monitorInfo = new Monitorinfo {CbSize = (uint) Marshal.SizeOf(typeof(Monitorinfo))};
            User32.GetMonitorInfo(hMonitor, ref monitorInfo);
            return monitorInfo;
        }

        internal static Monitorinfo MonitorInfoFromWindow(IntPtr hWnd)
        {
            var hMonitor = User32.MonitorFromWindow(hWnd, 2);
            var monitorInfo = new Monitorinfo {CbSize = (uint) Marshal.SizeOf(typeof(Monitorinfo))};
            User32.GetMonitorInfo(hMonitor, ref monitorInfo);
            return monitorInfo;
        }

        internal static void FindMaximumSingleMonitorRectangle(RECT windowRect, out RECT screenSubRect,
            out RECT monitorRect)
        {
            List<RECT> rects = new List<RECT>();
            User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT rect, IntPtr lpData) =>
                {
                    var monitorInfo = new Monitorinfo {CbSize = (uint) Marshal.SizeOf(typeof(Monitorinfo))};
                    User32.GetMonitorInfo(hMonitor, ref monitorInfo);
                    rects.Add(monitorInfo.RcWork);
                    return true;
                }, IntPtr.Zero);
            long num1 = 0;
            screenSubRect = new RECT()
            {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
            };
            monitorRect = new RECT()
            {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
            };
            foreach (RECT rect in rects)
            {
                RECT lprcSrc1 = rect;
                RECT lprcDst;
                User32.IntersectRect(out lprcDst, ref lprcSrc1, ref windowRect);
                long num2 = lprcDst.Width * lprcDst.Height;
                if (num2 > num1)
                {
                    screenSubRect = lprcDst;
                    monitorRect = rect;
                    num1 = num2;
                }
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        internal delegate bool EnumMonitorsDelegate(
            IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

        public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public delegate IntPtr WindowsHookProc(CbtHookAction code, IntPtr wParam, IntPtr lParam);

        internal enum ThreadPriority
        {
            THREAD_PRIORITY_IDLE = -15, // -0x0000000F
            THREAD_PRIORITY_LOWEST = -2,
            THREAD_PRIORITY_BELOW_NORMAL = -1,
            THREAD_PRIORITY_NORMAL = 0,
            THREAD_PRIORITY_ABOVE_NORMAL = 1,
            THREAD_PRIORITY_HIGHEST = 2,
            THREAD_PRIORITY_TIME_CRITICAL = 15, // 0x0000000F
            THREAD_MODE_BACKGROUND_BEGIN = 65536, // 0x00010000
            THREAD_MODE_BACKGROUND_END = 131072, // 0x00020000
        }

        private static int _vsmProcessUiBackgroundPriorityTaskQueue;

        public static int ProcessUiBackgroundTask
        {
            get
            {
                if (_vsmProcessUiBackgroundPriorityTaskQueue == 0)
                    _vsmProcessUiBackgroundPriorityTaskQueue = User32.RegisterWindowMessage("ProcessUiBackgroundTask{A982313C-756C-4da9-8BD0-0C375A45784B}");
                return _vsmProcessUiBackgroundPriorityTaskQueue;
            }
        }
    }
}