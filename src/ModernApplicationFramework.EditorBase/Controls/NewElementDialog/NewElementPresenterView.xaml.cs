using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Interfaces.Controls;

namespace ModernApplicationFramework.EditorBase.Controls.NewElementDialog
{
    public partial class NewElementPresenterView : IItemDoubleClickable
    {
        public event EventHandler<ItemDoubleClickedEventArgs> ItemDoubledClicked
        {
            add
            {
                var eventHandler = _itemDoubleClicked;
                EventHandler<ItemDoubleClickedEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler =
                        Interlocked.CompareExchange(ref _itemDoubleClicked,
                            (EventHandler<ItemDoubleClickedEventArgs>)Delegate.Combine(comparand, value), comparand);
                } while (eventHandler != comparand);
            }
            remove
            {
                var eventHandler = _itemDoubleClicked;
                EventHandler<ItemDoubleClickedEventArgs> comparand;
                do
                {
                    comparand = eventHandler;
                    eventHandler = Interlocked.CompareExchange(ref _itemDoubleClicked,
                        (EventHandler<ItemDoubleClickedEventArgs>)Delegate.Remove(comparand, value), comparand);
                }
                while (eventHandler != comparand);
            }
        }

        private EventHandler<ItemDoubleClickedEventArgs> _itemDoubleClicked;

        public NewElementPresenterView()
        {
            InitializeComponent();
        }

        private void ListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            ResizeColumn();
        }

        private void ResizeColumn()
        {
            var gridView = ListView.View as GridView;
            if (gridView?.Columns == null)
                return;
            gridView.Columns[0].Width = ListView.ActualWidth - 25.0;
        }

        private void ListView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeColumn();
        }

        private void ListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || ListView.SelectedItem == null || _itemDoubleClicked == null || ButtonBaseInVisualTree(sender, e.OriginalSource as DependencyObject))
                return;
            _itemDoubleClicked(this, new ItemDoubleClickedEventArgs(ListView.SelectedItem));
        }

        private bool ButtonBaseInVisualTree(object parent, DependencyObject child)
        {
            for (; child != null && child != parent; child = child is Visual || child is Visual3D ? VisualTreeHelper.GetParent(child) : LogicalTreeHelper.GetParent(child))
            {
                if (child is ButtonBase)
                    return true;
            }
            return false;
        }
    }
}
