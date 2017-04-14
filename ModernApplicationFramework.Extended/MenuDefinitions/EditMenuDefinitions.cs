using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class EditMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition EditMenu =
            new MenuDefinition(MainMenuBarDefinition.MainMenuBarGroup, 1, "&Edit");

        [Export] public static CommandBarGroupDefinition EditUndoRedoMenuGroup =
            new CommandBarGroupDefinition(EditMenu, 0);

        [Export] public static CommandBarItemDefinition EditUndoMenuItem =
            new CommandBarCommandItemDefinition<UndoCommandDefinition>(EditUndoRedoMenuGroup, 0);

        [Export] public static CommandBarItemDefinition EditRedoMenuItem =
            new CommandBarCommandItemDefinition<RedoCommandDefinition>(EditUndoRedoMenuGroup, 0);
    }
}