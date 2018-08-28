using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.Elements;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBarDefinitions;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Demo.Modules.ComboBoxMenuTest.Commands;
using ModernApplicationFramework.Extended.Demo.Modules.Commands;

namespace ModernApplicationFramework.Extended.Demo
{
    public static class WindowMenuDefinitions
    {
        [Export] public static CommandBarItem TestMenu = new CommandBarMenuItem(
            new Guid("{C374077C-1EAD-41E3-9E93-96F2DD812752}"), "&Test", MainMenuBarDefinition.MainMenuBarGroup, 14);

        [Export] public static CommandBarGroup TestGroup1 = new CommandBarGroup(TestMenu, int.MaxValue);

        [Export] public static CommandBarItem TestCommand =
            new CommandBarCommandItem<TestCommandDefinition>(
                new Guid("{3C4FD58F-7EF4-4E0F-A993-8E0349850395}"), TestGroup1, 1);

        [Export] public static CommandBarItem TestSub =
            new CommandBarMenuItem(new Guid("{3F544F5A-8D20-428F-AC3A-40763840314E}"), "Test", TestGroup1, 0, true,
                CommandBarFlags.CommandFlagNone);

        [Export] public static CommandBarGroup TestGroup2 = new CommandBarGroup(TestSub, int.MaxValue);

        [Export] public static CommandBarItem TestSub1 =
            new CommandBarCommandItem<UndoCommandDefinition>(
                new Guid("{356C05BB-2635-43A1-B62C-1A07E2FF9671}"), TestGroup2, 0);



        [Export] public static CommandBarItem TestSubSub =
            new CommandBarMenuItem(new Guid("{BCFA7117-EBEC-4BDB-8FFE-D153F0BF789B}"), "TestSub", TestGroup2, 0, true,
                CommandBarFlags.CommandFlagNone);

        [Export]
        public static CommandBarItem MenuControllerItem =
            new CommandBarMenuController<TestMenuControllerDefinition>(
                new Guid("{A459EAA6-E5B0-474D-8DEB-7353FBB9C15E}"),
                TestGroup1, uint.MinValue);

        [Export] public static CommandBarGroup TestGroup4 = new CommandBarGroup(TestSubSub, int.MaxValue);

        [Export] public static CommandBarItem TestSubSub1 =
            new CommandBarCommandItem<UndoCommandDefinition>(
                new Guid("{87570FD3-C180-4588-A3BB-418D3A7F6283}"), TestGroup4, 0);


        //[Export] public static CommandBarItemDataSource SplitItem =
        //    new SplitButtonDataSource<MultiUndoCommandDefinition>(
        //        new Guid("{D7CBAF7A-724D-427F-8B14-319740835130}"), TestGroup1, 0);

        //[Export] public static CommandBarItemDataSource ComboItem =
        //    new CommandBarComboItem<ComboBoxCommandDefinition>(
        //        new Guid("{25EEF4F2-6B3E-4C38-AF3E-F4342426CD4F}"), TestGroup1, 0, false, false);


        //[Export] public static CommandBarItemDefinition MenuControllerItem = new CommandBarMenuControllerDefinitionT<TestMenuControllerDefinition>(MainMenuBarDefinition.MainMenuBarGroup, uint.MinValue);

    }
}
