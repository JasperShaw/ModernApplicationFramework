using System;
using System.Collections;
using System.Windows;
using Caliburn.Micro;

namespace ModernApplicationFramework.Extended.UIContext
{
    internal sealed class UiContextImpl : IUiContextEvents
    {
        internal static UiContextImpl Instance = new UiContextImpl();
        private static readonly UiContext DummyContext = new UiContext(Guid.Empty, 0, false, false);
        private FrugalMap _registeredContexts;

        private readonly Lazy<IUiContextManager> _contextManager = new Lazy<IUiContextManager>(() => IoC.Get<IUiContextManager>());

        private uint _eventCookie;

        private UiContextImpl()
        {
        }

        public UiContext Register(Guid contextGuid, bool isKnown)
        {
            var result = _contextManager.Value.GetUiContextCookie(contextGuid, out var cookie);
            if (result < 0)
                return DummyContext;
            lock (this)
            {
                var registeredContext = _registeredContexts[(int) cookie];
                if (registeredContext != DependencyProperty.UnsetValue)
                    return (UiContext) registeredContext;

                if (_eventCookie == 0)
                {
                    var instance = ExecutionContextTrackerHelper.Instance;
                    var contextElementGuid = instance?.SetAndGetContextElement(new Guid("{C64A39C6-DA78-4B8F-AFDF-EA3FE13E9389}"), Guid.Empty) ?? Guid.Empty;
                    try
                    {
                        _contextManager.Value.AdviseContextEvents(this, out _eventCookie);
                    }
                    finally
                    {
                        instance?.SetContextElement(new Guid("{C64A39C6-DA78-4B8F-AFDF-EA3FE13E9389}"), contextElementGuid);
                    }
                }

                _contextManager.Value.IsUiContextActive(cookie, out var active);
                var uiContext = new UiContext(contextGuid, cookie, active, isKnown);
                _registeredContexts[(int) cookie] = uiContext;
                return uiContext;
            }
        }

        public void SetContext(UiContext context)
        {
            _contextManager.Value.SetUiContext(context.Cookie, context.IsActive);
        }

        public int OnUiContextChanged(uint cookie, bool active)
        {
            var context = _registeredContexts[(int) cookie];
            if (context != DependencyProperty.UnsetValue)
                ((UiContext)context)?.OnActivated(active);
            return 0;
        }
    }

    internal struct FrugalMap
    {
        internal FrugalMapBase MapStore;

        public object this[int key]
        {
            get
            {
                if (MapStore != null)
                    return MapStore.Search(key);
                return DependencyProperty.UnsetValue;
            }
            set
            {
                if (value != DependencyProperty.UnsetValue)
                {
                    if (MapStore == null)
                        MapStore = new SingleObjectMap();
                    var state = MapStore.InsertEntry(key, value);
                    if (state == FrugalMapStoreState.Success)
                        return;
                    FrugalMapBase newMap;
                    if (FrugalMapStoreState.ThreeObjectMap == state)
                        newMap = new ThreeObjectMap();
                    else if (FrugalMapStoreState.SixObjectMap == state)
                        newMap = new SixObjectMap();
                    else if (FrugalMapStoreState.Array == state)
                        newMap = new ArrayObjectMap();
                    else if (FrugalMapStoreState.SortedArray == state)
                    {
                        newMap = new SortedObjectMap();
                    }
                    else
                    {
                        if (FrugalMapStoreState.Hashtable != state)
                            throw new InvalidOperationException();
                        newMap = new HashObjectMap();
                    }
                    MapStore.Promote(newMap);
                    MapStore = newMap;
                    int num = (int)MapStore.InsertEntry(key, value);
                }
                else
                {
                    if (MapStore == null)
                        return;
                    MapStore.RemoveEntry(key);
                    if (MapStore.Count != 0)
                        return;
                    MapStore = null;
                }
            }
        }

        public void Sort()
        {
            MapStore?.Sort();
        }

        public void GetKeyValuePair(int index, out int key, out object value)
        {
            if (MapStore == null)
                throw new ArgumentOutOfRangeException(nameof(index));
            MapStore.GetKeyValuePair(index, out key, out value);
        }

        public void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            MapStore?.Iterate(list, callback);
        }

        public int Count => MapStore?.Count ?? 0;
    }

    internal sealed class SingleObjectMap : FrugalMapBase
    {
        private Entry _loneEntry;

        public SingleObjectMap()
        {
            _loneEntry.Key = int.MaxValue;
            _loneEntry.Value = DependencyProperty.UnsetValue;
        }


        public override FrugalMapStoreState InsertEntry(int key, object value)
        {
            if (int.MaxValue != _loneEntry.Key && key != _loneEntry.Key)
                return FrugalMapStoreState.ThreeObjectMap;
            _loneEntry.Key = key;
            _loneEntry.Value = value;
            return FrugalMapStoreState.Success;
        }

        public override void RemoveEntry(int key)
        {
            if (key != _loneEntry.Key)
                return;
            _loneEntry.Key = int.MaxValue;
            _loneEntry.Value = DependencyProperty.UnsetValue;
        }

        public override object Search(int key)
        {
            if (key == _loneEntry.Key)
                return _loneEntry.Value;
            return DependencyProperty.UnsetValue;
        }

        public override void Sort()
        {
        }

        public override void GetKeyValuePair(int index, out int key, out object value)
        {
            if (index == 0)
            {
                value = _loneEntry.Value;
                key = _loneEntry.Key;
            }
            else
            {
                value = DependencyProperty.UnsetValue;
                key = int.MaxValue;
                throw new ArgumentOutOfRangeException();
            }
        }

        public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        {
            if (Count != 1)
                return;
            callback(list, _loneEntry.Key, _loneEntry.Value);
        }

        public override void Promote(FrugalMapBase newMap)
        {
            if (newMap.InsertEntry(_loneEntry.Key, _loneEntry.Value) != FrugalMapStoreState.Success)
                throw new ArgumentException(nameof(newMap));
        }

        public override int Count => int.MaxValue != _loneEntry.Key ? 1 : 0;
    }

    internal abstract class FrugalMapBase
    {
        public abstract FrugalMapStoreState InsertEntry(int key, object value);

        public abstract void RemoveEntry(int key);

        public abstract object Search(int key);

        public abstract void Sort();

        public abstract void GetKeyValuePair(int index, out int key, out object value);

        public abstract void Iterate(ArrayList list, FrugalMapIterationCallback callback);

        public abstract void Promote(FrugalMapBase newMap);

        public abstract int Count { get; }

        internal struct Entry
        {
            public int Key;
            public object Value;
        }
    }

    internal enum FrugalMapStoreState
    {
        Success,
        ThreeObjectMap,
        SixObjectMap,
        Array,
        SortedArray,
        Hashtable,
    }

    internal delegate void FrugalMapIterationCallback(ArrayList list, int key, object value);

    internal sealed class ThreeObjectMap : FrugalMapBase
    {
        private const int SIZE = 3;
        private ushort _count;
        private bool _sorted;
        private Entry _entry0;
        private Entry _entry1;
        private Entry _entry2;

        public override FrugalMapStoreState InsertEntry(int key, object value)
        {
            switch (_count)
            {
                case 1:
                    if (_entry0.Key == key)
                    {
                        _entry0.Value = value;
                        return FrugalMapStoreState.Success;
                    }
                    break;
                case 2:
                    if (_entry0.Key == key)
                    {
                        _entry0.Value = value;
                        return FrugalMapStoreState.Success;
                    }
                    if (_entry1.Key == key)
                    {
                        _entry1.Value = value;
                        return FrugalMapStoreState.Success;
                    }
                    break;
                case 3:
                    if (_entry0.Key == key)
                    {
                        _entry0.Value = value;
                        return FrugalMapStoreState.Success;
                    }
                    if (_entry1.Key == key)
                    {
                        _entry1.Value = value;
                        return FrugalMapStoreState.Success;
                    }
                    if (_entry2.Key == key)
                    {
                        _entry2.Value = value;
                        return FrugalMapStoreState.Success;
                    }
                    break;
            }
            if (3 <= _count)
                return FrugalMapStoreState.SixObjectMap;
            switch (_count)
            {
                case 0:
                    _entry0.Key = key;
                    _entry0.Value = value;
                    _sorted = true;
                    break;
                case 1:
                    _entry1.Key = key;
                    _entry1.Value = value;
                    _sorted = false;
                    break;
                case 2:
                    _entry2.Key = key;
                    _entry2.Value = value;
                    _sorted = false;
                    break;
            }
            ++_count;
            return FrugalMapStoreState.Success;
        }

        public override void RemoveEntry(int key)
        {
            switch (_count)
            {
                case 1:
                    if (_entry0.Key != key)
                        break;
                    _entry0.Key = int.MaxValue;
                    _entry0.Value = DependencyProperty.UnsetValue;
                    --_count;
                    break;
                case 2:
                    if (_entry0.Key == key)
                    {
                        _entry0 = _entry1;
                        _entry1.Key = int.MaxValue;
                        _entry1.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry1.Key != key)
                        break;
                    _entry1.Key = int.MaxValue;
                    _entry1.Value = DependencyProperty.UnsetValue;
                    --_count;
                    break;
                case 3:
                    if (_entry0.Key == key)
                    {
                        _entry0 = _entry1;
                        _entry1 = _entry2;
                        _entry2.Key = int.MaxValue;
                        _entry2.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry1.Key == key)
                    {
                        _entry1 = _entry2;
                        _entry2.Key = int.MaxValue;
                        _entry2.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry2.Key != key)
                        break;
                    _entry2.Key = int.MaxValue;
                    _entry2.Value = DependencyProperty.UnsetValue;
                    --_count;
                    break;
            }
        }

        public override object Search(int key)
        {
            if (_count > 0)
            {
                if (_entry0.Key == key)
                    return _entry0.Value;
                if (_count > 1)
                {
                    if (_entry1.Key == key)
                        return _entry1.Value;
                    if (_count > 2 && _entry2.Key == key)
                        return _entry2.Value;
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public override void Sort()
        {
            if (_sorted || _count <= 1)
                return;
            if (_entry0.Key > _entry1.Key)
            {
                Entry entry0 = _entry0;
                _entry0 = _entry1;
                _entry1 = entry0;
            }
            if (_count > 2 && _entry1.Key > _entry2.Key)
            {
                Entry entry1 = _entry1;
                _entry1 = _entry2;
                _entry2 = entry1;
                if (_entry0.Key > _entry1.Key)
                {
                    Entry entry0 = _entry0;
                    _entry0 = _entry1;
                    _entry1 = entry0;
                }
            }
            _sorted = true;
        }

        public override void GetKeyValuePair(int index, out int key, out object value)
        {
            if (index < _count)
            {
                switch (index)
                {
                    case 0:
                        key = _entry0.Key;
                        value = _entry0.Value;
                        break;
                    case 1:
                        key = _entry1.Key;
                        value = _entry1.Value;
                        break;
                    case 2:
                        key = _entry2.Key;
                        value = _entry2.Value;
                        break;
                    default:
                        key = int.MaxValue;
                        value = DependencyProperty.UnsetValue;
                        break;
                }
            }
            else
            {
                key = int.MaxValue;
                value = DependencyProperty.UnsetValue;
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        {
            if (_count <= 0)
                return;
            if (_count >= 1)
                callback(list, _entry0.Key, _entry0.Value);
            if (_count >= 2)
                callback(list, _entry1.Key, _entry1.Value);
            if (_count != 3)
                return;
            callback(list, _entry2.Key, _entry2.Value);
        }

        public override void Promote(FrugalMapBase newMap)
        {
            if (newMap.InsertEntry(_entry0.Key, _entry0.Value) != FrugalMapStoreState.Success)
                throw new ArgumentException(nameof(newMap));
            if (newMap.InsertEntry(_entry1.Key, _entry1.Value) != FrugalMapStoreState.Success)
                throw new ArgumentException(nameof(newMap));
            if (newMap.InsertEntry(_entry2.Key, _entry2.Value) != FrugalMapStoreState.Success)
                throw new ArgumentException(nameof(newMap));
        }

        public override int Count => _count;
    }

    internal sealed class SixObjectMap : FrugalMapBase
    {
        private const int SIZE = 6;
        private ushort _count;
        private bool _sorted;
        private Entry _entry0;
        private Entry _entry1;
        private Entry _entry2;
        private Entry _entry3;
        private Entry _entry4;
        private Entry _entry5;

        public override FrugalMapStoreState InsertEntry(int key, object value)
        {
            if (_count > 0)
            {
                if (_entry0.Key == key)
                {
                    _entry0.Value = value;
                    return FrugalMapStoreState.Success;
                }
                if (_count > 1)
                {
                    if (_entry1.Key == key)
                    {
                        _entry1.Value = value;
                        return FrugalMapStoreState.Success;
                    }
                    if (_count > 2)
                    {
                        if (_entry2.Key == key)
                        {
                            _entry2.Value = value;
                            return FrugalMapStoreState.Success;
                        }
                        if (_count > 3)
                        {
                            if (_entry3.Key == key)
                            {
                                _entry3.Value = value;
                                return FrugalMapStoreState.Success;
                            }
                            if (_count > 4)
                            {
                                if (_entry4.Key == key)
                                {
                                    _entry4.Value = value;
                                    return FrugalMapStoreState.Success;
                                }
                                if (_count > 5 && _entry5.Key == key)
                                {
                                    _entry5.Value = value;
                                    return FrugalMapStoreState.Success;
                                }
                            }
                        }
                    }
                }
            }
            if (6 <= _count)
                return FrugalMapStoreState.Array;
            _sorted = false;
            switch (_count)
            {
                case 0:
                    _entry0.Key = key;
                    _entry0.Value = value;
                    _sorted = true;
                    break;
                case 1:
                    _entry1.Key = key;
                    _entry1.Value = value;
                    break;
                case 2:
                    _entry2.Key = key;
                    _entry2.Value = value;
                    break;
                case 3:
                    _entry3.Key = key;
                    _entry3.Value = value;
                    break;
                case 4:
                    _entry4.Key = key;
                    _entry4.Value = value;
                    break;
                case 5:
                    _entry5.Key = key;
                    _entry5.Value = value;
                    break;
            }
            ++_count;
            return FrugalMapStoreState.Success;
        }

        public override void RemoveEntry(int key)
        {
            switch (_count)
            {
                case 1:
                    if (_entry0.Key != key)
                        break;
                    _entry0.Key = int.MaxValue;
                    _entry0.Value = DependencyProperty.UnsetValue;
                    --_count;
                    break;
                case 2:
                    if (_entry0.Key == key)
                    {
                        _entry0 = _entry1;
                        _entry1.Key = int.MaxValue;
                        _entry1.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry1.Key != key)
                        break;
                    _entry1.Key = int.MaxValue;
                    _entry1.Value = DependencyProperty.UnsetValue;
                    --_count;
                    break;
                case 3:
                    if (_entry0.Key == key)
                    {
                        _entry0 = _entry1;
                        _entry1 = _entry2;
                        _entry2.Key = int.MaxValue;
                        _entry2.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry1.Key == key)
                    {
                        _entry1 = _entry2;
                        _entry2.Key = int.MaxValue;
                        _entry2.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry2.Key != key)
                        break;
                    _entry2.Key = int.MaxValue;
                    _entry2.Value = DependencyProperty.UnsetValue;
                    --_count;
                    break;
                case 4:
                    if (_entry0.Key == key)
                    {
                        _entry0 = _entry1;
                        _entry1 = _entry2;
                        _entry2 = _entry3;
                        _entry3.Key = int.MaxValue;
                        _entry3.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry1.Key == key)
                    {
                        _entry1 = _entry2;
                        _entry2 = _entry3;
                        _entry3.Key = int.MaxValue;
                        _entry3.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry2.Key == key)
                    {
                        _entry2 = _entry3;
                        _entry3.Key = int.MaxValue;
                        _entry3.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry3.Key != key)
                        break;
                    _entry3.Key = int.MaxValue;
                    _entry3.Value = DependencyProperty.UnsetValue;
                    --_count;
                    break;
                case 5:
                    if (_entry0.Key == key)
                    {
                        _entry0 = _entry1;
                        _entry1 = _entry2;
                        _entry2 = _entry3;
                        _entry3 = _entry4;
                        _entry4.Key = int.MaxValue;
                        _entry4.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry1.Key == key)
                    {
                        _entry1 = _entry2;
                        _entry2 = _entry3;
                        _entry3 = _entry4;
                        _entry4.Key = int.MaxValue;
                        _entry4.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry2.Key == key)
                    {
                        _entry2 = _entry3;
                        _entry3 = _entry4;
                        _entry4.Key = int.MaxValue;
                        _entry4.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry3.Key == key)
                    {
                        _entry3 = _entry4;
                        _entry4.Key = int.MaxValue;
                        _entry4.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry4.Key != key)
                        break;
                    _entry4.Key = int.MaxValue;
                    _entry4.Value = DependencyProperty.UnsetValue;
                    --_count;
                    break;
                case 6:
                    if (_entry0.Key == key)
                    {
                        _entry0 = _entry1;
                        _entry1 = _entry2;
                        _entry2 = _entry3;
                        _entry3 = _entry4;
                        _entry4 = _entry5;
                        _entry5.Key = int.MaxValue;
                        _entry5.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry1.Key == key)
                    {
                        _entry1 = _entry2;
                        _entry2 = _entry3;
                        _entry3 = _entry4;
                        _entry4 = _entry5;
                        _entry5.Key = int.MaxValue;
                        _entry5.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry2.Key == key)
                    {
                        _entry2 = _entry3;
                        _entry3 = _entry4;
                        _entry4 = _entry5;
                        _entry5.Key = int.MaxValue;
                        _entry5.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry3.Key == key)
                    {
                        _entry3 = _entry4;
                        _entry4 = _entry5;
                        _entry5.Key = int.MaxValue;
                        _entry5.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry4.Key == key)
                    {
                        _entry4 = _entry5;
                        _entry5.Key = int.MaxValue;
                        _entry5.Value = DependencyProperty.UnsetValue;
                        --_count;
                        break;
                    }
                    if (_entry5.Key != key)
                        break;
                    _entry5.Key = int.MaxValue;
                    _entry5.Value = DependencyProperty.UnsetValue;
                    --_count;
                    break;
            }
        }

        public override object Search(int key)
        {
            if (_count > 0)
            {
                if (_entry0.Key == key)
                    return _entry0.Value;
                if (_count > 1)
                {
                    if (_entry1.Key == key)
                        return _entry1.Value;
                    if (_count > 2)
                    {
                        if (_entry2.Key == key)
                            return _entry2.Value;
                        if (_count > 3)
                        {
                            if (_entry3.Key == key)
                                return _entry3.Value;
                            if (_count > 4)
                            {
                                if (_entry4.Key == key)
                                    return _entry4.Value;
                                if (_count > 5 && _entry5.Key == key)
                                    return _entry5.Value;
                            }
                        }
                    }
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public override void Sort()
        {
            if (_sorted || _count <= 1)
                return;
            bool flag;
            do
            {
                flag = false;
                if (_entry0.Key > _entry1.Key)
                {
                    Entry entry0 = _entry0;
                    _entry0 = _entry1;
                    _entry1 = entry0;
                    flag = true;
                }
                if (_count > 2)
                {
                    if (_entry1.Key > _entry2.Key)
                    {
                        Entry entry1 = _entry1;
                        _entry1 = _entry2;
                        _entry2 = entry1;
                        flag = true;
                    }
                    if (_count > 3)
                    {
                        if (_entry2.Key > _entry3.Key)
                        {
                            Entry entry2 = _entry2;
                            _entry2 = _entry3;
                            _entry3 = entry2;
                            flag = true;
                        }
                        if (_count > 4)
                        {
                            if (_entry3.Key > _entry4.Key)
                            {
                                Entry entry3 = _entry3;
                                _entry3 = _entry4;
                                _entry4 = entry3;
                                flag = true;
                            }
                            if (_count > 5 && _entry4.Key > _entry5.Key)
                            {
                                Entry entry4 = _entry4;
                                _entry4 = _entry5;
                                _entry5 = entry4;
                                flag = true;
                            }
                        }
                    }
                }
            }
            while (flag);
            _sorted = true;
        }

        public override void GetKeyValuePair(int index, out int key, out object value)
        {
            if (index < _count)
            {
                switch (index)
                {
                    case 0:
                        key = _entry0.Key;
                        value = _entry0.Value;
                        break;
                    case 1:
                        key = _entry1.Key;
                        value = _entry1.Value;
                        break;
                    case 2:
                        key = _entry2.Key;
                        value = _entry2.Value;
                        break;
                    case 3:
                        key = _entry3.Key;
                        value = _entry3.Value;
                        break;
                    case 4:
                        key = _entry4.Key;
                        value = _entry4.Value;
                        break;
                    case 5:
                        key = _entry5.Key;
                        value = _entry5.Value;
                        break;
                    default:
                        key = int.MaxValue;
                        value = DependencyProperty.UnsetValue;
                        break;
                }
            }
            else
            {
                key = int.MaxValue;
                value = DependencyProperty.UnsetValue;
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        {
            if (_count <= 0)
                return;
            if (_count >= 1)
                callback(list, _entry0.Key, _entry0.Value);
            if (_count >= 2)
                callback(list, _entry1.Key, _entry1.Value);
            if (_count >= 3)
                callback(list, _entry2.Key, _entry2.Value);
            if (_count >= 4)
                callback(list, _entry3.Key, _entry3.Value);
            if (_count >= 5)
                callback(list, _entry4.Key, _entry4.Value);
            if (_count != 6)
                return;
            callback(list, _entry5.Key, _entry5.Value);
        }

        public override void Promote(FrugalMapBase newMap)
        {
            if (newMap.InsertEntry(_entry0.Key, _entry0.Value) != FrugalMapStoreState.Success)
                throw new ArgumentException(nameof(newMap));
            if (newMap.InsertEntry(_entry1.Key, _entry1.Value) != FrugalMapStoreState.Success)
                throw new ArgumentException(nameof(newMap));
            if (newMap.InsertEntry(_entry2.Key, _entry2.Value) != FrugalMapStoreState.Success)
                throw new ArgumentException(nameof(newMap));
            if (newMap.InsertEntry(_entry3.Key, _entry3.Value) != FrugalMapStoreState.Success)
                throw new ArgumentException(nameof(newMap));
            if (newMap.InsertEntry(_entry4.Key, _entry4.Value) != FrugalMapStoreState.Success)
                throw new ArgumentException(nameof(newMap));
            if (newMap.InsertEntry(_entry5.Key, _entry5.Value) != FrugalMapStoreState.Success)
                throw new ArgumentException(nameof(newMap));
        }

        public override int Count => _count;
    }

    internal sealed class ArrayObjectMap : FrugalMapBase
    {
        private const int MINSIZE = 9;
        private const int MAXSIZE = 15;
        private const int GROWTH = 3;
        private ushort _count;
        private bool _sorted;
        private Entry[] _entries;

        public override FrugalMapStoreState InsertEntry(int key, object value)
        {
            for (int index = 0; index < (int)_count; ++index)
            {
                if (_entries[index].Key == key)
                {
                    _entries[index].Value = value;
                    return FrugalMapStoreState.Success;
                }
            }
            if (15 <= _count)
                return FrugalMapStoreState.SortedArray;
            if (_entries != null)
            {
                _sorted = false;
                if (_entries.Length <= _count)
                {
                    Entry[] entryArray = new Entry[_entries.Length + 3];
                    Array.Copy(_entries, 0, entryArray, 0, _entries.Length);
                    _entries = entryArray;
                }
            }
            else
            {
                _entries = new Entry[9];
                _sorted = true;
            }
            _entries[_count].Key = key;
            _entries[_count].Value = value;
            ++_count;
            return FrugalMapStoreState.Success;
        }

        public override void RemoveEntry(int key)
        {
            for (int destinationIndex = 0; destinationIndex < (int)_count; ++destinationIndex)
            {
                if (_entries[destinationIndex].Key == key)
                {
                    int length = _count - destinationIndex - 1;
                    if (length > 0)
                        Array.Copy(_entries, destinationIndex + 1, _entries, destinationIndex, length);
                    _entries[_count - 1].Key = int.MaxValue;
                    _entries[_count - 1].Value = DependencyProperty.UnsetValue;
                    --_count;
                    break;
                }
            }
        }

        public override object Search(int key)
        {
            for (int index = 0; index < (int)_count; ++index)
            {
                if (key == _entries[index].Key)
                    return _entries[index].Value;
            }
            return DependencyProperty.UnsetValue;
        }

        public override void Sort()
        {
            if (_sorted || _count <= 1)
                return;
            QSort(0, _count - 1);
            _sorted = true;
        }

        public override void GetKeyValuePair(int index, out int key, out object value)
        {
            if (index < _count)
            {
                value = _entries[index].Value;
                key = _entries[index].Key;
            }
            else
            {
                value = DependencyProperty.UnsetValue;
                key = int.MaxValue;
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        {
            if (_count <= 0)
                return;
            for (int index = 0; index < (int)_count; ++index)
                callback(list, _entries[index].Key, _entries[index].Value);
        }

        public override void Promote(FrugalMapBase newMap)
        {
            for (int index = 0; index < _entries.Length; ++index)
            {
                if (newMap.InsertEntry(_entries[index].Key, _entries[index].Value) != FrugalMapStoreState.Success)
                    throw new ArgumentException(nameof(newMap));
            }
        }

        public override int Count => _count;

        private int Compare(int left, int right)
        {
            return _entries[left].Key - _entries[right].Key;
        }

        private int Partition(int left, int right)
        {
            int num = right;
            int index1 = left - 1;
            int index2 = right;
            while (true)
            {
                do
                {
                } while (Compare(++index1, num) < 0);
                do
                    ;
                while (Compare(num, --index2) < 0 && index2 != left);
                if (index1 < index2)
                {
                    Entry entry = _entries[index2];
                    _entries[index2] = _entries[index1];
                    _entries[index1] = entry;
                }
                else
                    break;
            }
            Entry entry1 = _entries[right];
            _entries[right] = _entries[index1];
            _entries[index1] = entry1;
            return index1;
        }

        private void QSort(int left, int right)
        {
            if (left >= right)
                return;
            int num = Partition(left, right);
            QSort(left, num - 1);
            QSort(num + 1, right);
        }
    }

    internal sealed class SortedObjectMap : FrugalMapBase
    {
        private int _lastKey = int.MaxValue;
        internal int _count;
        private Entry[] _entries;

        public override FrugalMapStoreState InsertEntry(int key, object value)
        {
            int insertIndex = FindInsertIndex(key, out var found);
            if (found)
            {
                _entries[insertIndex].Value = value;
                return FrugalMapStoreState.Success;
            }
            if (128 <= _count)
                return FrugalMapStoreState.Hashtable;
            if (_entries != null)
            {
                if (_entries.Length <= _count)
                {
                    Entry[] entryArray = new Entry[_entries.Length + 8];
                    Array.Copy(_entries, 0, entryArray, 0, _entries.Length);
                    _entries = entryArray;
                }
            }
            else
                _entries = new Entry[16];
            if (insertIndex < _count)
                Array.Copy(_entries, insertIndex, _entries, insertIndex + 1, _count - insertIndex);
            else
                _lastKey = key;
            _entries[insertIndex].Key = key;
            _entries[insertIndex].Value = value;
            ++_count;
            return FrugalMapStoreState.Success;
        }

        public override void RemoveEntry(int key)
        {
            int insertIndex = FindInsertIndex(key, out var found);
            if (!found)
                return;
            int length = _count - insertIndex - 1;
            if (length > 0)
                Array.Copy(_entries, insertIndex + 1, _entries, insertIndex, length);
            else
                _lastKey = _count <= 1 ? int.MaxValue : _entries[_count - 2].Key;
            _entries[_count - 1].Key = int.MaxValue;
            _entries[_count - 1].Value = DependencyProperty.UnsetValue;
            --_count;
        }

        public override object Search(int key)
        {
            int insertIndex = FindInsertIndex(key, out var found);
            if (found)
                return _entries[insertIndex].Value;
            return DependencyProperty.UnsetValue;
        }

        public override void Sort()
        {
        }

        public override void GetKeyValuePair(int index, out int key, out object value)
        {
            if (index < _count)
            {
                value = _entries[index].Value;
                key = _entries[index].Key;
            }
            else
            {
                value = DependencyProperty.UnsetValue;
                key = int.MaxValue;
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        {
            if (_count <= 0)
                return;
            for (int index = 0; index < _count; ++index)
                callback(list, _entries[index].Key, _entries[index].Value);
        }

        public override void Promote(FrugalMapBase newMap)
        {
            for (int index = 0; index < _entries.Length; ++index)
            {
                if (newMap.InsertEntry(_entries[index].Key, _entries[index].Value) != FrugalMapStoreState.Success)
                    throw new ArgumentException(nameof(newMap));
            }
        }

        private int FindInsertIndex(int key, out bool found)
        {
            int index1 = 0;
            if (_count > 0 && key <= _lastKey)
            {
                int num = _count - 1;
                do
                {
                    int index2 = (num + index1) / 2;
                    if (key <= _entries[index2].Key)
                        num = index2;
                    else
                        index1 = index2 + 1;
                }
                while (index1 < num);
                found = key == _entries[index1].Key;
            }
            else
            {
                index1 = _count;
                found = false;
            }
            return index1;
        }

        public override int Count => _count;
    }

    internal sealed class HashObjectMap : FrugalMapBase
    {
        private static object NullValue = new object();
        internal const int MINSIZE = 163;
        internal Hashtable _entries;

        public override FrugalMapStoreState InsertEntry(int key, object value)
        {
            if (_entries == null)
                _entries = new Hashtable(163);
            _entries[key] = value == NullValue || value == null ? NullValue : value;
            return FrugalMapStoreState.Success;
        }

        public override void RemoveEntry(int key)
        {
            _entries.Remove(key);
        }

        public override object Search(int key)
        {
            object entry = _entries[key];
            if (entry == NullValue || entry == null)
                return DependencyProperty.UnsetValue;
            return entry;
        }

        public override void Sort()
        {
        }

        public override void GetKeyValuePair(int index, out int key, out object value)
        {
            if (index < _entries.Count)
            {
                IDictionaryEnumerator enumerator = _entries.GetEnumerator();
                enumerator.MoveNext();
                for (int index1 = 0; index1 < index; ++index1)
                    enumerator.MoveNext();
                key = (int)enumerator.Key;
                if (enumerator.Value != NullValue && enumerator.Value != null)
                    value = enumerator.Value;
                else
                    value = DependencyProperty.UnsetValue;
            }
            else
            {
                value = DependencyProperty.UnsetValue;
                key = int.MaxValue;
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        {
            IDictionaryEnumerator enumerator = _entries.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int key = (int)enumerator.Key;
                object obj = enumerator.Value == NullValue || enumerator.Value == null ? DependencyProperty.UnsetValue : enumerator.Value;
                callback(list, key, obj);
            }
        }

        public override void Promote(FrugalMapBase newMap)
        {
            throw new InvalidOperationException();
        }

        public override int Count => _entries.Count;
    }
}