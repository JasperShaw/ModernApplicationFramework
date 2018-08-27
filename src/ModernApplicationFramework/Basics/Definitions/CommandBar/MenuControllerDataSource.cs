using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    internal class MenuControllerDataSource : CommandBarItemDataSource
    {
        private CommandBarItemDataSource _anchorItem;

        public CommandBarItemDataSource AnchorItem
        {
            get => _anchorItem;
            set
            {
                if (Equals(value, _anchorItem)) return;
                _anchorItem = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<CommandBarDataSource> Items { get; }

        public override CommandControlTypes UiType => CommandControlTypes.MenuController;

        public MenuControllerDataSource(Guid id, string text, uint sortOrder, CommandBarGroup group, CommandMenuControllerDefinition itemDefinition,
            bool isCustom = false, CommandBarFlags flgas = CommandBarFlags.CommandFlagNone)
            : base(text, sortOrder, group, itemDefinition, isCustom, flgas)
        {
            if (itemDefinition == null)
                throw new ArgumentNullException(nameof(itemDefinition));
            Id = id;
            var items = new List<CommandBarItemDataSource>();
            foreach (var valuePair in itemDefinition.Model.Items)
            {
                var item = CreateInstance(valuePair.Key) as CommandBarItemDataSource;
                if (item == null)
                    continue;
                if (valuePair.Value)
                    item.Flags.FixMenuController = true;
                items.Add(item);
            }
            Items = items;
            AnchorItem = items.FirstOrDefault();
        }

        public override Guid Id { get; }
    }
}