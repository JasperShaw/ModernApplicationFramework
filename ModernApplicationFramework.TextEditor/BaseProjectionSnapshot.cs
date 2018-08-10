using System;
using System.Collections.ObjectModel;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.TextEditor.Text;

namespace ModernApplicationFramework.TextEditor
{
    internal abstract class BaseProjectionSnapshot : BaseSnapshot, IProjectionSnapshot
    {
        protected int TotalLineCount = 1;
        protected int TotalLength;

        protected BaseProjectionSnapshot(ITextVersion version, StringRebuilder builder)
            : base(version, builder)
        {
        }

        public ReadOnlyCollection<SnapshotPoint> MapToSourceSnapshots(int position)
        {
            return MapInsertionPointToSourceSnapshots(position, null);
        }

        internal abstract ReadOnlyCollection<SnapshotPoint> MapInsertionPointToSourceSnapshots(int position, ITextBuffer excludedBuffer);

        internal abstract ReadOnlyCollection<SnapshotSpan> MapReplacementSpanToSourceSnapshots(Span replacementSpan, ITextBuffer excludedBuffer);

        public abstract int SpanCount { get; }

        public new abstract IProjectionBufferBase TextBuffer { get; }

        public abstract ReadOnlyCollection<ITextSnapshot> SourceSnapshots { get; }

        public abstract ITextSnapshot GetMatchingSnapshot(ITextBuffer textBuffer);

        public abstract ITextSnapshot GetMatchingSnapshotInClosure(ITextBuffer targetBuffer);

        public abstract ITextSnapshot GetMatchingSnapshotInClosure(Predicate<ITextBuffer> match);

        public abstract ReadOnlyCollection<SnapshotSpan> GetSourceSpans(int startSpanIndex, int count);

        public abstract ReadOnlyCollection<SnapshotSpan> GetSourceSpans();

        public abstract SnapshotPoint MapToSourceSnapshot(int position);

        public abstract SnapshotPoint MapToSourceSnapshot(int position, PositionAffinity affinity);

        public abstract SnapshotPoint? MapFromSourceSnapshot(SnapshotPoint point, PositionAffinity affinity);

        public abstract ReadOnlyCollection<SnapshotSpan> MapToSourceSnapshots(Span span);

        public abstract ReadOnlyCollection<SnapshotSpan> MapToSourceSnapshotsForRead(Span span);

        public abstract ReadOnlyCollection<Span> MapFromSourceSnapshot(SnapshotSpan span);
    }
}