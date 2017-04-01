using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class ViewMenuDefinitions
    {
        [Export] public static MenuDefinition ViewMenu = new MenuDefinition(MainMenuBarDefinition.MainMenuBar, 2, "View", "&View");

        [Export] public static MenuItemGroupDefinition ScreenViewGroup = new MenuItemGroupDefinition(ViewMenu, 5);

        [Export] public static MenuItemGroupDefinition ToolsViewGroup = new MenuItemGroupDefinition(ViewMenu, 4);

        [Export] public static MenuItemDefinition FullScreen = new CommandMenuItemDefinition<FullScreenCommandDefinition>(ScreenViewGroup, 2);
    }
}
