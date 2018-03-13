using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernApplicationFramework.Controls.Utilities
{
    public static class SelectorDoubleClickCommandBehavior
    {
        public static readonly DependencyProperty HandleDoubleClickProperty = DependencyProperty.RegisterAttached(
            "HandleDoubleClick", typeof(bool), typeof(SelectorDoubleClickCommandBehavior), new FrameworkPropertyMetadata(false, OnHandleDoubleClickChanged));


        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command", typeof(ICommand), typeof(SelectorDoubleClickCommandBehavior), new FrameworkPropertyMetadata(null));


        public static bool GetHandleDoubleClick(DependencyObject d)
        {
            return (bool)d.GetValue(HandleDoubleClickProperty);
        }

        public static void SetHandleDoubleClick(DependencyObject d, bool value)
        {
            d.SetValue(HandleDoubleClickProperty, value);
        }

        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }


        public static void SetCommand(DependencyObject d,
            ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        private static void OnHandleDoubleClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Selector selector))
                return;
            if (!(bool)e.NewValue)
                return;
            selector.MouseDoubleClick -= Selector_MouseDoubleClick;
            selector.MouseDoubleClick += Selector_MouseDoubleClick;

        }

        private static void Selector_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ItemsControl itemsControl) || !(e.OriginalSource is DependencyObject origSender))
                return;
            var container = ItemsControl.ContainerFromElement(itemsControl, origSender);
            if (container == null || Equals(container, DependencyProperty.UnsetValue))
                return;
            var activatedItem = itemsControl.ItemContainerGenerator.ItemFromContainer(container);

            if (activatedItem == null)
                return;
            var command = (ICommand)itemsControl.GetValue(CommandProperty);
            if (command != null && command.CanExecute(activatedItem))
                command.Execute(activatedItem);
        }
    }
}