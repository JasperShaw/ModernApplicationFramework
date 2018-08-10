using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public class TagSpan<T> : ITagSpan<T> where T : ITag
    {
        public SnapshotSpan Span { get; }
        public T Tag { get; }

        public TagSpan(SnapshotSpan span, T tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            Span = span;
            Tag = tag;
        }
    }
}