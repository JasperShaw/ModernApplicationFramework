using System.ComponentModel.Composition;
using ModernApplicationFramework.MVVM.Commands;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Controls.MenuDefinitions
{
    public static class MenuDefinitions
    {
        [Export] public static MenuItemDefinition FileMenu = new MenuItemDefinition("File", 0);


        [Export] public static MenuItemDefinition SubMenuNew = new MenuItemDefinition("New", 0, FileMenu);

        [Export]
        public static MenuItemDefinition SubMenuItemNewFile =
            new MenuItemDefinition<NewFileCommandDefinition>("New File", 4, SubMenuNew);

        [Export]
        public static MenuItemDefinition SubMenuOpen = new MenuItemDefinition("Open", 1, FileMenu);

        [Export]
        public static MenuItemDefinition SubMenuItemOpenFile =
            new MenuItemDefinition<OpenFileCommandDefinition>("New File", 4, SubMenuOpen);

        [Export] public static MenuItemDefinition FileMenuSeparator = new MenuItemDefinition("Separator", 2, FileMenu);

  
        [Export] public static MenuItemDefinition SubItemSave = new MenuItemDefinition<SaveFileCommandDefinition>(
            "Save", 3, FileMenu);

        [Export]
        public static MenuItemDefinition SubItemSaveAs = new MenuItemDefinition<SaveFileAsCommandDefinition>(
            "Save As", 4, FileMenu);

        [Export]
        public static MenuItemDefinition SaveAll = new MenuItemDefinition<SaveAllFilesCommandDefinition>("Save All", 5, FileMenu);

        [Export]
        public static MenuItemDefinition CloseProgramm = new MenuItemDefinition<CloseProgammCommandDefinition>("Close", int.MaxValue, FileMenu);



        [Export] public static MenuItemDefinition EditMenu = new MenuItemDefinition("Edit", 1);

        [Export] public static MenuItemDefinition Undo = new MenuItemDefinition<UndoCommandDefinition>("Undo", 0, EditMenu);

        [Export] public static MenuItemDefinition Redo = new MenuItemDefinition<RedoCommandDefinition>("Redo", 1, EditMenu);




        [Export] public static MenuItemDefinition ViewMenu = new MenuItemDefinition("View", 2);

        [Export] public static MenuItemDefinition ToolsMenu = new MenuItemDefinition("Tools", 5);

        [Export]
        public static MenuItemDefinition Settings = new MenuItemDefinition<OpenSettingsCommandDefinition>("Settings", int.MaxValue, ToolsMenu);

        [Export] public static MenuItemDefinition HelpMenu = new MenuItemDefinition("Help", int.MaxValue);
    }
}
