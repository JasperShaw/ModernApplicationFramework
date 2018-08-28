using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Interfaces
{
    /// <summary>
    /// A collection container holding all commandbar definitions and exclusions
    /// </summary>
    public interface ICommandBarDefinitionHost
    {
        /// <summary>
        /// Collection of all group definitions
        /// </summary>
        ObservableCollection<CommandBarGroup> ItemGroupDefinitions { get; }

        /// <summary>
        /// Collection of all commandbar item definitions
        /// </summary>
        ObservableCollection<CommandBarDataSource> ItemDefinitions { get; }

        /// <summary>
        /// Collection of excluded commandbar item definitions
        /// </summary>
        IReadOnlyCollection<CommandBarDataSource> ExcludedItemDefinitions { get; }

        /// <summary>
        /// Collection of excluded command definitions
        /// </summary>
        IReadOnlyCollection<CommandDefinitionBase> ExcludedCommandDefinitions { get; }

        IReadOnlyList<CommandBarGroup> GetSortedGroupsOfDefinition(CommandBarDataSource definition, bool onlyGroupsWithVisibleItems = true);

        Func<CommandBarGroup, IReadOnlyList<CommandBarItemDataSource>> GetItemsOfGroup { get; }
    }
}
