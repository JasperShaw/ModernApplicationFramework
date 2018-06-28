using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Exception;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;
using MenuItem = ModernApplicationFramework.Controls.Menu.MenuItem;

namespace ModernApplicationFramework.Controls.Extensions
{
    /// <inheritdoc />
    /// <summary>
    /// Extension allowing menu items to populate themselves when opened
    /// </summary>
    /// <seealso cref="T:System.Windows.DependencyObject" />
    public class MenuBehavior : DependencyObject
    {
        public static readonly DependencyProperty UpdateCommandUiItemsProperty = DependencyProperty.RegisterAttached(
            "UpdateCommandUiItems", typeof(bool), typeof(MenuBehavior), new PropertyMetadata(false, OnUpdateCommandUiItemsChanged));

        public static bool GetUpdateCommandUiItems(DependencyObject control)
        {
            return (bool)control.GetValue(UpdateCommandUiItemsProperty);
        }

        public static void SetUpdateCommandUiItems(DependencyObject control, bool value)
        {
            control.SetValue(UpdateCommandUiItemsProperty, value);
        }

        private static void OnUpdateCommandUiItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (d is MenuItem menuItem)
            {
                menuItem.SubmenuOpened += OnSubmenuOpened;
                if (menuItem.IsSubmenuOpen)
                    OnSubmenuOpened(menuItem, new RoutedEventArgs());
            }
            else if (d is System.Windows.Controls.ContextMenu contextMenu)
            {
                contextMenu.Opened += ContextMenuOpened;
                if (contextMenu.IsOpen)
                    ContextMenuOpened(contextMenu, new RoutedEventArgs());
            }

        }

        private static void ContextMenuOpened(object sender, RoutedEventArgs e)
        {
            var contextMenu = (ContextMenu)sender;
            if (sender == null)
                return;
            UpdateChekcedStatus(contextMenu);
            var menuItems = contextMenu.Items.OfType<IDummyListMenuItem>().ToList();
            if (menuItems.Count == 0)
                return;
            try
            {
                var commandRouter = IoC.Get<ICommandRouter>();
                foreach (var item in menuItems)
                    item.Update(commandRouter.GetCommandHandler(item.CommandBarItemDefinition.CommandDefinition));
            }
            catch (ContractNotFoundException)
            {
            }
        }

        private static void OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            if (sender == null)
                return;
            UpdateChekcedStatus(menuItem);
            var menuItems = menuItem.Items.OfType<IDummyListMenuItem>().ToList();
            if (menuItems.Count == 0)
                return;

            var commandRouter = IoC.Get<ICommandRouter>();
            foreach (var item in menuItems)
                item.Update(commandRouter.GetCommandHandler(item.CommandBarItemDefinition.CommandDefinition));
        }



        private static void UpdateChekcedStatus(ItemsControl menuItem)
        {
            var menuItems = menuItem.Items.OfType<MenuItem>().ToList();
            if (menuItems.Count == 0)
                return;
            foreach (var item in menuItems)
            {
                if (item.DataContext is CommandBarDefinitionBase definition &&
                    definition.CommandDefinition is CommandDefinition command)
                    definition.IsChecked = command.IsChecked;
            }

        }
    }
}
