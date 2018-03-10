using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.EditorBase.Commands;

namespace ModernApplicationFramework.EditorBase.CommandBarDefinitions.ContextMenuDefinitions
{
    public static class TabbedDocumentContextMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition CloseCommandItemDefinition =
            new CommandBarCommandItemDefinition<SaveActiveFileCommandDefinition>(
                new Guid("{82C6BF8D-8040-4250-89AE-DF400DC97CE0}"),
                Docking.ContextMenuDefinitions.DocumentContextMenuDefinition.DocumentCloseContextMenuGroup, 0, true, false, false, true);


        [Export] public static CommandBarGroupDefinition DocumentFileContextMenuGroup =
            new CommandBarGroupDefinition(
                Docking.ContextMenuDefinitions.DocumentContextMenuDefinition.DocumentContextMenu, 1);

        [Export]
        public static CommandBarItemDefinition CopyFilePathDefinition =
            new CommandBarCommandItemDefinition<CopyFullPathCommandDefinition>(
                new Guid("{88BF0F31-02A0-45A0-8429-1B9E2FD7E812}"), DocumentFileContextMenuGroup, 0, true, false, false, true);

        [Export]
        public static CommandBarItemDefinition OpenContainingFolderDefinition =
            new CommandBarCommandItemDefinition<OpenContainingFolderCommandDefinition>(
                new Guid("{EE2AF1FE-8107-4412-99E5-0128870D6B05} "), DocumentFileContextMenuGroup, 1, true, false, false, true);
    }
}
