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
        }

        public void Clear()
        {
            if (_innerList == null)
                return;
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
            return _innerList.Remove(item);
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
        }

        public void RemoveAt(int index)
        {
            _innerList?.RemoveAt(index);
        }

        public CategoryKeyGesture this[int index]
        {
            get => _innerList[index];
            set
            {
                if (_innerList == null)
                    _innerList = new List<CategoryKeyGesture>(index);
                _innerList[index] = value;
            }
        }

        protected void OnGestureAdded(GestureChangedEventArgs e)
        {
            GestursChanged?.Invoke(this, e);
        }

        protected void OnGestureRemoved(GestureChangedEventArgs e)
        {
            GestursChanged?.Invoke(this, e);
        }
    }


    //public class GestureCollection : BaseDictionary<CommandGestureCategory, KeyGesture>
    //{
    //    private Dictionary<CommandGestureCategory, KeyGesture> _innerDictionary;

    //    public delegate void GestursChangedEventHandler(object sender, GestureChangedEventArgs e);

    //    public event GestursChangedEventHandler GestursChanged;

    //    public override int Count => _innerDictionary?.Count ?? 0;

    //    public override void Clear()
    //    {
    //        if (_innerDictionary == null)
    //            return;
    //        _innerDictionary.Clear();
    //        _innerDictionary = null;
    //    }

    //    public override void Add(CommandGestureCategory key, KeyGesture value)
    //    {
    //        if (_innerDictionary == null)
    //            _innerDictionary = new Dictionary<CommandGestureCategory, KeyGesture>(1);
    //        _innerDictionary.Add(key, value);
    //    }

    //    public override bool Remove(CommandGestureCategory key)
    //    {
    //        if (_innerDictionary == null)
    //            return false;
    //        return _innerDictionary.Remove(key);
    //    }

    //    public override bool TryGetValue(CommandGestureCategory key, out KeyGesture value)
    //    {
    //        value = default(KeyGesture);
    //        return _innerDictionary != null && _innerDictionary.TryGetValue(key, out value);
    //    }

    //    public override IEnumerator<KeyValuePair<CommandGestureCategory, KeyGesture>> GetEnumerator()
    //    {
    //        if (_innerDictionary != null)
    //            return _innerDictionary.GetEnumerator();
    //        return new Dictionary<CommandGestureCategory, KeyGesture>(0).GetEnumerator();
    //    }

    //    protected override void SetValue(CommandGestureCategory key, KeyGesture value)
    //    {
    //        if (_innerDictionary == null || !_innerDictionary.ContainsKey(key))
    //            return;
    //        _innerDictionary[key] = value;
    //    }

    //    public override bool ContainsKey(CommandGestureCategory key)
    //    {
    //        if (_innerDictionary == null)
    //            return false;
    //        return _innerDictionary.ContainsKey(key);
    //    }

    //    protected void OnGestureAdded(GestureChangedEventArgs e)
    //    {
    //        GestursChanged?.Invoke(this, e);
    //    }

    //    protected void OnGestureRemoved(GestureChangedEventArgs e)
    //    {
    //        GestursChanged?.Invoke(this, e);
    //    }
    //}

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


    public class CategoryKeyGesture
    {
        public CategoryKeyGesture(CommandGestureCategory category, MultiKeyGesture keyGesture)
        {
            Category = category;
            KeyGesture = keyGesture;
        }

        public CommandGestureCategory Category { get; }

        public MultiKeyGesture KeyGesture { get; }

        public string Text => $"{KeyGesture.DisplayString} ({Category.Name})";
    }
}
