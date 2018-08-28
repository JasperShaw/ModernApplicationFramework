using System;
using System.Collections.Generic;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces
{
    /// <inheritdoc />
    /// <summary>
    /// A manager to provide context menus by commandbar definitions
    /// </summary>
    public interface IContextMenuHost : ICommandBarHost
    {
        /// <summary>
        /// Gets a <see cref="ContextMenu"/> by a given <see cref="CommandBarItem"/>
        /// </summary>
        /// <param name="contextMenuDataSource">The definition to search</param>
        /// <returns>Returns the context menu</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        ContextMenu GetContextMenu(CommandBarItem contextMenuDataSource);

        /// <summary>
        /// Gets a <see cref="ContextMenu"/> by a given <see cref="Guid"/>
        /// </summary>
        /// <param name="contextMenuDefinition">The id of the definition</param>
        /// <returns>Returns the context menu</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        ContextMenu GetContextMenu(Guid contextMenuDefinition);
    }
}
