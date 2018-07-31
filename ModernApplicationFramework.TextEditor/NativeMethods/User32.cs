using System;
using System.Runtime.InteropServices;

namespace ModernApplicationFramework.TextEditor.NativeMethods
{
    internal static class User32
    {
        [DllImport("user32.dll")]
        internal static extern int GetCaretBlinkTime();

        [DllImport("Gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int index);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hdc);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromPoint(NativeMethods.POINT pt, int dwFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref NativeMethods.MONITORINFO lpmi);

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(int dwThread);
    }
}
