using System;
using System.Collections;
using System.Collections.Generic;
using ModernApplicationFramework.Interfaces.Search;

namespace ModernApplicationFramework.Basics.Search.Internal
{
    internal class EnumerableSearchOptionsCollection : EnumerableComCollection<IEnumWindowSearchOptions, IWindowSearchOption>
    {
        public EnumerableSearchOptionsCollection(IEnumWindowSearchOptions enumerator)
            : base(enumerator)
        {
        }


        public override int Clone(IEnumWindowSearchOptions enumerator, out IEnumWindowSearchOptions clone)
        {
            enumerator.Clone(out var enumClone);
            clone = enumClone;
            return 0;
        }

        public override int NextItems(IEnumWindowSearchOptions enumerator, uint count, IWindowSearchOption[] items, out uint fetched)
        {
            return enumerator.Next(count, items, out fetched);
        }

        public override int Reset(IEnumWindowSearchOptions enumerator)
        {
            enumerator.Reset();
            return 0;
        }

        public override int Skip(IEnumWindowSearchOptions enumerator, uint count)
        {
            return enumerator.Skip(count);
        }
    }

    public abstract class EnumerableComCollection<TEnumerator, TEnumerated> : IEnumerable<TEnumerated>, IEnumerable, IComEnumeratorRelay<TEnumerator, TEnumerated>
    {
        protected const int DefaultCacheSize = 8;
        private readonly TEnumerator _wrappedEnumerator;
        private readonly int _cacheSize;

        protected EnumerableComCollection(TEnumerator enumerator, int cacheSize)
        {
            if (enumerator == null)
                throw new ArgumentNullException(nameof(enumerator));
            if (cacheSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(cacheSize));
            _cacheSize = cacheSize;
            _wrappedEnumerator = Clone(enumerator);        
        }

        protected EnumerableComCollection(TEnumerator enumerator) : this(enumerator, DefaultCacheSize)
        {
        }

        public IEnumerator<TEnumerated> GetEnumerator()
        {
            return CreateEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return CreateEnumerator();
        }

        public abstract int Clone(TEnumerator enumerator, out TEnumerator newInstance);

        public abstract int NextItems(TEnumerator enumerator, uint count, TEnumerated[] items, out uint fetched);

        public abstract int Reset(TEnumerator enumerator);

        public abstract int Skip(TEnumerator enumerator, uint count);

        private TEnumerator Clone(TEnumerator original)
        {
            Clone(original, out var clone);
            return clone;
        }

        private IEnumerator<TEnumerated> CreateEnumerator()
        {
            return new Enumerator(this, Clone(_wrappedEnumerator), _cacheSize);
        }

        private class Enumerator : IEnumerator<TEnumerated> 
        {
            private readonly IComEnumeratorRelay<TEnumerator, TEnumerated> _relay;
            private readonly TEnumerator _wrappedComEnumerator;
            private readonly TEnumerated[] _cache;
            private int _currentIndex;
            private int _validCachedItemCount;
            private bool _fetchAgain;


            object IEnumerator.Current => Current;

            public TEnumerated Current => _cache[_currentIndex];

            private bool MoreItems
            {
                get
                {
                    if (!_fetchAgain)
                        return NextIndex <= _validCachedItemCount;
                    return true;
                }
            }

            private bool NeedToFetch
            {
                get
                {
                    if (_fetchAgain)
                        return NextIndex >= _validCachedItemCount;
                    return false;
                }
            }

            private int NextIndex => _currentIndex + 1;

            private int CacheSize => _cache.Length;

            public Enumerator(IComEnumeratorRelay<TEnumerator, TEnumerated> relay, TEnumerator enumerator, int cacheSize)
            {
                if (enumerator == null)
                    throw new ArgumentNullException(nameof(enumerator));
                if (cacheSize <= 0)
                    throw new ArgumentOutOfRangeException(nameof(cacheSize));
                _relay = relay ?? throw new ArgumentNullException(nameof(relay));
                _fetchAgain = true;
                _wrappedComEnumerator = enumerator;
                _cache = new TEnumerated[cacheSize];
                _currentIndex = cacheSize;
                _validCachedItemCount = 0;
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            public bool MoveNext()
            {
                if (!MoreItems)
                    return false;
                if (NeedToFetch)
                    Fetch();
                else if (NextIndex < CacheSize)
                    ++_currentIndex;
                return MoreItems;
            }

            public void Reset()
            {
                _relay.Reset(_wrappedComEnumerator);
                ResetCache(0);
                _fetchAgain = true;
            }

            private bool Fetch()
            {
                _relay.NextItems(_wrappedComEnumerator, (uint) CacheSize, _cache, out var fetched);
                ResetCache((int) fetched);
                _fetchAgain = _validCachedItemCount == CacheSize;
                return _validCachedItemCount > 0;
            }

            private void ResetCache(int validCachedItemCount)
            {
                _currentIndex = 0;
                _validCachedItemCount = validCachedItemCount;
                for (var i = _validCachedItemCount; i < CacheSize; ++i)
                    _cache[i] = default;
            }
        }
    }

    public interface IComEnumeratorRelay<TEnumerator, in TEnumerated>
    {
        int Clone(TEnumerator enumerator, out TEnumerator newInstance);

        int NextItems(TEnumerator enumerator, uint count, TEnumerated[] items, out uint fetched);

        int Reset(TEnumerator enumerator);

        int Skip(TEnumerator enumerator, uint count);
    }
}
