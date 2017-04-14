using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;

namespace ModernApplicationFramework.Basics.ToolbarHostViewModel
{
    public static class ContextMenuDefinition
    {
        [Export] public static Definitions.ContextMenu.ContextMenuDefinition ToolbarsContextMenu =
            new Definitions.ContextMenu.ContextMenuDefinition(ContextMenuCategory.OtherContextMenusCategory,
                "Toolbar List");

        [Export] public static CommandBarGroupDefinition ToolBarListGroup = new CommandBarGroupDefinition(ToolbarsContextMenu, 0);

        [Export] public static MenuItemDefinition ToolBarList = new CommandMenuItemDefinition<ListToolBarsCommandListDefinition>(ToolBarListGroup, 0);

        [Export] public static CommandBarGroupDefinition CustomizeGroup = new CommandBarGroupDefinition(ToolbarsContextMenu, int.MaxValue);

        [Export] public static MenuItemDefinition Customize = new CommandMenuItemDefinition<CustomizeMenuCommandDefinition>(CustomizeGroup, 0);
    }
}
