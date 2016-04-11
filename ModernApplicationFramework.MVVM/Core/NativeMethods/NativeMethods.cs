using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ModernApplicationFramework.MVVM.Core.NativeMethods
{
    internal class NativeMethods
    {
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        private const int GwlExstyle = -20;
        private const int WsExDlgmodalframe = 0x0001;
        private const int SwpNosize = 0x0001;
        private const int SwpNomove = 0x0002;
        private const int SwpNozorder = 0x0004;
        private const int SwpFramechanged = 0x0020;

        public static void RemoveIcon(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            int extendedStyle = GetWindowLong(hwnd, GwlExstyle);
            SetWindowLong(hwnd, GwlExstyle, extendedStyle | WsExDlgmodalframe);
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SwpNomove | SwpNosize | SwpNozorder | SwpFramechanged);
        }
    }
}
