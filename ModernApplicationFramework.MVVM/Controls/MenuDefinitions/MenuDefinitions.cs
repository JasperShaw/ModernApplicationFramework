using System.ComponentModel.Composition;
using ModernApplicationFramework.MVVM.Commands;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Controls.MenuDefinitions
{
    public static class MenuDefinitions
    {
        [Export] public static MenuItemDefinition FileMenu = new MenuItemDefinition("_File", 0);


        [Export] public static MenuItemDefinition SubMenuNew = new MenuItemDefinition("New", 0, FileMenu);

        [Export]
        public static MenuItemDefinition SubMenuItemNewFile =
            new MenuItemDefinition<NewFileCommandDefinition>("New File", 4, SubMenuNew);

        [Export]
        public static MenuItemDefinition SubMenuOpen = new MenuItemDefinition("Open", 1, FileMenu);

        [Export]
        public static MenuItemDefinition SubMenuItemOpenFile =
            new MenuItemDefinition<OpenFileCommandDefinition>("New File", 4, SubMenuOpen);

        [Export] public static MenuItemDefinition FileMenuSeparator = new MenuItemDefinition("Separator", 2, FileMenu, true);


        [Export]
        public static MenuItemDefinition CloseActiveDocument = new MenuItemDefinition<CloseActiveDocumentCommandDefinition>("Close Document", 3, FileMenu);

        [Export]
        public static MenuItemDefinition FileMenuSeparator2 = new MenuItemDefinition("Separator2", 4, FileMenu, true);

        [Export] public static MenuItemDefinition SubItemSave = new MenuItemDefinition<SaveFileCommandDefinition>(
            "Save", 4, FileMenu);

        [Export]
        public static MenuItemDefinition SubItemSaveAs = new MenuItemDefinition<SaveFileAsCommandDefinition>(
            "Save As", 4, FileMenu);

        [Export]
        public static MenuItemDefinition SaveAll = new MenuItemDefinition<SaveAllFilesCommandDefinition>("Save All", 5, FileMenu);

        [Export]
        public static MenuItemDefinition FileMenuSeparatorLast = new MenuItemDefinition("SeparatorLast", int.MaxValue, FileMenu, true);

        [Export]
        public static MenuItemDefinition CloseProgramm = new MenuItemDefinition<CloseProgammCommandDefinition>("Close", int.MaxValue, FileMenu);



        [Export] public static MenuItemDefinition EditMenu = new MenuItemDefinition("_Edit", 1);

        [Export] public static MenuItemDefinition Undo = new MenuItemDefinition<UndoCommandDefinition>("Undo", 0, EditMenu);

        [Export] public static MenuItemDefinition Redo = new MenuItemDefinition<RedoCommandDefinition>("Redo", 1, EditMenu);

        [Export]
        public static MenuItemDefinition EditMenuSeparator = new MenuItemDefinition("Separator2", 2, EditMenu, true);




        [Export] public static MenuItemDefinition ViewMenu = new MenuItemDefinition("_View", 2);

        [Export]

        public static MenuItemDefinition ViewMenuSeparator = new MenuItemDefinition("Separator", 9, ViewMenu, true);

        [Export] public static MenuItemDefinition FullScreen =
            new MenuItemDefinition<FullScreenCommandDefinition>("Full Screen", 10, ViewMenu);

        [Export] public static MenuItemDefinition ToolsMenu = new MenuItemDefinition("_Tools", 5);

        [Export]
        public static MenuItemDefinition Settings = new MenuItemDefinition<OpenSettingsCommandDefinition>("Settings", int.MaxValue, ToolsMenu);

        [Export] public static MenuItemDefinition HelpMenu = new MenuItemDefinition("_Help", int.MaxValue);
    }
}
