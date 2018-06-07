using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    public static class ToolboxContextMenuDefinition
    {
        [Export]
        public static ContextMenuDefinition ToolboxContextMenu =
            new ContextMenuDefinition(new Guid("{70967216-21B8-454D-8361-201F127A6D32}"), ContextMenuCategory.OtherContextMenusCategory, "Toolbox");

        [Export]
        public static CommandBarGroupDefinition BasicEditGroup =
            new CommandBarGroupDefinition(ToolboxContextMenu, uint.MinValue);

        [Export]
        public static CommandBarItemDefinition CloseCommandItemDefinition =
            new CommandBarCommandItemDefinition<DeleteCommandDefinition>(new Guid("{180D3794-7914-4B4C-BD5E-F4A101FAF831}"), BasicEditGroup, 1);
    }
}
