using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Outlining;
using ModernApplicationFramework.Text.Ui.Tagging;
using ModernApplicationFramework.Text.Utilities;

namespace ModernApplicationFramework.Modules.Editor.Outlining
{
    internal class Collapsed : Collapsible, ICollapsed
    {
        public IEnumerable<ICollapsed> CollapsedChildren
        {
            get
            {
                if (!IsValid)
                    throw new InvalidOperationException(
                        "This collapsed region is no longer valid, as it has been expanded already.");
                return Node.Children.Select(child => child.Item);
            }
        }

        public bool IsValid => IsCollapsed;
        public TrackingSpanNode<Collapsed> Node { get; set; }

        public Collapsed(ITrackingSpan extent, IOutliningRegionTag tag)
            : base(extent, tag)
        {
            IsCollapsed = true;
        }

        public void Invalidate()
        {
            IsCollapsed = false;
            Node = null;
        }
    }
}