using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Extended.Modules.OutputTool
{
    public static class MenuDefinitions
    {
        [Export] public static CommandBarItemDefinition Output =
            new CommandBarCommandItemDefinition<OpenOutputToolCommandDefinition>(new Guid("{F1605A96-5EDF-48B7-A63A-3402FC3710CA}"), 
                Extended.MenuDefinitions.ViewMenuDefinitions.ToolsViewGroup, 2);
    }
}