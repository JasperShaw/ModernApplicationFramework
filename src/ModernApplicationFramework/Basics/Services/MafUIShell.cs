using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Structs;
using ModernApplicationFramework.Utilities.Interfaces;
using Point = System.Windows.Point;

namespace ModernApplicationFramework.Basics.Services
{
    /// <inheritdoc />
    /// <summary>
    /// A service providing access to basic windowing functionality.
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Services.IMafUIShell" />
    [Export(typeof(IMafUIShell))]
    public class MafUIShell : IMafUIShell
    {
        /// <inheritdoc />
        /// <summary>
        /// Returns the HWND that can be used to parent modal dialogs.
        /// </summary>
        /// <param name="phwnd">Pointer to a window handle that can be used to parent modal dialogs.</param>
        /// <returns>
        /// If the method succeeds, it returns 0. If it fails, it returns an error code.
        /// </returns>
        public int GetDialogOwnerHwnd(out IntPtr phwnd)
        {
            phwnd = GetActiveWindowHandle();
            if (phwnd == IntPtr.Zero)
                return -1;
            return 0;
        }

        /// <inheritdoc />
        /// <summary>
        /// Enables or disables a frame's modeless dialog box.
        /// </summary>
        /// <param name="fEnable">1 when exiting a modal state. 0 when entering a modal state.</param>
        /// <returns>
        /// If the method succeeds, it returns 0. If it fails, it returns an error code.
        /// </returns>
        public IntPtr EnableModeless(int fEnable)
        {
            var handle = GetActiveWindowHandle();
            EnableModeless(fEnable, handle);
            return handle;
        }


        public void EnableModeless(int fEnable, IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return;
            if (fEnable == 1)
                User32.EnableWindow(handle, true);
            if (fEnable == 0)
                User32.EnableWindow(handle, false);
        }

        /// <inheritdoc />
        /// <summary>
        /// Centers the provided dialog box HWND on the parent HWND (if provided), or on the main window.
        /// </summary>
        /// <param name="hwndDialog">Specifies HWND dialog.</param>
        /// <param name="hwndParent">Specifies HWND parent.</param>
        /// <returns>
        /// If the method succeeds, it returns 0. If it fails, it returns an error code.
        /// </returns>
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

        /// <inheritdoc />
        /// <summary>
        /// Returns the name of the application.
        /// </summary>
        /// <param name="pbstrAppName">Pointer to the name of the application</param>
        /// <returns>
        /// If the method succeeds, it returns 0. If it fails, it returns an error code.
        /// </returns>
        public int GetAppName(out string pbstrAppName)
        {
            pbstrAppName = string.Empty;
            var env = IoC.Get<IEnvironmentVariables>();
            if (env == null)
                return -1;
            pbstrAppName = env.ApplicationName;
            return 0;
        }


        /// <summary>
        /// Shows a context menu at specified position.
        /// </summary>
        /// <param name="position">The absolte position.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void ShowContextMenu(Point position)
        {
            var cm = new ContextMenu();
            cm.Placement = PlacementMode.Absolute;
            cm.HorizontalOffset = position.X;
            cm.VerticalOffset = position.Y;
            cm.IsOpen = true;
        }

        /// <summary>
        /// Gets the pointer to the active window.
        /// </summary>
        /// <returns>The point to the active window</returns>
        protected IntPtr GetActiveWindowHandle()
        {
            return User32.GetActiveWindow();
        }

        private static RECT CenterRectOnSingleMonitor(RECT parentRect, int childWidth, int childHeight)
        {
            NativeMethods.FindMaximumSingleMonitorRectangle(parentRect, out RECT screenSubRect,
                out var monitorRect);
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

        private static int GetWindowByHandle(IntPtr handle, out Window window)
        {
            window = null;
            if (handle == IntPtr.Zero)
                return -1;
            var hwndSource = HwndSource.FromHwnd(handle);
            if (hwndSource == null)
                return -1;
            window = hwndSource.RootVisual as Window;
            if (window == null)
                return -1;
            return 0;
        }
    }
}
