using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public class TrackingTagSpan<T> where T : ITag
    {
        public ITrackingSpan Span { get; }
        public T Tag { get; }

        public TrackingTagSpan(ITrackingSpan span, T tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            Span = span ?? throw new ArgumentNullException(nameof(span));
            Tag = tag;
        }
    }
}