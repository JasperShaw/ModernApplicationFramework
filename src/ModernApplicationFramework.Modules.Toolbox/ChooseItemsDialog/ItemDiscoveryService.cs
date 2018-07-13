using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    [Export(typeof(ItemDiscoveryService))]
    internal class ItemDiscoveryService
    {
        private static ItemDiscoveryService _instance;

        private readonly List<ItemType> _itemTypes = new List<ItemType>();

        public static ItemDiscoveryService Instance => _instance ?? (_instance = IoC.Get<ItemDiscoveryService>());

        public IEnumerable<ItemType> ItemTypes => _itemTypes;

        [ImportingConstructor]
        private ItemDiscoveryService([ImportMany] IEnumerable<IChooseItemsPageInfo> pageInfos)
        {
            foreach (var pageInfo in pageInfos)
                RegisterItemDiscovery(pageInfo);
        }

        private void RegisterItemDiscovery(IChooseItemsPageInfo pageInfo)
        {
            _itemTypes.Add(new ItemType(pageInfo.Id, pageInfo.Name, pageInfo.Order, pageInfo.Columns, pageInfo.Selector,
                pageInfo.ItemFactory));
        }


        internal class ItemType
        {
            public Guid Guid { get; }

            public IItemDataFactory ItemFactory { get; }

            public IEnumerable<ColumnInformation> ListColumns { get; }
            public string Name { get; }

            public int Order { get; }

            public Predicate<ToolboxItemDefinitionBase> Selector { get; }

            public ItemType(Guid guid, string name, int order, IEnumerable<ColumnInformation> listColumns,
                Predicate<ToolboxItemDefinitionBase> selector, IItemDataFactory factory)
            {
                Guid = guid;
                Name = name;
                Order = order;
                ListColumns = listColumns;
                Selector = selector;
                ItemFactory = factory;
            }
        }
    }
}