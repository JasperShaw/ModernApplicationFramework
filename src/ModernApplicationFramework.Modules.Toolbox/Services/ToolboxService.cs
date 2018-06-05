using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Services
{
    [Export(typeof(IToolboxService))]
    public class ToolboxService : IToolboxService
    {
        private readonly IToolboxItemStateCache _stateCache;

        internal static IToolboxService Instance { get; private set; }

        [ImportingConstructor]
        internal  ToolboxService(IToolboxItemStateCache stateCache)
        {
            _stateCache = stateCache;
            Instance = this;
        }

        public IReadOnlyCollection<IToolboxCategory> GetToolboxItemSource(Type layoutItemType)
        {
            return _stateCache.GetToolboxItems(layoutItemType);
        }

        public void StoreItemSource(Type layoutItemType, IReadOnlyCollection<IToolboxCategory> itemsSource)
        {
            _stateCache.StoreToolboxItems(layoutItemType, itemsSource);
        }

        public IReadOnlyCollection<string> GetAllToolboxCategoryNames()
        {
            return new List<string>();
        }
    }
}
