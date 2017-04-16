using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces
{
    public interface IContextMenuHost : ICommandBarHost
    {
        ModernApplicationFramework.Controls.ContextMenu GetContextMenu(ContextMenuDefinition contextMenuDefinition);
    }
}
