using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Core.Comparers
{
    public class SortOrderComparer<TCommandBarDefinitionBase> : IComparer<Basics.Definitions.CommandBar.CommandBarDefinitionBase>
    {
        public int Compare(Basics.Definitions.CommandBar.CommandBarDefinitionBase x, Basics.Definitions.CommandBar.CommandBarDefinitionBase y)
        {
            if (!(x is TCommandBarDefinitionBase) || !(y is TCommandBarDefinitionBase))
                throw new ArgumentException("This converter does not support the provided objects");

            var sortOrder1 = x.SortOrder;
            var sortOrder2 = y.SortOrder;

            return sortOrder1.CompareTo(sortOrder2);
        }
    }
}
