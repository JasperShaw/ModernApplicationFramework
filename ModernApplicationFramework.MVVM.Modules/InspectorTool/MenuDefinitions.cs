using System.ComponentModel.Composition;
using ModernApplicationFramework.MVVM.Modules.InspectorTool.Commands;
using ModernApplicationFramework.Utilities;

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
