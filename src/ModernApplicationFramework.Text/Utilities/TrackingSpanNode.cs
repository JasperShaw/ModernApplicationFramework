using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Utilities
{
    public sealed class TrackingSpanNode<T>
    {
        public List<TrackingSpanNode<T>> Children { get; }

        public T Item { get; }

        public ITrackingSpan TrackingSpan { get; }

        public TrackingSpanNode(T item, ITrackingSpan trackingSpan)
            : this(item, trackingSpan, new List<TrackingSpanNode<T>>())
        {
        }

        public TrackingSpanNode(T item, ITrackingSpan trackingSpan, List<TrackingSpanNode<T>> children)
        {
            Item = item;
            TrackingSpan = trackingSpan;
            Children = children;
        }

        internal void Advance(ITextVersion toVersion)
        {
            TrackingSpan?.GetSpan(toVersion);
            foreach (var child in Children)
                child.Advance(toVersion);
        }
    }
}