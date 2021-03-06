﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ModernApplicationFramework.Utilities
{
    /// <inheritdoc />
    /// <summary>
    /// An extended <see cref="T:System.Collections.ObjectModel.ObservableCollection`1" /> that has a <see cref="E:ModernApplicationFramework.Utilities.ObservableCollectionEx`1.ItemPropertyChanged" /> event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="T:System.Collections.ObjectModel.ObservableCollection`1" />
    public sealed class ObservableCollectionEx<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler ItemPropertyChanged;

        public ObservableCollectionEx()
        {
            CollectionChanged += TrulyObservableCollection_CollectionChanged;
        }

        private void TrulyObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object item in e.NewItems)
                {
                    if (item is INotifyPropertyChanged notifyPropertyChanged)
                        notifyPropertyChanged.PropertyChanged += Item_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is INotifyPropertyChanged notifyPropertyChanged)
                        notifyPropertyChanged.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs a = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(a);
            ItemPropertyChanged?.Invoke(sender, e);
        }

        public void Sort<TKey>(Func<T, TKey> keySelector)
        {
            InternalSort(Items.OrderBy(keySelector));
        }

        public void SortDescending<TKey>(Func<T, TKey> keySelector)
        {
            InternalSort(Items.OrderByDescending(keySelector));
        }

        private void InternalSort(IEnumerable<T> sortedItems)
        {
            var sortedItemsList = sortedItems.ToList();

            foreach (var item in sortedItemsList)
            {
                Move(IndexOf(item), sortedItemsList.IndexOf(item));
            }
        }
    }
}
