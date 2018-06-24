using System;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Layout
{
    public class LayoutChangeEventArgs : EventArgs
    {
        public ILayoutItem OldLayoutItem { get; }
        public ILayoutItem NewLayoutItem { get; }

        public LayoutChangeEventArgs(ILayoutItem oldLayoutItem, ILayoutItem newLayoutItem)
        {
            OldLayoutItem = oldLayoutItem;
            NewLayoutItem = newLayoutItem;
        }
    }

    public class LayoutBaselChangeEventArgs : EventArgs
    {
        public ILayoutItemBase OldLayoutItem { get; }
        public ILayoutItemBase NewLayoutItem { get; }

        public LayoutBaselChangeEventArgs(ILayoutItemBase oldLayoutItem, ILayoutItemBase newLayoutItem)
        {
            OldLayoutItem = oldLayoutItem;
            NewLayoutItem = newLayoutItem;
        }
    }
}