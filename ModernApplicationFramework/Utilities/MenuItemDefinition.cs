using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Commands.Service;

namespace ModernApplicationFramework.Utilities
{
    public class MenuItemDefinition
    {
        public MenuItemDefinition(string name, int priority)
        {
            Name = name;
            Priority = priority;
            Parent = null;
            Definitions = new List<CommandDefinition>();
        }

        public MenuItemDefinition(string name, int priority, MenuItemDefinition parent)
        {
            Name = name;
            Priority = priority;
            Parent = parent;
            Definitions = new List<CommandDefinition>();
        }

        public MenuItemDefinition(string name, int priority, MenuItemDefinition parent,
                                  IList<CommandDefinition> definitions)
        {
            Name = name;
            Priority = priority;
            Definitions = definitions;
            Parent = parent;
        }

        public IList<CommandDefinition> Definitions { get; }

        public bool HasItems => Definitions.Any();

        public bool HasParent => Parent != null;

        public string Name { get; }
        public MenuItemDefinition Parent { get; }

        public int Priority { get; }
    }

    public class MenuItemDefinition<T> : MenuItemDefinition where T : CommandDefinition
    {
        public MenuItemDefinition(string name, int priority, MenuItemDefinition parent) : base(name, priority, parent)
        {
            var t = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Definitions.Add(t);
        }
    }
}