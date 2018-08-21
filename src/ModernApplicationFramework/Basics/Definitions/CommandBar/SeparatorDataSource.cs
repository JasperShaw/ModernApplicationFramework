using System;
using System.Diagnostics;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc />
    /// <summary>
    /// A special command bar item definition, representing an separator
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarItemDefinition" />
    [DebuggerDisplay("<Separator>")]
    public sealed class SeparatorDataSource : CommandBarItemDataSource
    {
        /// <summary>
        /// Returns a new instance of a separator command bar item
        /// </summary>
        public static SeparatorDataSource NewInstance => new SeparatorDataSource();

        public override Guid Id => Guid.Empty;

        private SeparatorDataSource() : base(null, uint.MaxValue, null, new SeparatorCommandDefinition(),
            true, false, false, false)
        {
        }

        private class SeparatorCommandDefinition : CommandDefinitionBase
        {
            public override string Name => null;
            public override string NameUnlocalized => null;
            public override string Text => null;
            public override string ToolTip => null;
            public override bool IsList => false;
            public override CommandCategory Category => null;
            public override CommandControlTypes ControlType => CommandControlTypes.Separator;
            public override Guid Id => new Guid("{7951DD77-A074-4489-9037-8CEEEC630C5C}");
        }
    }
}