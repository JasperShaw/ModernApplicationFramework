using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Commands
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition Test = new MenuItemDefinition<TestCommandDefinition>("Test", 1, Controls.MenuDefinitions.MenuDefinitions.FileMenu);
    }
}
