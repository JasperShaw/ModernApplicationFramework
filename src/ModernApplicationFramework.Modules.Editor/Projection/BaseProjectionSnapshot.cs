using System;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Modules.Editor.Text;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;

namespace ModernApplicationFramework.Modules.Editor.Projection
{
    internal abstract class BaseProjectionSnapshot : BaseSnapshot, IProjectionSnapshot
    {
        protected int TotalLength;
        protected int TotalLineCount = 1;

        public abstract ReadOnlyCollection<ITextSnapshot> SourceSnapshots { get; }

        public abstract int SpanCount { get; }

        public new abstract IProjectionBufferBase TextBuffer { get; }

        protected BaseProjectionSnapshot(ITextVersion version, StringRebuilder builder)
            : base(version, builder)
        {
        }

        public abstract ITextSnapshot GetMatchingSnapshot(ITextBuffer textBuffer);

        public abstract ITextSnapshot GetMatchingSnapshotInClosure(ITextBuffer targetBuffer);

        public abstract ITextSnapshot GetMatchingSnapshotInClosure(Predicate<ITextBuffer> match);

        public abstract ReadOnlyCollection<SnapshotSpan> GetSourceSpans(int startSpanIndex, int count);

        public abstract ReadOnlyCollection<SnapshotSpan> GetSourceSpans();

        public abstract SnapshotPoint? MapFromSourceSnapshot(SnapshotPoint point, PositionAffinity affinity);

        public abstract ReadOnlyCollection<Span> MapFromSourceSnapshot(SnapshotSpan span);

        public abstract SnapshotPoint MapToSourceSnapshot(int position);

        public abstract SnapshotPoint MapToSourceSnapshot(int position, PositionAffinity affinity);

        public ReadOnlyCollection<SnapshotPoint> MapToSourceSnapshots(int position)
        {
            return MapInsertionPointToSourceSnapshots(position, null);
        }

        public abstract ReadOnlyCollection<SnapshotSpan> MapToSourceSnapshots(Span span);

        public abstract ReadOnlyCollection<SnapshotSpan> MapToSourceSnapshotsForRead(Span span);

        internal abstract ReadOnlyCollection<SnapshotPoint> MapInsertionPointToSourceSnapshots(int position,
            ITextBuffer excludedBuffer);

        internal abstract ReadOnlyCollection<SnapshotSpan> MapReplacementSpanToSourceSnapshots(Span replacementSpan,
            ITextBuffer excludedBuffer);
    }
}