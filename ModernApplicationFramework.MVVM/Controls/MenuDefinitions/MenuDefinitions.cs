using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.MVVM.Commands;

namespace ModernApplicationFramework.MVVM.Controls.MenuDefinitions
{
    public static class MenuDefinitions
    {
        [Export] public static MenuItemDefinitionOld FileMenu = new MenuItemDefinitionOld("_File", 0);


        [Export] public static MenuItemDefinitionOld SubMenuNew = new MenuItemDefinitionOld("New", 0, FileMenu);

        [Export]
        public static MenuItemDefinitionOld SubMenuItemNewFile =
            new MenuItemDefinitionOld<NewFileCommandDefinition>("New File", 4, SubMenuNew);

        [Export]
        public static MenuItemDefinitionOld SubMenuOpen = new MenuItemDefinitionOld("Open", 1, FileMenu);

        [Export]
        public static MenuItemDefinitionOld SubMenuItemOpenFile =
            new MenuItemDefinitionOld<OpenFileCommandDefinition>("New File", 4, SubMenuOpen);

        [Export] public static MenuItemDefinitionOld FileMenuSeparator = new MenuItemDefinitionOld("Separator", 2, FileMenu, true);


        [Export]
        public static MenuItemDefinitionOld CloseActiveDocument = new MenuItemDefinitionOld<CloseActiveDocumentCommandDefinition>("Close Document", 3, FileMenu);

        [Export]
        public static MenuItemDefinitionOld FileMenuSeparator2 = new MenuItemDefinitionOld("Separator2", 4, FileMenu, true);

        [Export] public static MenuItemDefinitionOld SubItemSave = new MenuItemDefinitionOld<SaveFileCommandDefinition>(
            "Save", 4, FileMenu);

        [Export]
        public static MenuItemDefinitionOld SubItemSaveAs = new MenuItemDefinitionOld<SaveFileAsCommandDefinition>(
            "Save As", 4, FileMenu);

        [Export]
        public static MenuItemDefinitionOld SaveAll = new MenuItemDefinitionOld<SaveAllFilesCommandDefinition>("Save All", 5, FileMenu);

        [Export]
        public static MenuItemDefinitionOld FileMenuSeparatorLast = new MenuItemDefinitionOld("SeparatorLast", int.MaxValue, FileMenu, true);

        [Export]
        public static MenuItemDefinitionOld CloseProgramm = new MenuItemDefinitionOld<CloseProgammCommandDefinition>("Close", int.MaxValue, FileMenu);



        [Export] public static MenuItemDefinitionOld EditMenu = new MenuItemDefinitionOld("_Edit", 1);

        [Export] public static MenuItemDefinitionOld Undo = new MenuItemDefinitionOld<UndoCommandDefinition>("Undo", 0, EditMenu);

        [Export] public static MenuItemDefinitionOld Redo = new MenuItemDefinitionOld<RedoCommandDefinition>("Redo", 1, EditMenu);

        [Export]
        public static MenuItemDefinitionOld EditMenuSeparator = new MenuItemDefinitionOld("Separator2", 2, EditMenu, true);




        [Export] public static MenuItemDefinitionOld ViewMenu = new MenuItemDefinitionOld("_View", 2);

        [Export]

        public static MenuItemDefinitionOld ViewMenuSeparator = new MenuItemDefinitionOld("Separator", 9, ViewMenu, true);

        [Export] public static MenuItemDefinitionOld FullScreen =
            new MenuItemDefinitionOld<FullScreenCommandDefinition>("Full Screen", 10, ViewMenu);

        [Export] public static MenuItemDefinitionOld ToolsMenu = new MenuItemDefinitionOld("_Tools", 5);

        [Export]
        public static MenuItemDefinitionOld Settings = new MenuItemDefinitionOld<OpenSettingsCommandDefinition>("Settings", int.MaxValue, ToolsMenu);
    }
}
