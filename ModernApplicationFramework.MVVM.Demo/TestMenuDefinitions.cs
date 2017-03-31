using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.MVVM.Demo
{
    public static class WindowMenuDefinitions
    {
        [Export] public static MenuDefinition TestMenu = new MenuDefinition(14, "TestMenu", "&Test");

        [Export] public static MenuItemGroupDefinition TestGroup1 = new MenuItemGroupDefinition(TestMenu, int.MaxValue);

        [Export] public static MenuItemDefinition TestSub = new CustomSubHeaderMenuItemDefinition("Test", TestGroup1, 0);

        [Export] public static MenuItemGroupDefinition TestGroup2 = new MenuItemGroupDefinition(TestSub, int.MaxValue);

        [Export] public static MenuItemDefinition TestSub1 = new CommandMenuItemDefinition<UndoCommandDefinition>(TestGroup2, 0);

    }
}
