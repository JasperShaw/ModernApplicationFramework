using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class WindowMenuDefinitions
    {
        [Export] public static MenuItemDefinition WindowMenu = new MenuItemDefinition("_Window", 10);

        [Export] public static MenuItemDefinition FullScreen = new MenuItemDefinition<SwitchToDocumentCommandListDefinition>("Full Screen", 10, WindowMenu);
    }
}
