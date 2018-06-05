using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxService
    {
        IReadOnlyCollection<IToolboxCategory> GetToolboxItemSource(Type layoutItemType);

        void StoreItemSource(Type layoutItemType, IReadOnlyCollection<IToolboxCategory> itemsSource);

        IReadOnlyCollection<string> GetAllToolboxCategoryNames();
    }
}
