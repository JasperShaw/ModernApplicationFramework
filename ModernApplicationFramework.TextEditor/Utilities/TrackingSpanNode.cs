using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor.Utilities
{
    public sealed class TrackingSpanNode<T>
    {
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

        public T Item { get; }

        public ITrackingSpan TrackingSpan { get; }

        public List<TrackingSpanNode<T>> Children { get; }

        internal void Advance(ITextVersion toVersion)
        {
            TrackingSpan?.GetSpan(toVersion);
            foreach (TrackingSpanNode<T> child in Children)
                child.Advance(toVersion);
        }
    }
}