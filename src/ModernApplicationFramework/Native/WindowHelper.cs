/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Linq;
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

namespace ModernApplicationFramework.Native
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
            var dialogOwnerHwnd = service.GetDialogOwnerHwnd(out IntPtr handle);
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

            var handel = service.EnableModeless(0);
            try
            {
                var helper = new WindowInteropHelper(window) {Owner = parent};
                if (window.WindowStartupLocation == WindowStartupLocation.CenterOwner)
                    window.SourceInitialized += (param0, param1) =>
                    {
                        if (!User32.GetWindowRect(parent, out RECT lpRect))
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
                service.EnableModeless(1, handel);
            }
        }

        private static RECT CenterRectOnSingleMonitor(RECT parentRect, int childWidth, int childHeight)
        {
            NativeMethods.NativeMethods.FindMaximumSingleMonitorRectangle(parentRect, out RECT screenSubRect,
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