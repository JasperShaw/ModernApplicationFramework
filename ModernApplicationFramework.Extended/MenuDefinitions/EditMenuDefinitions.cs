using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class EditMenuDefinitions
    {
        [Export] public static MenuItemDefinition EditMenu = new MenuItemDefinition("_Edit", 1);

        [Export] public static MenuItemDefinition Undo = new MenuItemDefinition<UndoCommandDefinition>("Undo", 0, EditMenu);

        [Export] public static MenuItemDefinition Redo = new MenuItemDefinition<RedoCommandDefinition>("Redo", 1, EditMenu);
    }
}
