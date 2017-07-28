using System;
using System.Collections;
using System.Collections.Generic;

namespace ModernApplicationFramework.CommandBase.Input
{

    public class GestureCollection : IList<CategoryKeyGesture>
    {
        public delegate void GestursChangedEventHandler(object sender, GestureChangedEventArgs e);

        public event GestursChangedEventHandler GestursChanged;


        private List<CategoryKeyGesture> _innerList;

        public IEnumerator<CategoryKeyGesture> GetEnumerator()
        {
            return _innerList?.GetEnumerator() ??
                   new List<CategoryKeyGesture>(0).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(CategoryKeyGesture item)
        {
            if (_innerList == null)
                _innerList = new List<CategoryKeyGesture>(1);
            _innerList.Add(item);
            OnGestureChanged(new GestureChangedEventArgs(GestureChangedType.Added));
        }

        public void Clear()
        {
            if (_innerList == null)
                return;
            OnGestureChanged(new GestureChangedEventArgs(GestureChangedType.Removed));
            _innerList.Clear();
            _innerList = null;
        }

        public bool Contains(CategoryKeyGesture item)
        {
            return _innerList != null && _innerList.Contains(item);
        }

        public void CopyTo(CategoryKeyGesture[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(CategoryKeyGesture item)
        {
            if (_innerList == null)
                return false;
            var result = _innerList.Remove(item);
            if (!result)
                return false;
            OnGestureChanged(new GestureChangedEventArgs(GestureChangedType.Removed));
            return true;
        }

        public int Count => _innerList?.Count ?? 0;

        public bool IsReadOnly => false;

        public int IndexOf(CategoryKeyGesture item)
        {
            if (_innerList == null)
                return -1;
            return _innerList.IndexOf(item);
        }

        public void Insert(int index, CategoryKeyGesture item)
        {
            _innerList?.Insert(index, item);
            OnGestureChanged(new GestureChangedEventArgs(GestureChangedType.Added));
        }

        public void RemoveAt(int index)
        {
            _innerList?.RemoveAt(index);
            OnGestureChanged(new GestureChangedEventArgs(GestureChangedType.Removed));
        }

        public CategoryKeyGesture this[int index]
        {
            get => _innerList[index];
            set
            {
                if (_innerList == null)
                    _innerList = new List<CategoryKeyGesture>(index);
                _innerList[index] = value;
                OnGestureChanged(new GestureChangedEventArgs(GestureChangedType.ValueChanged));
            }
        }

        protected void OnGestureChanged(GestureChangedEventArgs e)
        {
            GestursChanged?.Invoke(this, e);
        }
    }
    public class GestureChangedEventArgs : EventArgs
    {
        public GestureChangedEventArgs(GestureChangedType type)
        {
            Type = type;
        }

        public GestureChangedType Type { get; }
    }

    public enum GestureChangedType
    {
        Added,
        Removed,
        ValueChanged
    }


    public class CategoryKeyGesture : IEquatable<CategoryKeyGesture>
    {
        public CategoryKeyGesture(CommandGestureCategory category, MultiKeyGesture keyGesture)
        {
            Category = category;
            KeyGesture = keyGesture;
        }

        public CommandGestureCategory Category { get; }

        public MultiKeyGesture KeyGesture { get; }

        public string Text => $"{KeyGesture.DisplayString} ({Category.Name})";

        public bool Equals(CategoryKeyGesture other)
        {
            return other != null && Equals(Category, other.Category) && Equals(KeyGesture, other.KeyGesture);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            var a = obj as CategoryKeyGesture;
            return a != null && Equals(a);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Category != null ? Category.GetHashCode() : 0) * 397) ^ (KeyGesture != null ? KeyGesture.GetHashCode() : 0);
            }
        }
    }
}
