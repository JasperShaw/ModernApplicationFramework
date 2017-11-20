using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class ViewMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition ViewMenu =
            new MenuDefinition(new Guid("{78420604-4494-454A-8498-51B6DC540539}"),
                MainMenuBarDefinition.MainMenuBarGroup, 2, CommandBar_Resources.MenuView_Name);

        [Export] public static CommandBarGroupDefinition ScreenViewGroup = new CommandBarGroupDefinition(ViewMenu, 5);

        [Export] public static CommandBarGroupDefinition ToolsViewGroup = new CommandBarGroupDefinition(ViewMenu, 4);

        [Export] public static CommandBarItemDefinition FullScreen =
            new CommandBarCommandItemDefinition<FullScreenCommandDefinition>(
                new Guid("{E0AEE33C-F300-43DC-8CD5-30E9C5389B78}"), ScreenViewGroup, 2);
    }
}