using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Modules.Toolbox.CommandDefinitions;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    public static class ToolboxContextMenuDefinition
    {
        [Export]
        public static ContextMenuDataSource ToolboxContextMenu =
            new ContextMenuDataSource(new Guid("{70967216-21B8-454D-8361-201F127A6D32}"), ContextMenuCategory.OtherContextMenusCategory, "Toolbox");

        [Export]
        public static CommandBarGroup BasicEditGroup =
            new CommandBarGroup(ToolboxContextMenu, uint.MinValue);

        [Export]
        public static CommandBarItemDataSource CutCommandItem =
            new CommandBarCommandItemDataSource<CutCommandDefinition>(new Guid("{7ACF2153-61EB-4C78-84D9-0F4261C13D37}"), BasicEditGroup, 0, true, true, false, true);

        [Export]
        public static CommandBarItemDataSource CopyCommandItem =
            new CommandBarCommandItemDataSource<CopyCommandDefinition>(new Guid("{913939F3-CED0-4810-8597-DAB9476B5792}"), BasicEditGroup, 1, true, true, false, true);

        [Export]
        public static CommandBarItemDataSource PasteCommandItem =
            new CommandBarCommandItemDataSource<PasteCommandDefinition>(new Guid("{59C5F4F3-F0D5-43F5-9525-931DE67405A4}"), BasicEditGroup, 2);

        [Export]
        public static CommandBarItemDataSource DeleteCommandItem =
            new CommandBarCommandItemDataSource<DeleteCommandDefinition>(new Guid("{180D3794-7914-4B4C-BD5E-F4A101FAF831}"), BasicEditGroup, 3, true, false, false, true);

        [Export]
        public static CommandBarItemDataSource RenametemItem =
            new CommandBarCommandItemDataSource<RenameToolboxItemCommandDefinition>(new Guid("{61AEA91B-E3EF-40DE-8969-696F4D056003}"), BasicEditGroup, uint.MaxValue, true, false, false, true);




        [Export]
        public static CommandBarGroup ItemDisplayCommandGroup =
            new CommandBarGroup(ToolboxContextMenu, 1);

        [Export]
        public static CommandBarItemDataSource ShowAllItemsItem =
            new CommandBarCommandItemDataSource<ToggleShowAllItemsCommandDefinition>(new Guid("{FBA2C1B5-993B-4939-9A0F-0D3025E9BA75}"), ItemDisplayCommandGroup, 0);



        [Export]
        public static CommandBarGroup ItemManageCommandGroup =
            new CommandBarGroup(ToolboxContextMenu, 2);

        [Export]
        public static CommandBarItemDataSource AddItemItem =
            new CommandBarCommandItemDataSource<AddItemCommandDefinition>(new Guid("{3CBD7ED4-B9CE-4134-A36E-B5DE6587FE94}"), ItemManageCommandGroup, 0);

        [Export]
        public static CommandBarItemDataSource SortItemsAlphabeticallyItem =
            new CommandBarCommandItemDataSource<SortItemsAlphabeticallyCommandDefinition>(new Guid("{DCDA21B7-179B-4F9D-805E-BC2266761A43}"), ItemManageCommandGroup, 1);

        [Export]
        public static CommandBarItemDataSource ResetToolboxItem =
            new CommandBarCommandItemDataSource<ResetToolboxCommandDefinition>(new Guid("{716EA4A5-19D5-4997-90DA-360214073D51}"), ItemManageCommandGroup, 2);





        [Export]
        public static CommandBarGroup CategoryCommandGroup =
            new CommandBarGroup(ToolboxContextMenu, 3);

        [Export]
        public static CommandBarItemDataSource AddCategoryItem =
            new CommandBarCommandItemDataSource<AddCategoryCommandDefinition>(new Guid("{AB8D308E-FD87-47B6-AD26-4FCB78CE9CEC}"), CategoryCommandGroup, 0);

        [Export]
        public static CommandBarItemDataSource RemoveCategoryItem =
            new CommandBarCommandItemDataSource<DeleteActiveToolbarCategoryCommandDefinition>(new Guid("{BF87A03B-EAFB-4778-8243-90BE338F67EA}"), CategoryCommandGroup, 1, true, false, false, true);

        [Export]
        public static CommandBarItemDataSource RenameCategoryItem =
            new CommandBarCommandItemDataSource<RenameToolboxCategoryCommandDefinition>(new Guid("{1D6FB604-BF18-44F1-8EE4-50C8015D29C8}"), CategoryCommandGroup, 2, true, false, false, true);

        [Export]
        public static CommandBarGroup ItemMoveCommandGroup =
            new CommandBarGroup(ToolboxContextMenu, 3);

        [Export]
        public static CommandBarItemDataSource MoveUpCommandItem=
            new CommandBarCommandItemDataSource<ToolboxNodeUpCommandDefinition>(new Guid("{F4E38EF2-9D9E-45B6-BD5D-72C3CE062F09}"), ItemMoveCommandGroup, 0);

        [Export]
        public static CommandBarItemDataSource MoveDownCommandItem =
            new CommandBarCommandItemDataSource<ToolboxNodeDownCommandDefinition>(new Guid("{F0411BF9-0D4A-46F0-84C6-E9CA0C303EA7}"), ItemMoveCommandGroup, 1);
    }
}
