using System.Collections.Generic;
using System.ComponentModel;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Layout
{
    public class LayoutItemsClosingEventArgs : CancelEventArgs
    {
        public IEnumerable<ILayoutItem> LayoutItems { get; }

        public LayoutItemsClosingEventArgs(IEnumerable<ILayoutItem> layoutItems)
        {
            LayoutItems = layoutItems;
        }
    }
}