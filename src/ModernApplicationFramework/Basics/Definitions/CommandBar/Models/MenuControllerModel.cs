using System;
using System.Collections.Generic;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar.Models
{
    public sealed class MenuControllerModel
    {
        public IReadOnlyCollection<KeyValuePair<CommandDefinitionBase, bool>> Items;

        public MenuControllerModel(IEnumerable<MenuControllerModelItem> definitionTypes)
        {
            var hashSet = new HashSet<MenuControllerModelItem>(definitionTypes);
            var items = new List<KeyValuePair<CommandDefinitionBase, bool>>();
            var handler = IoC.Get<ICommandService>();
            foreach (var entry in hashSet)
            {
                var command = handler.GetCommandDefinition(entry.Type);
                if (command != null)
                    items.Add(new KeyValuePair<CommandDefinitionBase, bool>(command, entry.IsFixed));
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
                if (!type.IsSubclassOf(typeof(CommandDefinitionBase)))
                    throw new ArgumentException();
                Type = type;
                IsFixed = isFixed;
            }
        }
    }
}
