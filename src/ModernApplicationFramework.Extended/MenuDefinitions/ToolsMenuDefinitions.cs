using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar.Commands;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class ToolsMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition ToolsMenu =
            new MenuDefinition(MainMenuBarDefinition.MainMenuBarGroup, 8, CommandBar_Resources.MenuTools_Name);

        [Export] public static CommandBarGroupDefinition SettingsGroup =
            new CommandBarGroupDefinition(ToolsMenu, int.MaxValue);

        [Export]
        public static CommandBarItemDefinition CustomizeDialog =
            new CommandBarCommandItemDefinition<CustomizeMenuCommandDefinition>(SettingsGroup, int.MaxValue - 1);

        [Export] public static CommandBarItemDefinition Settings =
            new CommandBarCommandItemDefinition<OpenSettingsCommandDefinition>(SettingsGroup, int.MaxValue);
    }
}