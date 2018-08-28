using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Basics.CommandBar.Elements;
using ModernApplicationFramework.Core.Comparers;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="CommandBarGroup"/> is a container that contains command bar items 
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase" />
    [DebuggerDisplay("<CommandBar Group>")]
    public class CommandBarGroup : CommandBarDataSource, ISortable
    {
        private CommandBarDataSource _parent;
        private uint _sortOrder;

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

        public override uint SortOrder
        {
            get => _sortOrder;
            set
            {
                if (value == _sortOrder)
                    return;
                _sortOrder = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The collection of all containing command bar items
        /// </summary>
        public List<CommandBarItemDataSource> Items { get; set; }

        public override Guid Id => Guid.Empty;

        public CommandBarGroup(CommandBarDataSource parent, uint sortOrder)
            : base(null, false)
        {
            _parent = parent;
            Parent?.ContainedGroups?.AddSorted(this, new SortOrderComparer<CommandBarGroup>());
            Items = new List<CommandBarItemDataSource>();
            _sortOrder = sortOrder;
        }

        public CommandBarGroup(CommandBarItem parentItem, uint sortOrder) : this(parentItem.ItemDataSource, sortOrder)
        {
        }
    }

    public interface ISortable
    {
        uint SortOrder { get; set; }
    }
}