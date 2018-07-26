using System;
using System.Collections.ObjectModel;

namespace ModernApplicationFramework.TextEditor
{
    public interface IProjectionSnapshot : ITextSnapshot
    {
        int SpanCount { get; }

        ReadOnlyCollection<ITextSnapshot> SourceSnapshots { get; }

        ITextSnapshot GetMatchingSnapshot(ITextBuffer textBuffer);

        ReadOnlyCollection<SnapshotSpan> GetSourceSpans(int startSpanIndex, int count);

        ReadOnlyCollection<SnapshotSpan> GetSourceSpans();

        SnapshotPoint MapToSourceSnapshot(int position, PositionAffinity affinity);

        ReadOnlyCollection<SnapshotPoint> MapToSourceSnapshots(int position);

        SnapshotPoint MapToSourceSnapshot(int position);

        SnapshotPoint? MapFromSourceSnapshot(SnapshotPoint point, PositionAffinity affinity);

        ReadOnlyCollection<SnapshotSpan> MapToSourceSnapshots(Span span);

        ReadOnlyCollection<Span> MapFromSourceSnapshot(SnapshotSpan span);

        ITextSnapshot GetMatchingSnapshotInClosure(ITextBuffer targetBuffer);

        ITextSnapshot GetMatchingSnapshotInClosure(Predicate<ITextBuffer> match);
    }
}