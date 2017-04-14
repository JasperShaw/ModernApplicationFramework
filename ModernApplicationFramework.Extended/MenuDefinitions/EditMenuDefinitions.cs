using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class EditMenuDefinitions
    {
        [Export] public static MenuDefinition EditMenu = new MenuDefinition(MainMenuBarDefinition.MainMenuBar,1,"&Edit");

        [Export] public static CommandBarGroupDefinition EditUndoRedoMenuGroup = new CommandBarGroupDefinition(EditMenu, 0);

        [Export] public static CommandBarItemDefinition EditUndoMenuItem = new CommandMenuItemDefinition<UndoCommandDefinition>(EditUndoRedoMenuGroup, 0);

        [Export] public static CommandBarItemDefinition EditRedoMenuItem = new CommandMenuItemDefinition<RedoCommandDefinition>(EditUndoRedoMenuGroup, 0);
    }
}
