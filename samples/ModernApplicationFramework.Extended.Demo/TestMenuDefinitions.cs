using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Creators;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Demo.Modules.ComboBoxMenuTest.Commands;
using ModernApplicationFramework.Extended.Demo.Modules.Commands;
using ModernApplicationFramework.Extended.MenuDefinitions;

namespace ModernApplicationFramework.Extended.Demo
{
    public static class WindowMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition TestMenu = new MenuDefinition(
            new Guid("{C374077C-1EAD-41E3-9E93-96F2DD812752}"), MainMenuBarDefinition.MainMenuBarGroup, 14, "&Test");

        [Export] public static CommandBarGroupDefinition TestGroup1 = new CommandBarGroupDefinition(TestMenu, int.MaxValue);

        [Export] public static CommandBarItemDefinition TestCommand =
            new CommandBarCommandItemDefinition<TestCommandDefinition>(
                new Guid("{3C4FD58F-7EF4-4E0F-A993-8E0349850395}"), TestGroup1, 1);

        [Export] public static CommandBarItemDefinition TestCommand2 =
            new CommandBarCommandItemDefinition<TestCommandDefinitionRealMulti>(
                new Guid("{1F363181-E4B4-4A37-AA43-C96CF37706A4}"), TestGroup1, 1);

        [Export] public static CommandBarItemDefinition TestSub =
            new MenuDefinition(new Guid("{3F544F5A-8D20-428F-AC3A-40763840314E}"), TestGroup1, 0, "Test", true);

        [Export] public static CommandBarGroupDefinition TestGroup2 = new CommandBarGroupDefinition(TestSub, int.MaxValue);

        [Export] public static CommandBarItemDefinition TestSub1 =
            new CommandBarCommandItemDefinition<UndoCommandDefinition>(
                new Guid("{356C05BB-2635-43A1-B62C-1A07E2FF9671}"), TestGroup2, 0);



        [Export] public static CommandBarItemDefinition TestSubSub =
            new MenuDefinition(new Guid("{BCFA7117-EBEC-4BDB-8FFE-D153F0BF789B}"), TestGroup2, 0, "TestSub", true);

        [Export] public static CommandBarItemDefinition MenuControllerItemMenu =
            new CommandBarMenuControllerDefinition<TestMenuControllerDefinition>(
                new Guid("{A459EAA6-E5B0-474D-8DEB-7353FBB9C15E}"), TestGroup1, uint.MinValue);

        [Export] public static CommandBarGroupDefinition TestGroup4 = new CommandBarGroupDefinition(TestSubSub, int.MaxValue);

        [Export] public static CommandBarItemDefinition TestSubSub1 =
            new CommandBarCommandItemDefinition<UndoCommandDefinition>(
                new Guid("{87570FD3-C180-4588-A3BB-418D3A7F6283}"), TestGroup4, 0);


        [Export] public static CommandBarItemDefinition SplitItem =
            new CommandBarSplitItemDefinition<MultiUndoCommandDefinition>(
                new Guid("{D7CBAF7A-724D-427F-8B14-319740835130}"),
                new NumberStatusStringCreator("Undo {0} Action{1}", "s"), TestGroup1, 0);

        [Export] public static CommandBarItemDefinition ComboItem =
            new CommandBarComboItemDefinition<ComboBoxCommandDefinition>(
                new Guid("{25EEF4F2-6B3E-4C38-AF3E-F4342426CD4F}"), TestGroup1, 0, false, false, false);


        //[Export] public static CommandBarItemDefinition MenuControllerItem = new CommandBarMenuControllerDefinitionT<TestMenuControllerDefinition>(MainMenuBarDefinition.MainMenuBarGroup, uint.MinValue);

    }
}
