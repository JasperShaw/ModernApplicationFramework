using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Ui.Outlining
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