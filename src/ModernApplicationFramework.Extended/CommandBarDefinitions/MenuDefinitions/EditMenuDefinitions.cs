using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.CommandBarDefinitions.MenuDefinitions
{
    public static class EditMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition EditMenu =
            new MenuDefinition(new Guid("{37DAAE85-3F83-4A99-8AD1-DF2F037C7DD4}"),
                MainMenuBarDefinition.MainMenuBarGroup, 1, CommandBar_Resources.MenuEdit_Name);

        [Export] public static CommandBarGroupDefinition EditUndoRedoMenuGroup =
            new CommandBarGroupDefinition(EditMenu, 0);

        [Export] public static CommandBarItemDefinition EditUndoMenuItem =
            new CommandBarCommandItemDefinition<UndoCommandDefinition>(
                new Guid("{809F1237-6427-4F30-83F0-C64C9AC2DD24}"), EditUndoRedoMenuGroup, 0);

        [Export] public static CommandBarItemDefinition EditRedoMenuItem =
            new CommandBarCommandItemDefinition<RedoCommandDefinition>(
                new Guid("{F41D3A9C-90CA-41EE-BEE3-D3283BEF1D52}"), EditUndoRedoMenuGroup, 0);
    }
}