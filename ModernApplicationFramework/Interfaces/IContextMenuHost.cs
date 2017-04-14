using System.Collections.ObjectModel;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Interfaces
{
    public interface IContextMenuHost
    {
        ObservableCollectionEx<ContextMenuDefinition> ContextMenuDefinitions { get; }
        ObservableCollectionEx<CommandBarGroupDefinition> MenuItemGroupDefinitions { get; }
        ObservableCollectionEx<CommandBarItemDefinition> MenuItemDefinitions { get; }
        ObservableCollection<CommandBarDefinitionBase> ExcludedContextMenuElementDefinitions { get; }

        void CreateAllContextMenus();

        ModernApplicationFramework.Controls.ContextMenu GetContextMenu(ContextMenuDefinition contextMenuDefinition);
    }
}
