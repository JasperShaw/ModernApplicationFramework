using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ModernApplicationFramework.Modules.Editor.NativeMethods
{
    internal class User32
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyCursor(IntPtr hCursor);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll")]
        internal static extern int GetCaretBlinkTime();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromPoint(NativeMethods.POINT pt, int dwFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref NativeMethods.Monitorinfo lpmi);

        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(int dwThread);

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);

        [DllImport("Gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int index);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LoadImage(IntPtr hinst, string lpszName, NativeMethods.ImageType uType, int cxDesired, int cyDesired, NativeMethods.ImageFormatRequest fuLoad);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int nAction, int nParam, ref int value, int ignore);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetSystemMetrics(int nIndex);

    }
}
