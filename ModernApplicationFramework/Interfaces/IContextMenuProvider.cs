using System.Windows.Controls;

namespace ModernApplicationFramework.Interfaces
{
    public interface IContextMenuProvider
    {
        ContextMenu Provide(object dataContext);
    }
}
