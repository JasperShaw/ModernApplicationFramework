using System;

namespace ModernApplicationFramework.Core.Events
{
    public class ItemDoubleClickedEventArgs : EventArgs
    {
        public object Item { get;}

        public ItemDoubleClickedEventArgs(object extension)
        {
            Item = extension;
        }
    }
}
