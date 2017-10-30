using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.MenuDefinitions;
using ModernApplicationFramework.WindowManagement.Commands;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement
{
    public static class MenuDefinitions
    {
        //LayoutGroup
        [Export]
        public static CommandBarGroupDefinition LayoutGroup =
            new CommandBarGroupDefinition(WindowMenuDefinitions.WindowMenu, 3);

        [Export]
        public static CommandBarItemDefinition SaveLayout =
            new CommandBarCommandItemDefinition<SaveCurrentLayoutCommandDefinition>(LayoutGroup, 0);

        //------------- Apply Layout Sub Menu
        [Export]
        public static CommandBarItemDefinition ApplyLayout =
            new MenuDefinition(LayoutGroup, 1, WindowManagement_Resources.MenuDefinition_ApplyLayout);


        [Export]
        public static CommandBarGroupDefinition LayoutApplyGroup =
            new CommandBarGroupDefinition(ApplyLayout, uint.MinValue);

        [Export]
        public static CommandBarItemDefinition ShowLayouts =
            new CommandBarCommandItemDefinition<ListWindowLayoutsCommandDefinition>(LayoutApplyGroup, 0);

        //--------------

        [Export]
        public static CommandBarItemDefinition ManageLayouts =
            new CommandBarCommandItemDefinition<ManageLayoutCommandDefinition>(LayoutGroup, 2);

        [Export]
        public static CommandBarItemDefinition ResetLayout =
            new CommandBarCommandItemDefinition<ResetLayoutCommandDefinition>(LayoutGroup, 3, true, false, false, true);
    }
}
