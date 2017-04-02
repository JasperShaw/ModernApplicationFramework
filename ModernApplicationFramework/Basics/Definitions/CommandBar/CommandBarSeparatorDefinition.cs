using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public sealed class CommandBarSeparatorDefinition : CommandBarDefinitionBase
    {
        public static CommandBarSeparatorDefinition MenuSeparatorDefinition => new CommandBarSeparatorDefinition();

        private CommandBarSeparatorDefinition() : base(null, uint.MinValue, new SeparatorCommandDefinition(), false, false)
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
            public sealed override string ShortcutText { get; set; }
            public override CommandControlTypes ControlType => CommandControlTypes.Separator;
        }
    }
}