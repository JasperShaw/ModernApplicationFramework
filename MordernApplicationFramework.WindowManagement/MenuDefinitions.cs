using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using MordernApplicationFramework.WindowManagement.Commands;

namespace MordernApplicationFramework.WindowManagement
{
    public static class MenuDefinitions
    {
        //LayoutGroup
        [Export]
        public static CommandBarGroupDefinition LayoutGroup =
            new CommandBarGroupDefinition(ModernApplicationFramework.Extended.MenuDefinitions.WindowMenuDefinitions.WindowMenu, 3);

        [Export]
        public static CommandBarItemDefinition SaveLayout =
            new CommandBarCommandItemDefinition<SaveCurrentLayoutCommandDefinition>(LayoutGroup, 0);

        [Export]
        public static CommandBarItemDefinition ManageLayouts =
            new CommandBarCommandItemDefinition<ManageLayoutCommandDefinition>(LayoutGroup, 2);
    }
}
