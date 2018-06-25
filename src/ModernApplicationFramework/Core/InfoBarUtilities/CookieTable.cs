using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Core.InfoBarUtilities
{
    public class CookieTable<TCookie, TValue> where TCookie : IComparable<TCookie>
    {
        private readonly object _syncLock = new object();
        private TCookie _currentCookie;
        private readonly CookieTraits<TCookie> _traits;
        private IDictionary<TCookie, TValue> _table;
        private PendingModifications _pendingMods;
        private uint _lockCount;

        public CookieTable(CookieTraits<TCookie> traits)
        {
            _traits = traits;
            _currentCookie = _traits.InvalidCookie;
        }

        private IDictionary<TCookie, TValue> Table
        {
            get => _table ?? (_table = new HybridDictionary<TCookie, TValue>());
            set => _table = value;
        }

        private PendingModifications PendingMods
        {
            get
            {
                if (!IsLocked)
                    throw new InvalidOperationException("Unlocked");
                return _pendingMods ?? (_pendingMods = new PendingModifications());
            }
            set => _pendingMods = value;
        }

        private bool HasPendingMods
        {
            get
            {
                bool flag = _pendingMods != null;
                if (flag && !IsLocked)
                    throw new NotSupportedException("Locked");
                return flag;
            }
        }

        private TCookie NextCookie
        {
            get
            {
                lock (_syncLock)
                {
                    IDictionary<TCookie, TValue> table = Table;
                    uint count = (uint)table.Count;
                    if (HasPendingMods)
                        count += (uint)PendingMods.PendingInsert.Count;
                    if (count >= _traits.UniqueCookies)
                        throw new ApplicationException("Empty");
                    do
                    {
                        _currentCookie = _traits.GetNextCookie(_currentCookie);
                    }
                    while (table.ContainsKey(_currentCookie) || HasPendingMods && PendingMods.PendingInsert.ContainsKey(_currentCookie));
                    return _currentCookie;
                }
            }
        }

        public TCookie Insert(TValue value)
        {
            lock (_syncLock)
            {
                using (new CookieTableLock(this))
                {
                    TCookie nextCookie = NextCookie;
                    PendingMods.PendingInsert.Add(nextCookie, value);
                    return nextCookie;
                }
            }
        }

        public bool Remove(TCookie cookie)
        {
            lock (_syncLock)
            {
                bool flag = false;
                using (new CookieTableLock(this))
                {
                    if (Table.ContainsKey(cookie) && !IsPendingDelete(cookie))
                    {
                        PendingMods.PendingDelete.Add(cookie);
                        flag = true;
                    }
                    if (!flag)
                    {
                        if (HasPendingMods)
                        {
                            if (PendingMods.PendingInsert.ContainsKey(cookie))
                            {
                                PendingMods.PendingInsert.Remove(cookie);
                                flag = true;
                            }
                        }
                    }
                }
                return flag;
            }
        }

        public void Clear()
        {
            lock (_syncLock)
            {
                using (new CookieTableLock(this))
                {
                    PendingMods.Reset();
                    PendingMods.PendingClear = true;
                }
            }
        }

        public bool ContainsCookie(TCookie cookie)
        {
            lock (_syncLock)
                return Table.ContainsKey(cookie);
        }

        public void ForEach(CookieTableCallback<TCookie, TValue> callback, bool skipRemoved)
        {
            using (new CookieTableLock(this))
            {
                Dictionary<TCookie, TValue> dictionary;
                lock (_syncLock)
                    dictionary = new Dictionary<TCookie, TValue>(Table);
                foreach (KeyValuePair<TCookie, TValue> keyValuePair in dictionary)
                {
                    if (!(IsPendingDelete(keyValuePair.Key) & skipRemoved))
                        callback(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }

        public void ForEach(CookieTableCallback<TCookie, TValue> callback)
        {
            ForEach(callback, true);
        }

        private bool IsPendingDelete(TCookie cookie)
        {
            lock (_syncLock)
            {
                if (!HasPendingMods)
                    return false;
                if (PendingMods.PendingClear)
                    return true;
                return PendingMods.PendingDelete.Contains(cookie);
            }
        }

        public bool TryGetValue(TCookie cookie, out TValue value)
        {
            lock (_syncLock)
            {
                bool flag = Table.TryGetValue(cookie, out value);
                if (flag && IsLocked && IsPendingDelete(cookie))
                    flag = false;
                return flag;
            }
        }

        public ICollection<TCookie> Cookies
        {
            get
            {
                lock (_syncLock)
                {
                    TCookie[] array = new TCookie[Table.Count];
                    Table.Keys.CopyTo(array, 0);
                    return array;
                }
            }
        }

        public TValue this[TCookie cookie]
        {
            get
            {
                if (!TryGetValue(cookie, out var obj))
                    throw new ArgumentException("Invalid cookie", nameof(cookie));
                return obj;
            }
        }

        public uint Size => (uint)Table.Count;

        public uint PendingSize
        {
            get
            {
                lock (_syncLock)
                {
                    uint num = Size;
                    if (IsLocked && HasPendingMods)
                        num = (!PendingMods.PendingClear ? num - (uint)PendingMods.PendingDelete.Count : 0U) + (uint)PendingMods.PendingInsert.Count;
                    return num;
                }
            }
        }

        public uint MaxSize => _traits.UniqueCookies;

        public void Lock()
        {
            lock (_syncLock)
                _lockCount = _lockCount + 1U;
        }

        public void Unlock()
        {
            lock (_syncLock)
            {
                if ((int)_lockCount == 0)
                    throw new InvalidOperationException("Unlocked");
                try
                {
                    if ((int)_lockCount != 1 || !HasPendingMods)
                        return;
                    if (PendingMods.PendingClear)
                    {
                        Table = null;
                    }
                    else
                    {
                        foreach (var key in PendingMods.PendingDelete)
                            Table.Remove(key);
                        if (Table.Count == 0)
                            Table = null;
                    }
                    foreach (var keyValuePair in PendingMods.PendingInsert)
                    {
                        Table?.Add(keyValuePair);
                    }
                    PendingMods = null;
                }
                finally
                {
                    _lockCount = _lockCount - 1U;
                }
            }
        }

        public bool IsLocked
        {
            get
            {
                lock (_syncLock)
                    return _lockCount > 0U;
            }
        }

        private struct CookieTableLock : IDisposable
        {
            private readonly CookieTable<TCookie, TValue> _cookieTable;

            public CookieTableLock(CookieTable<TCookie, TValue> cookieTable)
            {
                _cookieTable = cookieTable;
                _cookieTable.Lock();
            }

            public void Dispose()
            {
                _cookieTable.Unlock();
            }
        }

        private class PendingModifications
        {
            public readonly IDictionary<TCookie, TValue> PendingInsert = new HybridDictionary<TCookie, TValue>();
            public readonly IList<TCookie> PendingDelete = new List<TCookie>();
            public bool PendingClear;

            public void Reset()
            {
                PendingClear = false;
                PendingDelete.Clear();
                PendingInsert.Clear();
            }
        }
    }
}