using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Core.Comparers;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public class CommandBarGroupDefinition : CommandBarDefinitionBase
    {
        private CommandBarDefinitionBase _parent;

        public CommandBarItemDefinition LastItem => Items.LastOrDefault();
        public CommandBarItemDefinition FirstItem => Items.FirstOrDefault();

        public CommandBarDefinitionBase Parent
        {
            get => _parent;
            set
            {
                if (Equals(value, _parent)) return;
                _parent = value;
                OnPropertyChanged();
            }
        }

        public List<CommandBarItemDefinition> Items { get; set; }

        public CommandBarGroupDefinition(CommandBarDefinitionBase parent, uint sortOrder)
            : base(null, sortOrder, null, false, false, false)
        {
            _parent = parent;
            Parent?.ContainedGroups?.AddSorted(this, new SortOrderComparer<CommandBarGroupDefinition>());
            Items = new List<CommandBarItemDefinition>();
        }
    }
}