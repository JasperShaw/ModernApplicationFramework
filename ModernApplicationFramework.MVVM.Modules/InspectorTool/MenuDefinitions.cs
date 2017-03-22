using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.MVVM.Modules.InspectorTool.Commands;

namespace ModernApplicationFramework.MVVM.Modules.InspectorTool
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition FileMenuSeparator = new MenuItemDefinition("Separator", int.MaxValue, MVVM.Controls.MenuDefinitions.MenuDefinitions.ViewMenu, true);

        [Export]
        public static MenuItemDefinition Inspector = new MenuItemDefinition<OpenInstectorCommandDefinition>("Inspector", int.MaxValue, MVVM.Controls.MenuDefinitions.MenuDefinitions.ViewMenu);
    }
}
