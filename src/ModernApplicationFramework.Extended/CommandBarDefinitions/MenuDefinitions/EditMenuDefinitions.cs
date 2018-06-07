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
            new CommandBarGroupDefinition(EditMenu, 1);

        [Export] public static CommandBarItemDefinition EditUndoMenuItem =
            new CommandBarCommandItemDefinition<UndoCommandDefinition>(
                new Guid("{809F1237-6427-4F30-83F0-C64C9AC2DD24}"), EditUndoRedoMenuGroup, 0);

        [Export] public static CommandBarItemDefinition EditRedoMenuItem =
            new CommandBarCommandItemDefinition<RedoCommandDefinition>(
                new Guid("{F41D3A9C-90CA-41EE-BEE3-D3283BEF1D52}"), EditUndoRedoMenuGroup, 0);

        [Export]
        public static CommandBarGroupDefinition BasicEditGroup =
            new CommandBarGroupDefinition(EditMenu, 2);

        [Export]
        public static CommandBarItemDefinition DeleteItem =
            new CommandBarCommandItemDefinition<DeleteCommandDefinition>(
                new Guid("{156C413A-93BE-48C7-8219-B38D23013E53}"), BasicEditGroup, 0);

        //[Export]
        //public static CommandBarItemDefinition CopyMenuItem =
        //    new CommandBarCommandItemDefinition<CopyCommandDefinition>(
        //        new Guid("{0B5512CB-1DB5-4CBF-B7B6-A87DAF3B0322}"), EditUndoRedoMenuGroup, 1);
    }
}