using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxService
    {
        IReadOnlyCollection<IToolboxCategory> GetToolboxItemSource();

        IReadOnlyCollection<string> GetAllToolboxCategoryNames();

        void StoreCurrentLayout();

        void StoreAndApplyLayout(IEnumerable<IToolboxCategory> layout);

        void AddCategory(IToolboxCategory category, bool supressRefresh = false);

        void InsertCategory(int index, IToolboxCategory category, bool supressRefresh = false);

        void RemoveCategory(IToolboxCategory category, bool cascading = true, bool supressRefresh = false);

        IToolboxCategory GetCategoryById(Guid guid);

        IToolboxItem GetItemById(Guid guid);
    }
}
