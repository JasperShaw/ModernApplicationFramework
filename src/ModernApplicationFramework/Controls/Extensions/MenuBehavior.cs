using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Core.Exception;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;
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
            else if (d is ContextMenu contextMenu)
            {
                contextMenu.Opened += ContextMenuOpened;
                if (contextMenu.IsOpen)
                    ContextMenuOpened(contextMenu, new RoutedEventArgs());
            }

        }

        private static void ContextMenuOpened(object sender, RoutedEventArgs e)
        {
            var contextMenu = (ItemsControl)sender;
            if (sender == null)
                return;
            UpdateDefinition(in contextMenu);
            UpdateChekcedStatus(contextMenu);
            var menuItems = contextMenu.Items.OfType<IDummyListMenuItem>().ToList();
            if (menuItems.Count == 0)
                return;
            try
            {
                var commandRouter = IoC.Get<ICommandRouter>();
                foreach (var item in menuItems)
                    item.Update(commandRouter.GetCommandHandler(item.CommandBarItemDefinition.ItemDefinition));
            }
            catch (ContractNotFoundException)
            {
            }
        }

        private static void OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            var menuItem = (ItemsControl)sender;
            if (sender == null)
                return;
            
            UpdateDefinition(in menuItem);

            UpdateChekcedStatus(menuItem);
            var menuItems = menuItem.Items.OfType<IDummyListMenuItem>().ToList();
            if (menuItems.Count == 0)
                return;

            var commandRouter = IoC.Get<ICommandRouter>();
            foreach (var item in menuItems)
                item.Update(commandRouter.GetCommandHandler(item.CommandBarItemDefinition.ItemDefinition));
        }

        private static void UpdateDefinition(in ItemsControl control)
        {
            if (!(control.DataContext is CommandBarDataSource dataSource))
                return;
            var groups = dataSource.ContainedGroups;
            var array = new object[control.Items.Count];
            control.Items.CopyTo(array, 0);

            foreach (var group in groups.OrderBy(x => x.SortOrder))
            {
                group.InvalidateCommandItems();
                var first = group.FirstItem;
                var index = array.OfType<FrameworkElement>().IndexOf(x => x.DataContext == first);
                if (index > 1 && first is CommandBarItemDataSource item && item.PrecededBySeparator)
                {
                    if (!group.Items.Any(x => x.IsVisible))
                    {
                        var separatorItem = control.Items[index - 1] as FrameworkElement;
                        if (separatorItem?.DataContext is SeparatorDataSource separator)
                            separator.IsVisible = false;
                    }
                    else
                    {
                        var separatorItem = control.Items[index - 1] as FrameworkElement;
                        if (separatorItem?.DataContext is SeparatorDataSource separator)
                            separator.IsVisible = true;
                    }
                }
            }
        }

        private static void UpdateChekcedStatus(ItemsControl menuItem)
        {
            var menuItems = menuItem.Items.OfType<MenuItem>().ToList();
            if (menuItems.Count == 0)
                return;
            foreach (var item in menuItems)
            {
                if (item.DataContext is ButtonDataSource definition &&
                    definition.ItemDefinition is CommandDefinition command)
                    definition.IsChecked = command.Command.Checked;
            }

        }
    }
}
