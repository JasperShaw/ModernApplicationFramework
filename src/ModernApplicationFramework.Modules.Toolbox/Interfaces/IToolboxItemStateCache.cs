using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    internal interface IToolboxItemStateCache
    {
        IReadOnlyCollection<IToolboxCategory> GetState(Type key);

        IReadOnlyCollection<IToolboxCategory> GetDefaultAndCustomState();

        void StoreState(Type key, IReadOnlyCollection<IToolboxCategory> items);

        void StoreDefaultAndCustomState(IReadOnlyCollection<IToolboxCategory> itemsSource);

        IReadOnlyCollection<Type> GetKeys();
    }
}