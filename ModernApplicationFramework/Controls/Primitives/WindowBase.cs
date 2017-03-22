using System;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Standard;
using ModernApplicationFramework.Themes;

namespace ModernApplicationFramework.Controls.Primitives
{
    public abstract class WindowBase : Window, IHasTheme
    {
        public static readonly DependencyProperty HasMaximizeButtonProperty = DependencyProperty.Register(
            "HasMaximizeButton", typeof(bool), typeof(WindowBase),
            new PropertyMetadata(Boxes.BooleanFalse, OnWindowStyleChanged));

        public static readonly DependencyProperty HasMinimizeButtonProperty = DependencyProperty.Register(
            "HasMinimizeButton", typeof(bool), typeof(WindowBase),
            new PropertyMetadata(Boxes.BooleanFalse, OnWindowStyleChanged));

        public static readonly DependencyProperty HasDialogFrameProperty = DependencyProperty.Register(
            "HasDialogFrame", typeof(bool), typeof(WindowBase),
            new PropertyMetadata(Boxes.BooleanTrue, OnWindowStyleChanged));

        public static readonly DependencyProperty IsCloseButtonEnabledProperty = DependencyProperty.Register(
            "IsCloseButtonEnabled", typeof(bool), typeof(WindowBase),
            new PropertyMetadata(Boxes.BooleanTrue, OnWindowStyleChanged));

        private HwndSource _hwndSource;

        static WindowBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowBase),
                new FrameworkPropertyMetadata(typeof(WindowBase)));
        }

        public bool HasDialogFrame
        {
            get => (bool) GetValue(HasDialogFrameProperty);
            set => SetValue(HasDialogFrameProperty, value);
        }

        public bool HasMaximizeButton
        {
            get => (bool) GetValue(HasMaximizeButtonProperty);
            set => SetValue(HasMaximizeButtonProperty, value);
        }

        public bool HasMinimizeButton
        {
            get => (bool) GetValue(HasMinimizeButtonProperty);
            set => SetValue(HasMinimizeButtonProperty, value);
        }

        public bool IsCloseButtonEnabled
        {
            get => (bool) GetValue(IsCloseButtonEnabledProperty);
            set => SetValue(IsCloseButtonEnabledProperty, value);
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
            var windowLong1 = User32.GetWindowLong32(handle, -16);
            var num1 = !HasMaximizeButton ? windowLong1 & -65537 : windowLong1 | 65536;
            var num2 = !HasMinimizeButton ? num1 & -131073 : num1 | 131072;
            User32.SetWindowLong(handle, -16, num2);
            User32.GetWindowLong32(handle, -20);
            User32.SendMessage(handle, 128, new IntPtr(1), IntPtr.Zero);
            User32.SendMessage(handle, 128, new IntPtr(0), IntPtr.Zero);
            var systemMenu = User32.GetSystemMenu(handle, false);
            if (systemMenu != IntPtr.Zero)
            {
                var num5 = IsCloseButtonEnabled ? 0U : 1U;
                User32.EnableMenuItem(systemMenu, 61536U, 0U | num5);
            }
            User32.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, 35);
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
            get => _theme;
            set
            {
                if (value == null)
                {
                    var m = Application.Current.MainWindow as IHasTheme;
                    value = m != null ? m.Theme : new GenericTheme();
                }
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