using System;
using System.Collections.Generic;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Layout
{
    public class LayoutItemsClosedEventArgs : EventArgs
    {
        public IEnumerable<ILayoutItem> LayoutItems { get; }

        public LayoutItemsClosedEventArgs(IEnumerable<ILayoutItem> layoutItems)
        {
            LayoutItems = layoutItems;
        }
    }
}