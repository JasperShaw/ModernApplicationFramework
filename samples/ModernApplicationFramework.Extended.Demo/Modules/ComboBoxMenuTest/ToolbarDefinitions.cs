using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.Demo.Modules.ComboBoxMenuTest.Commands;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.Demo.Modules.ComboBoxMenuTest
{
    public static class ToolbarDefinitions
    {
        [Export] public static CommandBarItem ComboBox = new CommandBarToolbar(new Guid("{9DC799F9-7997-4DC0-87B0-A12D6FE26AF2}"), "ComboBoxTest", 1, true, Dock.Top);

        [Export] public static CommandBarGroup Group1 = new CommandBarGroup(ComboBox, 0);

        [Export]
        public static CommandBarItem ComboItem3 =
            new CommandBarComboBox<ComboBoxCommandDefinition>(new Guid("{525CEE81-5407-405B-9A3F-FF6133505495}"),
                Group1, 0);

        [Export]
        public static CommandBarItem ComboItem4 =
            new CommandBarComboBox<ComboBoxCommandDefinition>(new Guid("{28C34A28-06A6-48E9-942F-6659E506B38B}"),
                Group1, 1);

        [Export]
        public static CommandBarItem MenuControllerItem =
            new CommandBarMenuController<TestMenuControllerDefinition>(
                new Guid("{D564F680-DB8C-4D0B-B428-B450152D9B68}"),
                Group1, uint.MaxValue);

        [Export]
        public static CommandBarItem EditMenu =
            new CommandBarMenuItem(new Guid("{B99B7750-D09F-48DB-A16E-E695086F4660}"),
                CommandBar_Resources.MenuEdit_Name, Group1, 1);

        [Export]
        public static CommandBarGroup EditUndoRedoMenuGroup =
            new CommandBarGroup(EditMenu, 0);

        [Export]
        public static CommandBarItem EditUndoMenuItem =
            new CommandBarCommandItem<UndoCommandDefinition>(new Guid("{CDF6B0C1-C783-4B81-8670-07FADD484E9E}"), EditUndoRedoMenuGroup, 0);

        [Export]
        public static CommandBarItem EditRedoMenuItem =
            new CommandBarCommandItem<RedoCommandDefinition>(new Guid("{AD040E3F-AEA8-492B-81AB-57A3291CDCC6}"), EditUndoRedoMenuGroup, 0);



        [Export]
        public static CommandBarItem EditMenu2 =
            new CommandBarMenuItem(new Guid("{9E435A36-0D68-440F-BF6B-6D03A8AA1EC7}"), "Test", EditUndoRedoMenuGroup, 1);

        [Export]
        public static CommandBarGroup EditUndoRedoMenuGroup2 =
            new CommandBarGroup(EditMenu2, 0);

        [Export]
        public static CommandBarItem EditRedoMenuItem2 =
            new CommandBarCommandItem<RedoCommandDefinition>(new Guid("{A49013A2-6E7A-4158-A15F-8B7C1D648317}"), EditUndoRedoMenuGroup2, 0);





    }
}
