using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Interfaces
{
    public interface IContextMenuHost
    {
        ObservableCollectionEx<ContextMenuDefinition> ContextMenuDefinitions { get; }
        ObservableCollectionEx<MenuItemGroupDefinition> MenuItemGroupDefinitions { get; }
        ObservableCollectionEx<MenuItemDefinition> MenuItemDefinitions { get; }
        ObservableCollection<CommandBarDefinitionBase> ExcludedContextMenuElementDefinitions { get; }

        void CreateAllContextMenus();

        ModernApplicationFramework.Controls.ContextMenu GetContextMenu(ContextMenuDefinition contextMenuDefinition);
    }
}
