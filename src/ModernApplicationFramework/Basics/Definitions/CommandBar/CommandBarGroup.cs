using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Core.Comparers;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="CommandBarGroup"/> is a container that contains command bar items 
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase" />
    [DebuggerDisplay("<CommandBar Group>")]
    public class CommandBarGroup : CommandBarDataSource
    {
        private CommandBarDataSource _parent;

        /// <summary>
        /// Gets the last item of the group.
        /// </summary>
        public CommandBarItemDataSource LastItem => Items.LastOrDefault();

        /// <summary>
        /// Gets the first item of the group.
        /// </summary>
        public CommandBarItemDataSource FirstItem => Items.FirstOrDefault();

        /// <summary>
        /// The parent command bar element of the group
        /// </summary>
        public CommandBarDataSource Parent
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
        public List<CommandBarItemDataSource> Items { get; set; }

        public override Guid Id => Guid.Empty;

        public CommandBarGroup(CommandBarDataSource parent, uint sortOrder)
            : base(null, sortOrder, false)
        {
            _parent = parent;
            Parent?.ContainedGroups?.AddSorted(this, new SortOrderComparer<CommandBarGroup>());
            Items = new List<CommandBarItemDataSource>();
        }

        public CommandBarGroup(CommandBarItem parentItem, uint sortOrder) : this(parentItem.ItemDataSource, sortOrder)
        {
        }
    }
}