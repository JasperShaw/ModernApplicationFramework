using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using ModernApplicationFramework.Controls.ListBoxes;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Controls.Menu
{
    public class Menu : System.Windows.Controls.Menu, IExposeStyleKeys
    {
        private Window _priorActiveWindow;
        private static ResourceKey _buttonStyleKey;
        private static ResourceKey _menuControllerStyleKey;
        private static ResourceKey _comboBoxStyleKey;
        private static ResourceKey _menuStyleKey;
        private static ResourceKey _separatorStyleKey;

        static Menu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Menu), new FrameworkPropertyMetadata(typeof(Menu)));
        }
        
        public static ResourceKey ButtonStyleKey => _buttonStyleKey ?? (_buttonStyleKey = new StyleKey<Menu>());
        ResourceKey IExposeStyleKeys.MenuControllerStyleKey => MenuControllerStyleKey;

        ResourceKey IExposeStyleKeys.ComboBoxStyleKey => ComboBoxStyleKey;

        ResourceKey IExposeStyleKeys.MenuStyleKey => MenuStyleKey;

        ResourceKey IExposeStyleKeys.SeparatorStyleKey => SeparatorStyleKey;

        ResourceKey IExposeStyleKeys.ButtonStyleKey => ButtonStyleKey;

        public static ResourceKey MenuControllerStyleKey => _menuControllerStyleKey ?? (_menuControllerStyleKey = new StyleKey<Menu>());

        public static ResourceKey ComboBoxStyleKey => _comboBoxStyleKey ?? (_comboBoxStyleKey = new StyleKey<Menu>());

        public static ResourceKey MenuStyleKey => _menuStyleKey ?? (_menuStyleKey = new StyleKey<Menu>());

        public static ResourceKey SeparatorStyleKey => _separatorStyleKey ?? (_separatorStyleKey = new StyleKey<Menu>());

        protected override DependencyObject GetContainerForItemOverride()
        {
            var element = new MenuItem();
            return element;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!IsShiftF10(e))
                base.OnKeyDown(e);
            MenuUtilities.ProcessForDirectionalNavigation(e, this, Orientation.Horizontal);
        }

        private static bool IsShiftF10(KeyEventArgs e)
        {
            if (e.SystemKey == Key.F10)
                return (NativeMethods.ModifierKeys & ModifierKeys.Shift) == ModifierKeys.Shift;
            return false;
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            MenuUtilities.HandleOnContextMenuOpening(e, base.OnContextMenuOpening);
        }

        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            var current = Application.Current;
            if (current?.MainWindow != null)
            {
                var mainWindow = current.MainWindow;
                if (mainWindow.WindowState == WindowState.Minimized)
                    mainWindow.WindowState = WindowState.Normal;
                if (!mainWindow.IsActive)
                {
                    _priorActiveWindow = FindActiveWindow();
                    BringToTop(mainWindow);
                }
            }
            base.OnPreviewGotKeyboardFocus(e);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (_priorActiveWindow != null && !(e.NewFocus is MenuItem))
            {
                BringToTop(_priorActiveWindow);
                _priorActiveWindow = null;
            }
            base.OnLostKeyboardFocus(e);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            StyleUtilities.SelectStyleForItem(element as FrameworkElement, item, this);
        }

        private static Window FindActiveWindow()
        {
            return Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window.IsActive);
        }

        private void BringToTop(Window window)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            int flags = 19;
            User32.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, flags);
            window.Activate();
        }
    }
}