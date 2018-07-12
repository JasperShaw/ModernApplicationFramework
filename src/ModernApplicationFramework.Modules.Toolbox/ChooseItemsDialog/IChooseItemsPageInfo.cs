using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    public interface IChooseItemsPageInfo
    {
        string Name { get; }

        Guid Id { get; }

        int Order { get; }

        IEnumerable<ColumnInformation> Columns { get; }

        Predicate<ToolboxItemDefinitionBase> Selector { get; }

        IItemDataFactory ItemFactory { get; }
    }

    [Export(typeof(IChooseItemsPageInfo))]
    internal class AllToolboxItemsPageInfo : IChooseItemsPageInfo
    {
        public string Name => "All Items";
        public Guid Id => new Guid("{EFC137BE-4E39-4A7E-8D6E-7310B9CA40B9}");
        public int Order => 0;
        public IEnumerable<ColumnInformation> Columns => new List<ColumnInformation>
        {
            new ColumnInformation("Name", "Header")
        };

        public Predicate<ToolboxItemDefinitionBase> Selector => definition => true;
        public IItemDataFactory ItemFactory => DefaultItemDataFactory.Default;
    }

    [Export(typeof(IChooseItemsPageInfo))]
    internal class NotAllToolboxItemsPageInfo : IChooseItemsPageInfo
    {
        public string Name => "Not All Items";
        public Guid Id => new Guid("{3C4E8AAB-3192-4A3C-A6C2-F4D095813EED}");
        public int Order => 1;
        public IEnumerable<ColumnInformation> Columns => new List<ColumnInformation>
        {
            new ColumnInformation("Name", "Header 1"),
            new ColumnInformation("Name 2", "Header 2")
        };

        public Predicate<ToolboxItemDefinitionBase> Selector => definition => false;

        public IItemDataFactory ItemFactory => DefaultItemDataFactory.Default;
    }
}
