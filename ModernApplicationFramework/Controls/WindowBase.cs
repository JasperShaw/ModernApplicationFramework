using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.NativeMethods;
using ModernApplicationFramework.Core.Platform;
using ModernApplicationFramework.Core.Themes;

namespace ModernApplicationFramework.Controls
{
    public abstract class WindowBase : Window, IHasTheme
    {
        public static readonly DependencyProperty HasMaximizeButtonProperty = DependencyProperty.Register(
            "HasMaximizeButton", typeof(bool), typeof(WindowBase),
            new PropertyMetadata(Boxes.BoolFalse, OnWindowStyleChanged));

        public static readonly DependencyProperty HasMinimizeButtonProperty = DependencyProperty.Register(
            "HasMinimizeButton", typeof(bool), typeof(WindowBase),
            new PropertyMetadata(Boxes.BoolFalse, OnWindowStyleChanged));

        public static readonly DependencyProperty HasDialogFrameProperty = DependencyProperty.Register(
            "HasDialogFrame", typeof(bool), typeof(WindowBase),
            new PropertyMetadata(Boxes.BoolTrue, OnWindowStyleChanged));

        public static readonly DependencyProperty IsCloseButtonEnabledProperty = DependencyProperty.Register(
            "IsCloseButtonEnabled", typeof(bool), typeof(WindowBase),
            new PropertyMetadata(Boxes.BoolTrue, OnWindowStyleChanged));

        private HwndSource _hwndSource;

        static WindowBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowBase),
                new FrameworkPropertyMetadata(typeof(WindowBase)));
        }

        public bool HasDialogFrame
        {
            get { return (bool) GetValue(HasDialogFrameProperty); }
            set { SetValue(HasDialogFrameProperty, value); }
        }

        public bool HasMaximizeButton
        {
            get { return (bool) GetValue(HasMaximizeButtonProperty); }
            set { SetValue(HasMaximizeButtonProperty, value); }
        }

        public bool HasMinimizeButton
        {
            get { return (bool) GetValue(HasMinimizeButtonProperty); }
            set { SetValue(HasMinimizeButtonProperty, value); }
        }

        public bool IsCloseButtonEnabled
        {
            get { return (bool) GetValue(IsCloseButtonEnabledProperty); }
            set { SetValue(IsCloseButtonEnabledProperty, value); }
        }

        protected virtual void OnDialogThemeChanged() {}

        protected override void OnClosed(EventArgs e)
        {
            if (_hwndSource != null)
            {
                _hwndSource.Dispose();
                _hwndSource = null;
            }
            base.OnClosed(e);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            _hwndSource = (HwndSource) PresentationSource.FromVisual(this);
            _hwndSource?.AddHook(WndProcHook);
            UpdateWindowStyle();
            base.OnSourceInitialized(e);

            var theme = Owner as IHasTheme;
            if (theme != null)
                Theme = theme.Theme;
        }


        private static void OnWindowStyleChanged(DependencyObject dependencyObject,
                                                 DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((WindowBase) dependencyObject).UpdateWindowStyle();
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

        public event EventHandler<ThemeChangedEventArgs> OnThemeChanged;

        private Theme _theme;

        public Theme Theme
        {
            get { return _theme; }
            set
            {
                if (value == null)
                    throw new NoNullAllowedException();
                if (Equals(value, _theme))
                    return;
                var oldTheme = _theme;
                _theme = value;
                ChangeTheme(oldTheme, _theme);
                OnRaiseThemeChanged(new ThemeChangedEventArgs(value, oldTheme));
            }
        }

        private void ChangeTheme(Theme oldValue, Theme newValue)
        {
            var resources = Resources;
            resources.Clear();
            resources.MergedDictionaries.Clear();
            if (oldValue != null)
            {
                var resourceDictionaryToRemove =
                    resources.MergedDictionaries.FirstOrDefault(r => r.Source == oldValue.GetResourceUri());
                if (resourceDictionaryToRemove != null)
                    resources.MergedDictionaries.Remove(resourceDictionaryToRemove);
            }
            if (newValue != null)
                resources.MergedDictionaries.Add(new ResourceDictionary { Source = newValue.GetResourceUri() });
        }

        protected virtual void OnRaiseThemeChanged(ThemeChangedEventArgs e)
        {
            var handler = OnThemeChanged;
            handler?.Invoke(this, e);
        }
    }
}