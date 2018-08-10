using System;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.Text.Data.Projection
{
    public interface IProjectionSnapshot : ITextSnapshot
    {
        ReadOnlyCollection<ITextSnapshot> SourceSnapshots { get; }
        int SpanCount { get; }

        ITextSnapshot GetMatchingSnapshot(ITextBuffer textBuffer);

        ITextSnapshot GetMatchingSnapshotInClosure(ITextBuffer targetBuffer);

        ITextSnapshot GetMatchingSnapshotInClosure(Predicate<ITextBuffer> match);

        ReadOnlyCollection<SnapshotSpan> GetSourceSpans(int startSpanIndex, int count);

        ReadOnlyCollection<SnapshotSpan> GetSourceSpans();

        SnapshotPoint? MapFromSourceSnapshot(SnapshotPoint point, PositionAffinity affinity);

        ReadOnlyCollection<Span> MapFromSourceSnapshot(SnapshotSpan span);

        SnapshotPoint MapToSourceSnapshot(int position, PositionAffinity affinity);

        SnapshotPoint MapToSourceSnapshot(int position);

        ReadOnlyCollection<SnapshotPoint> MapToSourceSnapshots(int position);

        ReadOnlyCollection<SnapshotSpan> MapToSourceSnapshots(Span span);
    }
}