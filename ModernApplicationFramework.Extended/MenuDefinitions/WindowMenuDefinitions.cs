using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class WindowMenuDefinitions
    {
        [Export] public static MenuDefinition WindowMenu = new MenuDefinition(13, "WindowMenu", "_Window");

        [Export] public static MenuItemGroupDefinition OpenWindowsGroup = new MenuItemGroupDefinition(WindowMenu, int.MaxValue);

        [Export] public static MenuItemDefinition FullScreen = new CommandMenuItemDefinition<SwitchToDocumentCommandListDefinition>(OpenWindowsGroup, 0);
    }
}
