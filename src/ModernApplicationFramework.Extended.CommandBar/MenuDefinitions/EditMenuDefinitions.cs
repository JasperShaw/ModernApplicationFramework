using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBarDefinitions;

namespace ModernApplicationFramework.Extended.CommandBar.MenuDefinitions
{
    public static class EditMenuDefinitions
    {
        [Export] public static CommandBarGroup EditUndoRedoMenuGroup =
            new CommandBarGroup(TopLevelMenuDefinitions.EditMenu, 1);

        [Export] public static CommandBarItem EditUndoMenuItem =
            new CommandBarCommandItem<UndoCommandDefinition>(
                new Guid("{809F1237-6427-4F30-83F0-C64C9AC2DD24}"), EditUndoRedoMenuGroup, 0);

        [Export] public static CommandBarItem EditRedoMenuItem =
            new CommandBarCommandItem<RedoCommandDefinition>(
                new Guid("{F41D3A9C-90CA-41EE-BEE3-D3283BEF1D52}"), EditUndoRedoMenuGroup, 0);



        [Export]
        public static CommandBarGroup BasicEditGroup =
            new CommandBarGroup(TopLevelMenuDefinitions.EditMenu, 2);

        [Export]
        public static CommandBarItem DeleteItem =
            new CommandBarCommandItem<DeleteCommandDefinition>(
                new Guid("{156C413A-93BE-48C7-8219-B38D23013E53}"), BasicEditGroup, int.MaxValue);

        [Export]
        public static CommandBarItem CutMenuItem =
            new CommandBarCommandItem<CutCommandDefinition>(
                new Guid("{AA71F39E-6485-4142-A453-FF0D1CE1C7E6}"), BasicEditGroup, 0);

        [Export]
        public static CommandBarItem CopyMenuItem =
            new CommandBarCommandItem<CopyCommandDefinition>(
                new Guid("{0B5512CB-1DB5-4CBF-B7B6-A87DAF3B0322}"), BasicEditGroup, 1);

        [Export]
        public static CommandBarItem PasteMenuItem =
            new CommandBarCommandItem<PasteCommandDefinition>(
                new Guid("{D89F2DB8-BA86-4835-9D31-228862980304}"), BasicEditGroup, 2);
    }
}