using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Storage;

namespace ModernApplicationFramework.Editor
{
    [Export(typeof(IDataStorageService))]
    internal sealed class DataStorageService : IDataStorageService
    {
        private Dictionary<string, IDataStorage> _dataStorageCache;

        public IDataStorage GetDataStorage(string storageKey)
        {
            if (string.IsNullOrEmpty(storageKey))
                throw new ArgumentNullException(nameof(storageKey));
            if (_dataStorageCache == null)
                _dataStorageCache = new Dictionary<string, IDataStorage>();
            if (!_dataStorageCache.ContainsKey(storageKey))
            {
                var dataStorage = new DataStorage(storageKey);
                _dataStorageCache.Add(storageKey, dataStorage);
            }
            return _dataStorageCache[storageKey];
        }
    }
}