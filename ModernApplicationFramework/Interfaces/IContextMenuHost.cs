using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces
{
    public interface IContextMenuHost : ICommandBarHost
    {
        ContextMenu GetContextMenu(ContextMenuDefinition contextMenuDefinition);
    }
}
