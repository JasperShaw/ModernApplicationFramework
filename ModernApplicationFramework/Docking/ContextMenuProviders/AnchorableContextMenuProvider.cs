using System.Windows.Controls;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Docking.ContextMenuProviders
{
    public class AnchorableContextMenuProvider : IContextMenuProvider
    {
        public ContextMenu Provide(object dataContext)
        {
            return DockingManager.Instace.AnchorableContextMenu;
        }
    }
}
