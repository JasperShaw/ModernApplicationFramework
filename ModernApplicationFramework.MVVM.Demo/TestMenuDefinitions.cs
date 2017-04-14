using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.MenuDefinitions;
using ModernApplicationFramework.MVVM.Demo.Modules.Commands;

namespace ModernApplicationFramework.MVVM.Demo
{
    public static class WindowMenuDefinitions
    {
        [Export] public static MenuDefinition TestMenu = new MenuDefinition(MainMenuBarDefinition.MainMenuBar, 14, "Test", "&Test");

        [Export] public static CommandBarGroupDefinition TestGroup1 = new CommandBarGroupDefinition(TestMenu, int.MaxValue);

        [Export] public static MenuItemDefinition TestCommand = new CommandMenuItemDefinition<TestCommandDefinition>(TestGroup1, 1);

        [Export] public static MenuItemDefinition TestSub = new CustomSubHeaderMenuItemDefinition("Test", "Test", TestGroup1, 0);

        [Export] public static CommandBarGroupDefinition TestGroup2 = new CommandBarGroupDefinition(TestSub, int.MaxValue);

        [Export] public static MenuItemDefinition TestSub1 = new CommandMenuItemDefinition<UndoCommandDefinition>(TestGroup2, 0);



        [Export] public static MenuItemDefinition TestSubSub = new CustomSubHeaderMenuItemDefinition("TestSub", "TestSub", TestGroup2, 0);

        [Export] public static CommandBarGroupDefinition TestGroup4 = new CommandBarGroupDefinition(TestSubSub, int.MaxValue);

        [Export] public static MenuItemDefinition TestSubSub1 = new CommandMenuItemDefinition<UndoCommandDefinition>(TestGroup4, 0);

    }
}
