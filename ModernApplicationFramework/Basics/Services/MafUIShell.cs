using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Interop;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Structs;
using Point = System.Windows.Point;

namespace ModernApplicationFramework.Basics.Services
{
    [Export(typeof(IMafUIShell))]
    public class MafUIShell : IMafUIShell
    {
        public int GetDialogOwnerHwnd(out IntPtr phwnd)
        {
            phwnd = GetActiveWindowHandle();
            if (phwnd == IntPtr.Zero)
                return -1;
            return 0;
        }

        public int EnableModeless(int fEnable)
        {
            var handle = GetActiveWindowHandle();
            if (handle == IntPtr.Zero)
                return -1;
            if (fEnable == 1)
                User32.EnableWindow(handle, true);
            if (fEnable == 0)
                User32.EnableWindow(handle, false);
            return 0;
        }

        public int CenterDialogOnWindow(IntPtr hwndDialog, IntPtr hwndParent)
        {
            if (!User32.GetWindowRect(hwndParent, out RECT lpRect))
                return -1;
            var hwndSource = HwndSource.FromHwnd(hwndDialog);
            if (hwndSource?.CompositionTarget == null)
                return -1;

            int error = GetWindowByHandle(hwndDialog, out Window window);
            if (error != 0)
                return error;

            var point1 =
                hwndSource.CompositionTarget.TransformToDevice.Transform(new Point(window.ActualWidth,
                    window.ActualHeight));
            var rect = CenterRectOnSingleMonitor(lpRect, (int)point1.X, (int)point1.Y);
            var point2 =
                hwndSource.CompositionTarget.TransformFromDevice.Transform(new Point(rect.Left, rect.Top));
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = point2.X;
            window.Top = point2.Y;
            return 0;
        }

        protected IntPtr GetActiveWindowHandle()
        {
            return User32.GetActiveWindow();
        }

        private static RECT CenterRectOnSingleMonitor(RECT parentRect, int childWidth, int childHeight)
        {
            NativeMethods.FindMaximumSingleMonitorRectangle(parentRect, out RECT screenSubRect,
                out RECT monitorRect);
            return CenterInRect(screenSubRect, childWidth, childHeight, monitorRect);
        }

        private static RECT CenterInRect(RECT parentRect, int childWidth, int childHeight, RECT monitorClippingRect)
        {
            var rect = new RECT
            {
                Left = parentRect.Left + (parentRect.Width - childWidth) / 2,
                Top = parentRect.Top + (parentRect.Height - childHeight) / 2,
                Width = childWidth,
                Height = childHeight
            };
            rect.Left = Math.Min(rect.Right, monitorClippingRect.Right) - rect.Width;
            rect.Top = Math.Min(rect.Bottom, monitorClippingRect.Bottom) - rect.Height;
            rect.Left = Math.Max(rect.Left, monitorClippingRect.Left);
            rect.Top = Math.Max(rect.Top, monitorClippingRect.Top);
            return rect;
        }

        private int GetWindowByHandle(IntPtr handle, out Window window)
        {
            window = null;
            if (handle == IntPtr.Zero)
                return -1;
            HwndSource hwndSource = HwndSource.FromHwnd(handle);
            if (hwndSource == null)
                return -1;
            window = hwndSource.RootVisual as Window;
            if (window == null)
                return -1;
            return 0;
        }
    }
}
