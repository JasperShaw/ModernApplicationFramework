using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public class MappingTagSpan<T> : IMappingTagSpan<T> where T : ITag
    {
        public IMappingSpan Span { get; }
        public T Tag { get; }

        public MappingTagSpan(IMappingSpan span, T tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));
            Span = span ?? throw new ArgumentNullException(nameof(span));
            Tag = tag;
        }
    }
}