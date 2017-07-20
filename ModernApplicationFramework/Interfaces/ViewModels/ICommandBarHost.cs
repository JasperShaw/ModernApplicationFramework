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
        /// A list of all top-level <see cref="CommandBarDefinitionBase"/>
        /// </summary>
        ObservableCollection<CommandBarDefinitionBase> TopLevelDefinitions { get; }

        /// <summary>
        /// Builds all hosted command bar items and updates the UI
        /// </summary>
        void Build();

        /// <summary>
        /// Builds a specific command bar item and updates the UI
        /// </summary>
        /// <param name="definition">The item to build</param>
        void Build(CommandBarDefinitionBase definition);

        /// <summary>
        /// Builds a specific command bar item logically
        /// </summary>
        /// <param name="definition"></param>
        void BuildLogical(CommandBarDefinitionBase definition);

        /// <summary>
        /// Adds a <see cref="CommandBarDefinitionBase"/> to the parent's sub-tree
        /// </summary>
        /// <param name="definition">The definition to add</param>
        /// <param name="parent">The parent definition</param>
        /// <param name="addAboveSeparator">Indicates whether the new definition is right above a separator<see langword="true"/> if <see cref="parent"/> is a separator; <see langword="false"/> if not</param>
        void AddItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent,
            bool addAboveSeparator);

        /// <summary>
        /// Deletes a definition
        /// </summary>
        /// <param name="definition">The definition to delete</param>
        void DeleteItemDefinition(CommandBarItemDefinition definition);

        /// <summary>
        /// Gets the predecessor of a given <see cref="CommandBarItemDefinition"/>
        /// </summary>
        /// <param name="definition">The definition which predecessor is searched</param>
        /// <returns>The found <see cref="CommandBarItemDefinition"/>. Returns <see langword="null"/> if there was no predecessor</returns>
        CommandBarItemDefinition GetPreviousItem(CommandBarItemDefinition definition);

        /// <summary>
        /// Gets the successor of a given <see cref="CommandBarItemDefinition"/>
        /// </summary>
        /// <param name="definition">The definition which successor is searched</param>
        /// <returns>The found <see cref="CommandBarItemDefinition"/>. Returns <see langword="null"/> if there was no successor</returns>
        CommandBarItemDefinition GetNextItem(CommandBarItemDefinition definition);

        /// <summary>
        /// Gets the next <see cref="CommandBarItemDefinition"/> inside the same group
        /// </summary>
        /// <param name="definition">The definition which successor is searched</param>
        /// <returns>The found <see cref="CommandBarItemDefinition"/>. Returns <see langword="null"/> if there was no successor</returns>
        CommandBarItemDefinition GetNextItemInGroup(CommandBarItemDefinition definition);

        /// <summary>
        /// Gets the previous <see cref="CommandBarItemDefinition"/> inside the same group
        /// </summary>
        /// <param name="definition">The definition which predecessor is searched</param>
        /// <returns>The found <see cref="CommandBarItemDefinition"/>. Returns <see langword="null"/> if there was no predecessor</returns>
        CommandBarItemDefinition GetPreviousItemInGroup(CommandBarItemDefinition definition);

        /// <summary>
        /// Deletes a <see cref="CommandBarGroupDefinition"/> and moves all containing items as specified.
        /// </summary>
        /// <param name="group">The group to delte</param>
        /// <param name="option">The option where to move all containing items</param>
        void DeleteGroup(CommandBarGroupDefinition group, AppendTo option);

        /// <summary>
        /// Gets all top-level definitions
        /// </summary>
        /// <returns>Returns a list of all definitions</returns>
        IEnumerable<CommandBarDefinitionBase> GetMenuHeaderItemDefinitions();

        /// <summary>
        /// Adds a new group above a given element and inserts all following items of the old group into the new one.
        /// </summary>
        /// <param name="startingDefinition">The definition where to add the new group</param>
        void AddGroupAt(CommandBarItemDefinition startingDefinition);

        /// <summary>
        /// Moves a given <see cref="CommandBarItemDefinition"/> up and down and through groups.
        /// </summary>
        /// <param name="selectedListBoxDefinition">The definition to move</param>
        /// <param name="offset">The number of steps to move the item. A negative number means moving upwards.</param>
        /// <param name="parent">The parent <see cref="CommandBarDefinitionBase"/></param>
        void MoveItem(CommandBarItemDefinition selectedListBoxDefinition, int offset, CommandBarDefinitionBase parent);

        /// <summary>
        /// Gets the successor group
        /// </summary>
        /// <param name="group">The group which successor is searched</param>
        /// <returns>The found <see cref="CommandBarGroupDefinition"/>. Returns <see langword="null"/> if there was no successor</returns>
        CommandBarGroupDefinition GetNextGroup(CommandBarGroupDefinition group);


        /// <summary>
        /// Gets the pre group predecessor
        /// </summary>
        /// <param name="group">The group which predecessor is searched</param>
        /// <returns>The found <see cref="CommandBarGroupDefinition"/>. Returns <see langword="null"/> if there was no predecessor</returns>
        CommandBarGroupDefinition GetPreviousGroup(CommandBarGroupDefinition group);
    }
}