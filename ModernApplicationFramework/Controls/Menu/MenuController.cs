using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls.ListBoxes;
using ModernApplicationFramework.Core.Themes;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Controls.Menu
{
    public class MenuController : MenuItem, IExposeStyleKeys
    {
        public static readonly DependencyProperty AnchorItemProperty;

        private static ResourceKey _buttonStyleKey;
        private static ResourceKey _menuControllerStyleKey;
        private static ResourceKey _comboBoxStyleKey;
        private static ResourceKey _menuStyleKey;
        private static ResourceKey _separatorStyleKey;
        public new static ResourceKey ButtonStyleKey => _buttonStyleKey ?? (_buttonStyleKey = new StyleKey<MenuController>());

        public new static ResourceKey MenuControllerStyleKey => _menuControllerStyleKey ?? (_menuControllerStyleKey = new StyleKey<MenuController>());

        public new static ResourceKey ComboBoxStyleKey => _comboBoxStyleKey ?? (_comboBoxStyleKey = new StyleKey<MenuController>());

        public new static ResourceKey MenuStyleKey => _menuStyleKey ?? (_menuStyleKey = new StyleKey<MenuController>());

        public new static ResourceKey SeparatorStyleKey => _separatorStyleKey ?? (_separatorStyleKey = new StyleKey<MenuController>());

        ResourceKey IExposeStyleKeys.ButtonStyleKey => ButtonStyleKey;

        ResourceKey IExposeStyleKeys.MenuControllerStyleKey => MenuControllerStyleKey;

        ResourceKey IExposeStyleKeys.ComboBoxStyleKey => ComboBoxStyleKey;

        ResourceKey IExposeStyleKeys.MenuStyleKey => MenuStyleKey;

        ResourceKey IExposeStyleKeys.SeparatorStyleKey => SeparatorStyleKey;

        public CommandBarItemDefinition AnchorItem
        {
            get => (CommandBarItemDefinition) GetValue(AnchorItemProperty);
            set => SetValue(AnchorItemProperty, value);
        }

        static MenuController()
        {
            AnchorItemProperty = DependencyProperty.Register("AnchorItem", typeof(CommandBarItemDefinition), typeof(MenuController),
                new FrameworkPropertyMetadata(AnchorItemChanged, CoerceAnchorItemCallback));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuController), new FrameworkPropertyMetadata(typeof(MenuController)));
            EventManager.RegisterClassHandler(typeof(MenuItem), CommandExecutedRoutedEvent, new RoutedEventHandler(OnCommandExecuted));
        }

        private static void AnchorItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MenuController) d).AnchorItemChanged(e);
        }

        private void AnchorItemChanged(DependencyPropertyChangedEventArgs e)
        {
            var anchorItem = e.NewValue as CommandBarItemDefinition;
            var oldAnchorItem = e.OldValue as CommandBarItemDefinition;

            if (oldAnchorItem != null)
            {
                if (oldAnchorItem.CommandDefinition is CommandDefinition commandDefinition)
                    commandDefinition.Command.CanExecuteChanged -= Command_CanExecuteChanged;
            }
            if (anchorItem != null)
            {
                if (string.IsNullOrEmpty(anchorItem?.CommandDefinition?.IconSource?.OriginalString))
                    return;
                var myResourceDictionary = new ResourceDictionary { Source = anchorItem.CommandDefinition.IconSource };
                IconSource = myResourceDictionary[anchorItem.CommandDefinition.IconId];
                this.SetThemedIcon();

                if (anchorItem.CommandDefinition is CommandDefinition commandDefinition)
                    commandDefinition.Command.CanExecuteChanged += Command_CanExecuteChanged;
            }    
        }

        private void Command_CanExecuteChanged(object sender, System.EventArgs e)
        {
            var cd = AnchorItem.CommandDefinition as CommandDefinition;
            if (cd == null)
                return;
            IsEnabled = cd.Command.CanExecute(null);
        }

        public MenuController()
        {
            DteFocusHelper.HookAcquireFocus(this);
        }

        private static object CoerceAnchorItemCallback(DependencyObject d, object basevalue)
        {
            return ((MenuController) d).CoerceAnchorItemCallback(basevalue);
        }

        private object CoerceAnchorItemCallback(object basevalue)
        {
            if (basevalue != null && !(basevalue is CommandBarItemDefinition))
                return DependencyProperty.UnsetValue;
            return basevalue;
        }

        private static void OnCommandExecuted(object sender, RoutedEventArgs args)
        {
            var originalSource = args.OriginalSource as MenuItem;
            if (sender == null)
                return;
            var ancestor = originalSource.FindAncestor<MenuController>();
            if (ancestor == null)
                return;
            var dataContext = originalSource?.DataContext as CommandBarItemDefinition;
            ancestor.OnCommandExecuted(dataContext);
        }

        private void OnCommandExecuted(CommandBarItemDefinition dataContext)
        {
            IsSubmenuOpen = false;
            //if (this.IsAnchorCommandFixed())
            //    return;
            AnchorItem = dataContext;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                e.Handled = true;
                var dataContext = DataContext as CommandBarItemDefinition;
                if (dataContext == null)
                    return;
                if (dataContext.CommandDefinition is CommandDefinition commandDefinition)
                    commandDefinition.Command.Execute(null);
            }
            else
                base.OnKeyDown(e);
        }
    }
}
