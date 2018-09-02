using System.Windows.Controls;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Docking.ContextMenuProviders
{
    public class AnchorableContextMenuProvider : IContextMenuProvider
    {
        public static IContextMenuProvider Instance { get; } = new AnchorableContextMenuProvider();

        public ContextMenu Provide(object dataContext)
        {
            return DockingManager.Instance.AnchorableContextMenu;
        }
    }
}
