using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ModernApplicationFramework.Core.Utilities;
using MenuItem = ModernApplicationFramework.Controls.Menu.MenuItem;

namespace ModernApplicationFramework.Controls.Utilities
{
    internal static class MenuUtilities
    {
        internal static void HandleOnContextMenuOpening(ContextMenuEventArgs args, Action<ContextMenuEventArgs> baseHandler)
        {
            var originalSource = args.OriginalSource as DependencyObject;
            if (originalSource?.FindAncestor<Popup, DependencyObject>(VisualUtilities.GetVisualOrLogicalParent) != null)
                args.Handled = true;
            else
            {
                if (InputManager.Current.IsInMenuMode)
                    Keyboard.Focus(null);
                baseHandler(args);
            }
        }

        internal static bool HandleKeyDownForToolBarHostedMenuItem(System.Windows.Controls.MenuItem menuItem, KeyEventArgs e)
        {
            if (e.Handled || !ToolBar.GetIsToolBarHostedMenuItem(menuItem))
                return false;
            var ancestor = menuItem.FindAncestor<System.Windows.Controls.ToolBar>();
            if (ancestor == null)
                return false;
            var orientation = ancestor.Orientation;
            CorrectKeysForNavigation(e.Key, ancestor.FlowDirection, orientation);
            switch (e.Key)
            {
                case Key.Escape:
                    if (ancestor.IsOverflowOpen && !menuItem.IsSubmenuOpen)
                    {
                        ancestor.IsOverflowOpen = false;
                        e.Handled = true;
                    }
                    break;
                case Key.Left:
                    if (orientation == Orientation.Horizontal)
                        e.Handled = MoveFocusHorizontally(menuItem, FocusNavigationDirection.Previous);
                    break;
                case Key.Up:
                    if (orientation == Orientation.Vertical && !menuItem.IsSubmenuOpen)
                        e.Handled = menuItem.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    break;
                case Key.Right:
                    if (orientation == Orientation.Horizontal)
                        e.Handled = MoveFocusHorizontally(menuItem, FocusNavigationDirection.Next);
                    break;
                case Key.Down:
                    if (orientation == Orientation.Vertical && !menuItem.IsSubmenuOpen)
                        e.Handled = menuItem.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    break;
            }
            return e.Handled;
        }

        private static bool MoveFocusHorizontally(System.Windows.Controls.MenuItem menuItem, FocusNavigationDirection direction)
        {
            bool flag = menuItem.MoveFocus(new TraversalRequest(direction));
            if (Equals(menuItem, Keyboard.FocusedElement))
                menuItem.IsSubmenuOpen = false;
            return flag;
        }


        internal static void ProcessForDirectionalNavigation(KeyEventArgs e, ItemsControl itemsControl, Orientation orientation)
        {
            if (e.Handled)
                return;
            switch (CorrectKeysForNavigation(e.Key, itemsControl.FlowDirection, orientation))
            {
                case Key.Back:
                    FrameworkElement focusedElement1 = FocusManager.GetFocusedElement(itemsControl) as FrameworkElement;
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
                    FrameworkElement focusedElement2 = FocusManager.GetFocusedElement(itemsControl) as FrameworkElement;
                    if (focusedElement2 == null)
                        break;
                    e.Handled = focusedElement2.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    break;
                case Key.Right:
                    if (orientation != Orientation.Horizontal)
                        break;
                    FrameworkElement focusedElement3 = FocusManager.GetFocusedElement(itemsControl) as FrameworkElement;
                    if (focusedElement3 == null)
                        break;
                    e.Handled = focusedElement3.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    break;
            }
        }

        private static UIElement GetNavigationContainer(UIElement itemsControl)
        {
            var menuItem = itemsControl as MenuItem;
            var name = menuItem?.Template.FindName("PART_Popup", menuItem) as Popup;
            return name?.Child ?? itemsControl;
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
    }
}
