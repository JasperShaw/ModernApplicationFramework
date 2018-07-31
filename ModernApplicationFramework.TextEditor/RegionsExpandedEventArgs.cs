using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public class RegionsExpandedEventArgs : EventArgs
    {
        public IEnumerable<ICollapsible> ExpandedRegions { get; }

        public bool RemovalPending { get; }

        public RegionsExpandedEventArgs(IEnumerable<ICollapsible> expandedRegions)
            : this(expandedRegions, false)
        {
        }

        public RegionsExpandedEventArgs(IEnumerable<ICollapsible> expandedRegions, bool removalPending)
        {
            ExpandedRegions = expandedRegions;
            RemovalPending = removalPending;
        }
    }
}