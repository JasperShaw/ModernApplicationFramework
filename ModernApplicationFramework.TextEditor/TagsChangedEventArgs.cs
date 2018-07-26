using System;

namespace ModernApplicationFramework.TextEditor
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