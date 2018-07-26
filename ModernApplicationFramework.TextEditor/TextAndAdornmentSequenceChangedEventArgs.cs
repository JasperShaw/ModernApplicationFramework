using System;

namespace ModernApplicationFramework.TextEditor
{
    public class TextAndAdornmentSequenceChangedEventArgs : EventArgs
    {
        public IMappingSpan Span { get; }

        public TextAndAdornmentSequenceChangedEventArgs(IMappingSpan span)
        {
            Span = span ?? throw new ArgumentNullException(nameof(span));
        }
    }
}