using System;
using System.Collections.Generic;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Core.Comparers
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="IComparer{T}"/> that compared two <see cref="CommandBarDefinitionBase"/> by name
    /// </summary>
    /// <seealso cref="T:System.Collections.IComparer" />s
    public class SortOrderComparer<TCommandBarDefinitionBase> : IComparer<CommandBarDefinitionBase>
    {
        public int Compare(CommandBarDefinitionBase x, CommandBarDefinitionBase y)
        {
            if (!(x is TCommandBarDefinitionBase) || !(y is TCommandBarDefinitionBase))
                throw new ArgumentException("This converter does not support the provided objects");

            var sortOrder1 = x.SortOrder;
            var sortOrder2 = y.SortOrder;

            return sortOrder1.CompareTo(sortOrder2);
        }
    }
}
