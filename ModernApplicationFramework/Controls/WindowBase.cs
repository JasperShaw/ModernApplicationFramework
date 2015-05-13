using System;
using System.Windows;
using System.Windows.Interop;
using ModernApplicationFramework.Core.NativeMethods;
using ModernApplicationFramework.Core.Platform;

namespace ModernApplicationFramework.Controls
{
    public abstract class WindowBase : Window
    {
        public static readonly DependencyProperty HasMaximizeButtonProperty = DependencyProperty.Register(
            "HasMaximizeButton", typeof (bool), typeof (WindowBase), new PropertyMetadata(Boxes.BoolFalse, OnWindowStyleChanged));

        public static readonly DependencyProperty HasMinimizeButtonProperty = DependencyProperty.Register(
            "HasMinimizeButton", typeof (bool), typeof (WindowBase), new PropertyMetadata(Boxes.BoolFalse, OnWindowStyleChanged));

        public static readonly DependencyProperty HasDialogFrameProperty = DependencyProperty.Register(
            "HasDialogFrame", typeof(bool), typeof(WindowBase), new PropertyMetadata(Boxes.BoolTrue, OnWindowStyleChanged));

        public static readonly DependencyProperty IsCloseButtonEnabledProperty = DependencyProperty.Register(
            "IsCloseButtonEnabled", typeof(bool), typeof(WindowBase), new PropertyMetadata(Boxes.BoolTrue, OnWindowStyleChanged));

        private HwndSource _hwndSource;

        public bool IsCloseButtonEnabled
        {
            get { return (bool) GetValue(IsCloseButtonEnabledProperty); }
            set { SetValue(IsCloseButtonEnabledProperty, value); }
        }

        public bool HasDialogFrame
        {
            get { return (bool) GetValue(HasDialogFrameProperty); }
            set { SetValue(HasDialogFrameProperty, value); }
        }

        public bool HasMinimizeButton
        {
            get { return (bool) GetValue(HasMinimizeButtonProperty); }
            set { SetValue(HasMinimizeButtonProperty, value); }
        }

        public bool HasMaximizeButton
        {
            get { return (bool) GetValue(HasMaximizeButtonProperty); }
            set { SetValue(HasMaximizeButtonProperty, value); }
        }

        static WindowBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowBase), new FrameworkPropertyMetadata(typeof(WindowBase)));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            _hwndSource = (HwndSource) PresentationSource.FromVisual(this);
            _hwndSource?.AddHook(WndProcHook);
            UpdateWindowStyle();
            base.OnSourceInitialized(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_hwndSource != null)
            {
                _hwndSource.Dispose();
                _hwndSource = null;
            }
            base.OnClosed(e);
        }

        private void UpdateWindowStyle()
        {
            if (_hwndSource == null)
                return;
            var handle = _hwndSource.Handle;
            if (handle == IntPtr.Zero)
                return;
            var windowLong1 = NativeMethods.GetWindowLong2(handle, -16);
            var num1 = !HasMaximizeButton ? windowLong1 & -65537 : windowLong1 | 65536;
            var num2 = !HasMinimizeButton ? num1 & -131073 : num1 | 131072;
            NativeMethods.SetWindowLong(handle, -16, num2);
            NativeMethods.GetWindowLong2(handle, -20);
            NativeMethods.SendMessage(handle, 128, new IntPtr(1), IntPtr.Zero);
            NativeMethods.SendMessage(handle, 128, new IntPtr(0), IntPtr.Zero);
            var systemMenu = NativeMethods.GetSystemMenu(handle, false);
            if (systemMenu != IntPtr.Zero)
            {
                var num5 = IsCloseButtonEnabled ? 0U : 1U;
                NativeMethods.EnableMenuItem(systemMenu, 61536U, 0U | num5);
            }
            NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, 35);
        }

        private IntPtr WndProcHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if ((msg != 26 || wParam.ToInt32() != 67) && msg != 21)
                return IntPtr.Zero;
            OnDialogThemeChanged();
            handled = true;
            return IntPtr.Zero;
        }


        private static void OnWindowStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((WindowBase) dependencyObject).UpdateWindowStyle();
        }

        protected virtual void OnDialogThemeChanged() { }
    }
}
