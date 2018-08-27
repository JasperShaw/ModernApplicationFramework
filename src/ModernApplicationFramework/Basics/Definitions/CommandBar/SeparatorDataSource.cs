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
    internal sealed class SeparatorDataSource : CommandBarItemDataSource
    {
        /// <summary>
        /// Returns a new instance of a separator command bar item
        /// </summary>
        public static SeparatorDataSource NewInstance => new SeparatorDataSource();

        public override Guid Id => Guid.Empty;

        public override CommandControlTypes UiType => CommandControlTypes.Separator;

        private SeparatorDataSource() : base(null, uint.MaxValue, null, null, false)
        {
        }
    }
}