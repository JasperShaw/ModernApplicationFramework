using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.CommandBar.Hosts;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    /// <summary>
    /// This interface provides the basic structure required to host command bar items
    /// </summary>
    public interface ICommandBarHost
    {
        /// <summary>
        /// Reference to the used <see cref="ICommandBarDefinitionHost"/> instance
        /// </summary>
        ICommandBarDefinitionHost DefinitionHost { get; }

        /// <summary>
        /// A list of all top-level <see cref="CommandBarDataSource"/>
        /// </summary>
        ObservableCollection<CommandBarDataSource> TopLevelDefinitions { get; }

        /// <summary>
        /// Builds all hosted command bar items and updates the UI
        /// </summary>
        void Build();

        /// <summary>
        /// Builds a specific command bar item and updates the UI
        /// </summary>
        /// <param name="definition">The item to build</param>
        void Build(CommandBarDataSource definition);

        /// <summary>
        /// Builds a specific command bar item logically
        /// </summary>
        /// <param name="definition"></param>
        void BuildLogical(CommandBarDataSource definition);


        void BuildLogical(CommandBarDataSource definition, IReadOnlyList<CommandBarGroup> groups,
            Func<CommandBarGroup, IReadOnlyList<CommandBarItemDataSource>> itemFunc);

        /// <summary>
        /// Adds a <see cref="CommandBarDataSource"/> to the parent's sub-tree
        /// </summary>
        /// <param name="dataSource">The definition to add</param>
        /// <param name="parent">The parent definition</param>
        /// <param name="addAboveSeparator">Indicates whether the new definition is right above a separator<see langword="true"/> if <see cref="parent"/> is a separator; <see langword="false"/> if not</param>
        void AddItemDefinition(CommandBarItemDataSource dataSource, CommandBarDataSource parent,
            bool addAboveSeparator);

        /// <summary>
        /// Deletes a definition
        /// </summary>
        /// <param name="dataSource">The definition to delete</param>
        void DeleteItemDefinition(CommandBarItemDataSource dataSource);

        /// <summary>
        /// Gets the predecessor of a given <see cref="CommandBarItemDataSource"/>
        /// </summary>
        /// <param name="dataSource">The definition which predecessor is searched</param>
        /// <returns>The found <see cref="CommandBarItemDataSource"/>. Returns <see langword="null"/> if there was no predecessor</returns>
        CommandBarItemDataSource GetPreviousItem(CommandBarItemDataSource dataSource);

        /// <summary>
        /// Gets the successor of a given <see cref="CommandBarItemDataSource"/>
        /// </summary>
        /// <param name="dataSource">The definition which successor is searched</param>
        /// <returns>The found <see cref="CommandBarItemDataSource"/>. Returns <see langword="null"/> if there was no successor</returns>
        CommandBarItemDataSource GetNextItem(CommandBarItemDataSource dataSource);

        /// <summary>
        /// Gets the next <see cref="CommandBarItemDataSource"/> inside the same group
        /// </summary>
        /// <param name="dataSource">The definition which successor is searched</param>
        /// <returns>The found <see cref="CommandBarItemDataSource"/>. Returns <see langword="null"/> if there was no successor</returns>
        CommandBarItemDataSource GetNextItemInGroup(CommandBarItemDataSource dataSource);

        /// <summary>
        /// Gets the previous <see cref="CommandBarItemDataSource"/> inside the same group
        /// </summary>
        /// <param name="dataSource">The definition which predecessor is searched</param>
        /// <returns>The found <see cref="CommandBarItemDataSource"/>. Returns <see langword="null"/> if there was no predecessor</returns>
        CommandBarItemDataSource GetPreviousItemInGroup(CommandBarItemDataSource dataSource);

        /// <summary>
        /// Deletes a <see cref="CommandBarGroup"/> and moves all containing items as specified.
        /// </summary>
        /// <param name="group">The group to delte</param>
        /// <param name="option">The option where to move all containing items</param>
        void DeleteGroup(CommandBarGroup group, AppendTo option);

        /// <summary>
        /// Gets all top-level definitions
        /// </summary>
        /// <returns>Returns a list of all definitions</returns>
        IEnumerable<CommandBarDataSource> GetMenuHeaderItemDefinitions();

        /// <summary>
        /// Adds a new group above a given element and inserts all following items of the old group into the new one.
        /// </summary>
        /// <param name="startingItem">The definition where to add the new group</param>
        void AddGroupAt(CommandBarItemDataSource startingItem);

        /// <summary>
        /// Moves a given <see cref="CommandBarItemDataSource"/> up and down and through groups.
        /// </summary>
        /// <param name="selectedListBoxItem">The definition to move</param>
        /// <param name="offset">The number of steps to move the item. A negative number means moving upwards.</param>
        /// <param name="parent">The parent <see cref="CommandBarDataSource"/></param>
        void MoveItem(CommandBarItemDataSource selectedListBoxItem, int offset, CommandBarDataSource parent);

        /// <summary>
        /// Gets the successor group
        /// </summary>
        /// <param name="group">The group which successor is searched</param>
        /// <returns>The found <see cref="CommandBarGroup"/>. Returns <see langword="null"/> if there was no successor</returns>
        CommandBarGroup GetNextGroup(CommandBarGroup group);


        /// <summary>
        /// Gets the pre group predecessor
        /// </summary>
        /// <param name="group">The group which predecessor is searched</param>
        /// <returns>The found <see cref="CommandBarGroup"/>. Returns <see langword="null"/> if there was no predecessor</returns>
        CommandBarGroup GetPreviousGroup(CommandBarGroup group);


        /// <summary>
        /// Resets the specified definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        void Reset(CommandBarDataSource definition);
    }
}