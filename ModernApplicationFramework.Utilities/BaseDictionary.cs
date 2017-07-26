using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ModernApplicationFramework.Utilities
{
    public abstract class BaseDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private KeyCollection _keys;
        private ValueCollection _values;

        public abstract int Count { get; }
        public abstract void Clear();
        public abstract void Add(TKey key, TValue value);
        public abstract bool ContainsKey(TKey key);
        public abstract bool Remove(TKey key);
        public abstract bool TryGetValue(TKey key, out TValue value);
        public abstract IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();
        protected abstract void SetValue(TKey key, TValue value);

        public bool IsReadOnly => false;

        public ICollection<TKey> Keys => _keys ?? (_keys = new KeyCollection(this));

        public ICollection<TValue> Values => _values ?? (_values = new ValueCollection(this));

        public TValue this[TKey key]
        {
            get
            {
                if (!TryGetValue(key, out var value))
                    throw new KeyNotFoundException();

                return value;
            }
            set => SetValue(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            if (!TryGetValue(item.Key, out value))
                return false;

            return EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Copy(this, array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Contains(item) && Remove(item.Key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private abstract class Collection<T> : ICollection<T>
        {
            protected readonly IDictionary<TKey, TValue> Dictionary;

            protected Collection(IDictionary<TKey, TValue> dictionary)
            {
                Dictionary = dictionary;
            }

            public int Count => Dictionary.Count;

            public bool IsReadOnly => true;

            public void CopyTo(T[] array, int arrayIndex)
            {
                Copy(this, array, arrayIndex);
            }

            public virtual bool Contains(T item)
            {
                return this.Any(element => EqualityComparer<T>.Default.Equals(element, item));
            }

            public IEnumerator<T> GetEnumerator()
            {
                foreach (KeyValuePair<TKey, TValue> pair in Dictionary)
                    yield return GetItem(pair);
            }

            protected abstract T GetItem(KeyValuePair<TKey, TValue> pair);

            public bool Remove(T item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void Add(T item)
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            public void Clear()
            {
                throw new NotSupportedException("Collection is read-only.");
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class KeyCollection : Collection<TKey>
        {
            public KeyCollection(IDictionary<TKey, TValue> dictionary)
                : base(dictionary) { }

            protected override TKey GetItem(KeyValuePair<TKey, TValue> pair)
            {
                return pair.Key;
            }
            public override bool Contains(TKey item)
            {
                return Dictionary.ContainsKey(item);
            }
        }

        private class ValueCollection : Collection<TValue>
        {
            public ValueCollection(IDictionary<TKey, TValue> dictionary)
                : base(dictionary) { }

            protected override TValue GetItem(KeyValuePair<TKey, TValue> pair)
            {
                return pair.Value;
            }
        }

        private static void Copy<T>(ICollection<T> source, T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if ((array.Length - arrayIndex) < source.Count)
                throw new ArgumentException("Destination array is not large enough. Check array.Length and arrayIndex.");

            foreach (T item in source)
                array[arrayIndex++] = item;
        }
    }
}