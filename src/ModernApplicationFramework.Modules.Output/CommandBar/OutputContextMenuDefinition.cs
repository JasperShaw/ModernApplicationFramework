using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;

namespace ModernApplicationFramework.Modules.Output.CommandBar
{
    public static class OutputContextMenuDefinition
    {
        [Export]
        public static CommandBarItem ToolboxContextMenu =
            new CommandBarConxtexMenu(new Guid("{18E35741-10B1-47C0-87F0-83058900B907}"), ContextMenuCategory.OtherContextMenusCategory, "Output Window");

        [Export]
        public static CommandBarGroup CopyGroup =
            new CommandBarGroup(ToolboxContextMenu, uint.MinValue);

        [Export]
        public static CommandBarItem CopyCommandItem =
            new CommandBarCommandItem<CopyCommandDefinition>(new Guid("{C18EA117-A73C-4AA2-AD78-C05DE2711B94}"), CopyGroup, 0);
    }
}
