using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxService
    {
        IReadOnlyCollection<IToolboxCategory> GetToolboxItemSource();

        void StoreItemsSource(IEnumerable<IToolboxCategory> itemsSource);

        IReadOnlyCollection<string> GetAllToolboxCategoryNames();
    }
}
