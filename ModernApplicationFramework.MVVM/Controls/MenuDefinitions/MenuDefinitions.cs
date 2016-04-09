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
            "Save", 0, FileMenu);

        [Export]
        public static MenuItemDefinition SubItemSaveAs = new MenuItemDefinition<SaveFileAsCommandDefinition>(
            "Save As", 0, FileMenu);



        [Export] public static MenuItemDefinition Edit = new MenuItemDefinition("Edit", 1);

        [Export] public static MenuItemDefinition Undo = new MenuItemDefinition<UndoCommandDefinition>("Test", 0, Edit);

        [Export] public static MenuItemDefinition Redo = new MenuItemDefinition<RedoCommandDefinition>("Test", 1, Edit);
    }
}
