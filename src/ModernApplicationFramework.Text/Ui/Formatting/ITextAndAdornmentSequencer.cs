using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Text.Ui.Formatting
{
    public interface ITextAndAdornmentSequencer
    {
        event EventHandler<TextAndAdornmentSequenceChangedEventArgs> SequenceChanged;
        IBufferGraph BufferGraph { get; }

        ITextBuffer SourceBuffer { get; }

        ITextBuffer TopBuffer { get; }

        ITextAndAdornmentCollection CreateTextAndAdornmentCollection(ITextSnapshotLine topLine,
            ITextSnapshot sourceTextSnapshot);

        ITextAndAdornmentCollection CreateTextAndAdornmentCollection(SnapshotSpan topSpan,
            ITextSnapshot sourceTextSnapshot);
    }
}