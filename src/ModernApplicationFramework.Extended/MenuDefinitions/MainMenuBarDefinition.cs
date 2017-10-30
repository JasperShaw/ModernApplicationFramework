using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class MainMenuBarDefinition
    {
        [Export] public static MenuBarDefinition MainMenuBar = new MenuBarDefinition(CommandBar_Resources.MenuBarMain_Name, uint.MinValue);

        [Export] public static CommandBarGroupDefinition MainMenuBarGroup = new CommandBarGroupDefinition(MainMenuBar, uint.MinValue);
    }
}