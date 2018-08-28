using System;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.Menu
{
    /// <inheritdoc cref="MenuItem" />
    /// <summary>
    ///     A custom menu item control hosting one or more buttons.
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Controls.Menu.MenuItem" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Controls.IExposeStyleKeys" />
    public class MenuController : MenuItem, IExposeStyleKeys
    {
        public static readonly DependencyProperty AnchorItemProperty = DependencyProperty.Register(nameof(AnchorItem), typeof(CommandBarItemDataSource), typeof(MenuController), new FrameworkPropertyMetadata(null, CoerceAnchorItemCallback));

        private static ResourceKey _buttonStyleKey;
        private static ResourceKey _comboBoxStyleKey;
        private static ResourceKey _menuControllerStyleKey;
        private static ResourceKey _menuStyleKey;
        private static ResourceKey _separatorStyleKey;

        /// <summary>
        ///     The style key for a simple button
        /// </summary>
        public new static ResourceKey ButtonStyleKey =>
            _buttonStyleKey ?? (_buttonStyleKey = new StyleKey<MenuController>());

        /// <summary>
        ///     The style key for a combo box
        /// </summary>
        public new static ResourceKey ComboBoxStyleKey =>
            _comboBoxStyleKey ?? (_comboBoxStyleKey = new StyleKey<MenuController>());

        /// <summary>
        ///     The style key for a menu controller
        /// </summary>
        public new static ResourceKey MenuControllerStyleKey =>
            _menuControllerStyleKey ?? (_menuControllerStyleKey = new StyleKey<MenuController>());

        /// <summary>
        ///     The style key for a menu item
        /// </summary>
        public new static ResourceKey MenuStyleKey => _menuStyleKey ?? (_menuStyleKey = new StyleKey<MenuController>());

        /// <summary>
        ///     The style key for a separator
        /// </summary>
        public new static ResourceKey SeparatorStyleKey =>
            _separatorStyleKey ?? (_separatorStyleKey = new StyleKey<MenuController>());

        public CommandBarItemDataSource AnchorItem
        {
            get => (CommandBarItemDataSource) GetValue(AnchorItemProperty);
            set => SetValue(AnchorItemProperty, value);
        }

        ResourceKey IExposeStyleKeys.ButtonStyleKey => ButtonStyleKey;

        ResourceKey IExposeStyleKeys.ComboBoxStyleKey => ComboBoxStyleKey;

        ResourceKey IExposeStyleKeys.MenuControllerStyleKey => MenuControllerStyleKey;

        ResourceKey IExposeStyleKeys.MenuStyleKey => MenuStyleKey;

        ResourceKey IExposeStyleKeys.SeparatorStyleKey => SeparatorStyleKey;

        static MenuController()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuController),
                new FrameworkPropertyMetadata(typeof(MenuController)));
            EventManager.RegisterClassHandler(typeof(MenuItem), CommandExecutedRoutedEvent,
                new RoutedEventHandler(OnCommandExecuted));
        }

        public MenuController()
        {
            DteFocusHelper.HookAcquireFocus(this);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                e.Handled = true;
                var dataContext = DataContext as CommandBarItemDataSource;
                if (dataContext == null)
                    return;
                if (dataContext.ItemDefinition is CommandDefinition commandDefinition)
                    commandDefinition.Command.Execute(null);
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        private static void AnchorItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MenuController) d).AnchorItemChanged(e);
        }

        private static object CoerceAnchorItemCallback(DependencyObject d, object basevalue)
        {
            return ((MenuController) d).CoerceAnchorItemCallback(basevalue);
        }

        private static void OnCommandExecuted(object sender, RoutedEventArgs args)
        {
            var originalSource = args.OriginalSource as MenuItem;
            if (sender == null)
                return;
            var ancestor = originalSource.FindAncestor<MenuController>();
            if (ancestor == null)
                return;
            var dataContext = originalSource?.DataContext as CommandBarItemDataSource;
            ancestor.OnCommandExecuted(dataContext);
        }

        private void AnchorItemChanged(DependencyPropertyChangedEventArgs e)
        {
            var anchorItem = e.NewValue as CommandBarItemDataSource;

            if (e.OldValue is CommandBarItemDataSource oldAnchorItem)
                if (oldAnchorItem.ItemDefinition is CommandDefinition commandDefinition)
                    commandDefinition.Command.CanExecuteChanged -= Command_CanExecuteChanged;
            if (anchorItem != null)
                if (anchorItem.ItemDefinition is CommandDefinition commandDefinition)
                    commandDefinition.Command.CanExecuteChanged += Command_CanExecuteChanged;
        }

        private object CoerceAnchorItemCallback(object basevalue)
        {
            if (basevalue != null && !(basevalue is CommandBarItemDataSource))
                return DependencyProperty.UnsetValue;
            return basevalue;
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            if (!(AnchorItem.ItemDefinition is CommandDefinition cd))
                return;
            IsEnabled = cd.Command.CanExecute(null);
        }

        private void OnCommandExecuted(CommandBarItemDataSource dataContext)
        {
            IsSubmenuOpen = false;
            if (IsAnchorCommandFixed())
                return;
            AnchorItem = dataContext;
        }

        private bool IsAnchorCommandFixed()
        {
            if (AnchorItem == null)
                return false;
            return AnchorItem.Flags.FixMenuController;
        }
    }
}