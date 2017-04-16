using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public sealed class CommandBarSeparatorDefinition : CommandBarItemDefinition
    {
        public static CommandBarSeparatorDefinition SeparatorDefinition => new CommandBarSeparatorDefinition();

        private CommandBarSeparatorDefinition() : base(null, uint.MaxValue, null, new SeparatorCommandDefinition(), true, false, false, false)
        {
        }

        private class SeparatorCommandDefinition : DefinitionBase
        {
            public override string Name => null;
            public override string Text => null;
            public override string ToolTip => null;
            public override Uri IconSource => null;
            public override string IconId => null;
            public override bool IsList => false;
            public override CommandCategory Category => null;
            public override CommandControlTypes ControlType => CommandControlTypes.Separator;
            public sealed override string ShortcutText { get; set; }
        }
    }
}