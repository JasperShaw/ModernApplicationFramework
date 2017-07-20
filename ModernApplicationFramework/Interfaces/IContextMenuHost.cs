using System.Collections.Generic;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces
{
    /// <summary>
    /// A manager to provide context menus by commandbar definitions
    /// </summary>
    public interface IContextMenuHost : ICommandBarHost
    {
        /// <summary>
        /// Gets a <see cref="ContextMenu"/> by a given <see cref="ContextMenuDefinition"/>
        /// </summary>
        /// <param name="contextMenuDefinition">The definition to search</param>
        /// <returns>Returns the context menu</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        ContextMenu GetContextMenu(ContextMenuDefinition contextMenuDefinition);
    }
}
