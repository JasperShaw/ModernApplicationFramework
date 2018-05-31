using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    [Export(typeof(IToolboxItemStateCache))]
    internal class ToolboxItemStateCache : IToolboxItemStateCache
    {
        private readonly ToolboxItemsBuilder _builder;

        private readonly Dictionary<Type, IReadOnlyCollection<IToolboxCategory>> _store = new Dictionary<Type, IReadOnlyCollection<IToolboxCategory>>();

        [ImportingConstructor]
        public ToolboxItemStateCache(ToolboxItemsBuilder builder)
        {
            _builder = builder;
        }


        public IReadOnlyCollection<IToolboxCategory> GetToolboxItems(Type key)
        {
            if (!_store.TryGetValue(key, out var result))
            {
                result = _builder.Build(key);
                StoreToolboxItems(key, result);
            }
            return result;
        }

        public void StoreToolboxItems(Type key, IReadOnlyCollection<IToolboxCategory> items)
        {
            if (_store.ContainsKey(key))
                _store[key] = items;
            else
                _store.Add(key, items);
        }

        public IReadOnlyCollection<Type> GetKeys()
        {
            return _store.Keys;
        }
    }
}
