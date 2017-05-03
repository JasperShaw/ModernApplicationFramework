using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class WindowMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition WindowMenu =
            new MenuDefinition(MainMenuBarDefinition.MainMenuBarGroup, 13, CommandBar_Resources.MenuWindow_Name);

        [Export] public static CommandBarGroupDefinition OpenWindowsGroup =
            new CommandBarGroupDefinition(WindowMenu, int.MaxValue);

        [Export] public static CommandBarItemDefinition SwitchActiveLayoutDocument =
            new CommandBarCommandItemDefinition<SwitchToDocumentCommandListDefinition>(OpenWindowsGroup, 0);
    }
}