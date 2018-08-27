using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Core.Exception;
using ModernApplicationFramework.Interfaces.Controls;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
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
            var menuItem = (System.Windows.Controls.MenuItem)sender;
            if (sender == null)
                return;

            UpdateDefinition(in menuItem);

            UpdateChekcedStatus(menuItem);
            var menuItems = menuItem.Items.OfType<IDummyListMenuItem>().ToList();
            if (menuItems.Count == 0)
                return;

            var commandRouter = IoC.Get<ICommandRouter>();
            foreach (var item in menuItems)
                item.Update(commandRouter.GetCommandHandler(item.CommandBarItemDefinition.CommandDefinition));
        }

        private static void UpdateDefinition(in System.Windows.Controls.MenuItem control)
        {
            for (var i = 0; i < control.Items.Count; ++i)
            {
                var item = control.Items[i];

                if (item is FrameworkElement fe && fe.DataContext is CommandBarItemDataSource x && x.PrecededBySeparator && i - 1 > 0)
                {
                    if (!x.IsVisible)
                    {
                        var separatorItem = control.Items[i - 1] as FrameworkElement;
                        if (separatorItem?.DataContext is SeparatorDataSource separator)
                            separator.IsVisible = false;
                    }
                    else if (x.IsVisible)
                    {
                        if (x.PrecededBySeparator && i - 1 > 0)
                        {
                            var separatorItem = control.Items[i - 1] as FrameworkElement;
                            if (separatorItem?.DataContext is SeparatorDataSource separator)
                                separator.IsVisible = true;
                        }

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
                if (item.DataContext is CommandBarItemDataSource definition &&
                    definition.CommandDefinition is CommandDefinition command)
                    definition.IsChecked = command.Command.Checked;
            }

        }
    }
}
