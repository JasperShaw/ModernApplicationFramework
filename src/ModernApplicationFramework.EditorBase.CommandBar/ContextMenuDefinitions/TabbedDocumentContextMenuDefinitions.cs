using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions;

namespace ModernApplicationFramework.EditorBase.CommandBar.ContextMenuDefinitions
{
    public static class TabbedDocumentContextMenuDefinitions
    {
        [Export] public static CommandBarItemDataSource CloseCommandItem =
            new CommandBarCommandItemDataSource<SaveActiveFileCommandDefinition>(
                new Guid("{82C6BF8D-8040-4250-89AE-DF400DC97CE0}"),
                Docking.ContextMenuDefinitions.DocumentContextMenuDefinition.DocumentCloseContextMenuGroup, 0);


        [Export] public static CommandBarGroup DocumentFileContextMenuGroup =
            new CommandBarGroup(
                Docking.ContextMenuDefinitions.DocumentContextMenuDefinition.DocumentContextMenu, 1);

        [Export]
        public static CommandBarItemDataSource CopyFilePathItem =
            new CommandBarCommandItemDataSource<CopyFullPathCommandDefinition>(
                new Guid("{88BF0F31-02A0-45A0-8429-1B9E2FD7E812}"), DocumentFileContextMenuGroup, 0);

        [Export]
        public static CommandBarItemDataSource OpenContainingFolderItem =
            new CommandBarCommandItemDataSource<OpenContainingFolderCommandDefinition>(
                new Guid("{EE2AF1FE-8107-4412-99E5-0128870D6B05} "), DocumentFileContextMenuGroup, 1);
    }
}
