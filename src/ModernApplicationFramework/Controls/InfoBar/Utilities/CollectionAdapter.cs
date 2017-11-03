using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace ModernApplicationFramework.Controls.InfoBar.Utilities
{
    internal abstract class CollectionAdapter<TSource, TTarget> : ReadOnlyObservableCollection<TTarget>, IWeakEventListener
    {
        private IEnumerable _view;

        protected CollectionAdapter()
            : base(new SuspendableObservableCollection<TTarget>())
        {
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (!(managerType == typeof(CollectionChangedEventManager)))
                return false;
            OnInnerCollectionChanged((IEnumerable)sender, (NotifyCollectionChangedEventArgs)e);
            return true;
        }

        private void OnInnerCollectionChanged(IEnumerable innerCollection, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int index = 0; index < e.NewItems.Count; ++index)
                        InsertSourceItem(e.NewStartingIndex + index, (TSource)e.NewItems[index]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (int index = 0; index < e.OldItems.Count; ++index)
                        RemoveSourceItem(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (int index = 0; index < e.NewItems.Count; ++index)
                        ReplaceSourceItem(e.OldStartingIndex + index, (TSource)e.NewItems[index]);
                    break;
                case NotifyCollectionChangedAction.Move:
                    for (int index = 0; index < e.NewItems.Count; ++index)
                        MoveSourceItem(e.OldStartingIndex + index, e.NewStartingIndex + index);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ResetSourceItems(_view);
                    break;
            }
        }

        protected SuspendableObservableCollection<TTarget> InnerItems => (SuspendableObservableCollection<TTarget>)Items;

        protected void Initialize(IEnumerable source)
        {
            _view = source;
            while (true)
            {
                using (EnumerableSnapshot enumerableSnapshot = new EnumerableSnapshot(_view))
                {
                    foreach (TSource source1 in enumerableSnapshot)
                    {
                        Items.Add(AdaptItem(source1));
                        if (enumerableSnapshot.DetectedChange)
                            break;
                    }
                    if (enumerableSnapshot.DetectedChange)
                        Items.Clear();
                    else
                        break;
                }
            }
            if (!(_view is INotifyCollectionChanged view))
                return;
            CollectionChangedEventManager.AddListener(view, this);
        }

        protected virtual void InsertSourceItem(int index, TSource item)
        {
            InnerItems.Insert(index, AdaptItem(item));
        }

        protected virtual void RemoveSourceItem(int index)
        {
            InnerItems.RemoveAt(index);
        }

        protected virtual void MoveSourceItem(int sourceIndex, int targetIndex)
        {
            InnerItems.Move(sourceIndex, targetIndex);
        }

        protected virtual void ReplaceSourceItem(int index, TSource item)
        {
            InnerItems[index] = AdaptItem(item);
        }

        protected virtual void ResetSourceItems(IEnumerable newItems)
        {
            using (InnerItems.SuspendChangeNotification())
            {
                InnerItems.Clear();
                int index = 0;
                if (newItems == null)
                    return;
                foreach (TSource newItem in newItems)
                {
                    InsertSourceItem(index, newItem);
                    ++index;
                }
            }
        }

        protected abstract TTarget AdaptItem(TSource item);
    }
}