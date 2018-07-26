using System;
using ModernApplicationFramework.TextEditor.Text;

namespace ModernApplicationFramework.TextEditor
{
    public interface ITextAndAdornmentSequencer
    {
        IBufferGraph BufferGraph { get; }

        ITextBuffer TopBuffer { get; }

        ITextBuffer SourceBuffer { get; }

        ITextAndAdornmentCollection CreateTextAndAdornmentCollection(ITextSnapshotLine topLine, ITextSnapshot sourceTextSnapshot);

        ITextAndAdornmentCollection CreateTextAndAdornmentCollection(SnapshotSpan topSpan, ITextSnapshot sourceTextSnapshot);

        event EventHandler<TextAndAdornmentSequenceChangedEventArgs> SequenceChanged;
    }
}