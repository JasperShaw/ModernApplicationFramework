using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Structs;
using ModernApplicationFramework.Utilities;
using Point = System.Windows.Point;

namespace ModernApplicationFramework.Basics.Services
{
    public static class WindowHelper
    {
        public static int ShowModalElement(Window window, INotifyPropertyChanged dataSource)
        {

            var dialogOwnerHandle = GetDialogOwnerHandle();
            return ShowModalElement(window, dataSource, dialogOwnerHandle);
        }

        public static int ShowModalElement(Window window, INotifyPropertyChanged dataSource, IntPtr parent)
        {
            if (window == null)
                throw new COMException("Window cannot be null");
            window.DataContext = dataSource;
            var service = IoC.Get<IMafUIShell>();
            if (service == null)
                throw new COMException("Shell not found", -2147467259);
            return ShowModal(window, parent);
        }

        internal static bool GetParentWindowHandle(this Visual element, out IntPtr hwnd)
        {
            hwnd = IntPtr.Zero;
            if (!(PresentationSource.FromVisual(element) is HwndSource wpfHandle))
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
                if (GetParentWindowHandle(element, out var parentHwnd))
                    NativeMethods.SetOwner(new WindowInteropHelper(window).EnsureHandle(), parentHwnd);
            }
        }

        internal static void SetParentWindowToNull(this Window window)
        {
            if (window.Owner != null)
                window.Owner = null;
            else
                NativeMethods.SetOwner(new WindowInteropHelper(window).EnsureHandle(), IntPtr.Zero);
        }

        public static int ShowModal(Window window)
        {
            var dialogOwnerHandle = GetDialogOwnerHandle();
            return ShowModal(window, dialogOwnerHandle);
        }

        public static IntPtr GetDialogOwnerHandle()
        {
            var service = IoC.Get<IMafUIShell>();
            if (service == null)
                throw new COMException("Cannot get IMafUIShell service.", -2147467259);
            var dialogOwnerHwnd = service.GetDialogOwnerHwnd(out var handle);
            if (dialogOwnerHwnd != 0)
                throw new COMException("Cannot get parent window handle from shell.", dialogOwnerHwnd);
            return handle;
        }

        public static int ShowModal(Window window, IntPtr parent)
        {
            Validate.IsNotNull(window, "window");
            var service = IoC.Get<IMafUIShell>();
            if (service == null)
                throw new COMException("Cannot get IMafUIShell service.", -2147467259);

            var handle = User32.GetActiveWindow();
            service.EnableModeless(0);
            try
            {
                var helper = new WindowInteropHelper(window) {Owner = parent};
                if (window.WindowStartupLocation == WindowStartupLocation.CenterOwner)
                    window.SourceInitialized += (param0, param1) =>
                    {
                        if (!User32.GetWindowRect(parent, out var lpRect))
                            return;
                        var hwndSource = HwndSource.FromHwnd(helper.Handle);
                        if (hwndSource?.CompositionTarget == null)
                            return;
                        var point1 =
                            hwndSource.CompositionTarget.TransformToDevice.Transform(new Point(window.ActualWidth,
                                window.ActualHeight));
                        var rect = CenterRectOnSingleMonitor(lpRect, (int) point1.X, (int) point1.Y);
                        var point2 =
                            hwndSource.CompositionTarget.TransformFromDevice.Transform(new Point(rect.Left, rect.Top));
                        window.WindowStartupLocation = WindowStartupLocation.Manual;
                        window.Left = point2.X;
                        window.Top = point2.Y;
                    };
                var nullable = window.ShowDialog();
                return nullable.HasValue ? (nullable.Value ? 1 : 2) : 0;
            }
            finally
            {
                service.EnableModeless(1, handle);
            }
        }

        private static RECT CenterRectOnSingleMonitor(RECT parentRect, int childWidth, int childHeight)
        {
            NativeMethods.FindMaximumSingleMonitorRectangle(parentRect, out var screenSubRect,
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


        public static bool CreateChildElement(FrameworkElement element, FrameworkElement parent)
        {
            element.Visibility = Visibility.Visible;
            switch (parent)
            {
                case Decorator decorator:
                    decorator.Child = element;
                    return true;
                case ContentControl contentControl:
                    contentControl.Content = element;
                    return true;
                case Panel panel:
                    panel.Children.Add(element);
                    return true;
            }
            return false;
        }
    }
}