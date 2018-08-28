using System;
using System.Collections.Generic;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.CommandBar.Models
{
    public sealed class MenuControllerModel
    {
        public IReadOnlyCollection<KeyValuePair<CommandBarItemDefinition, bool>> Items;

        public MenuControllerModel(IEnumerable<MenuControllerModelItem> definitionTypes)
        {
            var hashSet = new HashSet<MenuControllerModelItem>(definitionTypes);
            var items = new List<KeyValuePair<CommandBarItemDefinition, bool>>();
            var handler = IoC.Get<ICommandBarItemService>();
            foreach (var entry in hashSet)
            {
                var command = handler.GetItemDefinition(entry.Type);
                if (command != null)
                    items.Add(new KeyValuePair<CommandBarItemDefinition, bool>(command, entry.IsFixed));
            }
            Items = items;
        }

        public struct MenuControllerModelItem
        {
            public Type Type { get; }

            public bool IsFixed { get; }

            public MenuControllerModelItem(Type type) : this(type, false)
            {
            }

            public MenuControllerModelItem(Type type, bool isFixed)
            {
                if (!type.IsSubclassOf(typeof(CommandBarItemDefinition)))
                    throw new ArgumentException();
                Type = type;
                IsFixed = isFixed;
            }
        }
    }
}
