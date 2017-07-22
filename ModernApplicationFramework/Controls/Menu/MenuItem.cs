using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls.Buttons;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.Menu
{
    /// <summary>
    /// A custom menu item control which visual style changes based on it's data model. Supports themable icons.
    /// </summary>
    /// <seealso cref="System.Windows.Controls.MenuItem" />
    /// <seealso cref="ModernApplicationFramework.Interfaces.Controls.IThemableIconContainer" />
    /// <seealso cref="ModernApplicationFramework.Interfaces.Controls.IExposeStyleKeys" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    /// <inheritdoc cref="System.Windows.Controls.MenuItem" />
    /// <seealso cref="T:System.Windows.Controls.MenuItem" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Controls.IThemableIconContainer" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Controls.IExposeStyleKeys" />
    /// <seealso cref="T:System.ComponentModel.INotifyPropertyChanged" />
    public class MenuItem : System.Windows.Controls.MenuItem, IThemableIconContainer, IExposeStyleKeys,
        INotifyPropertyChanged
    {
        /// <summary>
        /// Indicates whether this item was created by the application's user
        /// </summary>
        public static DependencyProperty IsUserCreatedMenuProperty;

        /// <summary>
        /// Indicates whether this item is hosted by a tool bar
        /// </summary>
        public static DependencyProperty IsPlacedOnToolBarProperty;

        public static readonly RoutedEvent CommandExecutedRoutedEvent;

        private static ResourceKey _buttonStyleKey;
        private static ResourceKey _menuControllerStyleKey;
        private static ResourceKey _comboBoxStyleKey;
        private static ResourceKey _menuStyleKey;
        private static ResourceKey _separatorStyleKey;

        static MenuItem()
        {
            CommandExecutedRoutedEvent = EventManager.RegisterRoutedEvent("CommandExecuted", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(MenuItem));
            IsUserCreatedMenuProperty = DependencyProperty.Register("IsUserCreatedMenu", typeof(bool), typeof(MenuItem),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse));
            IsPlacedOnToolBarProperty = DependencyProperty.Register("IsPlacedOnToolBar", typeof(bool), typeof(MenuItem),
                new FrameworkPropertyMetadata(Boxes.BooleanFalse));
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
            DataContextChanged += MenuItem_DataContextChanged;
        }

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

        public CommandDefinitionButton HostContainer => this.FindAncestor<CommandDefinitionButton>();

        /// <summary>
        /// Gets the parent tool bar.
        /// </summary>
        public ToolBar ParentToolBar => this.FindAncestor<ToolBar>();

        /// <summary>
        /// Gets the maximum width of the menu.
        /// </summary>
        public static double MaxMenuWidth => 660.0;

        public static ResourceKey ButtonStyleKey => _buttonStyleKey ?? (_buttonStyleKey = new StyleKey<MenuItem>());

        public static ResourceKey MenuControllerStyleKey =>
            _menuControllerStyleKey ?? (_menuControllerStyleKey = new StyleKey<MenuItem>());

        public static ResourceKey ComboBoxStyleKey =>
            _comboBoxStyleKey ?? (_comboBoxStyleKey = new StyleKey<MenuItem>());

        public static ResourceKey MenuStyleKey => _menuStyleKey ?? (_menuStyleKey = new StyleKey<MenuItem>());

        public new static ResourceKey SeparatorStyleKey =>
            _separatorStyleKey ?? (_separatorStyleKey = new StyleKey<MenuItem>());

        ResourceKey IExposeStyleKeys.MenuControllerStyleKey => MenuControllerStyleKey;

        ResourceKey IExposeStyleKeys.ComboBoxStyleKey => ComboBoxStyleKey;

        ResourceKey IExposeStyleKeys.MenuStyleKey => MenuStyleKey;

        ResourceKey IExposeStyleKeys.SeparatorStyleKey => SeparatorStyleKey;

        ResourceKey IExposeStyleKeys.ButtonStyleKey => ButtonStyleKey;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        /// <summary>
        ///     This is the original source to icon. Usually this is a resource containing a
        ///     <see cref="T:System.Windows.Controls.Viewbox" /> element.
        /// </summary>
        public object IconSource { get; protected set; }

        private void MenuItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var itemDefinition = DataContext as CommandBarDefinitionBase;
            if (string.IsNullOrEmpty(itemDefinition?.CommandDefinition?.IconSource?.OriginalString))
                return;
            var myResourceDictionary = new ResourceDictionary { Source = itemDefinition.CommandDefinition.IconSource };
            IconSource = myResourceDictionary[itemDefinition.CommandDefinition.IconId];
            this.SetThemedIcon();
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}