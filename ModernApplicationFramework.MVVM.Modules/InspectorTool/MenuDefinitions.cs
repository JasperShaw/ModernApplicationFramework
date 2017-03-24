using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Modules.InspectorTool.Commands;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool
{
    public static class MenuDefinitions
    {
        [Export] public static MenuItemGroupDefinition PropertiesGroup = new MenuItemGroupDefinition(Extended.MenuDefinitions.ViewMenuDefinitions.ViewMenu, int.MaxValue);

        [Export] public static MenuItemDefinition Inspector = new CommandMenuItemDefinition<OpenInstectorCommandDefinition>(PropertiesGroup, 0);
    }
}