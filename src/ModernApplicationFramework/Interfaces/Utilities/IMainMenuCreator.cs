using System;
using System.Collections.Generic;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    /// <inheritdoc />
    /// <summary>
    /// A factory to create the main menu
    /// </summary>
    public interface IMainMenuCreator : ICreatorBase
    {
        /// <summary>
        /// Creates the environment's menu bar
        /// </summary>
        /// <param name="model">The data model of the menu host</param>
        void CreateMenuBar(IMenuHostViewModel model);

        /// <summary>
        /// Creats a <see cref="MenuItem"/> based on the given data model
        /// </summary>
        /// <param name="contextMenuDefinition">The data model of the new menu item</param>
        /// <returns>Returns the menu item</returns>
        MenuItem CreateMenuItem(CommandBarDataSource contextMenuDefinition, IReadOnlyList<CommandBarGroupDefinition> groups, Func<CommandBarGroupDefinition, IReadOnlyList<CommandBarItemDataSource>> itemsFunc);
    }
}