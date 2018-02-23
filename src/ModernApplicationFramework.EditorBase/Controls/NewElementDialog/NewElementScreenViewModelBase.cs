using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.ComboBox;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.EditorBase.Commands;
using ModernApplicationFramework.EditorBase.Core;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.Interfaces.Controls;
using ListSortDirection = ModernApplicationFramework.EditorBase.Core.ListSortDirection;

namespace ModernApplicationFramework.EditorBase.Controls.NewElementDialog
{
    public abstract class NewElementScreenViewModelBase<T> : Screen, IExtensionDialogItemPresenter<T>
    {
        private int _selectedIndex;
        private IExtensionDefinition _selectedItem;
        private ComboBoxDataSource _sortDataSource;
        private IEnumerable<IExtensionDefinition> _itemSource;
        private EventHandler<ItemDoubleClickedEventArgs> _itemDoubleClicked;
        private INewElementExtensionsProvider _selectedProvider;

        // ReSharper disable once StaticMemberInGenericType
        private static readonly Func<IExtensionDefinition, IExtensionDefinition, int> NameCompare = (s, t) =>
            string.Compare(s.Name, t.Name, StringComparison.CurrentCulture);

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
        public event RoutedPropertyChangedEventHandler<object> ProviderSelectionChanged;

        public abstract bool UsesNameProperty { get; }
        public abstract bool UsesPathProperty { get; }
        public abstract bool CanOpenWith { get; }
        public virtual bool IsLargeIconsViewButtonVisible => false;
        public virtual bool IsSmallIconsViewButtonVisible => true;
        public virtual bool IsMediumIconsViewButtonVisible => true;
        public abstract string NoItemsMessage { get; }
        public abstract string NoItemSelectedMessage { get; }
        public abstract ObservableCollection<INewElementExtensionsProvider> Providers { get; }

        public INewElementExtensionsProvider SelectedProvider
        {
            get => _selectedProvider;
            set
            {
                if (Equals(value, _selectedProvider))
                    return;
                var tmp = _selectedProvider;
                _selectedProvider = value;
                OnProviderSelectionChanged(new RoutedPropertyChangedEventArgs<object>(tmp, value));
            }
        }

        public IEnumerable<IExtensionDefinition> ItemSource
        {
            get => _itemSource;
            set
            {
                if (Equals(value, _itemSource)) return;
                // Create List so we can be sure to have an ListCollectionView view source
                _itemSource = new List<IExtensionDefinition>(value);
                NotifyOfPropertyChange();
            }
        }

        public IExtensionDefinition SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (Equals(value, _selectedItem)) return;
                _selectedItem = value;
                NotifyOfPropertyChange();
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value == _selectedIndex)
                    return;

                _selectedIndex = value;
                NotifyOfPropertyChange();
            }
        }

        public virtual ObservableCollection<ISortingComboboxItem> SortItems { get; set; } =
            new ObservableCollection<ISortingComboboxItem>
            {
                new SortingComboboxItem("Standard", ListSortDirection.Ascending, (s, t) =>
                    {
                        if ( s.SortOrder == t.SortOrder && s.Name != null && t.Name != null)
                            return string.Compare(s.Name, t.Name, StringComparison.CurrentCulture);
                        return s.SortOrder.CompareTo(t.SortOrder);
                    }),
                new SortingComboboxItem("Name ascending", ListSortDirection.Ascending, NameCompare),
                new SortingComboboxItem("Name descending", ListSortDirection.Descending, NameCompare)
            };

        public ComboBoxDataSource SortDataSource
        {
            get => _sortDataSource;
            set
            {
                if (Equals(value, _sortDataSource)) return;
                _sortDataSource = value;
                NotifyOfPropertyChange();
            }
        }

        protected NewElementScreenViewModelBase()
        {
            ViewModelBinder.Bind(this, new NewElementPresenterView(), null);
        }

        public abstract T CreateResult(string name, string path);

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            if (view is IItemDoubleClickable doubleClickable)
                doubleClickable.ItemDoubledClicked += DoubleClickable_ItemDoubledClicked;
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            SortDataSource = new ComboBoxDataSource(SortItems);

            if (SortItems != null && SortItems.Count > 0 && SortDataSource.SelectedIndex < 0)
                SortDataSource.ChangeDisplayedItem(0);
            SortDataSource.PropertyChanged += SortDataSource_PropertyChanged;
        }

        protected virtual void OnProviderSelectionChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            ProviderSelectionChanged?.Invoke(this, e);
            OnUIThread(() =>
            {
                if (ItemSource.Any() && SelectedIndex < 0)
                    SelectedIndex = 0;
            });
        }

        private void SortDataSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SortDataSource.DisplayedItem))
                ChangeSorting(SortDataSource.DisplayedItem as ISortingComboboxItem);
        }

        private void ChangeSorting(IComparer sortOrder)
        {
            if (ItemSource == null)
                return;
            if (!(CollectionViewSource.GetDefaultView(ItemSource) is ListCollectionView defaultView) || defaultView.CustomSort == sortOrder)
                return;
            defaultView.CustomSort = sortOrder;
            defaultView.Refresh();
        }

        private void DoubleClickable_ItemDoubledClicked(object sender, ItemDoubleClickedEventArgs e)
        {
            _itemDoubleClicked?.Invoke(this, e);
        }
    }
}