using System;
using System.Collections.Generic;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ContextMenu = ModernApplicationFramework.Controls.Menu.ContextMenu;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    /// <summary>
    /// A Factory to create context menus
    /// </summary>
    public interface IContextMenuCreator : ICreatorBase
    {
        /// <summary>
        /// Creates a context menu
        /// </summary>
        /// <param name="contextMenuDefinition">The data model of the context menu</param>
        /// <returns>Returns the context menu</returns>
        ContextMenu CreateContextMenu(CommandBarDefinitionBase contextMenuDefinition, IReadOnlyList<CommandBarGroupDefinition> groups, Func<CommandBarGroupDefinition, IReadOnlyList<CommandBarItemDefinition>> itemsFunc);
    }
}
