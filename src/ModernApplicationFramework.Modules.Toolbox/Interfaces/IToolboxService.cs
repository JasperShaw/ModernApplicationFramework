using System;
using System.Collections.Generic;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxService
    {
        IReadOnlyCollection<IToolboxCategory> GetToolboxItemSource();

        IReadOnlyCollection<string> GetAllToolboxCategoryNames();

        void StoreCurrentLayout();

        void StoreAndApplyLayout(IEnumerable<IToolboxCategory> layout);

        void AddCategory(IToolboxCategory category, bool suppressRefresh = false);

        void InsertCategory(int index, IToolboxCategory category, bool supressRefresh = false);

        void RemoveCategory(IToolboxCategory category, bool cascading = true, bool supressRefresh = false);

        IToolboxCategory GetCategoryById(Guid guid);

        IToolboxItem GetItemById(Guid guid);

        IEnumerable<IToolboxItem> FindItemsByDefintion(ToolboxItemDefinitionBase definition);

        bool ToolboxHasItem(ToolboxItemDefinitionBase definition);

        IToolboxCategory GetSelectedCategory();
    }
}
