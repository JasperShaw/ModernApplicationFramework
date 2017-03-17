using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using ModernApplicationFramework.Core.NativeMethods;
using ModernApplicationFramework.Core.Standard;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Controls
{
    public class ToolBar : System.Windows.Controls.ToolBar
    {
        private static readonly DependencyPropertyKey IsOverflowToggleButtonVisiblePropertyKey;
        public static readonly DependencyProperty IsOverflowToggleButtonVisibleProperty;
        public static readonly RoutedEvent HasOverflowItemsChangedEvent;
        public static readonly DependencyProperty IsQuickCustomizeEnabledProperty;
        public static readonly DependencyProperty IsToolBarHostedMenuItemProperty;
        private ToggleButton overflowWidget;
        private bool isToolBarMode;

        static ToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBar), new FrameworkPropertyMetadata(typeof(ToolBar)));
            IsOverflowToggleButtonVisiblePropertyKey = DependencyProperty.RegisterReadOnly("IsOverflowToggleButtonVisible", typeof(bool), typeof(ToolBar), new FrameworkPropertyMetadata(Boxes.BooleanTrue));
            IsOverflowToggleButtonVisibleProperty = IsOverflowToggleButtonVisiblePropertyKey.DependencyProperty;
            HasOverflowItemsChangedEvent = EventManager.RegisterRoutedEvent("HasOverflowItemsChangedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToolBar));
            IsQuickCustomizeEnabledProperty = DependencyProperty.Register("IsQuickCustomizeEnabled", typeof(bool), typeof(ToolBar), new FrameworkPropertyMetadata(Boxes.BooleanTrue));
            IsToolBarHostedMenuItemProperty = DependencyProperty.RegisterAttached("IsToolBarHostedMenuItem", typeof(bool), typeof(ToolBar), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        }

        public static bool GetIsToolBarHostedMenuItem(MenuItem menuItem)
        {
            return (bool)menuItem.GetValue(IsToolBarHostedMenuItemProperty);
        }

        public static void SetIsToolBarHostedMenuItem(MenuItem menuItem, bool value)
        {
            menuItem.SetValue(IsToolBarHostedMenuItemProperty, Boxes.Box(value));
        }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);
            IsToolBarMode = IsKeyboardFocusWithin;
        }

        public ToolBar()
        {
            IsVisibleChanged += ToolBar_IsVisibleChanged;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property != HasOverflowItemsProperty && e.Property != IsQuickCustomizeEnabledProperty)
                return;
            Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action) (() =>
            {
                IsOverflowToggleButtonVisible = HasOverflowItems || IsQuickCustomizeEnabled;
            }));
            if (e.Property != HasOverflowItemsProperty)
                return;
            RaiseEvent(new RoutedEventArgs(HasOverflowItemsChangedEvent));
        }


        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            Utility.HandleOnContextMenuOpening(e, base.OnContextMenuOpening);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            ProcessForCommandMode(e);
            ProcessForDirectionalNavigation(e, this, Orientation);
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

        private static UIElement GetNavigationContainer(ItemsControl itemsControl)
        {
            MenuItem menuItem = itemsControl as MenuItem;
            Popup name = menuItem?.Template.FindName("PART_Popup", menuItem) as Popup;
            if (name?.Child != null)
                return name.Child;
            return itemsControl;
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

        private bool IsToolBarMode
        {
            set
            {
                if (isToolBarMode == value)
                    return;
                isToolBarMode = value;
                if (isToolBarMode)
                    return;
                RestorePreviousFocus();
            }
        }

        private void RestorePreviousFocus()
        {
            if (!IsKeyboardFocusWithin)
                return;
            Keyboard.Focus(null);
        }

        private void ProcessForCommandMode(KeyEventArgs e)
        {
            if (e.Handled)
                return;
            switch (e.Key)
            {
                case Key.Escape:
                    if (IsOverflowOpen)
                        IsOverflowOpen = false;
                    IsToolBarMode = false;
                    e.Handled = true;
                    break;
                case Key.System:
                    switch (e.SystemKey)
                    {
                        case Key.F10:
                            if ((NativeMethods.ModifierKeys & ModifierKeys.Shift) != ModifierKeys.None)
                                return;
                            IsToolBarMode = false;
                            e.Handled = true;
                            return;
                        case Key.LeftAlt:
                        case Key.RightAlt:
                            if (!IsComboBoxFocused() || (NativeMethods.ModifierKeys & ModifierKeys.Shift) != ModifierKeys.None)
                                IsToolBarMode = false;
                            e.Handled = true;
                            return;
                        default:
                            return;
                    }
            }
        }

        private bool IsComboBoxFocused()
        {
            UIElement focusedElement = Keyboard.FocusedElement as UIElement;
            if (focusedElement != null)
                return true;
            return false;
        }


        public event RoutedEventHandler HasOverflowItemsChanged
        {
            add => AddHandler(HasOverflowItemsChangedEvent, value);
            remove => RemoveHandler(HasOverflowItemsChangedEvent, value);
        }

        public bool IsQuickCustomizeEnabled
        {
            get => (bool)GetValue(IsQuickCustomizeEnabledProperty);
            set => SetValue(IsQuickCustomizeEnabledProperty, Boxes.Box(value));
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            overflowWidget = Template.FindName("OverflowButton", this) as ToggleButton;
        }

        private void ToolBar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var ancetor = this.FindAncestor<ToolBarTray>();

            foreach (var bar in ancetor.ToolBars)
            {
                bar.ClearValue(bar.Orientation == Orientation.Vertical ? HeightProperty : WidthProperty);
            }

        }

        public bool IsOverflowToggleButtonVisible
        {
            get => (bool)GetValue(IsOverflowToggleButtonVisibleProperty);
            private set => SetValue(IsOverflowToggleButtonVisiblePropertyKey, Boxes.Box(value));
        }
    }
}