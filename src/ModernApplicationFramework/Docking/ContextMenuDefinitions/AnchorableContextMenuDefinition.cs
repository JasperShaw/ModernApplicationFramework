using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Docking.CommandDefinitions;

namespace ModernApplicationFramework.Docking.ContextMenuDefinitions
{
    public class AnchorableContextMenuDefinition
    {
        [Export] public static ContextMenuDefinition AnchorableContextMenu =
            new ContextMenuDefinition(ContextMenuCategory.OtherContextMenusCategory, DockingResources.AnchorableContextMenu_Name);

        [Export] public static CommandBarGroupDefinition AnchorableContextMenuGroup =
            new CommandBarGroupDefinition(AnchorableContextMenu, uint.MinValue);

        [Export] public static CommandBarItemDefinition FloatCommandItemDefinition =
                new CommandBarCommandItemDefinition<FloatDockedWindowCommandDefinition>(AnchorableContextMenuGroup, 0)
            ;

        [Export] public static CommandBarItemDefinition DockCommandItemDefinition =
            new CommandBarCommandItemDefinition<DockWindowCommandDefinition>(AnchorableContextMenuGroup, 1);

        [Export] public static CommandBarItemDefinition DockAsTabbedCommandItemDefinition =
            new CommandBarCommandItemDefinition<DockAsTabbedDocumentCommandDefinition>(AnchorableContextMenuGroup, 2);

        [Export] public static CommandBarItemDefinition AutoHideWindowCommandItemDefinition =
            new CommandBarCommandItemDefinition<AutoHideWindowCommandDefinition>(AnchorableContextMenuGroup, 3);

        [Export] public static CommandBarItemDefinition HideCommandItemDefinition =
            new CommandBarCommandItemDefinition<HideDockedWindowCommandDefinition>(AnchorableContextMenuGroup, 4);
    }
}