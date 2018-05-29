using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Modules.Toolbox
{
    [Export(typeof(IToolboxItemStateCache))]
    internal class ToolboxItemStateCache : IToolboxItemStateCache
    {
        private readonly ToolboxItemsBuilder _builder;

        private readonly Dictionary<Type, IReadOnlyCollection<ToolboxItemCategory>> _store = new Dictionary<Type, IReadOnlyCollection<ToolboxItemCategory>>();

        [ImportingConstructor]
        public ToolboxItemStateCache(ToolboxItemsBuilder builder)
        {
            _builder = builder;
        }


        public IReadOnlyCollection<ToolboxItemCategory> GetToolboxItems(Type key)
        {
            if (!_store.TryGetValue(key, out var result))
            {
                result = _builder.Build(key);
                StoreToolboxItems(key, result);
            }
            return result;
        }

        public void StoreToolboxItems(Type key, IReadOnlyCollection<ToolboxItemCategory> items)
        {
            if (_store.ContainsKey(key))
                _store[key] = items;
            else
                _store.Add(key, items);
        }
    }

    internal interface IToolboxItemStateCache
    {
        IReadOnlyCollection<ToolboxItemCategory> GetToolboxItems(Type key);

        void StoreToolboxItems(Type key, IReadOnlyCollection<ToolboxItemCategory> items);
    }
}
