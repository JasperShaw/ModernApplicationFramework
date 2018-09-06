using System;
using System.Collections.Generic;
using System.Diagnostics;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    internal class WeakKeyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : class
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly Dictionary<WeakReference<TKey>, TValue> _dictionary;

        private readonly IEqualityComparer<TKey> _keyComparer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _capacity;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int Count => _dictionary.Count;

        public TValue this[TKey key]
        {
            get
            {
                var wrappedKey = new WeakReference<TKey>(key, _keyComparer, true);
                var value = _dictionary[wrappedKey];
                return value;
            }
            set
            {
                var wrappedKey = new WeakReference<TKey>(key, _keyComparer);
                if (_dictionary.Count == _capacity && !ContainsKey(key))
                {
                    Scavenge();
                    if (_dictionary.Count == _capacity)
                    {
                        _capacity = _dictionary.Count * 2;
                    }
                }
                _dictionary[wrappedKey] = value;
            }
        }

        public WeakKeyDictionary(IEqualityComparer<TKey> keyComparer = null, int capacity = 10)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _capacity = capacity;
            IEqualityComparer<WeakReference<TKey>> equalityComparer = new WeakReferenceEqualityComparer<TKey>(_keyComparer);
            _dictionary = new Dictionary<WeakReference<TKey>, TValue>(_capacity, equalityComparer);
        }

        public bool ContainsKey(TKey key)
        {
            var contained = TryGetValue(key, out _);
            return contained;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(new WeakReference<TKey>(key, _keyComparer, true), out value);
        }

        public bool Remove(TKey key)
        {
            return _dictionary.Remove(new WeakReference<TKey>(key, _keyComparer, true));
        }

        public int Scavenge()
        {
            List<WeakReference<TKey>> remove = null;

            foreach (var weakKey in _dictionary.Keys)
            {
                if (!weakKey.IsAlive)
                {
                    remove = remove ?? new List<WeakReference<TKey>>();
                    remove.Add(weakKey);
                }
            }

            if (remove != null)
            {
                foreach (var entry in remove)
                {
                    _dictionary.Remove(entry);
                }

                return remove.Count;
            }

            return 0;
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private Dictionary<WeakReference<TKey>, TValue>.Enumerator _enumerator;

            private KeyValuePair<TKey, TValue> _current;

            internal Enumerator(WeakKeyDictionary<TKey, TValue> dictionary)
            {
                Validate.IsNotNull(dictionary, nameof(dictionary));

                _enumerator = dictionary._dictionary.GetEnumerator();
                _current = default;
            }

            public KeyValuePair<TKey, TValue> Current => _current;

            object System.Collections.IEnumerator.Current => Current;

            public bool MoveNext()
            {
                while (_enumerator.MoveNext())
                {
                    var key = _enumerator.Current.Key.Target;
                    if (key != null)
                    {
                        _current = new KeyValuePair<TKey, TValue>(key, _enumerator.Current.Value);
                        return true;
                    }
                }

                return false;
            }

            void System.Collections.IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }
        }

        private struct WeakReference<T>
            where T : class
        {
            private readonly int _hashcode;
            private readonly WeakReference _weakReference;
            private readonly T _notSoWeakTarget;

            internal T Target => _notSoWeakTarget ?? (T)_weakReference.Target;

            internal bool IsAlive => _notSoWeakTarget != null || _weakReference.IsAlive;

            internal WeakReference(T target, IEqualityComparer<T> equalityComparer, bool avoidWeakReferenceAllocation = false)
            {
                Validate.IsNotNull(target, nameof(target));
                Validate.IsNotNull(equalityComparer, nameof(equalityComparer));

                _notSoWeakTarget = avoidWeakReferenceAllocation ? target : null;
                _weakReference = avoidWeakReferenceAllocation ? null : new WeakReference(target);
                _hashcode = equalityComparer.GetHashCode(target);
            }

            public override int GetHashCode()
            {
                return _hashcode;
            }

            public override bool Equals(object obj)
            {
                if (obj is WeakReference<T> other)
                {
                    return _weakReference.Equals(other._weakReference);
                }

                return false;
            }
        }

        private class WeakReferenceEqualityComparer<T> : IEqualityComparer<WeakReference<T>>
            where T : class
        {
            private readonly IEqualityComparer<T> _underlyingComparer;

            internal WeakReferenceEqualityComparer(IEqualityComparer<T> comparer)
            {
                Validate.IsNotNull(comparer, nameof(comparer));
                _underlyingComparer = comparer;
            }

            public int GetHashCode(WeakReference<T> item)
            {
                return item.GetHashCode();
            }

            public bool Equals(WeakReference<T> left, WeakReference<T> right)
            {
                return _underlyingComparer.Equals(left.Target, right.Target);
            }
        }
    }
}