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
        public static readonly DependencyProperty ActiveViewProperty = DependencyProperty.Register(nameof(ActiveView),
            typeof(ViewStyle), typeof(NewElementPresenterView),
            new FrameworkPropertyMetadata(ViewStyle.MediumIcons, FrameworkPropertyMetadataOptions.None, null, null));


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
        private readonly DataTemplate _mMediumIconTemplate;
        private readonly DataTemplate _mSmallIconTemplate;
        private readonly DataTemplate _mLargeIconTemplate;

        public ViewStyle ActiveView
        {
            get => (ViewStyle) GetValue(ActiveViewProperty);
            set
            {
                if (ActiveView == value)
                    return;
                SetValue(ActiveViewProperty, value);

                if (BtntMediumIcons == null || BtntLargeIcons == null || BtntSmallIcons == null)
                    return;
                switch (value)
                {
                    case ViewStyle.SmallIcons:
                        BtntLargeIcons.IsEnabled = BtntMediumIcons.IsEnabled = true;
                        BtntLargeIcons.IsChecked = false;
                        BtntMediumIcons.IsChecked = false;
                        BtntSmallIcons.IsChecked = true;
                        ListView.ItemTemplate = _mSmallIconTemplate;
                        break;
                    case ViewStyle.LargeIcons:
                        BtntMediumIcons.IsEnabled = BtntSmallIcons.IsEnabled = true;
                        BtntLargeIcons.IsChecked = true;
                        BtntMediumIcons.IsChecked = false;
                        BtntSmallIcons.IsChecked = false;
                        ListView.ItemTemplate = _mLargeIconTemplate;
                        break;
                    default:
                        BtntLargeIcons.IsEnabled = BtntSmallIcons.IsEnabled = true;
                        BtntLargeIcons.IsChecked = false;
                        BtntMediumIcons.IsChecked = true;
                        BtntSmallIcons.IsChecked = false;
                        ListView.ItemTemplate = _mMediumIconTemplate;
                        break;
                }
            }
        }

        public NewElementPresenterView()
        {
            InitializeComponent();
            _mMediumIconTemplate = (DataTemplate) Resources["MediumIconTemplate"];
            _mSmallIconTemplate = (DataTemplate) Resources["SmallIconTemplate"];
            _mLargeIconTemplate = (DataTemplate) Resources["LargeIconTemplate"];
            Loaded += NewElementPresenterView_Loaded;  
        }

        private void NewElementPresenterView_Loaded(object sender, RoutedEventArgs e)
        {
            if (BtntMediumIcons.Visibility == Visibility.Visible)
                ActiveView = ViewStyle.MediumIcons;
            else if (BtntMediumIcons.Visibility == Visibility.Collapsed &&
                     BtntLargeIcons.Visibility == Visibility.Visible)
                ActiveView = ViewStyle.LargeIcons;
            else
                ActiveView = ViewStyle.SmallIcons;
        }

        private void ListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            ResizeColumn();
        }

        private void ResizeColumn()
        {
            if (!(ListView.View is GridView gridView) || gridView.Columns.Count != 1 || ActiveView != ViewStyle.MediumIcons)
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

        private static bool ButtonBaseInVisualTree(object parent, DependencyObject child)
        {
            for (; child != null && child != parent; child = child is Visual || child is Visual3D ? VisualTreeHelper.GetParent(child) : LogicalTreeHelper.GetParent(child))
            {
                if (child is ButtonBase)
                    return true;
            }
            return false;
        }

        private void Btnt_OnChecked(object sender, RoutedEventArgs e)
        {
            if (Equals(sender, BtntLargeIcons))
                ActiveView = ViewStyle.LargeIcons;
            else if (Equals(sender, BtntMediumIcons))
                ActiveView = ViewStyle.MediumIcons;
            else
                ActiveView = ViewStyle.SmallIcons;
        }
    }

    public enum ViewStyle
    {
        SmallIcons,
        MediumIcons,
        LargeIcons
    }
}
