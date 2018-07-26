using System;

namespace ModernApplicationFramework.TextEditor
{
    public class MappingTagSpan<T> : IMappingTagSpan<T> where T : ITag
    {
        public T Tag { get; }

        public IMappingSpan Span { get; }

        public MappingTagSpan(IMappingSpan span, T tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            Span = span ?? throw new ArgumentNullException(nameof(span));
            Tag = tag;
        }
    }
}