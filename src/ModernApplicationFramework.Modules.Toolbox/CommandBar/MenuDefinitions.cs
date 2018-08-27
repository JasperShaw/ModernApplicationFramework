using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Modules.Toolbox.CommandDefinitions;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    public static class MenuDefinitions
    {
        [Export]
        public static CommandBarItem Toolbox =
            new CommandBarCommandItem<OpenToolboxCommandDefinition>(
                new Guid("{33E9B20E-D1A8-4924-BEA0-0C197479E677}"), Extended.CommandBar.MenuDefinitions.ViewMenuDefinitions.ToolsViewGroup, 3);
    }
}
