using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Extended.Modules.InspectorTool.Commands;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool
{
    public static class MenuDefinitions
    {
        [Export] public static CommandBarGroupDefinition PropertiesGroup = new CommandBarGroupDefinition(Extended.MenuDefinitions.ViewMenuDefinitions.ViewMenu, int.MaxValue);

        [Export] public static MenuItemDefinition Inspector = new CommandMenuItemDefinition<OpenInstectorCommandDefinition>(PropertiesGroup, 0);
    }
}