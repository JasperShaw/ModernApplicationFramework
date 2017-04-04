using System.ComponentModel.Composition;
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

        [Export] public static MenuItemGroupDefinition ToolBarListGroup = new MenuItemGroupDefinition(ToolbarsContextMenu, 0);

        [Export] public static MenuItemDefinition ToolBarList = new CommandMenuItemDefinition<ListToolBarsCommandListDefinition>(ToolBarListGroup, 0);

        [Export] public static MenuItemGroupDefinition CustomizeGroup = new MenuItemGroupDefinition(ToolbarsContextMenu, int.MaxValue);

        [Export] public static MenuItemDefinition Customize = new CommandMenuItemDefinition<CustomizeMenuCommandDefinition>(CustomizeGroup, 0);
    }
}
