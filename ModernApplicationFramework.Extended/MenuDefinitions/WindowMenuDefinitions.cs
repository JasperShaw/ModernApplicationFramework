using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class WindowMenuDefinitions
    {
        [Export] public static MenuDefinition WindowMenu = new MenuDefinition(MainMenuBarDefinition.MainMenuBar, 13, "&Window");

        [Export] public static CommandBarGroupDefinition OpenWindowsGroup = new CommandBarGroupDefinition(WindowMenu, int.MaxValue);

        [Export] public static CommandBarItemDefinition SwitchActiveLayoutDocument = new CommandMenuItemDefinition<SwitchToDocumentCommandListDefinition>(OpenWindowsGroup, 0);
    }
}
