using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Docking.NativeMethods;

namespace ModernApplicationFramework.Docking
{
    internal static class WindowHelper
    {
        internal static bool GetParentWindowHandle(this Visual element, out IntPtr hwnd)
        {
            hwnd = IntPtr.Zero;
            var wpfHandle = PresentationSource.FromVisual(element) as HwndSource;

            if (wpfHandle == null)
                return false;

            hwnd = User32.GetParent(wpfHandle.Handle);
            if (hwnd == IntPtr.Zero)
                hwnd = wpfHandle.Handle;
            return true;
        }

        internal static void SetParentToMainWindowOf(this Window window, Visual element)
        {
            var wndParent = Window.GetWindow(element);
            if (wndParent != null)
            {
                window.Owner = wndParent;
            }
            else
            {
                IntPtr parentHwnd;
                if (GetParentWindowHandle(element, out parentHwnd))
                    NativeMethods.NativeMethods.SetOwner(new WindowInteropHelper(window).EnsureHandle(), parentHwnd);
            }
        }

        internal static void SetParentWindowToNull(this Window window)
        {
            if (window.Owner != null)
                window.Owner = null;
            else
                NativeMethods.NativeMethods.SetOwner(new WindowInteropHelper(window).EnsureHandle(), IntPtr.Zero);
        }
    }
}
