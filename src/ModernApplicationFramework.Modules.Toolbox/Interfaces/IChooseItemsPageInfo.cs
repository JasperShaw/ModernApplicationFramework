using System;
using System.Collections.Generic;
using ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IChooseItemsPageInfo
    {
        IEnumerable<ColumnInformation> Columns { get; }

        Guid Id { get; }

        IItemDataFactory ItemFactory { get; }
        string Name { get; }

        int Order { get; }

        Predicate<ToolboxItemDefinitionBase> Selector { get; }

        bool ShowAssembly { get; }

        bool ShowVersion { get; }
    }
}