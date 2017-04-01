using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Menu;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class MainMenuBarDefinition
    {
        [Export] public static MenuBarDefinition MainMenuBar = new MenuBarDefinition("Menu Bar", uint.MinValue);
    }
}
