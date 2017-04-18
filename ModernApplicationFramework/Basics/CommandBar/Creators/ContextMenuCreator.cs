using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Interfaces.Utilities;
using ContextMenu = ModernApplicationFramework.Controls.ContextMenu;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
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