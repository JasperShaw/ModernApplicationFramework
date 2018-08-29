using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.Commands;
using ModernApplicationFramework.Basics.CommandBar.Elements;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBar.Resources;
using ModernApplicationFramework.Extended.CommandBarDefinitions;

namespace ModernApplicationFramework.Extended.CommandBar.MenuDefinitions
{
    public static class ViewMenuDefinitions
    {
        [Export] public static CommandBarGroup ScreenViewGroup = new CommandBarGroup(TopLevelMenuDefinitions.ViewMenu, 4);

        [Export] public static CommandBarGroup ToolsViewGroup = new CommandBarGroup(TopLevelMenuDefinitions.ViewMenu, 3);


        [Export] public static CommandBarItem ToolBarListMenu =
            new CommandBarMenuItem(new Guid("{8757BD60-6254-4778-8E34-C2DBFFB60D3E}"), CommandBar_Resources.ToolbarsMenu_Name, ScreenViewGroup, 0);

        [Export] public static CommandBarGroup ToolBarListGroup = new CommandBarGroup(ToolBarListMenu, uint.MinValue);


        [Export] public static CommandBarItem ToolBarList =
            new CommandBarCommandItem<ListToolBarsCommandListDefinition>(
                new Guid("{01730EBA-0C2E-4690-93E4-008B431874FF}"), ToolBarListGroup, 0);


        [Export] public static CommandBarItem FullScreen =
            new CommandBarCommandItem<FullScreenCommandDefinition>(
                new Guid("{E0AEE33C-F300-43DC-8CD5-30E9C5389B78}"), ScreenViewGroup, 2);
   
    }
}