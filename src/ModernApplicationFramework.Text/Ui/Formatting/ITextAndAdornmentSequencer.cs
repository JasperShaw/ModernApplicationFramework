using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Text.Ui.Formatting
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