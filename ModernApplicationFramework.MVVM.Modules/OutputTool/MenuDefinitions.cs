using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Extended.Modules.OutputTool
{
    public static class MenuDefinitions
    {
        [Export]
        public static CommandBarItemDefinition Output = new CommandBarCommandItemDefinition<OpenOutputToolCommandDefinition>(Extended.MenuDefinitions.ViewMenuDefinitions.ToolsViewGroup, 2);
    }
}