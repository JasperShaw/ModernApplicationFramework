using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor
{
    public class RegionsCollapsedEventArgs : EventArgs
    {
        public IEnumerable<ICollapsed> CollapsedRegions { get; }

        public RegionsCollapsedEventArgs(IEnumerable<ICollapsed> collapsedRegions)
        {
            CollapsedRegions = collapsedRegions;
        }
    }
}