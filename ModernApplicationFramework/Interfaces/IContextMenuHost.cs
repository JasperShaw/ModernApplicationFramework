using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Interfaces
{
    public interface IContextMenuHost : ICommandBarHost
    {
        ObservableCollectionEx<ContextMenuDefinition> ContextMenuDefinitions { get; }

        ModernApplicationFramework.Controls.ContextMenu GetContextMenu(ContextMenuDefinition contextMenuDefinition);
    }
}
