using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal class Collapsed : Collapsible, ICollapsed
    {
        public TrackingSpanNode<Collapsed> Node { get; set; }

        public bool IsValid => IsCollapsed;

        public IEnumerable<ICollapsed> CollapsedChildren
        {
            get
            {
                if (!IsValid)
                    throw new InvalidOperationException("This collapsed region is no longer valid, as it has been expanded already.");
                return Node.Children.Select(child => child.Item);
            }
        }

        public void Invalidate()
        {
            IsCollapsed = false;
            Node = null;
        }

        public Collapsed(ITrackingSpan extent, IOutliningRegionTag tag)
            : base(extent, tag)
        {
            IsCollapsed = true;
        }
    }
}