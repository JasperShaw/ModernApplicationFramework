using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;
using ModernApplicationFramework.Modules.Toolbox.State;

namespace ModernApplicationFramework.Modules.Toolbox.Services
{
    [Export(typeof(IToolboxService))]
    public class ToolboxService : IToolboxService
    {
        private readonly IToolboxItemStateCache _stateCache;
        private readonly ToolboxItemHost _host;

        internal static IToolboxService Instance { get; private set; }

        [ImportingConstructor]
        internal  ToolboxService(IToolboxItemStateCache stateCache, ToolboxItemHost host)
        {
            _stateCache = stateCache;
            _host = host;
            Instance = this;
        }

        public IReadOnlyCollection<IToolboxCategory> GetToolboxItemSource(Type layoutItemType)
        {
            var state = _stateCache.GetState(layoutItemType).ToList();
            InjectDefaultCustomItems(ref state);
            return state;
        }

        public void StoreItemSource(Type layoutItemType, IReadOnlyCollection<IToolboxCategory> itemsSource)
        {
            _stateCache.StoreState(layoutItemType, itemsSource);
            _stateCache.StoreDefaultAndCustomState(itemsSource.Where(x => x.IsCustom || x == ToolboxItemCategory.DefaultCategory).ToList());
        }

        public IReadOnlyCollection<string> GetAllToolboxCategoryNames()
        {
            return new List<string>();
        }

        private void InjectDefaultCustomItems(ref List<IToolboxCategory> categories)
        {
            var startIndex = categories.FindIndex(x => x.IsCustom || x == ToolboxItemCategory.DefaultCategory);
            categories.RemoveAll(x => x.IsCustom || x == ToolboxItemCategory.DefaultCategory);
            categories.InsertRange(startIndex, _stateCache.GetDefaultAndCustomState());
        }
    }
}
