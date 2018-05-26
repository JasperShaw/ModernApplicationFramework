using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace ModernApplicationFramework.Modules.Toolbox.Services
{
    [Export(typeof(IToolboxService))]
    public class ToolboxService : IToolboxService
    {
        public IReadOnlyCollection<ToolboxItemCategory> GetToolboxItemSource(Type layoutItemType)
        {
            return IoC.Get<ToolboxItemsBuilder>().Build(layoutItemType);
        }
    }
}
