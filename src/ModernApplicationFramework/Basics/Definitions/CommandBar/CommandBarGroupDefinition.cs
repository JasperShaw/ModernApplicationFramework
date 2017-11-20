using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Core.Comparers;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="CommandBarGroupDefinition"/> is a container that contains command bar items 
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase" />
    public class CommandBarGroupDefinition : CommandBarDefinitionBase
    {
        private CommandBarDefinitionBase _parent;

        /// <summary>
        /// Gets the last item of the group.
        /// </summary>
        public CommandBarItemDefinition LastItem => Items.LastOrDefault();

        /// <summary>
        /// Gets the first item of the group.
        /// </summary>
        public CommandBarItemDefinition FirstItem => Items.FirstOrDefault();

        /// <summary>
        /// The parent command bar element of the group
        /// </summary>
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

        /// <summary>
        /// The collection of all containing command bar items
        /// </summary>
        public List<CommandBarItemDefinition> Items { get; set; }

        public override Guid Id => Guid.Empty;

        public CommandBarGroupDefinition(CommandBarDefinitionBase parent, uint sortOrder)
            : base(null, sortOrder, null, false, false, false)
        {
            _parent = parent;
            Parent?.ContainedGroups?.AddSorted(this, new SortOrderComparer<CommandBarGroupDefinition>());
            Items = new List<CommandBarItemDefinition>();
        }
    }
}