using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.MostRecentlyUsedManager
{
    [DebuggerDisplay("Count = {Items.Count}")]
    public abstract class MruManager<T> : IMruManager<T> where T: MruItem
    {
        private int _maxCount;

        public int MaxCount
        {
            get => _maxCount;
            set
            {
                if (_maxCount == value)
                    return;
                _maxCount = value;
                EnsureMaxCount();
            }
        }

        public int Count => Items.Count;

        public ObservableCollection<T> Items { get; }

        protected MruManager(int maxCount)
        {
            MaxCount = maxCount;
            Items = new ObservableCollection<T>();
        }

        public void AddItem(string persistenceData)
        {
            if (!IsValidMruItem(persistenceData) || TryPromoteItem(persistenceData))
                return;
            if (!EnsureMaxCount(1))
                return;
            T obj = CreateItem(persistenceData);
            Items.Insert(0, obj);
            OnItemAdded(obj);
        }

        public void OpenItem(int index)
        {
            ValidateIndex(index);
            OnItemOpened(Items[index]);
        }

        public void RemoveItemAt(int index)
        {
            ValidateIndex(index);
            var obj = Items[index];
            Items.RemoveAt(index);
            OnItemRemoved(obj);
        }

        protected abstract T CreateItem(string persistenceData);

        protected virtual bool IsValidMruItem(string stringValue)
        {
            return true;
        }

        protected virtual void OnItemAdded(T item)
        {
        }

        protected virtual void OnItemRemoved(T item)
        {
        }

        protected virtual void OnItemPromoted(T item)
        {
        }

        protected virtual void OnItemOpened(T item)
        {
            PromoteItem(item);
        }

        protected void PromoteItem(T item)
        {
            PromoteItemAt(Items.IndexOf(item));
        }

        private bool EnsureMaxCount(int minFreeSlots = 0)
        {
            if (Items == null)
                return false;
            var num = MaxCount - minFreeSlots;
            for (var index = Items.Count - 1; index >= 0 && Items.Count > num; --index)
            {
                if (!Items[index].Pinned)
                    RemoveItemAt(index);
            }
            return num >= Items.Count;
        }

        private bool TryPromoteItem(string persistenceData)
        {
            var index = Items.IndexOf(i => i.Matches(persistenceData));
            if (index <= -1)
                return false;
            PromoteItemAt(index);
            return true;
        }

        private void PromoteItemAt(int index)
        {
            if (index == -1)
                return;
            var obj = Items[index];
            if (index <= 0)
                return;
            Items.Move(index, 0);
            OnItemPromoted(obj);
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));
        }
    }
}
