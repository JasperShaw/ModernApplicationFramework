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
}