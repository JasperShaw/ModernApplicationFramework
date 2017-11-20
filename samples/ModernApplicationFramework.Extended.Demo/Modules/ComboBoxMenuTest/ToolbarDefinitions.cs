using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Demo.Modules.ComboBoxMenuTest.Commands;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.Demo.Modules.ComboBoxMenuTest
{
    public static class ToolbarDefinitions
    {
        [Export] public static ToolbarDefinition ComboBox = new ToolbarDefinition(new Guid("{9DC799F9-7997-4DC0-87B0-A12D6FE26AF2}"), "ComboBoxTest", 1, true, Dock.Top);

        [Export] public static CommandBarGroupDefinition Group1 = new CommandBarGroupDefinition(ComboBox, 0);

        [Export] public static CommandBarItemDefinition ComboItem = new CommandBarComboItemDefinition<ComboBoxCommandDefinition>(new Guid("{D142CB05-16D2-4C89-A953-924A3F4CF972}"), Group1, 0, false, false);


        [Export]
        public static CommandBarItemDefinition MenuControllerItem =
            new CommandBarMenuControllerDefinition<TestMenuControllerDefinition>(new Guid("{D564F680-DB8C-4D0B-B428-B450152D9B68}"), Group1, uint.MinValue);




        [Export]
        public static CommandBarItemDefinition EditMenu =
            new MenuDefinition(new Guid("{B99B7750-D09F-48DB-A16E-E695086F4660}"), Group1, 1, CommandBar_Resources.MenuEdit_Name);

        [Export]
        public static CommandBarGroupDefinition EditUndoRedoMenuGroup =
            new CommandBarGroupDefinition(EditMenu, 0);

        [Export]
        public static CommandBarItemDefinition EditUndoMenuItem =
            new CommandBarCommandItemDefinition<UndoCommandDefinition>(new Guid("{CDF6B0C1-C783-4B81-8670-07FADD484E9E}"), EditUndoRedoMenuGroup, 0);

        [Export]
        public static CommandBarItemDefinition EditRedoMenuItem =
            new CommandBarCommandItemDefinition<RedoCommandDefinition>(new Guid("{AD040E3F-AEA8-492B-81AB-57A3291CDCC6}"), EditUndoRedoMenuGroup, 0);

        
        
        [Export]
        public static CommandBarItemDefinition EditMenu2 =
            new MenuDefinition(new Guid("{9E435A36-0D68-440F-BF6B-6D03A8AA1EC7}"), EditUndoRedoMenuGroup, 1, "Test");

        [Export]
        public static CommandBarGroupDefinition EditUndoRedoMenuGroup2 =
            new CommandBarGroupDefinition(EditMenu2, 0);

        [Export]
        public static CommandBarItemDefinition EditRedoMenuItem2 =
            new CommandBarCommandItemDefinition<RedoCommandDefinition>(new Guid("{A49013A2-6E7A-4158-A15F-8B7C1D648317}"), EditUndoRedoMenuGroup2, 0);





    }
}
