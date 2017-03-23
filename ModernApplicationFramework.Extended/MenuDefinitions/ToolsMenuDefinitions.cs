using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class ToolsMenuDefinitions
    {
        [Export] public static MenuItemDefinition ToolsMenu = new MenuItemDefinition("_Tools", 5);

        [Export] public static MenuItemDefinition Settings = new MenuItemDefinition<OpenSettingsCommandDefinition>("Settings", int.MaxValue, ToolsMenu);
    }
}
