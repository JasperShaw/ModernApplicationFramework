using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using ModernApplicationFramework.Core.Standard;
using ModernApplicationFramework.Core.Utilities;

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
            if (!this.IsShiftF10(e))
                base.OnKeyDown(e);
            ProcessForDirectionalNavigation(e, (ItemsControl)this, Orientation.Horizontal);
        }

        private bool IsShiftF10(KeyEventArgs e)
        {
            if (e.SystemKey == Key.F10)
                return (NativeMethods.ModifierKeys & ModifierKeys.Shift) == ModifierKeys.Shift;
            return false;
        }



        internal static void ProcessForDirectionalNavigation(KeyEventArgs e, ItemsControl itemsControl, Orientation orientation)
        {
            if (e.Handled)
                return;
            switch (CorrectKeysForNavigation(e.Key, itemsControl.FlowDirection, orientation))
            {
                case Key.Back:
                    FrameworkElement focusedElement1 = FocusManager.GetFocusedElement((DependencyObject)itemsControl) as FrameworkElement;
                    if (focusedElement1 == null)
                        break;
                    e.Handled = focusedElement1.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    break;
                case Key.End:
                    e.Handled = GetNavigationContainer(itemsControl).MoveFocus(new TraversalRequest(FocusNavigationDirection.Last));
                    break;
                case Key.Home:
                    e.Handled = GetNavigationContainer(itemsControl).MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                    break;
                case Key.Left:
                    if (orientation != Orientation.Horizontal)
                        break;
                    FrameworkElement focusedElement2 = FocusManager.GetFocusedElement((DependencyObject)itemsControl) as FrameworkElement;
                    if (focusedElement2 == null)
                        break;
                    e.Handled = focusedElement2.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    break;
                case Key.Right:
                    if (orientation != Orientation.Horizontal)
                        break;
                    FrameworkElement focusedElement3 = FocusManager.GetFocusedElement((DependencyObject)itemsControl) as FrameworkElement;
                    if (focusedElement3 == null)
                        break;
                    e.Handled = focusedElement3.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    break;
            }
        }

        internal static Key CorrectKeysForNavigation(Key key, FlowDirection flowDirection, Orientation orientation)
        {
            if (flowDirection == FlowDirection.RightToLeft && orientation == Orientation.Horizontal)
            {
                switch (key)
                {
                    case Key.End:
                        return Key.Home;
                    case Key.Home:
                        return Key.End;
                    case Key.Left:
                        return Key.Right;
                    case Key.Right:
                        return Key.Left;
                }
            }
            return key;
        }

        private static UIElement GetNavigationContainer(ItemsControl itemsControl)
        {
            MenuItem menuItem = itemsControl as MenuItem;
            if (menuItem != null)
            {
                Popup name = menuItem.Template.FindName("PART_Popup", (FrameworkElement)menuItem) as Popup;
                if (name != null && name.Child != null)
                    return name.Child;
            }
            return (UIElement)itemsControl;
        }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            //Utility.HandleOnContextMenuOpening(e, new Action<ContextMenuEventArgs>(((FrameworkElement)this).OnContextMenuOpening));
        }

        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            Application current = Application.Current;
            if (current != null && current.MainWindow != null)
            {
                Window mainWindow = current.MainWindow;
                if (mainWindow.WindowState == WindowState.Minimized)
                    mainWindow.WindowState = WindowState.Normal;
                if (!mainWindow.IsActive)
                {
                    this._priorActiveWindow = this.FindActiveWindow();
                    this.BringToTop(mainWindow);
                }
            }
            base.OnPreviewGotKeyboardFocus(e);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (this._priorActiveWindow != null && !(e.NewFocus is MenuItem))
            {
                this.BringToTop(this._priorActiveWindow);
                this._priorActiveWindow = (Window)null;
            }
            base.OnLostKeyboardFocus(e);
        }

        private void BringToTop(Window window)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            int flags = 19;
            NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, flags);
        }

        private Window FindActiveWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.IsActive)
                    return window;
            }
            return (Window)null;
        }



    }


    internal static class Utility
    {
        internal static void HandleOnContextMenuOpening(ContextMenuEventArgs args, Action<ContextMenuEventArgs> baseHandler)
        {
            DependencyObject originalSource = args.OriginalSource as DependencyObject;
            if (originalSource != null && originalSource.FindAncestor<Popup, DependencyObject>(new Func<DependencyObject, DependencyObject>(ExtensionMethods.GetVisualOrLogicalParent)) != null)
            {
                args.Handled = true;
            }
            else
            {
                if (InputManager.Current.IsInMenuMode)
                    Keyboard.Focus((IInputElement)null);
                baseHandler(args);
            }
        }

    }

}