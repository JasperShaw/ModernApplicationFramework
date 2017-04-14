using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class ViewMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition ViewMenu =
            new MenuDefinition(MainMenuBarDefinition.MainMenuBarGroup, 2, "&View");

        [Export] public static CommandBarGroupDefinition ScreenViewGroup = new CommandBarGroupDefinition(ViewMenu, 5);

        [Export] public static CommandBarGroupDefinition ToolsViewGroup = new CommandBarGroupDefinition(ViewMenu, 4);

        [Export] public static CommandBarItemDefinition FullScreen =
            new CommandBarCommandItemDefinition<FullScreenCommandDefinition>(ScreenViewGroup, 2);
    }
}