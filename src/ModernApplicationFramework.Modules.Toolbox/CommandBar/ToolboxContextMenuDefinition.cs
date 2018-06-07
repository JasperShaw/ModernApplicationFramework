using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    public static class ToolboxContextMenuDefinition
    {
        [Export]
        public static ContextMenuDefinition ToolboxContextMenu =
            new ContextMenuDefinition(new Guid("{70967216-21B8-454D-8361-201F127A6D32}"), ContextMenuCategory.OtherContextMenusCategory, "Toolbox");

        [Export]
        public static CommandBarGroupDefinition BasicEditGroup =
            new CommandBarGroupDefinition(ToolboxContextMenu, uint.MinValue);

        [Export]
        public static CommandBarItemDefinition CloseCommandItemDefinition =
            new CommandBarCommandItemDefinition<DeleteCommandDefinition>(new Guid("{180D3794-7914-4B4C-BD5E-F4A101FAF831}"), BasicEditGroup, 1, true, false, false, true);





        [Export]
        public static CommandBarGroupDefinition CategoryCommandGroup =
            new CommandBarGroupDefinition(ToolboxContextMenu, 3);

        [Export]
        public static CommandBarItemDefinition AddCategoryDefinition =
            new CommandBarCommandItemDefinition<AddCategoryCommandDefinition>(new Guid("{AB8D308E-FD87-47B6-AD26-4FCB78CE9CEC}"), CategoryCommandGroup, 0, true, false, false, true);

        [Export]
        public static CommandBarItemDefinition RemoveCategoryDefinition =
            new CommandBarCommandItemDefinition<DeleteActiveToolbarCategoryCommandDefinition>(new Guid("{BF87A03B-EAFB-4778-8243-90BE338F67EA}"), CategoryCommandGroup, 1, true, false, false, true);

        [Export]
        public static CommandBarItemDefinition RenameCategoryDefinition =
            new CommandBarCommandItemDefinition<RenameToolboxCategoryCommandDefinition>(new Guid("{1D6FB604-BF18-44F1-8EE4-50C8015D29C8}"), CategoryCommandGroup, 2, true, false, false, true);



        [Export]
        public static CommandBarGroupDefinition ItemMoveCommandGroup =
            new CommandBarGroupDefinition(ToolboxContextMenu, 3);
    }
}
