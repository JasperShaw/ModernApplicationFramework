using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Formatting
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