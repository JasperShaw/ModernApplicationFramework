using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Extended.Annotations;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    internal class ToolboxControlledPageDataSource : INotifyPropertyChanged
    {
        private bool _ascendingSort;
        private ItemDataSource _caretItem;
        private string _currentDefinition;
        private string _filterString = string.Empty;
        private bool _filterUpdateInProgress;
        private IList<ItemDataSource> _items;
        private bool _listItemsAdded;
        private bool _listPopulationComplete;
        private double _loadedDefinitions;
        private uint _sortColumnIndex;
        private double _totalDefititions;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool AscendingSort
        {
            get => _ascendingSort;
            set
            {
                if (value == _ascendingSort) return;
                _ascendingSort = value;
                OnPropertyChanged();
                SortItems();
            }
        }

        public ItemDataSource CaretItem
        {
            get => _caretItem;
            set
            {
                if (Equals(value, _caretItem)) return;
                _caretItem = value;
                OnPropertyChanged();
            }
        }

        public string CurrentDefinition
        {
            get => _currentDefinition;
            set
            {
                if (value == _currentDefinition) return;
                _currentDefinition = value;
                OnPropertyChanged();
            }
        }

        public string FilterString
        {
            get => _filterString;
            set
            {
                if (value == _filterString) return;
                _filterString = value;
                OnPropertyChanged();
                FilterItems();
            }
        }

        public bool FilterUpdateInProgress
        {
            get => _filterUpdateInProgress;
            set
            {
                if (value == _filterUpdateInProgress) return;
                _filterUpdateInProgress = value;
                OnPropertyChanged();
            }
        }

        public Guid Guid { get; }

        public IItemDataFactory ItemFactory { get; }

        public IList<ItemDataSource> Items
        {
            get => _items;
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                OnPropertyChanged();
            }
        }

        public ICollection<ColumnInformation> ListColumns { get; }

        public bool ListItemsAdded
        {
            get => _listItemsAdded;
            set
            {
                if (value == _listItemsAdded) return;
                _listItemsAdded = value;
                OnPropertyChanged();
            }
        }

        public bool ListPopulationComplete
        {
            get => _listPopulationComplete;
            set
            {
                if (value == _listPopulationComplete) return;
                _listPopulationComplete = value;
                OnPropertyChanged();
            }
        }

        public double LoadedDefinitions
        {
            get => _loadedDefinitions;
            set
            {
                if (value.Equals(_loadedDefinitions)) return;
                _loadedDefinitions = value;
                OnPropertyChanged();
            }
        }

        public string Name { get; }

        public int Order { get; }

        public Predicate<ToolboxItemDefinitionBase> Selector { get; }

        public uint SortColumnIndex
        {
            get => _sortColumnIndex;
            set
            {
                if (value == _sortColumnIndex) return;
                _sortColumnIndex = value;
                OnPropertyChanged();
                SortItems();
            }
        }

        public double TotalDefititions
        {
            get => _totalDefititions;
            set
            {
                if (value.Equals(_totalDefititions)) return;
                _totalDefititions = value;
                OnPropertyChanged();
            }
        }

        public ToolboxControlledPageDataSource(ItemDiscoveryService.ItemType itemType)
        {
            ItemFactory = itemType.ItemFactory;
            Guid = itemType.Guid;
            Name = itemType.Name;
            Order = itemType.Order;
            ListColumns = itemType.ListColumns.ToList();
            Selector = itemType.Selector;
            Items = new ObservableCollection<ItemDataSource>();
        }

        protected ToolboxControlledPageDataSource(ToolboxControlledPageDataSource baseDataSource)
        {
            ItemFactory = baseDataSource.ItemFactory;
            Guid = baseDataSource.Guid;
            Name = baseDataSource.Name;
            Order = baseDataSource.Order;
            Items = baseDataSource.Items;
            Selector = baseDataSource.Selector;
        }

        public void InsertSortedAndFiltered(ItemDataSource item)
        {
            FilterItem(item, FilterString);
            var items = Items;
            var index = BinarySearch(item, items, CompareItems, out var matched);
            if (matched)
            {
                var toRemove = items.ElementAt(index);
                toRemove.PropertyChanged -= ItemPropertyChanged;
                items.RemoveAt(index);
            }

            item.PropertyChanged += ItemPropertyChanged;
            items.Insert(index, item);
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static int BinarySearch<T>(T itemToMatch, IList<T> collection, Func<T, T, int> comparator,
            out bool matched)
        {
            return BinarySearch(itemToMatch, collection, 0, collection.Count - 1, comparator, out matched);
        }

        private static int BinarySearch<T>(T itemToMatch, IList<T> collection, int first, int last,
            Func<T, T, int> comparator, out bool matched)
        {
            if (first > last)
            {
                matched = false;
                return first;
            }

            var index = (first + last) / 2;
            var num = comparator(itemToMatch, collection[index]);
            if (num == 0)
            {
                matched = true;
                return index;
            }

            if (num < 0)
            {
                if (index != first)
                    return BinarySearch(itemToMatch, collection, first, index - 1, comparator, out matched);
                matched = false;
                return first;
            }

            if (index != last)
                return BinarySearch(itemToMatch, collection, index + 1, last, comparator, out matched);
            matched = false;
            return last + 1;
        }

        private static int CompareItems(ItemDataSource first, ItemDataSource second, ColumnInformation column)
        {
            var name1 = first.Name;
            var name2 = second.Name;
            if (name1 == null && name2 == null)
                return 0;
            if (name1 == null)
                return -1;
            if (name2 == null)
                return 1;
            if (column is ICustomSortColumn customSortColumn)
                return customSortColumn.Compare(name1, name2);
            return string.Compare(name1, name2, StringComparison.CurrentCulture);
        }

        private static void FilterItem(ItemDataSource item, string filter)
        {
            var flag = string.IsNullOrEmpty(filter) || item.SearchableStrings.Any(s =>
                           CultureInfo.CurrentUICulture.CompareInfo.IndexOf(s, filter, CompareOptions.IgnoreCase) >= 0);
            item.IsVisible = flag;
        }

        private int CompareItems(ItemDataSource first, ItemDataSource second)
        {
            var num1 = SortColumnIndex;
            var columns = ListColumns;
            var column = columns.ElementAt((int) num1);
            var flag = AscendingSort;
            var num2 = CompareItems(first, second, column);
            if (num2 != 0)
                return num2 * (flag ? 1 : -1);
            for (uint index = 0; index < columns.Count; ++index)
            {
                if (index == num1)
                    continue;
                num2 = CompareItems(first, second, columns.ElementAt((int) index));
                if (num2 != 0)
                    break;
            }

            return num2 * (flag ? 1 : -1);
        }

        private void FilterItems()
        {
            if (FilterUpdateInProgress)
                return;
            var filter = FilterString.Trim();
            FilterUpdateInProgress = true;
            try
            {
                foreach (var item in Items)
                    FilterItem(item, filter);
            }
            finally
            {
                FilterUpdateInProgress = false;
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is ItemDataSource item))
                return;
            if (e.PropertyName == nameof(ItemDataSource.IsChecked))
                PropertyChanged?.Invoke(item, e);
        }

        private void SortItems()
        {
            var num = SortColumnIndex;
            var colums = ListColumns;
            if (colums.Count == 0 || num > colums.Count)
                return;

            var list = Items.ToList();
            list.Sort(CompareItems);

            var newItemList = new ObservableCollection<ItemDataSource>();
            foreach (var itemDataSource in list)
                newItemList.Add(itemDataSource);
            Items = newItemList;
        }
    }
}