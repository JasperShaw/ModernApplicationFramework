using System;
using System.Collections;
using System.Collections.Generic;

namespace ModernApplicationFramework.Input
{
    public class GestureCollection : IList<CategoryGestureMapping>
    {
        public delegate void GestursChangedEventHandler(object sender, GestureCollectionChangedEventArgs e);

        public event GestursChangedEventHandler GestursChanged;


        private List<CategoryGestureMapping> _innerList;

        public IEnumerator<CategoryGestureMapping> GetEnumerator()
        {
            return _innerList?.GetEnumerator() ??
                   new List<CategoryGestureMapping>(0).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(CategoryGestureMapping item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (_innerList == null)
                _innerList = new List<CategoryGestureMapping>(1);
            _innerList.Add(item);
            OnGestureChanged(new GestureCollectionChangedEventArgs(GestureCollectionChangedType.Added, item));
        }

        public void Clear()
        {
            if (_innerList == null)
                return;
            OnGestureChanged(new GestureCollectionChangedEventArgs(GestureCollectionChangedType.Cleared, _innerList));
            _innerList.Clear();
            _innerList = null;
        }

        public bool Contains(CategoryGestureMapping item)
        {
            return _innerList != null && _innerList.Contains(item);
        }

        public void CopyTo(CategoryGestureMapping[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(CategoryGestureMapping item)
        {
            if (_innerList == null)
                return false;
            var result = _innerList.Remove(item);
            if (!result)
                return false;
            OnGestureChanged(new GestureCollectionChangedEventArgs(GestureCollectionChangedType.Removed, item));
            return true;
        }

        public int Count => _innerList?.Count ?? 0;

        public bool IsReadOnly => false;

        public int IndexOf(CategoryGestureMapping item)
        {
            if (_innerList == null)
                return -1;
            return _innerList.IndexOf(item);
        }

        public void Insert(int index, CategoryGestureMapping item)
        {
            _innerList?.Insert(index, item);
            OnGestureChanged(new GestureCollectionChangedEventArgs(GestureCollectionChangedType.Added, item));
        }

        public void RemoveAt(int index)
        {
            if (_innerList == null)
                return;
            var item = _innerList[index];
            _innerList.RemoveAt(index);
            OnGestureChanged(new GestureCollectionChangedEventArgs(GestureCollectionChangedType.Removed, item));
        }

        public CategoryGestureMapping this[int index]
        {
            get => _innerList[index];
            set
            {
                if (_innerList == null)
                    _innerList = new List<CategoryGestureMapping>(index);
                _innerList[index] = value;
            }
        }

        protected void OnGestureChanged(GestureCollectionChangedEventArgs e)
        {
            GestursChanged?.Invoke(this, e);
        }
    }
}
