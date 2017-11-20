using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls.Utilities;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls
{
    /// <inheritdoc cref="System.Windows.Controls.ToolBar"/>
    /// <summary>
    /// A custom <see cref="T:System.Windows.Controls.ToolBar" /> control that holds its own data model 
    /// and styles its items based on their type
    /// </summary>
    /// <seealso cref="T:System.Windows.Controls.ToolBar" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Controls.IExposeStyleKeys" />
    public class ToolBar : System.Windows.Controls.ToolBar, IExposeStyleKeys
    {
        private static readonly DependencyPropertyKey IsOverflowToggleButtonVisiblePropertyKey;
        public static readonly DependencyProperty IsOverflowToggleButtonVisibleProperty;
        public static readonly RoutedEvent HasOverflowItemsChangedEvent;
        public static readonly DependencyProperty IsQuickCustomizeEnabledProperty;
        public static readonly DependencyProperty IsToolBarHostedMenuItemProperty;
        public static readonly DependencyProperty IsStretchingProperty;
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
            IsStretchingProperty = DependencyProperty.Register("IsStretching", typeof(bool), typeof(ToolBar), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        }

        public ToolBar()
        {
            IsVisibleChanged += ToolBar_IsVisibleChanged;
        }

        public ToolBar(CommandBarDefinitionBase definition) : this()
        {
            DataContext = definition;
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
            get => (bool)GetValue(IsQuickCustomizeEnabledProperty);
            set => SetValue(IsQuickCustomizeEnabledProperty, Boxes.Box(value));
        }

        public bool IsStretching
        {
            get => (bool)GetValue(IsStretchingProperty);
            set => SetValue(IsStretchingProperty, Boxes.Box(value));
        }

        public bool IsOverflowToggleButtonVisible
        {
            get => (bool)GetValue(IsOverflowToggleButtonVisibleProperty);
            private set => SetValue(IsOverflowToggleButtonVisiblePropertyKey, Boxes.Box(value));
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

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property != HasOverflowItemsProperty && e.Property != IsQuickCustomizeEnabledProperty)
                return;
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                (Action)(() => { IsOverflowToggleButtonVisible = HasOverflowItems || IsQuickCustomizeEnabled; }));
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
            if (Keyboard.FocusedElement is UIElement focusedElement)
                return focusedElement.FindAncestorOrSelf<ComboBox.ComboBox>() != null;
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
            var ancestor = this.FindAncestor<ToolBarTray>();

            foreach (var bar in ancestor.ToolBars)
                bar.ClearValue(bar.Orientation == Orientation.Vertical ? HeightProperty : WidthProperty);
        }

        private static ResourceKey _buttonStyleKey;
        private static ResourceKey _menuControllerStyleKey;
        private static ResourceKey _comboBoxStyleKey;
        private static ResourceKey _menuStyleKey;
        private static ResourceKey _separatorStyleKey;


        public new static ResourceKey ButtonStyleKey => _buttonStyleKey ?? (_buttonStyleKey = new StyleKey<ToolBar>());
        ResourceKey IExposeStyleKeys.MenuControllerStyleKey => MenuControllerStyleKey;
        ResourceKey IExposeStyleKeys.ComboBoxStyleKey => ComboBoxStyleKey;
        ResourceKey IExposeStyleKeys.MenuStyleKey => MenuStyleKey;
        ResourceKey IExposeStyleKeys.SeparatorStyleKey => SeparatorStyleKey;
        ResourceKey IExposeStyleKeys.ButtonStyleKey => ButtonStyleKey;

        public static ResourceKey MenuControllerStyleKey => _menuControllerStyleKey ?? (_menuControllerStyleKey = new StyleKey<ToolBar>());
        public new static ResourceKey ComboBoxStyleKey => _comboBoxStyleKey ?? (_comboBoxStyleKey = new StyleKey<ToolBar>());
        public new static ResourceKey MenuStyleKey => _menuStyleKey ?? (_menuStyleKey = new StyleKey<ToolBar>());
        public new static ResourceKey SeparatorStyleKey => _separatorStyleKey ?? (_separatorStyleKey = new StyleKey<ToolBar>());

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            StyleUtilities.SelectStyleForItem(element as FrameworkElement, item, this);
        }
    }
}