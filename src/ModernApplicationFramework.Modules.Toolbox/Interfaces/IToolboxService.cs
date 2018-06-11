using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxService
    {
        IReadOnlyCollection<IToolboxCategory> GetToolboxItemSource(Type layoutItemType);

        void StoreItemSource(Type layoutItemType, IReadOnlyCollection<IToolboxCategory> itemsSource);

        void StoreItemSource(Type layoutItemType, IEnumerable<IToolboxCategory> itemsSource);


        //void StoreItemSource(Type type, IEnumerable<Guid> categoriesToStore);


        IReadOnlyCollection<string> GetAllToolboxCategoryNames();
    }
}
