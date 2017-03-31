using System;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public sealed class CommandBarSeparatorDefinition : CommandBarDefinitionBase
    {
        public override uint SortOrder { get; set; }
        public override string Text { get; set; } = null;
        public override bool IsCustom => false;
        public override bool IsChecked { get; set; }
        public override DefinitionBase CommandDefinition { get; }

        public CommandBarSeparatorDefinition()
        {
            CommandDefinition = new SeparatorCommandDefinition();
        }

        private class SeparatorCommandDefinition : DefinitionBase
        {
            public override string Name => null;
            public override string Text => null;
            public override string ToolTip => null;
            public override Uri IconSource => null;
            public override string IconId => null;
            public override bool IsList => false;
            public override CommandControlTypes ControlType => CommandControlTypes.Separator;
        }
    }
}
