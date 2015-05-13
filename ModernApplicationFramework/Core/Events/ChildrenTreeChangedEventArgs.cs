using System;
using ModernApplicationFramework.Core.Platform;

namespace ModernApplicationFramework.Core.Events
{
    public class ChildrenTreeChangedEventArgs : EventArgs
    {
        public ChildrenTreeChange Change { get; private set; }
        public ChildrenTreeChangedEventArgs(ChildrenTreeChange change)
        {
            Change = change;
        }
    }
}
