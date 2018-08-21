using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    /// <inheritdoc cref="IContextMenuCreator" />
    /// <summary>
    /// Implementation of <see cref="IContextMenuCreator"/>
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.CommandBar.Creators.MenuCreatorBase" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.Utilities.IContextMenuCreator" />
    [Export(typeof(IContextMenuCreator))]
    public class ContextMenuCreator : MenuCreatorBase, IContextMenuCreator
    {
        public ContextMenu CreateContextMenu(CommandBarDataSource contextMenuDefinition)
        {
            var contextMenu = new ContextMenu(contextMenuDefinition);
            CreateRecursive(ref contextMenu, contextMenuDefinition);
            return contextMenu;
        }

        public ContextMenu CreateContextMenu(CommandBarDataSource contextMenuDefinition, IReadOnlyList<CommandBarGroupDefinition> groups, Func<CommandBarGroupDefinition, IReadOnlyList<CommandBarItemDataSource>> itemsFunc)
        {
            var contextMenu = new ContextMenu(contextMenuDefinition);
            CreateRecursive(ref contextMenu, contextMenuDefinition, groups, itemsFunc);
            return contextMenu;
        }
    }
}