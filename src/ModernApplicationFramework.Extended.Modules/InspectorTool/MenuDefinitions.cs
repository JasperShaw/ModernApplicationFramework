using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Extended.Modules.InspectorTool.Commands;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool
{
    public static class MenuDefinitions
    {
        [Export] public static CommandBarGroupDefinition PropertiesGroup = new CommandBarGroupDefinition(Extended.MenuDefinitions.ViewMenuDefinitions.ViewMenu, int.MaxValue);

        [Export] public static CommandBarItemDefinition Inspector =
            new CommandBarCommandItemDefinition<OpenInspectorCommandDefinition>(new Guid("{65C35BD5-0961-415B-978A-40B3B7C46378}"), PropertiesGroup, 0);
    }
}