using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class ViewMenuDefinitions
    {
        [Export] public static MenuDefinition ViewMenu = new MenuDefinition(2, "ViewMeu", "_View");

        [Export] public static MenuItemGroupDefinition ScreenViewGroup = new MenuItemGroupDefinition(ViewMenu, 5);

        [Export] public static MenuItemGroupDefinition ToolsViewGroup = new MenuItemGroupDefinition(ViewMenu, 4);

        [Export] public static MenuItemDefinition FullScreen = new CommandMenuItemDefinition<FullScreenCommandDefinition>(ScreenViewGroup, 2);
    }
}
