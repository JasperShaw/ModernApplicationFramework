using System;
using System.Collections.Generic;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    /// <summary>
    /// Offers basic methods to populate an <see cref="ItemsControl"/>
    /// </summary>
    public interface ICreatorBase
    {
        /// <summary>
        /// Creates a sub-tree of an <see cref="ItemsControl"/> recursively 
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="ItemsControl"/></typeparam>
        /// <param name="itemsControl">The <see cref="ItemsControl"/> that should be filled</param>
        /// <param name="itemDefinition">The datamodel of the current item</param>
        //void CreateRecursive<T>(ref T itemsControl, CommandBarDefinitionBase itemDefinition) where T : ItemsControl;
        void CreateRecursive<T>(ref T itemsControl, CommandBarDataSource itemDefinition,
            IReadOnlyList<CommandBarGroup> groups, Func<CommandBarGroup, IReadOnlyList<CommandBarItemDataSource>> itemFunc)
            where T : ItemsControl;

        /// <summary>
        /// Gets all single level sub-definitions of a given <see cref="CommandBarDataSource"/>
        /// </summary>
        /// <param name="dataSource">The <see cref="CommandBarDataSource"/> to find its sub-definitions</param>
        /// <param name="options">An option to include separators to the result.</param>
        /// <returns>Returns a list with all found definitions</returns>
        IEnumerable<CommandBarItemDataSource> GetSingleSubDefinitions(CommandBarDataSource dataSource, CommandBarCreationOptions options);



        IEnumerable<CommandBarItemDataSource> GetSingleSubDefinitions(CommandBarDataSource menuDefinition,
            IReadOnlyList<CommandBarGroup> groups,
            Func<CommandBarGroup, IReadOnlyList<CommandBarItemDataSource>> items,
            CommandBarCreationOptions options = CommandBarCreationOptions.DisplaySeparatorsOnlyIfGroupNotEmpty);
    }

    [Flags]
    public enum CommandBarCreationOptions
    {
        DisplaySeparatorsInAnyCase = 1,
        DisplaySeparatorsOnlyIfGroupNotEmpty = 2,
        DisplayInvisibleItems = 4,
    }
}