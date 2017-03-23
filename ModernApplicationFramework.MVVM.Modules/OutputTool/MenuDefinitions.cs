using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions;

namespace ModernApplicationFramework.Extended.Modules.OutputTool
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition Output = new MenuItemDefinition<OpenOutputToolCommandDefinition>("Output", 10, Extended.MenuDefinitions.ViewMenuDefinitions.ViewMenu);
    }
}