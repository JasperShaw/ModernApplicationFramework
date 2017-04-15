using System.Collections.Generic;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ContextMenu = ModernApplicationFramework.Controls.ContextMenu;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IContextMenuCreator
    {
        ContextMenu CreateContextMenu(CommandBarDefinitionBase contextMenuDefinition);
        void CreateContextMenuTree(CommandBarDefinitionBase definition, ItemsControl contextMenu);
        IEnumerable<CommandBarItemDefinition> GetContextMenuItemDefinitions(CommandBarDefinitionBase contextMenuDefinition);
    }
}
