using System;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Core.Events
{
    public class ItemDoubleClickedEventArgs : EventArgs
    {
        public object Extension { get;}

        public ItemDoubleClickedEventArgs(object extension)
        {
            Extension = extension;
        }
    }
}
