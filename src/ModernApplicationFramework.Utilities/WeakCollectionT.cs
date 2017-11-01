using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ModernApplicationFramework.Utilities
{
    public class WeakCollection<T> : IEnumerable<T> where T : class
    {
        private List<WeakReference> _innerList = new List<WeakReference>();

        public void Add(T item)
        {
            _innerList.Add(new WeakReference(item));
        }

        public void Clear()
        {
            _innerList.Clear();
        }

        public bool Remove(T item)
        {
            for (int index = 0; index < _innerList.Count; ++index)
            {
                if (_innerList[index].Target == item)
                {
                    _innerList.RemoveAt(index);
                    return true;
                }
            }
            return false;
        }

        public IList<T> ToList()
        {
            List<T> objList = new List<T>(_innerList.Count);
            foreach (WeakReference inner in _innerList)
            {
                if (inner.Target is T target)
                    objList.Add(target);
            }
            if (objList.Count != _innerList.Count)
                Prune(objList.Count);
            return objList;
        }

        public int GetAliveItemsCount()
        {
            return _innerList.Count(weakRef => weakRef.IsAlive);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        private void Prune(int anticipatedSize)
        {
            List<WeakReference> innerList = _innerList;
            _innerList = new List<WeakReference>(anticipatedSize);
            foreach (WeakReference weakReference in innerList)
            {
                if (weakReference.IsAlive)
                    _innerList.Add(weakReference);
            }
        }
    }
}
