using System;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Layout
{
    public class LayoutDeactivateEventArgs : EventArgs
    {
        public ILayoutItem Item { get; }

        public bool Close { get; }

        public LayoutDeactivateEventArgs(ILayoutItem item, bool close)
        {
            Item = item;
            Close = close;
        }
    }
}