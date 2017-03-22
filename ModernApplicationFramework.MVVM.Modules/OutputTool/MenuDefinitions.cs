using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions;

namespace ModernApplicationFramework.MVVM.Modules.OutputTool
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition Output = new MenuItemDefinition<OpenOutputToolCommandDefinition>("Test", 1, Controls.MenuDefinitions.MenuDefinitions.ViewMenu);
    }
}
