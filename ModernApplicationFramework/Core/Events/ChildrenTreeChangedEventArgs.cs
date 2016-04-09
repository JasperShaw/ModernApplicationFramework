using System;
using ModernApplicationFramework.Core.Platform;

namespace ModernApplicationFramework.Core.Events
{
    public class ChildrenTreeChangedEventArgs : EventArgs
    {
        public ChildrenTreeChangedEventArgs(ChildrenTreeChange change)
        {
            Change = change;
        }

        public ChildrenTreeChange Change { get; private set; }
    }
}