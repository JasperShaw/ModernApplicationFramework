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
        public ContextMenu CreateContextMenu(CommandBarDefinitionBase contextMenuDefinition)
        {
            var contextMenu = new ContextMenu(contextMenuDefinition);
            CreateRecursive(ref contextMenu, contextMenuDefinition);
            return contextMenu;
        }
    }
}