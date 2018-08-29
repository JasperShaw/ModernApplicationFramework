using System;
using System.Collections.Generic;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking
{
    public class AnchorablesClosedEventArgs : EventArgs
    {
        public IEnumerable<LayoutAnchorable> Anchors { get; }
        public AnchorableCloseMode Mode { get; }

        public AnchorablesClosedEventArgs(IEnumerable<LayoutAnchorable> anchors, AnchorableCloseMode mode)
        {
            Anchors = anchors;
            Mode = mode;
        }
    }
}