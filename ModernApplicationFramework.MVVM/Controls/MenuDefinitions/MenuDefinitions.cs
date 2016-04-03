using System.ComponentModel.Composition;
using ModernApplicationFramework.MVVM.Commands;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Controls.MenuDefinitions
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition FileMenu = new MenuItemDefinition("File", 0);

        [Export]
        public static MenuItemDefinition Edit = new MenuItemDefinition("Edit", 1);

        [Export]
        public static MenuItemDefinition Undo = new MenuItemDefinition<UndoCommandDefinition>("Test", 0, Edit);

        [Export]
        public static MenuItemDefinition Redo = new MenuItemDefinition<RedoCommandDefinition>("Test", 1, Edit);
    }
}
