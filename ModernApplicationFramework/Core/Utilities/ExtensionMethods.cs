using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using ModernApplicationFramework.Core.NativeMethods;

namespace ModernApplicationFramework.Core.Utilities
{
    public static class ExtensionMethods
    {
        public static bool IsConnectedToPresentationSource(this DependencyObject obj)
        {
            return PresentationSource.FromDependencyObject(obj) != null;
        }

        public static void RaiseEvent(this EventHandler eventHandler, object source)
        {
            RaiseEvent(eventHandler, source, EventArgs.Empty);
        }

        public static void RaiseEvent(this EventHandler eventHandler, object source, EventArgs args)
        {
            eventHandler?.Invoke(source, args);
        }

        public static bool AcquireWin32Focus(this DependencyObject obj, out IntPtr previousFocus)
        {
            HwndSource hwndSource = PresentationSource.FromDependencyObject(obj) as HwndSource;
            if (hwndSource != null)
            {
                previousFocus = User32.GetFocus();
                if (previousFocus != hwndSource.Handle)
                {
                    User32.SetFocus(hwndSource.Handle);
                    return true;
                }
            }
            previousFocus = IntPtr.Zero;
            return false;
        }
    }
}