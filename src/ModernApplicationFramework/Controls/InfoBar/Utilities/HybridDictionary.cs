using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.InfoBar.Utilities
{
    internal class HybridDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private ICollection<KeyValuePair<TKey, TValue>> _inner;
        private readonly IEqualityComparer<TKey> _keyComparer;
        internal const int CutoverPoint = 32;

        public HybridDictionary()
            : this(0, null)
        {
        }

        public HybridDictionary(IEqualityComparer<TKey> keyComparer)
            : this(0, keyComparer)
        {
        }

        [Obsolete("Use the custructor that takes a custom comparer instead.")]
        public HybridDictionary(bool caseInsensitive)
            : this(0, caseInsensitive)
        {
        }

        public HybridDictionary(int capacity)
            : this(capacity, null)
        {
        }

        [Obsolete("Use the custructor that takes a custom comparer instead.")]
        public HybridDictionary(int capacity, bool caseInsensitive)
            : this(capacity, caseInsensitive ? (IEqualityComparer<TKey>)StringComparer.OrdinalIgnoreCase : null)
        {
        }

        public HybridDictionary(int capacity, IEqualityComparer<TKey> keyComparer)
        {
            Validate.IsWithinRange(capacity, 0, int.MaxValue, nameof(capacity));
            _inner = capacity <= 32 ? (capacity == 0 ? null : new List<KeyValuePair<TKey, TValue>>(capacity)) : (ICollection<KeyValuePair<TKey, TValue>>)new Dictionary<TKey, TValue>(capacity, keyComparer);
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        public bool ContainsKey(TKey key)
        {
            if (_inner == null)
                return false;
            Dictionary<TKey, TValue> asDictionary = AsDictionary;
            if (asDictionary != null)
                return asDictionary.ContainsKey(key);
            return IndexOfKey(List, key) != -1;
        }

        public ICollection<TKey> Keys
        {
            get
            {
                if (_inner == null)
                    return new TKey[0];
                Dictionary<TKey, TValue> asDictionary = AsDictionary;
                if (asDictionary != null)
                    return asDictionary.Keys;
                return _inner.Select(kvp => kvp.Key).ToArray();
            }
        }

        public bool Remove(TKey key)
        {
            if (_inner == null)
                return false;
            Dictionary<TKey, TValue> asDictionary = AsDictionary;
            if (asDictionary != null)
                return asDictionary.Remove(key);
            List<KeyValuePair<TKey, TValue>> list = List;
            int index = IndexOfKey(list, key);
            if (index < 0)
                return false;
            list.RemoveAt(index);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_inner == null)
            {
                value = default(TValue);
                return false;
            }
            Dictionary<TKey, TValue> asDictionary = AsDictionary;
            if (asDictionary != null)
                return asDictionary.TryGetValue(key, out value);
            List<KeyValuePair<TKey, TValue>> list = List;
            int index = IndexOfKey(list, key);
            if (index < 0)
            {
                value = default(TValue);
                return false;
            }
            value = list[index].Value;
            return true;
        }

        public ICollection<TValue> Values
        {
            get
            {
                if (_inner == null)
                    return new TValue[0];
                Dictionary<TKey, TValue> asDictionary = AsDictionary;
                if (asDictionary != null)
                    return asDictionary.Values;
                return _inner.Select(kvp => kvp.Value).ToArray();
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (_inner == null)
                    throw new KeyNotFoundException();
                Dictionary<TKey, TValue> asDictionary = AsDictionary;
                if (asDictionary != null)
                    return asDictionary[key];
                List<KeyValuePair<TKey, TValue>> list = List;
                int index = IndexOfKey(list, key);
                if (index < 0)
                    throw new KeyNotFoundException();
                return list[index].Value;
            }
            set => Insert(key, value, false);
        }

        private Dictionary<TKey, TValue> AsDictionary => _inner as Dictionary<TKey, TValue>;

        private List<KeyValuePair<TKey, TValue>> List => (List<KeyValuePair<TKey, TValue>>)_inner;

        private int IndexOfKey(List<KeyValuePair<TKey, TValue>> list, TKey key)
        {
            for (int index = 0; index < list.Count; ++index)
            {
                if (_keyComparer.Equals(list[index].Key, key))
                    return index;
            }
            return -1;
        }

        private void Insert(TKey key, TValue value, bool add)
        {
            Validate.IsNotNull(key, nameof(key));
            if (_inner == null)
            {
                _inner = new List<KeyValuePair<TKey, TValue>>()
                {
                    new KeyValuePair<TKey, TValue>(key, value)
                };
            }
            else
            {
                Dictionary<TKey, TValue> asDictionary = AsDictionary;
                if (asDictionary != null)
                {
                    if (add)
                        asDictionary.Add(key, value);
                    else
                        asDictionary[key] = value;
                }
                else
                {
                    List<KeyValuePair<TKey, TValue>> list = List;
                    int index = IndexOfKey(list, key);
                    if (index >= 0)
                    {
                        if (add)
                            throw new ArgumentException("Adding a duplicate key");
                        list[index] = new KeyValuePair<TKey, TValue>(key, value);
                    }
                    else if (list.Count < 32)
                        list.Add(new KeyValuePair<TKey, TValue>(key, value));
                    else
                        UpgradeToDictionary(list).Add(key, value);
                }
            }
        }

        private Dictionary<TKey, TValue> UpgradeToDictionary(List<KeyValuePair<TKey, TValue>> list)
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(33, _keyComparer);
            foreach (KeyValuePair<TKey, TValue> keyValuePair in list)
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);
            list.Clear();
            _inner = dictionary;
            return dictionary;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _inner = null;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!TryGetValue(item.Key, out var obj))
                return false;
            if (obj is IEquatable<TValue> equatable)
                return equatable.Equals(item.Value);
            if (obj is IComparable<TValue> comparable)
                return comparable.CompareTo(item.Value) == 0;
            return obj.Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _inner?.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                if (_inner != null)
                    return _inner.Count;
                return 0;
            }
        }

        public bool IsReadOnly => false;

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _inner != null && _inner.Remove(item);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (_inner != null)
                return _inner.GetEnumerator();
            return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}