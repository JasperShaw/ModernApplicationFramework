using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Native;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Controls
{
    public class Menu : System.Windows.Controls.Menu
    {
        private Window _priorActiveWindow;

        static Menu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Menu), new FrameworkPropertyMetadata(typeof(Menu)));
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
                    _priorActiveWindow = WindowHelper.FindActiveWindow();
                    WindowHelper.BringToTop(mainWindow);
                }
            }
            base.OnPreviewGotKeyboardFocus(e);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (_priorActiveWindow != null && !(e.NewFocus is MenuItem))
            {
                WindowHelper.BringToTop(_priorActiveWindow);
                _priorActiveWindow = null;
            }
            base.OnLostKeyboardFocus(e);
        }
    }
}