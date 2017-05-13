using System.Windows.Controls;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Docking.ContextMenuProviders
{
    public class DocumentTabModelContextMenuProvider : IContextMenuProvider
    {
        public ContextMenu Provide(object dataContext)
        {
            if (dataContext is LayoutAnchorable)
                return DockingManager.Instace.AnchorableAsDocumentContextMenu;
            return DockingManager.Instace.DocumentContextMenu;
        }
    }
}
