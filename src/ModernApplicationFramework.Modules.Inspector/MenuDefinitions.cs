using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.Elements;
using ModernApplicationFramework.Modules.Inspector.Commands;

namespace ModernApplicationFramework.Modules.Inspector
{
    public static class MenuDefinitions
    {
        [Export] public static CommandBarGroup PropertiesGroup = new CommandBarGroup(Extended.CommandBarDefinitions.TopLevelMenuDefinitions.ViewMenu, int.MaxValue);

        [Export] public static CommandBarItem Inspector =
            new CommandBarCommandItem<OpenInspectorCommandDefinition>(new Guid("{65C35BD5-0961-415B-978A-40B3B7C46378}"), PropertiesGroup, 0);
    }
}