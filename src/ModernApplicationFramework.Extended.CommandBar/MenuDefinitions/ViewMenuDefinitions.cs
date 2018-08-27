using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBarDefinitions;

namespace ModernApplicationFramework.Extended.CommandBar.MenuDefinitions
{
    public static class ViewMenuDefinitions
    {
        [Export] public static CommandBarGroup ScreenViewGroup = new CommandBarGroup(TopLevelMenuDefinitions.ViewMenu, 4);

        [Export] public static CommandBarGroup ToolsViewGroup = new CommandBarGroup(TopLevelMenuDefinitions.ViewMenu, 3);

        [Export] public static CommandBarItem FullScreen =
            new CommandBarCommandItem<FullScreenCommandDefinition>(
                new Guid("{E0AEE33C-F300-43DC-8CD5-30E9C5389B78}"), ScreenViewGroup, 2);
    }
}