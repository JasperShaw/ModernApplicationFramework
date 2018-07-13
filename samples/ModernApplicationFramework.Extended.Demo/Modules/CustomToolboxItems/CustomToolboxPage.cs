using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Extended.Demo.Modules.CustomToolboxItems
{
    public sealed class CustomToolboxPage
    {
        [Export(typeof(IChooseItemsPageInfo))]
        internal class NotAllToolboxItemsPageInfo : IChooseItemsPageInfo
        {
            public string Name => "Not All Items";
            public Guid Id => new Guid("{3C4E8AAB-3192-4A3C-A6C2-F4D095813EED}");
            public int Order => 1;
            public bool ShowAssembly { get; } = true;
            public bool ShowVersion { get; } = true;

            public IEnumerable<ColumnInformation> Columns => new List<ColumnInformation>
            {
                new ColumnInformation("Name", "Name"),
                new ColumnInformation("Description", "Description")
            };

            public Predicate<ToolboxItemDefinitionBase> Selector => SelectorMethod;

            private static bool SelectorMethod(ToolboxItemDefinitionBase obj)
            {
                return obj is DescriptionToolboxItemDefinition;
            }

            public IItemDataFactory ItemFactory => DescriptionItemDataFactory.Default;
        }
    }
}
