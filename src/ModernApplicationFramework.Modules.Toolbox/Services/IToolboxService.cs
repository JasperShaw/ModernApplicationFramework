using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Toolbox.Services
{
    public interface IToolboxService
    {
        IReadOnlyCollection<ToolboxItemCategory> GetToolboxItemSource(Type layoutItemType);
    }
}
