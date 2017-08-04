using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ModernApplicationFramework.Input
{
    public class GestureCollection : IList<GestureScopeMapping>, INotifyCollectionChanged
    {
        private List<GestureScopeMapping> _innerList;

        public IEnumerator<GestureScopeMapping> GetEnumerator()
        {
            return _innerList?.GetEnumerator() ??
                   new List<GestureScopeMapping>(0).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(GestureScopeMapping item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (_innerList == null)
                _innerList = new List<GestureScopeMapping>(1);
            _innerList.Add(item);
            OnGestureChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            if (_innerList == null)
                return;
            OnGestureChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, _innerList));
            _innerList.Clear();
            _innerList = null;
        }

        public bool Contains(GestureScopeMapping item)
        {
            return _innerList != null && _innerList.Contains(item);
        }

        public void CopyTo(GestureScopeMapping[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(GestureScopeMapping item)
        {
            if (_innerList == null)
                return false;
            var index = _innerList.IndexOf(item);
            OnGestureChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            _innerList.RemoveAt(index);
            return true;
        }

        public int Count => _innerList?.Count ?? 0;

        public bool IsReadOnly => false;

        public int IndexOf(GestureScopeMapping item)
        {
            if (_innerList == null)
                return -1;
            return _innerList.IndexOf(item);
        }

        public void Insert(int index, GestureScopeMapping item)
        {
            if (_innerList == null)
                _innerList = new List<GestureScopeMapping>(index + 1);
            _innerList.Insert(index, item);
            OnGestureChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void RemoveAt(int index)
        {
            if (_innerList == null)
                return;
            var item = _innerList[index];
            _innerList.RemoveAt(index);
            OnGestureChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        public GestureScopeMapping this[int index]
        {
            get => _innerList[index];
            set
            {
                if (_innerList == null)
                    _innerList = new List<GestureScopeMapping>(index);
                _innerList[index] = value;
            }
        }

        protected void OnGestureChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
