using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Interfaces.Utilities;

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