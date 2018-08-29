using System;
using System.Windows;
using System.Windows.Interop;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.Extensions
{
    public static class WindowStyleHelper
    {
        public static readonly DependencyProperty HasMaximizeButtonProperty =
            DependencyProperty.RegisterAttached("HasMaximizeButton", typeof(bool), typeof(WindowStyleHelper),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnWindowStyleChanged));

        public static readonly DependencyProperty HasMinimizeButtonProperty =
            DependencyProperty.RegisterAttached("HasMinimizeButton", typeof(bool), typeof(WindowStyleHelper),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnWindowStyleChanged));

        public static bool GetHasMaximizeButton(Window window)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));
            return (bool) window.GetValue(HasMaximizeButtonProperty);
        }

        public static bool GetHasMinimizeButton(Window window)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));
            return (bool) window.GetValue(HasMinimizeButtonProperty);
        }

        public static void SetHasMaximizeButton(Window window, bool value)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));
            window.SetValue(HasMaximizeButtonProperty, Boxes.Box(value));
        }

        public static void SetHasMinimizeButton(Window window, bool value)
        {
            if (window == null)
                throw new ArgumentNullException(nameof(window));
            window.SetValue(HasMinimizeButtonProperty, Boxes.Box(value));
        }

        private static void OnWindowStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (Window) d;
            var source = PresentationSource.FromVisual(window) as HwndSource;
            if (source == null)
                return;
            UpdateWindowStyle(window, source.Handle);
        }

        private static void UpdateWindowStyle(Window window, IntPtr handle)
        {
            var windowLong = User32.GetWindowLong32(handle, -16);
            var num1 = !GetHasMaximizeButton(window) ? windowLong & -65537 : windowLong | 65536;
            var num2 = !GetHasMinimizeButton(window) ? num1 & -131073 : num1 | 131072;
            User32.SetWindowLong(handle, -16, num2);
        }
    }
}