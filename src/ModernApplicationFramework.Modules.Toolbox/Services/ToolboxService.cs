using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Modules.Toolbox.Services
{
    [Export(typeof(IToolboxService))]
    public class ToolboxService : IToolboxService
    {
        private readonly IToolboxItemStateCache _stateCache;

        [ImportingConstructor]
        internal  ToolboxService(IToolboxItemStateCache stateCache)
        {
            _stateCache = stateCache;
        }

        public IReadOnlyCollection<ToolboxItemCategory> GetToolboxItemSource(Type layoutItemType)
        {
            return _stateCache.GetToolboxItems(layoutItemType);
        }

        public void StoreItemSource(Type layoutItemType, IReadOnlyCollection<ToolboxItemCategory> itemsSource)
        {
            _stateCache.StoreToolboxItems(layoutItemType, itemsSource);
        }
    }
}
