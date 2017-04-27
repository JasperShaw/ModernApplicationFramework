using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Native.Standard;

namespace ModernApplicationFramework.Controls
{
    public class MenuItem : System.Windows.Controls.MenuItem, IThemableIconContainer, IExposeStyleKeys, INotifyPropertyChanged
    {
        public object IconSource { get; }
        public static DependencyProperty IsUserCreatedMenuProperty;
        public static DependencyProperty IsPlacedOnToolBarProperty;

        public static readonly RoutedEvent CommandExecutedRoutedEvent;

        public bool IsUserCreatedMenu
        {
            get => (bool)GetValue(IsUserCreatedMenuProperty);
            set => SetValue(IsUserCreatedMenuProperty, Boxes.Box(value));
        }

        public bool IsPlacedOnToolBar
        {
            get => (bool)GetValue(IsPlacedOnToolBarProperty);
            set => SetValue(IsPlacedOnToolBarProperty, Boxes.Box(value));
        }

        public Button HostContainer => this.FindAncestor<Button>();

        public static double MaxMenuWidth => 660.0;

        static MenuItem()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(typeof(MenuItem)));
            CommandExecutedRoutedEvent = EventManager.RegisterRoutedEvent("CommandExecuted", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MenuItem));
            IsUserCreatedMenuProperty = DependencyProperty.Register("IsUserCreatedMenu", typeof(bool), typeof(MenuItem), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
            IsPlacedOnToolBarProperty = DependencyProperty.Register("IsPlacedOnToolBar", typeof(bool), typeof(MenuItem), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        }

        public MenuItem(CommandBarDefinitionBase definitionBase) : this()
        {
            DataContext = definitionBase;
            if (string.IsNullOrEmpty(definitionBase.CommandDefinition?.IconSource?.OriginalString))
                return;
            var myResourceDictionary = new ResourceDictionary { Source = definitionBase.CommandDefinition.IconSource };
            IconSource = myResourceDictionary[definitionBase.CommandDefinition.IconId];
        }

        public MenuItem()
        {
            var themeManager = IoC.Get<IThemeManager>();
            themeManager.OnThemeChanged += ThemeManager_OnThemeChanged;
            IsEnabledChanged += MenuItem_IsEnabledChanged;
        }

        private void MenuItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.SetThemedIcon();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            this.SetThemedIcon();
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            NotifyPropertyChanged("ParentToolBar");
            NotifyPropertyChanged("HostContainer");
            base.OnVisualParentChanged(oldParent);
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged.RaiseEvent(this, propertyName);
        }


        private void ThemeManager_OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            this.SetThemedIcon();
        }

        private static ResourceKey _buttonStyleKey;
        private static ResourceKey _menuControllerStyleKey;
        private static ResourceKey _comboBoxStyleKey;
        private static ResourceKey _menuStyleKey;
        private static ResourceKey _separatorStyleKey;

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

        protected override void OnClick()
        {
            base.OnClick();
            RaiseEvent(new RoutedEventArgs(CommandExecutedRoutedEvent, this));
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (!Equals(e.NewFocus, this))
                return;
            var templateChild = GetTemplateChild("PART_FocusTarget") as UIElement;
            templateChild?.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            StyleUtilities.SelectStyleForItem(element as FrameworkElement, item, this);
        }

        ResourceKey IExposeStyleKeys.MenuControllerStyleKey => MenuControllerStyleKey;

        ResourceKey IExposeStyleKeys.ComboBoxStyleKey => ComboBoxStyleKey;

        ResourceKey IExposeStyleKeys.MenuStyleKey => MenuStyleKey;

        ResourceKey IExposeStyleKeys.SeparatorStyleKey => SeparatorStyleKey;

        ResourceKey IExposeStyleKeys.ButtonStyleKey => ButtonStyleKey;

        public static ResourceKey ButtonStyleKey => _buttonStyleKey ?? (_buttonStyleKey = new StyleKey<MenuItem>());

        public static ResourceKey MenuControllerStyleKey => _menuControllerStyleKey ?? (_menuControllerStyleKey = new StyleKey<MenuItem>());

        public static ResourceKey ComboBoxStyleKey => _comboBoxStyleKey ?? (_comboBoxStyleKey = new StyleKey<MenuItem>());

        public static ResourceKey MenuStyleKey => _menuStyleKey ?? (_menuStyleKey = new StyleKey<MenuItem>());

        public new static ResourceKey SeparatorStyleKey => _separatorStyleKey ?? (_separatorStyleKey = new StyleKey<MenuItem>());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}