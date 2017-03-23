using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Extended.Modules.InspectorTool.Commands;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition FileMenuSeparator = new MenuItemDefinition("InspectorSeparator", int.MaxValue, Extended.MenuDefinitions.ViewMenuDefinitions.ViewMenu, true);

        [Export]
        public static MenuItemDefinition Inspector = new MenuItemDefinition<OpenInstectorCommandDefinition>("Inspector", int.MaxValue, Extended.MenuDefinitions.ViewMenuDefinitions.ViewMenu);
    }
}