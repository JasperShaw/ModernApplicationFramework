using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Docking.CommandDefinitions;

namespace ModernApplicationFramework.Docking.ContextMenuDefinitions
{
    public class AnchorableContextMenuDefinition
    {
        [Export] public static ContextMenuDefinition AnchorableContextMenu =
            new ContextMenuDefinition(new Guid("{B8615A48-C325-46ED-9F3D-135A1DF2E6D6}"), ContextMenuCategory.OtherContextMenusCategory, DockingResources.AnchorableContextMenu_Name);

        [Export] public static CommandBarGroupDefinition AnchorableContextMenuGroup =
            new CommandBarGroupDefinition(AnchorableContextMenu, uint.MinValue);

        [Export] public static CommandBarItemDefinition FloatCommandItemDefinition =
                new CommandBarCommandItemDefinition<FloatDockedWindowCommandDefinition>(new Guid("{BA3AB1EE-7A35-4E8D-9B77-6EFAE261CE2C}"), AnchorableContextMenuGroup, 0)
            ;

        [Export] public static CommandBarItemDefinition DockCommandItemDefinition =
            new CommandBarCommandItemDefinition<DockWindowCommandDefinition>(new Guid("{41C19548-2E82-4ACD-AE21-6097B811B9F2}"), AnchorableContextMenuGroup, 1);

        [Export] public static CommandBarItemDefinition DockAsTabbedCommandItemDefinition =
            new CommandBarCommandItemDefinition<DockAsTabbedDocumentCommandDefinition>(new Guid("{9B0C7432-5B92-4982-ADB3-2D27A1A5817C}"), AnchorableContextMenuGroup, 2);

        [Export] public static CommandBarItemDefinition AutoHideWindowCommandItemDefinition =
            new CommandBarCommandItemDefinition<AutoHideWindowCommandDefinition>(new Guid("{7F7181B2-BECE-4589-A347-13D3C539A58E}"), AnchorableContextMenuGroup, 3);

        [Export] public static CommandBarItemDefinition HideCommandItemDefinition =
            new CommandBarCommandItemDefinition<HideDockedWindowCommandDefinition>(new Guid("{F074C243-8AE2-49EF-AF3B-D4334BDC73C4}"), AnchorableContextMenuGroup, 4);
    }
}