using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class ToolsMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition ToolsMenu =
            new MenuDefinition(MainMenuBarDefinition.MainMenuBarGroup, 8, "&Tools");

        [Export] public static CommandBarGroupDefinition SettingsGroup =
            new CommandBarGroupDefinition(ToolsMenu, int.MaxValue);

        [Export] public static CommandBarItemDefinition Settings =
            new CommandBarCommandItemDefinition<OpenSettingsCommandDefinition>(SettingsGroup, int.MaxValue);
    }
}