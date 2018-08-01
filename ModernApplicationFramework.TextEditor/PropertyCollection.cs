using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ModernApplicationFramework.TextEditor
{
    public class PropertyCollection
    {
        private readonly object _syncLock = new object();
        private HybridDictionary _properties;

        public void AddProperty(object key, object property)
        {
            lock (_syncLock)
            {
                if (_properties == null)
                    _properties = new HybridDictionary();
                _properties.Add(key, property);
            }
        }

        public bool RemoveProperty(object key)
        {
            lock (_syncLock)
            {
                if (_properties == null || !_properties.Contains(key))
                    return false;
                _properties.Remove(key);
                return true;
            }
        }

        public T GetOrCreateSingletonProperty<T>(object key, Func<T> creator) where T : class
        {
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));
            lock (_syncLock)
            {
                if (_properties == null)
                    _properties = new HybridDictionary();
                else if (_properties.Contains(key))
                    return (T)_properties[key];
                var obj = creator();
                if (_properties.Contains(key))
                    obj = (T)_properties[key];
                else
                    _properties.Add(key, obj);
                return obj;
            }
        }

        public T GetOrCreateSingletonProperty<T>(Func<T> creator) where T : class
        {
            return GetOrCreateSingletonProperty(typeof(T), creator);
        }

        public TProperty GetProperty<TProperty>(object key)
        {
            return (TProperty)GetProperty(key);
        }

        public object GetProperty(object key)
        {
            lock (_syncLock)
            {
                if (_properties == null)
                    throw new KeyNotFoundException(nameof(key));
                var property = _properties[key];
                if (property == null && !_properties.Contains(key))
                    throw new KeyNotFoundException(nameof(key));
                return property;
            }
        }

        public bool TryGetProperty<TProperty>(object key, out TProperty property)
        {
            lock (_syncLock)
            {
                if (_properties != null)
                {
                    var property1 = _properties[key];
                    if (property1 == null)
                    {
                        if (!_properties.Contains(key))
                        {
                            property = default;
                            return false;
                        }
                    }
                    property = (TProperty)property1;
                    return true;
                }
            }
            property = default;
            return false;
        }

        public bool ContainsProperty(object key)
        {
            lock (_syncLock)
                return _properties != null && _properties.Contains(key);
        }

        public object this[object key]
        {
            get => GetProperty(key);
            set => SetProperty(key, value);
        }

        public ReadOnlyCollection<KeyValuePair<object, object>> PropertyList
        {
            get
            {
                var keyValuePairList = new List<KeyValuePair<object, object>>();
                lock (_syncLock)
                {
                    if (_properties != null)
                    {
                        foreach (DictionaryEntry property in _properties)
                            keyValuePairList.Add(new KeyValuePair<object, object>(property.Key, property.Value));
                    }
                }
                return keyValuePairList.AsReadOnly();
            }
        }

        private void SetProperty(object key, object property)
        {
            lock (_syncLock)
            {
                if (_properties == null)
                    _properties = new HybridDictionary();
                _properties[key] = property;
            }
        }
    }
}