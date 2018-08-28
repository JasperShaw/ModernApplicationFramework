using System;
using System.Collections.Generic;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.DataSources;

namespace ModernApplicationFramework.Core.Comparers
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="IComparer{T}"/> that compared two <see cref="CommandBarDataSource"/> by name
    /// </summary>
    /// <seealso cref="T:System.Collections.IComparer" />s
    public class SortOrderComparer<TCommandBarDefinitionBase> : IComparer<ISortable>
    {
        public int Compare(ISortable x, ISortable y)
        {
            if (!(x is TCommandBarDefinitionBase) || !(y is TCommandBarDefinitionBase))
                throw new ArgumentException("This converter does not support the provided objects");

            var sortOrder1 = x.SortOrder;
            var sortOrder2 = y.SortOrder;

            return sortOrder1.CompareTo(sortOrder2);
        }
    }
}
