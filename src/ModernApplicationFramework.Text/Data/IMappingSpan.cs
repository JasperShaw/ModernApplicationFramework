using System;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Text.Data
{
    public interface IMappingSpan
    {
        NormalizedSnapshotSpanCollection GetSpans(ITextBuffer targetBuffer);

        NormalizedSnapshotSpanCollection GetSpans(ITextSnapshot targetSnapshot);

        NormalizedSnapshotSpanCollection GetSpans(Predicate<ITextBuffer> match);

        IMappingPoint Start { get; }

        IMappingPoint End { get; }

        ITextBuffer AnchorBuffer { get; }

        IBufferGraph BufferGraph { get; }
    }
}