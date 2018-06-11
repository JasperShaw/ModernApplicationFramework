using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxService
    {
        IReadOnlyCollection<IToolboxCategory> GetToolboxItemSource();

        IReadOnlyCollection<string> GetAllToolboxCategoryNames();

        void StoreItemsSource(IEnumerable<IToolboxCategory> itemsSource);
    }
}
