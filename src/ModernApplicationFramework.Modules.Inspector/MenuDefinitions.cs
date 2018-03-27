using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Extended.CommandBarDefinitions.MenuDefinitions;
using ModernApplicationFramework.Modules.Inspector.Commands;

namespace ModernApplicationFramework.Modules.Inspector
{
    public static class MenuDefinitions
    {
        [Export] public static CommandBarGroupDefinition PropertiesGroup = new CommandBarGroupDefinition(ViewMenuDefinitions.ViewMenu, int.MaxValue);

        [Export] public static CommandBarItemDefinition Inspector =
            new CommandBarCommandItemDefinition<OpenInspectorCommandDefinition>(new Guid("{65C35BD5-0961-415B-978A-40B3B7C46378}"), PropertiesGroup, 0);
    }
}