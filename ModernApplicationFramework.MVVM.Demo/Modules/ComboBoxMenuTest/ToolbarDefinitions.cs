using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.MenuDefinitions;
using ModernApplicationFramework.Extended.Properties;
using ModernApplicationFramework.MVVM.Demo.Modules.ComboBoxMenuTest.Commands;

namespace ModernApplicationFramework.MVVM.Demo.Modules.ComboBoxMenuTest
{
    public static class ToolbarDefinitions
    {
        [Export] public static ToolbarDefinition ComboBox = new ToolbarDefinition("ComboBoxTest", 0, true, Dock.Top);

        [Export] public static CommandBarGroupDefinition Group1 = new CommandBarGroupDefinition(ComboBox, 0);

        [Export] public static CommandBarItemDefinition ComboItem = new CommandBarComboItemDefinition<ComboBoxCommandDefinition>(Group1, 0, false, false, false);


        [Export]
        public static CommandBarItemDefinition MenuControllerItem =
            new CommandBarMenuControllerDefinition<TestMenuControllerDefinition>(Group1, uint.MinValue);




        [Export]
        public static CommandBarItemDefinition EditMenu =
            new MenuDefinition(Group1, 1, CommandBar_Resources.MenuEdit_Name);

        [Export]
        public static CommandBarGroupDefinition EditUndoRedoMenuGroup =
            new CommandBarGroupDefinition(EditMenu, 0);

        [Export]
        public static CommandBarItemDefinition EditUndoMenuItem =
            new CommandBarCommandItemDefinition<UndoCommandDefinition>(EditUndoRedoMenuGroup, 0);

        [Export]
        public static CommandBarItemDefinition EditRedoMenuItem =
            new CommandBarCommandItemDefinition<RedoCommandDefinition>(EditUndoRedoMenuGroup, 0);

        
        
        [Export]
        public static CommandBarItemDefinition EditMenu2 =
            new MenuDefinition(EditUndoRedoMenuGroup, 1, "Test");

        [Export]
        public static CommandBarGroupDefinition EditUndoRedoMenuGroup2 =
            new CommandBarGroupDefinition(EditMenu2, 0);

        [Export]
        public static CommandBarItemDefinition EditRedoMenuItem2 =
            new CommandBarCommandItemDefinition<RedoCommandDefinition>(EditUndoRedoMenuGroup2, 0);





    }
}
