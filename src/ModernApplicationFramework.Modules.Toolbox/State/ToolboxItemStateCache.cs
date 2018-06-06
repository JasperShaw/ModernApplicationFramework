using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.State
{
    [Export(typeof(IToolboxItemStateCache))]
    internal class ToolboxItemStateCache : IToolboxItemStateCache
    {
        private readonly ToolboxItemsBuilder _builder;

        private readonly Dictionary<Type, IReadOnlyCollection<IToolboxCategory>> _store = new Dictionary<Type, IReadOnlyCollection<IToolboxCategory>>();

        private IEnumerable<IToolboxCategory> _defaultCustomState = new List<IToolboxCategory>{ ToolboxItemCategory.DefaultCategory };

        [ImportingConstructor]
        public ToolboxItemStateCache(ToolboxItemsBuilder builder)
        {
            _builder = builder;
        }


        public IReadOnlyCollection<IToolboxCategory> GetState(Type key)
        {
            if (!_store.TryGetValue(key, out var result))
            {
                result = _builder.Build(key);
                StoreState(key, result);
            }
            return result;
        }

        public IReadOnlyCollection<IToolboxCategory> GetDefaultAndCustomState()
        {
            return _defaultCustomState.ToList();
        }

        public void StoreState(Type key, IReadOnlyCollection<IToolboxCategory> items)
        {
            if (_store.ContainsKey(key))
                _store[key] = items;
            else
                _store.Add(key, items);
        }

        public void StoreDefaultAndCustomState(IReadOnlyCollection<IToolboxCategory> itemsSource)
        {
            _defaultCustomState = itemsSource;
        }

        public IReadOnlyCollection<Type> GetKeys()
        {
            return _store.Keys;
        }
    }
}
