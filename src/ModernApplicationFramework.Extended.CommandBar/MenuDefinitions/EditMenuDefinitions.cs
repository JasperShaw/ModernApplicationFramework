using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBarDefinitions;

namespace ModernApplicationFramework.Extended.CommandBar.MenuDefinitions
{
    public static class EditMenuDefinitions
    {
        [Export] public static CommandBarGroupDefinition EditUndoRedoMenuGroup =
            new CommandBarGroupDefinition(TopLevelMenuDefinitions.EditMenu, 1);

        [Export] public static CommandBarItemDefinition EditUndoMenuItem =
            new CommandBarCommandItemDefinition<UndoCommandDefinition>(
                new Guid("{809F1237-6427-4F30-83F0-C64C9AC2DD24}"), EditUndoRedoMenuGroup, 0);

        [Export] public static CommandBarItemDefinition EditRedoMenuItem =
            new CommandBarCommandItemDefinition<RedoCommandDefinition>(
                new Guid("{F41D3A9C-90CA-41EE-BEE3-D3283BEF1D52}"), EditUndoRedoMenuGroup, 0);



        [Export]
        public static CommandBarGroupDefinition BasicEditGroup =
            new CommandBarGroupDefinition(TopLevelMenuDefinitions.EditMenu, 2);

        [Export]
        public static CommandBarItemDefinition DeleteItem =
            new CommandBarCommandItemDefinition<DeleteCommandDefinition>(
                new Guid("{156C413A-93BE-48C7-8219-B38D23013E53}"), BasicEditGroup, int.MaxValue, true, false, false, true);

        [Export]
        public static CommandBarItemDefinition CutMenuItem =
            new CommandBarCommandItemDefinition<CutCommandDefinition>(
                new Guid("{AA71F39E-6485-4142-A453-FF0D1CE1C7E6}"), BasicEditGroup, 0);

        [Export]
        public static CommandBarItemDefinition CopyMenuItem =
            new CommandBarCommandItemDefinition<CopyCommandDefinition>(
                new Guid("{0B5512CB-1DB5-4CBF-B7B6-A87DAF3B0322}"), BasicEditGroup, 1);
    }
}