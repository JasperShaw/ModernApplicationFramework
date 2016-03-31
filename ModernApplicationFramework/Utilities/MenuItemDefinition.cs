using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Commands;

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

        public MenuItemDefinition(string name, int priority, MenuItemDefinition parent, IList<CommandDefinition> definitions)
        {
            Name = name;
            Priority = priority;
            Definitions = definitions;
            Parent = parent;
        }

        public string Name { get; }
        public MenuItemDefinition Parent { get; }
        public IList<CommandDefinition> Definitions { get; }

        public int Priority { get; }

        public bool HasParent => Parent != null;

        public bool HasItems => Definitions.Any();
    }
}
