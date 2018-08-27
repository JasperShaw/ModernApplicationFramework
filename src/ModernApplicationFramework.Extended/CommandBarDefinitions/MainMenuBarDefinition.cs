using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.CommandBarDefinitions
{
    public static class MainMenuBarDefinition
    {
        [Export] public static CommandBarItem MainMenuBar = new MainMenuBar(CommandBar_Resources.MenuBarMain_Name);

        [Export] public static CommandBarGroup MainMenuBarGroup = new CommandBarGroup(MainMenuBar, uint.MinValue);
    }
}