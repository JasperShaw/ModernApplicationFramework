using System.Linq;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.Command;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.Controls.Utilities
{
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
            var menuItem = (MenuItem)d;
            menuItem.SubmenuOpened += OnSubmenuOpened;
            if (menuItem.IsSubmenuOpen)
                OnSubmenuOpened(menuItem, new RoutedEventArgs());
        }

        private static void OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            if (sender == null)
                return;
            var menuItems = menuItem.Items.OfType<IDummyListMenuItem>().ToList();
            if (menuItems.Count == 0)
                return;
            var commandRouter = IoC.Get<ICommandRouter>();
            foreach (var item in menuItems)
                item.Update(commandRouter.GetCommandHandler(item.CommandBarItemDefinition.CommandDefinition));
        }
    }
}
