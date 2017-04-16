using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ContextMenu = ModernApplicationFramework.Controls.ContextMenu;

namespace ModernApplicationFramework.Interfaces.Utilities
{
    public interface IContextMenuCreator : IMenuCreator
    {
        ContextMenu CreateContextMenu(CommandBarDefinitionBase contextMenuDefinition);
    }
}
