using System;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Text.Data
{
    public interface IMappingSpan
    {
        ITextBuffer AnchorBuffer { get; }

        IBufferGraph BufferGraph { get; }

        IMappingPoint End { get; }

        IMappingPoint Start { get; }
        NormalizedSnapshotSpanCollection GetSpans(ITextBuffer targetBuffer);

        NormalizedSnapshotSpanCollection GetSpans(ITextSnapshot targetSnapshot);

        NormalizedSnapshotSpanCollection GetSpans(Predicate<ITextBuffer> match);
    }
}