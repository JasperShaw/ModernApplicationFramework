using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;

namespace ModernApplicationFramework.Extended.Modules.OutputTool
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition Output = new CommandMenuItemDefinition<OpenOutputToolCommandDefinition>(Extended.MenuDefinitions.ViewMenuDefinitions.ToolsViewGroup, 2);
    }
}