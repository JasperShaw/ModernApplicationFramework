using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    internal partial class ChooseItemsPage
    {
        private long _lastTypeAheadTickCount = int.MaxValue;
        private string _typeAheadString;

        private readonly ToolboxControlledPageDataSource _data;
        private bool _filterRefreshInProgress;

        private ICollectionView ListCollectionView => CollectionViewSource.GetDefaultView(_data.Items);

        internal ChooseItemsPage(ToolboxControlledPageDataSource dataSource)
        {
            _data = dataSource;
            if (_data == null)
                throw new ArgumentException("Data source cannot be null");

            DataContext = dataSource;
            InitializeComponent();
            AddColumns(dataSource);
            InitializeSortColumnFromDataSource();
            ListView.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnColumnHeaderDividerDragged), true);
            ListView.SelectionChanged += ListView_SelectionChanged;
            _data.PropertyChanged += OnProertyChanged;
            SetViewFilterAndSubscribeToItemsCollectionChanges();
        }

        private ChooseItemsPage()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var previousCaretItem = _data.CaretItem;
            if (!(e.OriginalSource is ListView listView))
                return;

            if (e.AddedItems.Count > 0)
            {
                _data.CaretItem = e.AddedItems.Count == 1 || previousCaretItem == null
                    ? (ItemDataSource)e.AddedItems[0]
                    : e.AddedItems.Cast<ItemDataSource>().OrderBy(item => DistanceBetween(listView, previousCaretItem, item)).Last();
            }
            else
            {
                if (listView.SelectedItems.Count <= 0 || previousCaretItem == null || listView.SelectedItems.Contains(previousCaretItem))
                    return;
                _data.CaretItem = listView.SelectedItems.Cast<ItemDataSource>()
                    .OrderBy(item => DistanceBetween(listView, previousCaretItem, item)).Last();
            }
        }

        private void OnProertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Execute.OnUIThread(() =>
            {
                if (e.PropertyName == nameof(ToolboxControlledPageDataSource.ListPopulationComplete))
                {
                    if (_data.ListPopulationComplete)
                    {
                        foreach (var column in GridView.Columns)
                        {
                            column.Width = column.ActualWidth;
                            column.Width = double.NaN;
                        }
                    }
                    ListView.SelectedIndex = 0;
                }
                else if (e.PropertyName == nameof(ToolboxControlledPageDataSource.CaretItem))
                    FocusCaretItem();
                else if (e.PropertyName == nameof(ToolboxControlledPageDataSource.Items))
                    SetViewFilterAndSubscribeToItemsCollectionChanges();
                else
                {
                    if (e.PropertyName != nameof(ToolboxControlledPageDataSource.FilterUpdateInProgress))
                        return;
                    _filterRefreshInProgress = _data.FilterUpdateInProgress;
                    if (_filterRefreshInProgress)
                        return;
                    ListCollectionView.Refresh();
                }
            });
        }

        private void SetViewFilterAndSubscribeToItemsCollectionChanges()
        {
            ListCollectionView.Filter = item => ((ItemDataSource)item).IsVisible;
            var items = _data.Items;
            if (items == null)
                return;
            ((INotifyCollectionChanged)items).CollectionChanged += OnItemsCollectionChanged;
            foreach (INotifyPropertyChanged notifyPropertyChanged in (IEnumerable)items)
                notifyPropertyChanged.PropertyChanged += OnItemPropertyChanged;
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(e.PropertyName == nameof(ItemDataSource.IsVisible) || _filterRefreshInProgress))
                return;
            ListCollectionView.Refresh();
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged oldItem in e.OldItems)
                    oldItem.PropertyChanged -= OnItemPropertyChanged;
            }
            if (e.NewItems == null)
                return;
            foreach (INotifyPropertyChanged newItem in e.NewItems)
                newItem.PropertyChanged += OnItemPropertyChanged;
        }

        private void FocusCaretItem()
        {
            var caret = _data.CaretItem;
            ListView.ScrollIntoView(caret);
            var item = (ListViewItem)ListView.ItemContainerGenerator.ContainerFromItem(caret);
            if (item != null)
                item.Focus();
            else
                ListView.ItemContainerGenerator.StatusChanged += FocusCaretItemWhenContainerIsAvailable;
        }

        private void FocusCaretItemWhenContainerIsAvailable(object sender, EventArgs e)
        {
            if (ListView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return;
            ListView.ItemContainerGenerator.StatusChanged -= FocusCaretItemWhenContainerIsAvailable;
            FocusCaretItem();
        }

        private int DistanceBetween(ItemsControl listView, ItemDataSource previousCaretItem, ItemDataSource item)
        {
            return Math.Abs(listView.Items.IndexOf(previousCaretItem) - listView.Items.IndexOf(item));
        }

        private void ItemKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                ItemDoubleClicked(sender, e);
            else if (e.Key == Key.Add || e.Key == Key.OemPlus)
                CheckSelectedItems(e, true);
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                CheckSelectedItems(e, false);
            else
            {
                var c = (char) NativeMethods.User32.MapVirtualKey((uint) KeyInterop.VirtualKeyFromKey(e.Key), 2);
                if (!char.IsLetterOrDigit(c) && !char.IsPunctuation(c) && !char.IsSymbol(c))
                    return;
                AddCharToTypeAhead(c);
                e.Handled = true;
            }
        }

        private void AddCharToTypeAhead(char c)
        {
            var tickCount = Environment.TickCount;
            bool skipCurrentItem;

            if (tickCount < _lastTypeAheadTickCount || tickCount - _lastTypeAheadTickCount >= 2000 ||
                _typeAheadString == null)
            {
                _typeAheadString = c.ToString();
                skipCurrentItem = true;
            }
            else if (_typeAheadString.Length == 1 && _typeAheadString[0] == c)
                skipCurrentItem = true;
            else
            {
                _typeAheadString += c.ToString();
                skipCurrentItem = false;
            }
            SelectNextItemStartingWith(_typeAheadString, skipCurrentItem);
            _lastTypeAheadTickCount = tickCount;

        }

        private void SelectNextItemStartingWith(string s, bool skipCurrentItem)
        {
            var flag = true;
            var num = ListView.SelectedIndex;
            if (skipCurrentItem)
                num = (num + 1) % ListView.Items.Count;
            for (var index = num; flag || index != ListView.SelectedIndex; index = (index + 1) % ListView.Items.Count)
            {
                if (((ItemDataSource) ListView.Items[index]).Name.StartsWith(s,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    ListView.SelectedIndex = index;
                    return;
                }
                flag = false;
            }
            if (s.Length != 1)
                return;
            SystemSounds.Beep.Play();
        }

        private void CheckSelectedItems(RoutedEventArgs e, bool shouldBeChecked)
        {
            foreach (ItemDataSource selectedItem in ListView.SelectedItems)
                selectedItem.IsChecked = shouldBeChecked;
            e.Handled = true;
        }

        private void ItemDoubleClicked(object sender, RoutedEventArgs e)
        {
            var flag = ((ItemDataSource)((ContentControl)sender).Content).IsChecked;
            foreach (ItemDataSource selectedItem in ListView.SelectedItems)
                selectedItem.IsChecked = !flag;
            e.Handled = true;
        }

        private void ItemCheckboxClicked(object sender, RoutedEventArgs e)
        {
            var ancestor = ((Visual) sender).FindAncestor<ListViewItem>();
            if (!ancestor.IsSelected)
                return;
            var flag = ((ItemDataSource) ancestor.Content).IsChecked;
            foreach (ItemDataSource selectedItem in ListView.SelectedItems)
                selectedItem.IsChecked = flag;
        }

        internal void AddColumns(ToolboxControlledPageDataSource dataSource)
        {
            foreach (var column in dataSource.ListColumns)
            {
                GridView.Columns.Add(new GridViewColumn
                {
                    Header = column.Text,
                    Width = double.NaN,
                    DisplayMemberBinding = new Binding(column.Name)
                });
            }
        }

        private void InitializeSortColumnFromDataSource()
        {

        }

        private static void OnColumnHeaderDividerDragged(object sender, DragDeltaEventArgs e)
        {
            if (!(((FrameworkElement)e.OriginalSource).TemplatedParent is GridViewColumnHeader parent) || parent.Content != null)
                return;
            parent.Column.Width = double.NaN;
        }
    }
}
