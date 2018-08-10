using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Tagging
{
    public class TagsChangedEventArgs : EventArgs
    {
        public IMappingSpan Span { get; }

        public TagsChangedEventArgs(IMappingSpan span)
        {
            Span = span ?? throw new ArgumentNullException(nameof(span));
        }
    }
}