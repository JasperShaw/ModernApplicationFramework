using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    internal interface IToolboxItemStateCache
    {
        IReadOnlyCollection<IToolboxCategory> GetToolboxItems(Type key);

        void StoreToolboxItems(Type key, IReadOnlyCollection<IToolboxCategory> items);
    }
}