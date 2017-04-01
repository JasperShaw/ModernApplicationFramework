using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class ToolsMenuDefinitions
    {
        [Export] public static MenuDefinition ToolsMenu = new MenuDefinition(MainMenuBarDefinition.MainMenuBar, 8, "Tools", "&Tools");

        [Export] public static MenuItemGroupDefinition SettingsGroup = new MenuItemGroupDefinition(ToolsMenu, int.MaxValue);

        [Export] public static MenuItemDefinition Settings = new CommandMenuItemDefinition<OpenSettingsCommandDefinition>(SettingsGroup, int.MaxValue);
    }
}
