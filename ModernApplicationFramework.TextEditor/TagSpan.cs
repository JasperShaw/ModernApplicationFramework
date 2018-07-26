using System;

namespace ModernApplicationFramework.TextEditor
{
    public class TagSpan<T> : ITagSpan<T> where T : ITag
    {
        public T Tag { get; }

        public SnapshotSpan Span { get; }

        public TagSpan(SnapshotSpan span, T tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            Span = span;
            Tag = tag;
        }
    }
}