using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class ViewMenuDefinitions
    {
        [Export] public static MenuItemDefinition ViewMenu = new MenuItemDefinition("_View", 2);

        [Export] public static MenuItemDefinition FullScreen = new MenuItemDefinition<FullScreenCommandDefinition>("Full Screen", 10, ViewMenu);
    }
}
