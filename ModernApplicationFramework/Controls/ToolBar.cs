using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Standard;

namespace ModernApplicationFramework.Controls
{
    public class ToolBar : System.Windows.Controls.ToolBar
    {
        private static readonly DependencyPropertyKey IsOverflowToggleButtonVisiblePropertyKey;
        public static readonly DependencyProperty IsOverflowToggleButtonVisibleProperty;
        public static readonly RoutedEvent HasOverflowItemsChangedEvent;
        public static readonly DependencyProperty IsQuickCustomizeEnabledProperty;
        public static readonly DependencyProperty IsToolBarHostedMenuItemProperty;
        private bool _isToolBarMode;

        static ToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBar), new FrameworkPropertyMetadata(typeof(ToolBar)));
            IsOverflowToggleButtonVisiblePropertyKey = DependencyProperty.RegisterReadOnly(
                "IsOverflowToggleButtonVisible", typeof(bool), typeof(ToolBar),
                new FrameworkPropertyMetadata(Boxes.BooleanTrue));
            IsOverflowToggleButtonVisibleProperty = IsOverflowToggleButtonVisiblePropertyKey.DependencyProperty;
            HasOverflowItemsChangedEvent = EventManager.RegisterRoutedEvent("HasOverflowItemsChangedEvent",
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToolBar));
            IsQuickCustomizeEnabledProperty = DependencyProperty.Register("IsQuickCustomizeEnabled", typeof(bool),
                typeof(ToolBar), new FrameworkPropertyMetadata(Boxes.BooleanTrue));
            IsToolBarHostedMenuItemProperty = DependencyProperty.RegisterAttached("IsToolBarHostedMenuItem",
                typeof(bool), typeof(ToolBar), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        }

        public ToolBar()
        {
            IsVisibleChanged += ToolBar_IsVisibleChanged;
        }

        internal ToggleButton OverflowWidget { get; private set; }

        private bool IsToolBarMode
        {
            set
            {
                if (_isToolBarMode == value)
                    return;
                _isToolBarMode = value;
                if (_isToolBarMode)
                    return;
                RestorePreviousFocus();
            }
        }

        public bool IsQuickCustomizeEnabled
        {
            get => (bool) GetValue(IsQuickCustomizeEnabledProperty);
            set => SetValue(IsQuickCustomizeEnabledProperty, Boxes.Box(value));
        }

        public bool IsOverflowToggleButtonVisible
        {
            get => (bool) GetValue(IsOverflowToggleButtonVisibleProperty);
            private set => SetValue(IsOverflowToggleButtonVisiblePropertyKey, Boxes.Box(value));
        }

        public static bool GetIsToolBarHostedMenuItem(MenuItem menuItem)
        {
            return (bool) menuItem.GetValue(IsToolBarHostedMenuItemProperty);
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

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property != HasOverflowItemsProperty && e.Property != IsQuickCustomizeEnabledProperty)
                return;
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                (Action) (() => { IsOverflowToggleButtonVisible = HasOverflowItems || IsQuickCustomizeEnabled; }));
            if (e.Property != HasOverflowItemsProperty)
                return;
            RaiseEvent(new RoutedEventArgs(HasOverflowItemsChangedEvent));
        }


        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            MenuUtilities.HandleOnContextMenuOpening(e, base.OnContextMenuOpening);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            ProcessForCommandMode(e);
            MenuUtilities.ProcessForDirectionalNavigation(e, this, Orientation);
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
                            if (!IsComboBoxFocused() || (NativeMethods.ModifierKeys & ModifierKeys.Shift) !=
                                ModifierKeys.None)
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
            var focusedElement = Keyboard.FocusedElement as UIElement;
            if (focusedElement != null)
                return true;
            return false;
        }


        public event RoutedEventHandler HasOverflowItemsChanged
        {
            add => AddHandler(HasOverflowItemsChangedEvent, value);
            remove => RemoveHandler(HasOverflowItemsChangedEvent, value);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OverflowWidget = Template.FindName("OverflowButton", this) as ToggleButton;
        }

        private void ToolBar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var ancetor = this.FindAncestor<ToolBarTray>();

            foreach (var bar in ancetor.ToolBars)
                bar.ClearValue(bar.Orientation == Orientation.Vertical ? HeightProperty : WidthProperty);
        }
    }
}